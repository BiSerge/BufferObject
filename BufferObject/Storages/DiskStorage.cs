using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace BufferObject.Storages
{
    [Serializable]
    public class DiskStorage : IStorage, IDisposable
    {
        //List<string> ListOfFiles = new List<string>();
        //[NonSerialized]
        //string[] _listOfFiles = { "file001.dat", "file002.dat", "file003.dat", "file004.dat" }; // список имен файлов	Удалить?
        string[] _listOfFiles;      // список имен файлов
        string _nameSklad = "";		// имя склада для формирования имени файла
        int _listOfFilesCount  = 0; // колич. сохр. файлов
        int _tovarCount = 0;        // колч. записанного товара
        int _currentFile = 0;       // индекс текущего имени файла       
        int _tovarMax = 50;         // макс. колич. товара в 1 файле        !!! 50% от макс. колч. в памяти
        int _filesMax = 4;			// макс. колич. файлов
        [NonSerialized]
        Stream _fStreamSave;
        [NonSerialized]
        Stream _fStreamRead;
        [NonSerialized]
        BinaryFormatter _binFormat = new BinaryFormatter();
        [NonSerialized]
        ReaderWriterLockSlim _cacheFile = new ReaderWriterLockSlim();


        public DiskStorage(string _nameSklad)
        {
            _currentFile = 0;
            this._nameSklad = _nameSklad;
            _listOfFiles = new string[4];

            for (int i = 0; i < _filesMax; i++)
                _listOfFiles[i] = _nameSklad + "00" + i.ToString() + ".dat";

            OpenFileStreamSave(_listOfFiles[_currentFile]);
        }

        ~ DiskStorage()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            _fStreamSave.Close();

            BinaryFormatter binFormat = new BinaryFormatter();
            using (Stream fStream = new FileStream("DiskStorage.dat", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                binFormat.Serialize(fStream, this);
            }
        }

        public void AddGoods(Goods _tovar)
        {
            _cacheFile.EnterWriteLock();
            try
            {
                _tovarCount++;

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

        public Goods GetGoods()
        {
            //Goods myGoods;
            //(Goods)binFormat.Deserialize(fStream);
            return (Goods)_binFormat.Deserialize(_fStreamRead);
        }

        private void OpenFileStreamSave(string _file)
        {
            _fStreamSave = new FileStream(_file, FileMode.Create, FileAccess.Write);
        }

        private void OpenFileStreamRead(string _file)
        {
            _fStreamRead = new FileStream(_file, FileMode.Create, FileAccess.Write);
        }

        private void NextListOfFiles()
        {
            string _tempString = "";
            _tempString = _listOfFiles[0];
            for(int i = 0; i < 3; i++)
                _listOfFiles[i] = _listOfFiles[i + 1];
            _listOfFiles[3] = _tempString;
        }
    }
}
