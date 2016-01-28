
namespace BufferObject.WorkStorage
{
    public class Logistic
    {
        // TODO Возможно нужна обработка если товар не перещен
        public bool MoveGoods(Storage myStorage1, Storage myStorage2)
        {
            Goods myTovar = myStorage1.GetGoods();
            if (myTovar != null)
            {
                myStorage2.AddGoods(myTovar);
                return true;
            }
            else
                return false;
        }
    }
}
