using System;
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
        private string myFileName;
        
        public Storage(string myName)
        {
        	myFileName += myName + ".dat";
        	
        	if (File.Exists(myFileName))
        	{
        		LoadSklad(myFileName);
        	}
        }
        
        ~Storage()
        {
        	SaveSklad(myFileName);
        }
        
        public void AddTovar(Goods tovar)
        {
            myQueue.Enqueue(tovar);
        }

        public Goods GetTovar()
        {
            Goods myGoods;
            if (myQueue.TryDequeue(out myGoods))
                return myGoods;
            else
                return null;

            //myQueue.TrimExcess();
            //return myGoods;
        }

        public string GetSpisok()
        {
            string spisok = "";

            foreach (Goods tov in myQueue)
                spisok += tov.Name + "; ";

            return spisok;
        }
        
        public int GetCount()
        {
        	return myQueue.Count;
        }
        
        private void SaveSklad(string myFileName)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            using (Stream fStream = new FileStream(myFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                binFormat.Serialize(fStream, myQueue);
                Console.WriteLine("--> Сохранение склада в файл");
            }            
        }
        
        private void LoadSklad(string myFileName)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            using (Stream fStream = File.OpenRead(myFileName))
            {
                myQueue = (ConcurrentQueue<Goods>)binFormat.Deserialize(fStream);
                Console.WriteLine("--> Чтение склада из файла");
            }
        }
    }
}
