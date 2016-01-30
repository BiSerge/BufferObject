﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Concurrent;

namespace BufferObject
{
    [Serializable]
    public class Storage
    {
        private ConcurrentQueue<Goods> myQueue = new ConcurrentQueue<Goods>();
        private int myMaxGoods;
        private string myName;
        private string myFileName;
        //private const string OverflowStorage = "Переполнение склада.";

        public Storage(int myMaxGoods, string myName)
        {
            this.myMaxGoods = myMaxGoods;
            this.myName = myName;
            myFileName = myName + ".dat";            
        	
        	if (File.Exists(myFileName))
        	{
        		LoadSklad(myFileName);
        	}
        }
        
        ~Storage()
        {
        	SaveSklad(myFileName);
        }

        public void AddGoods(Goods tovar)
        {
            try
            {
                if (myQueue.Count >= myMaxGoods)
                    throw new OverflowException("Переполнение склада " + myName);
                else
                    myQueue.Enqueue(tovar);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Исключение: " + ex.Message);
            }
        }

        public Goods GetGoods()
        {
            Goods myGoods;
            if (myQueue.TryDequeue(out myGoods))
                return myGoods;
            else
                return null;
        }
        
        public int GetCount()
        {
        	return myQueue.Count;
        }
        
        private void SaveSklad(string myFileName)
        {
            try
            {
                BinaryFormatter binFormat = new BinaryFormatter();
                using (Stream fStream = new FileStream(myFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    binFormat.Serialize(fStream, myQueue);
                    Console.WriteLine("--> Сохранение склада в файл");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка записи данных склада в файл " + ex.Message);
            }
        }
        
        private void LoadSklad(string myFileName)
        {
            try
            {
                BinaryFormatter binFormat = new BinaryFormatter();
                using (Stream fStream = File.OpenRead(myFileName))
                {
                    myQueue = (ConcurrentQueue<Goods>)binFormat.Deserialize(fStream);
                    Console.WriteLine("--> Чтение склада из файла");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка чтения данных склада из файл " + ex.Message);
            }
        }
    }
}
