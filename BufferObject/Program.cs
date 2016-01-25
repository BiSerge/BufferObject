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
        private static int ChainLength = 20;
        private static int ThreadsManufactur = 3;
        private int ThreadsLogist;
        private int ThreadsConsumer;

        static void Main(string[] args)
        {
            Storage mySklad1 = new Storage("Sklad1");
            Storage mySklad2 = new Storage("Sklad2");

            Console.WriteLine("На складе 1 находится товар: ");
            Console.WriteLine(mySklad1.GetSpisok());
            Console.WriteLine("Всего: {0} шт.", mySklad1.GetCount());
            Console.WriteLine();

            //Manufacturer myManufacturer1 = new Manufacturer("Яблоко", mySklad);
            //for (int i = 0; i < 5; i++)
            //    myManufacturer1.PlaceStorage();

            //Manufacturer myManufacturer2 = new Manufacturer("Бублик", mySklad);
            //for (int i = 0; i < 5; i++)
            //    myManufacturer2.PlaceStorage();
            //Boolean myBol = await AccessAsync();

            ResultManufacturerAsync(mySklad1, mySklad2, ThreadsManufactur, ChainLength).GetAwaiter();
            //ResultLogisticAsync(mySklad1, mySklad2).GetAwaiter();

            //Console.WriteLine("На склад помещен товар, теперь на складе {0} шт. товара:", mySklad.GetCount());
            //Console.WriteLine(mySklad.GetSpisok());
            //Console.WriteLine();

            Console.WriteLine("Press any key to continue . . . ");
            Console.ReadKey(true);
        }

        static async Task ResultManufacturerAsync(Storage mySklad1, Storage mySklad2, int k, int j)
        {
            Task[] myTask = new Task[k + 3];
            for (int i = 0; i < k; i++)
                myTask[i] = ManufacturerAsync("Яблоко", mySklad1, j);
            for (int i = 3; i < 6; i++)
                myTask[i] = LogisticAsync(mySklad1, mySklad2);

            await Task.WhenAll(myTask);

            Console.WriteLine("На склад помещен товар, теперь на складе {0} шт. товара:", mySklad1.GetCount());
            //Console.WriteLine(mySklad1.GetSpisok());
            Console.WriteLine();

            Console.WriteLine("На 1 складе осталось {0} шт. товара:", mySklad1.GetCount());
            //Console.WriteLine(mySklad1.GetSpisok());
            Console.WriteLine("На 2 склае всего {0} шт. товара:", mySklad2.GetCount());
            //Console.WriteLine(mySklad2.GetSpisok());
            Console.WriteLine();
        }

        static Task ManufacturerAsync(string NameGoods, Storage mySklad, int j)
        {
            return Task.Run(() =>
            {
                Manufacturer myManufacturer = new Manufacturer();
                for (int i = 0; i < j; i++)
                { 
                    myManufacturer.PlaceStorage(NameGoods, mySklad);
                    Console.WriteLine("Положить на склад 1 поток - {0}", Thread.CurrentThread.ManagedThreadId);
                    Random rnd = new Random();
                    Thread.Sleep(rnd.Next(0, 1000));                   
                }
            });
        }

        static Task LogisticAsync(Storage mySklad1, Storage mySklad2)
        {
            return Task.Run(() =>
            {
                Logistic myLogistic = new Logistic();
                for (int i = 0; i < 10; i++)
                {
                    myLogistic.MoveGoods(mySklad1, mySklad2);
                    Console.WriteLine("Забрать на склад 2 - {0}", Thread.CurrentThread.ManagedThreadId);
                    Random rnd = new Random();                    
                    Thread.Sleep(rnd.Next(0, 1000));
                }
            });
        }
    }
}
