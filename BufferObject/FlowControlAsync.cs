using System;
using System.Threading;
using System.Threading.Tasks;
using BufferObject.WorkStorage;

namespace BufferObject
{
    class FlowControlAsync
    {
        // Построитель потоков
        public async Task ResultManufacturerAsync(Storage Sklad1, Storage Sklad2, int ChainLength, 
            int ThreadsManufactur, int ThreadsLogist, int ThreadsConsumer)
        {
            Task[] myTask = new Task[ThreadsManufactur + ThreadsLogist + ThreadsConsumer];  // Масив всех потоков
            RndArray(myTask);

            for (int i = 0; i < ThreadsManufactur; i++)
                myTask[i] = ManufacturerAsync(Sklad1, ChainLength);                         // Производители

            for (int i = ThreadsManufactur; i < ThreadsManufactur + ThreadsLogist; i++)
                myTask[i] = LogisticAsync(Sklad1, Sklad2, ChainLength);                     // Логисты

            for (int i = ThreadsManufactur + ThreadsLogist; i < ThreadsManufactur + ThreadsLogist + ThreadsConsumer; i++)
                myTask[i] = ConsumerAsync(Sklad2, ChainLength);                             // Покупатели

            await Task.WhenAll(myTask);     // Запуск всех потоков

            Console.WriteLine();
            Console.WriteLine("На 1 складе осталось {0} шт. товара:", Sklad1.GetCount());
            Console.WriteLine("На 2 складе осталось {0} шт. товара:", Sklad2.GetCount());
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
        private Task ManufacturerAsync(Storage Sklad, int ChainLength)
        {
            return Task.Run(() =>
            {
                Manufacturer myManufacturer = new Manufacturer();
                for (int i = 0; i < ChainLength; i++)
                {
                    try
                    {
                        myManufacturer.PlaceStorage(Sklad);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Исключение: " + ex.Message);
                    }

                    Console.WriteLine("На склад 1, поток - {0}, проход - {1}", Thread.CurrentThread.ManagedThreadId, i);
                    
                    // Имитация произвольных обращений
                    Random rnd = new Random();
                    Thread.Sleep(rnd.Next(0, 1000));
                }
            });
        }

        // Задача для потока перемещения с 1 склада на 2
        private Task LogisticAsync(Storage Sklad1, Storage Sklad2, int ChainLength)
        {
            return Task.Run(() =>
            {
                Logistic myLogistic = new Logistic();
                for (int i = 0; i < ChainLength; i++)
                {
                    try
                    {
                        if (myLogistic.MoveGoods(Sklad1, Sklad2))
                            Console.WriteLine("На склад 2, поток - {0}, проход - {1}", Thread.CurrentThread.ManagedThreadId, i);
                        else
                            Console.WriteLine("На складе 1 нет товара!!! Поток - {0}, проход - {1}", Thread.CurrentThread.ManagedThreadId, i);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Исключение: " + ex.Message);
                    }

                    // Имитация произвольных обращений
                    Random rnd = new Random();
                    Thread.Sleep(rnd.Next(0, 1000));
                }
            });
        }

        // Задача для потока "потребителей"
        private Task ConsumerAsync(Storage Sklad2, int ChainLength)
        {
            return Task.Run(() =>
            {
                Consumer myConsumer = new Consumer();
                for (int i = 0; i < ChainLength; i++)
                {
                    if (myConsumer.GetGoods(Sklad2))
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
