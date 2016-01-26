﻿using System;
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

        static void Main(string[] args)
        {
            try
            {
                ChainLength = int.Parse(ConfigurationManager.AppSettings[0]);
                ThreadsManufactur = int.Parse(ConfigurationManager.AppSettings[1]);
                ThreadsLogist = int.Parse(ConfigurationManager.AppSettings[2]);
                ThreadsConsumer = int.Parse(ConfigurationManager.AppSettings[3]);
            }
            catch
            {
                Console.WriteLine("Ошибка чтения параметров! Будут применены параметры по умолчанию");
            }

            Storage mySklad1 = new Storage("Sklad1");   // Создание складов
            Storage mySklad2 = new Storage("Sklad2");

            ClassAsync myClassAsync = new ClassAsync(); // Клас потоков

            Console.WriteLine("На складе 1 товар в количестве: {0} шт.", mySklad1.GetCount());
            Console.WriteLine();

            // Вызов построителя потоков
            myClassAsync.ResultManufacturerAsync(mySklad1, mySklad2, ChainLength, ThreadsManufactur, ThreadsLogist, ThreadsConsumer).GetAwaiter();

            Console.WriteLine("Press any key to continue . . . ");
            Console.ReadKey(true);
        }        
    }
}
