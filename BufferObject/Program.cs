using System;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using BufferObject.WorkStorage;

namespace BufferObject
{
    class Program
    {
        private static int ChainLength          = 10;      // Длина тестовой цепочки
        private static int ThreadsManufactur    = 3;       // Колич. потоков для производителей
        private static int ThreadsLogist        = 3;       // Колич. потоков для логистов
        private static int ThreadsConsumer      = 3;       // Колич. потоков для потребителей
        private static int myMaxGoods           = 10;      // Объем склада

        static void Main(string[] args)
        {
            try
            {
                ChainLength         = int.Parse(ConfigurationManager.AppSettings[0]);
                ThreadsManufactur   = int.Parse(ConfigurationManager.AppSettings[1]);
                ThreadsLogist       = int.Parse(ConfigurationManager.AppSettings[2]);
                ThreadsConsumer     = int.Parse(ConfigurationManager.AppSettings[3]);
                myMaxGoods          = int.Parse(ConfigurationManager.AppSettings[4]);
            }
            catch
            {
                Console.WriteLine("Ошибка чтения параметров! Будут применены параметры по умолчанию");
            }

            Storage mySklad1 = new Storage(myMaxGoods, "Sklad1");   // Создание складов
            Storage mySklad2 = new Storage(myMaxGoods, "Sklad2");

            FlowControlAsync myClassAsync = new FlowControlAsync(); // Клас потоков

            Console.WriteLine("На складе 1 товар в количестве: {0} шт.", mySklad1.GetCount());
            Console.WriteLine();

            // Вызов построителя потоков
            Task task = myClassAsync.ResultManufacturerAsync(mySklad1, mySklad2, ChainLength, ThreadsManufactur, ThreadsLogist,
                ThreadsConsumer);
            task.Wait();

            Console.WriteLine("Press any key to continue . . . ");
            Console.ReadKey(true);
        }        
    }
}
