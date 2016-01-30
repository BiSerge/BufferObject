using System;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using BufferObject.WorkStorage;

namespace BufferObject
{
    class FlowControlAsync
    {
        // Построитель потоков
        public async Task ResultManufacturerAsync(Storage mySklad1, Storage mySklad2, int ChainLength, 
            int ThreadsManufactur, int ThreadsLogist, int ThreadsConsumer)
        {
            Task[] myTask = new Task[ThreadsManufactur + ThreadsLogist + ThreadsConsumer];    // Масив всех потоков
            RndArray(myTask);

            for (int i = 0; i < ThreadsManufactur; i++)
                myTask[i] = ManufacturerAsync(mySklad1, ChainLength);                    // Производители

            for (int i = ThreadsManufactur; i < ThreadsManufactur + ThreadsLogist; i++)
                myTask[i] = LogisticAsync(mySklad1, mySklad2, ChainLength);              // Логисты

            for (int i = ThreadsManufactur + ThreadsLogist; i < ThreadsManufactur + ThreadsLogist + ThreadsConsumer; i++)
                myTask[i] = ConsumerAsync(mySklad2, ChainLength);                        // Покупатели


            Task allTasks = Task.WhenAll(myTask);
            try
            {
                await allTasks;     // Запуск всех потоков
            }
            catch (Exception ex)
            {
                Console.WriteLine("===>");
                Console.WriteLine("Исключение: " + ex.Message);
                foreach (var inx in allTasks.Exception.InnerExceptions)
                {
                    Console.WriteLine("Внутренне исключение: " + inx.Message);
                }
                Console.WriteLine("===>");
            }

            Console.WriteLine();
            Console.WriteLine("На 1 складе осталось {0} шт. товара:", mySklad1.GetCount());
            Console.WriteLine("На 2 складе осталось {0} шт. товара:", mySklad2.GetCount());
        }

        private Task[] RndArray(Task[] myTask)    // Перемешиваю массив потоков
        {
            if (myTask.Length < 1) return myTask;
            var random = new Random();
            for (var i = 0; i < myTask.Length; i++)
            {
                var key = myTask[i];
                var rnd = random.Next(i, myTask.Length);
                myTask[i] = myTask[rnd];
                myTask[rnd] = key;
            }
            return myTask;
        }

        // Задача, "Производители" отдают товар на 1 склад
        private Task ManufacturerAsync(Storage mySklad, int ChainLength)
        {
            return Task.Run(() =>
            {
                Manufacturer myManufacturer = new Manufacturer();
                for (int i = 0; i < ChainLength; i++)
                {
                    myManufacturer.PlaceStorage(mySklad);
                    Console.WriteLine("На склад 1, поток - {0}, проход - {1}", Thread.CurrentThread.ManagedThreadId, i);
                    // Имитация произвольных обращений
                    Random rnd = new Random();
                    Thread.Sleep(rnd.Next(0, 1000));
                }
            });
        }

        // Задача для потока перемещения с 1 склада на 2
        private Task LogisticAsync(Storage mySklad1, Storage mySklad2, int ChainLength)
        {
            return Task.Run(() =>
            {
                Logistic myLogistic = new Logistic();
                for (int i = 0; i < ChainLength; i++)
                {
                    if (myLogistic.MoveGoods(mySklad1, mySklad2))
                        Console.WriteLine("На склад 2, поток - {0}, проход - {1}", Thread.CurrentThread.ManagedThreadId, i);
                    else
                        Console.WriteLine("На складе 1 нет товара!!! Поток - {0}, проход - {1}", Thread.CurrentThread.ManagedThreadId, i);
                    // Имитация произвольных обращений
                    Random rnd = new Random();
                    Thread.Sleep(rnd.Next(0, 1000));
                }
            });
        }

        // Задача для потока "потребителей"
        private Task ConsumerAsync(Storage mySklad2, int ChainLength)
        {
            return Task.Run(() =>
            {
                Consumer myConsumer = new Consumer();
                for (int i = 0; i < ChainLength; i++)
                {
                    if (myConsumer.GetGoods(mySklad2))
                        Console.WriteLine("На продажу поток - {0}, проход - {1}", Thread.CurrentThread.ManagedThreadId, i);
                    else
                        Console.WriteLine("На складе 2 нет товара!!! Поток - {0}, проход - {1}", Thread.CurrentThread.ManagedThreadId, i);
                    // Имитация произвольных обращений
                    Random rnd = new Random();
                    Thread.Sleep(rnd.Next(0, 1000));
                }
            });
        }
    }
}
