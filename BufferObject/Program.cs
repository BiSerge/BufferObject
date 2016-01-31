using System;
using System.Threading.Tasks;
using System.Configuration;

namespace BufferObject
{
    class Program
    {
        private static int myChainLength        = 10;   // Длина тестовой цепочки
        private static int myThreadsManufactur  = 3;    // Колич. потоков для производителей
        private static int myThreadsLogist      = 3;    // Колич. потоков для логистов
        private static int myThreadsConsumer    = 3;    // Колич. потоков для потребителей
        private static int myMaxGoods           = 10;   // Объем склада

        static void Main(string[] args)
        {
            try
            {
                myChainLength       = int.Parse(ConfigurationManager.AppSettings[0]);
                myThreadsManufactur = int.Parse(ConfigurationManager.AppSettings[1]);
                myThreadsLogist     = int.Parse(ConfigurationManager.AppSettings[2]);
                myThreadsConsumer   = int.Parse(ConfigurationManager.AppSettings[3]);
                myMaxGoods          = int.Parse(ConfigurationManager.AppSettings[4]);
            }
            catch
            {
                Console.WriteLine("Ошибка чтения параметров! Будут применены параметры по умолчанию");
            }

            try
            {
                using (Storage mySklad1 = new Storage(myMaxGoods, "Sklad1"), mySklad2 = new Storage(myMaxGoods, "Sklad2"))
                {
                    FlowControlAsync myClassAsync = new FlowControlAsync(); // Клас потоков

                    Console.WriteLine("На складе 1 товар в количестве: {0} шт.", mySklad1.GetCount());
                    Console.WriteLine("На складе 2 товар в количестве: {0} шт.", mySklad2.GetCount());
                    Console.WriteLine();

                    // Вызов построителя потоков
                    Task task = myClassAsync.ResultManufacturerAsync(mySklad1, mySklad2, myChainLength, myThreadsManufactur,
                        myThreadsLogist, myThreadsConsumer);
                    task.Wait();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Исключение: " + ex.Message);
            }

            Console.WriteLine("Press any key to continue . . . ");
            Console.ReadKey(true);
        }        
    }
}
