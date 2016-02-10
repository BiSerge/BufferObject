using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;

namespace BufferObject.Storages
{
    [Serializable]
    public class Storage : IStorage, IDisposable
    {
        private ConcurrentQueue<Goods> myQueue = new ConcurrentQueue<Goods>();
        private int myMaxGoods;
        private string myNameStorage;
        private string myFileName;

        int _tovarCount = 0;        // счет товара для записи в файл
        int _tovarAllCount = 0;     // сколько товара записано     
        int _tovarMax = 0;         // макс. колич. товара в 1 файле        !!! 50% от макс. колч. в памяти
        int _filesMax = 4;			// макс. колич. файлов

        Queue<string> _listFilesWrite = new Queue<string>();     // Список файлов для записи
        Queue<string> _listFilesRead = new Queue<string>();     // Список файлов для чтения

        Stream _fStreamSave;
        BinaryFormatter _binFormat = new BinaryFormatter();
        ReaderWriterLockSlim _cacheFile = new ReaderWriterLockSlim();

        bool _writeDisk = false;


        public Storage(int MaxGoods, string Name)
        {
            myMaxGoods = MaxGoods;
            _tovarMax = MaxGoods / 2;
            myNameStorage = Name;
            myFileName = Name + ".dat";

            if (File.Exists(myFileName))
                LoadSklad(myFileName);
        }

        public void Dispose()
        {
            SaveSklad(myFileName);

            if (_fStreamSave != null)
                _fStreamSave.Close();

            if (_cacheFile != null)
                _cacheFile.Dispose();
        }

        public void AddGoods(Goods tovar)
        {
            if (myQueue.Count >= myMaxGoods)
            {
                if (!_writeDisk)
                {
                    _writeDisk = true;
                    WriteDiskStart();
                }
                AddDiskGoods(tovar);

                //throw new OverflowException("Переполнение склада " + myNameStorage);
            }
            else
                myQueue.Enqueue(tovar);
        }

        public Goods GetGoods()
        {
            Goods myGoods;
            if (myQueue.TryDequeue(out myGoods))
                return myGoods;
            //else if (_listFilesRead.Count != 0)
            //    ReadDiskGoods();
            else
                return null;
        }

        public int GetCount()
        {
            return myQueue.Count + _tovarAllCount;
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

        private void SaveDiskStorage()
        {
            //BinaryFormatter binFormat = new BinaryFormatter();
            //using (Stream fStream = new FileStream("DiskStorage.dat", FileMode.Create, FileAccess.Write, FileShare.None))
            //{
            //    binFormat.Serialize(fStream, _diskStorage);
            //}
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

        private void WriteDiskStart()
        {
            for (int i = 0; i < _filesMax; i++)
                _listFilesWrite.Enqueue(myNameStorage + "00" + i.ToString() + ".dat");

            OpenFileStreamSave();
        }

        private void OpenFileStreamSave()
        {
            string _file = _listFilesWrite.Dequeue();
            _listFilesRead.Enqueue(_file);
            _fStreamSave = new FileStream(_file, FileMode.Create, FileAccess.Write);
        }
        
        public void AddDiskGoods(Goods _tovar)
        {
            _cacheFile.EnterWriteLock();
            try
            {
                _tovarCount++;
                _tovarAllCount++;

                if (_tovarCount > _tovarMax)
                {
                    if (_listFilesWrite.Count == 0)
                        throw new OverflowException("Переполнение склада !!!!!");
                    _tovarCount = 1;
                    OpenFileStreamSave();
                }

                _binFormat.Serialize(_fStreamSave, _tovar);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка записи данных. " + ex.Message);
            }
            finally
            {
                _cacheFile.ExitWriteLock();
            }
        }

        private void ReadDiskGoods()
        {
            string _file = _listFilesRead.Dequeue();
            _listFilesWrite.Enqueue(_file);
            _cacheFile.EnterReadLock();
            try
            {
                BinaryFormatter binFormat = new BinaryFormatter();
                using (Stream fStream = new FileStream(_file, FileMode.Open, FileAccess.Read))
                {
                    for (int i = 0; i < _tovarMax; i++)
                    {
                        myQueue.Enqueue((Goods)binFormat.Deserialize(fStream));
                    }
                }
            }
            finally
            {
                _cacheFile.ExitReadLock();
            }
        }
    }
}
