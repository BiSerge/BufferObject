using BufferObject.Storages;

namespace BufferObject.WorkStorage
{
    public class Consumer   // Потребитель
    {
        public bool GetGoods(Storage Storage)
        {
            if (Storage.GetGoods() != null)
                return true;
            else
                return false;
        }
    }
}
