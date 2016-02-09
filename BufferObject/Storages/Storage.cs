﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Concurrent;
using System.Threading;

namespace BufferObject.Storages
{
    [Serializable]
    public class Storage : IStorage, IDisposable
    {
        private ConcurrentQueue<Goods> myQueue = new ConcurrentQueue<Goods>();
        private int myMaxGoods;
        private string myNameStorage;
        private string myFileName;
        //private DiskStorage _diskStorage = new DiskStorage();
        //private DiskStorage _diskStorage;
        int _sklad = 1;

        string[] _listOfFiles;      // список имен файлов
        string _nameSklad = "";		// имя склада для формирования имени файла
        int _listOfFilesCount = 0; // колич. сохр. файлов
        int _tovarCount = 0;        // счет товара для записи в файл
        int _tovarAllCount = 0;     // сколько товара записано
        int _currentFile = 0;       // индекс текущего имени файла       
        int _tovarMax = 50;         // макс. колич. товара в 1 файле        !!! 50% от макс. колч. в памяти
        int _filesMax = 4;			// макс. колич. файлов
        //[NonSerialized]
        Stream _fStreamSave;
        //[NonSerialized]
        Stream _fStreamRead;
        //[NonSerialized]
        BinaryFormatter _binFormat = new BinaryFormatter();
        //[NonSerialized]
        ReaderWriterLockSlim _cacheFile = new ReaderWriterLockSlim();

        bool _writeDisk = false;


        public Storage(int MaxGoods, string Name)
        {
            myMaxGoods = MaxGoods;
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

            //if (_diskStorage != null)
            //    SaveDiskStorage();
        }

        public void AddGoods(Goods tovar)
        {
            if (myQueue.Count >= myMaxGoods)
            {
                if (!_writeDisk)
                {
                    _writeDisk = true;
                    WriteDiskStart();
                    //[Serializable]
                    //_diskStorage = new DiskStorage(myNameStorage);
                }
                //_diskStorage.AddGoods(tovar);
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
            _currentFile = 0;
            //this._nameSklad = _nameSklad;
            _listOfFiles = new string[_filesMax];

            for (int i = 0; i < _filesMax; i++)
                _listOfFiles[i] = myNameStorage + "00" + i.ToString() + ".dat";

            OpenFileStreamSave(_listOfFiles[_currentFile]);
        }

        private void OpenFileStreamSave(string _file)
        {
            _fStreamSave = new FileStream(_file, FileMode.Create, FileAccess.Write);
        }

        private void OpenFileStreamRead(string _file)
        {
            _fStreamRead = new FileStream(_file, FileMode.Create, FileAccess.Write);
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
                    _fStreamSave.Close();
                    _currentFile++;
                    if (_currentFile > _filesMax - 1)
                        throw new OverflowException("Переполнение склада !!!!!");
                    _tovarCount = 1;
                    _listOfFilesCount++;
                    OpenFileStreamSave(_listOfFiles[_currentFile]);
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
    }
}