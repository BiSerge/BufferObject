using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Concurrent;

namespace BufferObject
{
    [Serializable]
    public class Storage : IStorage, IDisposable
    {
        private ConcurrentQueue<Goods> myQueue = new ConcurrentQueue<Goods>();
        private int myMaxGoods;
        private string myNameStorage;

        public Storage(int MaxGoods, string Name)
        {
            myMaxGoods = MaxGoods;
            myNameStorage = Name;
            string myFileName = Name + ".dat";            
        	
        	if (File.Exists(myFileName))
        		LoadSklad(myFileName);
        }

        public void Dispose()
        {
            SaveSklad(myNameStorage + ".dat");
        }

        public void AddGoods(Goods tovar)
        {
            if (myQueue.Count >= myMaxGoods)
                throw new OverflowException("Переполнение склада " + myNameStorage);
            else
                myQueue.Enqueue(tovar);
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
        
        private void SaveSklad(string FileName)
        {
            if (myQueue.IsEmpty)    // если склад пуст удаляем файл, чтоб не далать загрузку пустого в конструкторе
            {
                if (File.Exists(FileName))
                    File.Delete(FileName);
            }
            else
            {
                try
                {
                    BinaryFormatter binFormat = new BinaryFormatter();
                    using (Stream fStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        binFormat.Serialize(fStream, myQueue);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Ошибка записи данных склада в файл " + ex.Message);
                }
            }
        }
        
        private void LoadSklad(string FileName)
        {
            try
            {
                BinaryFormatter binFormat = new BinaryFormatter();
                using (Stream fStream = File.OpenRead(FileName))
                {
                    myQueue = (ConcurrentQueue<Goods>)binFormat.Deserialize(fStream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка чтения данных склада из файла " + ex.Message);
            }
        }
    }
}
