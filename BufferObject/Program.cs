using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BufferObject.WorkStorage;

namespace BufferObject
{
    class Program
    {
        private static int ChainLength = 20;            // Длина тестовой цепочки
        private static int ThreadsManufactur = 3;       // Колич. потоков для производителей
        private int ThreadsLogist;                      // Колич. потоков для логистов
        private int ThreadsConsumer;                    // Колич. потоков для потребителей

        static void Main(string[] args)
        {
            Storage mySklad1 = new Storage("Sklad1");   // Создание складов
            Storage mySklad2 = new Storage("Sklad2");

            Console.WriteLine("На складе 1 товар в количестве: {0} шт.", mySklad1.GetCount());
            Console.WriteLine();

            // Вызов построителя потоков
            ResultManufacturerAsync(mySklad1, mySklad2, ThreadsManufactur, ChainLength).GetAwaiter();

            Console.WriteLine("Press any key to continue . . . ");
            Console.ReadKey(true);
        }

        // Построитель потоков
        static async Task ResultManufacturerAsync(Storage mySklad1, Storage mySklad2, int k, int j)
        {
            Task[] myTask = new Task[k + 3];    // Масив всех потоков
            for (int i = 0; i < k; i++)         
                myTask[i] = ManufacturerAsync("Яблоко", mySklad1, j);   // Производители

            for (int i = 3; i < 6; i++)
                myTask[i] = LogisticAsync(mySklad1, mySklad2);          // Логисты

            await Task.WhenAll(myTask);     // Запуск всех потоков

            Console.WriteLine("На склад помещен товар, теперь на складе {0} шт. товара:", mySklad1.GetCount());
            Console.WriteLine();

            Console.WriteLine("На 1 складе осталось {0} шт. товара:", mySklad1.GetCount());
            Console.WriteLine("На 2 склае всего {0} шт. товара:", mySklad2.GetCount());
            Console.WriteLine();
        }

        // Задача, "Производители" отдают товар на 1 склад
        static Task ManufacturerAsync(string NameGoods, Storage mySklad, int j)
        {
            return Task.Run(() =>
            {
                Manufacturer myManufacturer = new Manufacturer();
                for (int i = 0; i < j; i++)
                { 
                    myManufacturer.PlaceStorage(mySklad);
                    Console.WriteLine("Положить на склад 1 поток - {0}", Thread.CurrentThread.ManagedThreadId);
                    // Имитация произвольных обращений
                    Random rnd = new Random();
                    Thread.Sleep(rnd.Next(0, 1000));                   
                }
            });
        }

        // Задача для потока перемещения с 1 склада на 2
        static Task LogisticAsync(Storage mySklad1, Storage mySklad2)
        {
            return Task.Run(() =>
            {
                Logistic myLogistic = new Logistic();
                for (int i = 0; i < 10; i++)
                {
                    myLogistic.MoveGoods(mySklad1, mySklad2);
                    Console.WriteLine("Забрать на склад 2 поток - {0}", Thread.CurrentThread.ManagedThreadId);
                    // Имитация произвольных обращений
                    Random rnd = new Random();                    
                    Thread.Sleep(rnd.Next(0, 1000));
                }
            });
        }
    }
}
