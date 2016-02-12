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
        private string _fileParam = "";

        int _tovarCount = 0;        // счет товара для записи в файл
        int _tovarAllCount = 0;     // сколько товара записано     
        int _tovarMax = 0;         // макс. колич. товара в 1 файле        !!! 50% от макс. колч. в памяти
        int _filesMax = 4;			// макс. колич. файлов

        Queue<string> _listFilesWrite = new Queue<string>();     // Список файлов для записи
        Queue<string> _listFilesRead = new Queue<string>();     // Список файлов для чтения

        Stream _fStreamSave;
        BinaryFormatter _binFormat = new BinaryFormatter();
        ReaderWriterLockSlim _cacheFile = new ReaderWriterLockSlim();

        //static object _locker = new object();

        bool _writeDisk = false;


        public Storage(int MaxGoods, string Name)
        {
            myMaxGoods = MaxGoods;
            _tovarMax = MaxGoods / 2;
            myNameStorage = Name;
            myFileName = Name + ".dat";
            _fileParam = Name + "Param.dat";

            if (File.Exists(myFileName))
                LoadSklad(myFileName);

            if (File.Exists(_fileParam))
            {
                LoadParam(_fileParam);
                _writeDisk = true;
            }
        }

        public void Dispose()
        {
            SaveSklad(myFileName);
            SaveParam(_fileParam);

            if (_fStreamSave != null)
                _fStreamSave.Close();

            if (_cacheFile != null)
                _cacheFile.Dispose();
        }

        public void AddGoods(Goods tovar)
        {
            //_cacheFile.EnterWriteLock();
            //try
            //{
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
            //}
            //finally
            //{
            //    _cacheFile.ExitWriteLock();
            //}
        }

        public Goods GetGoods()
        {
            _cacheFile.EnterReadLock();
            try
            {
                Goods myGoods;
                if (myQueue.TryDequeue(out myGoods))
                    return myGoods;
                else if (_listFilesRead.Count != 0)
                {
                    ReadDiskGoods();
                    if (myQueue.TryDequeue(out myGoods))
                        return myGoods;
                    else
                        return null;
                }
                else
                    return null;
            }
            finally
            {
                _cacheFile.ExitReadLock();
            }
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

        private void SaveParam(string _fileParam)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            using (Stream fStream = new FileStream(_fileParam, FileMode.Create, FileAccess.Write))
            {
                binFormat.Serialize(fStream, _tovarCount);
                binFormat.Serialize(fStream, _tovarAllCount);
                binFormat.Serialize(fStream, _listFilesWrite);
                binFormat.Serialize(fStream, _listFilesRead);
            }
        }

        private void LoadParam(string _fileParam)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            using (Stream fStream = new FileStream(_fileParam, FileMode.Open, FileAccess.Read))
            {
                _tovarCount = (int)binFormat.Deserialize(fStream);
                _tovarAllCount = (int)binFormat.Deserialize(fStream);
                _listFilesWrite = ((Queue<string>)binFormat.Deserialize(fStream));
                _listFilesRead = ((Queue<string>)binFormat.Deserialize(fStream));
                //myQueue.Enqueue((Goods)binFormat.Deserialize(fStream));
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

        private void WriteDiskStart()
        {
            for (int i = 0; i < _filesMax; i++)
                _listFilesWrite.Enqueue(myNameStorage + "00" + i.ToString() + ".dat");

            OpenFileStreamSave();
        }

        private void OpenFileStreamSave()
        {
            string _file = _listFilesWrite.Peek();
            //_listFilesRead.Enqueue(_file);
            _fStreamSave = new FileStream(_file, FileMode.Create, FileAccess.Write);
        }
        
        public void AddDiskGoods(Goods _tovar)
        {
            _cacheFile.EnterWriteLock();
            try
            {
                if (_fStreamSave == null)
                    OpenFileStreamSave();
                _tovarCount++;
                _tovarAllCount++;

                if (_tovarCount > _tovarMax)
                {
                    if (_listFilesWrite.Count == 1)
                    {
                        _tovarAllCount--;
                        throw new OverflowException("Переполнение склада !!!!!");
                    }
                    
                    _tovarCount = 1;
                    _listFilesRead.Enqueue(_listFilesWrite.Dequeue());
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
            _cacheFile.EnterReadLock();
            try
            {
                string _file = _listFilesRead.Dequeue();
                _listFilesWrite.Enqueue(_file);
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
