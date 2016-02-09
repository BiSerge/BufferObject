
namespace BufferObject.Storages
{
    interface IStorage
    {
        void AddGoods(Goods tovar);
        Goods GetGoods();
    }
}
