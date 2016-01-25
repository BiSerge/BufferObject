
namespace BufferObject.WorkStorage
{
    public class Logistic
    {
        // TODO Возможно нужна обработка если товар не перещен
        public bool MoveGoods(Storage myStorage1, Storage myStorage2)
        {
            Goods myTovar = myStorage1.GetTovar();
            if (myTovar != null)
            {
                myStorage2.AddTovar(myTovar);
                return true;
            }
            else
                return false;
        }
    }
}
