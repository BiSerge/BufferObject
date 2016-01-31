
namespace BufferObject.WorkStorage
{
    public class Logistic   // Перемещает товар со склада А на склад Б
    {
        public bool MoveGoods(Storage TakeStorage, Storage PlaceStorage)
        {
            Goods myTovar = TakeStorage.GetGoods();
            if (myTovar != null)
            {
                PlaceStorage.AddGoods(myTovar);
                return true;
            }
            else
                return false;
        }
    }
}
