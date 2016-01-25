using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BufferObject.WorkStorage
{
    public class Consumer
    {
        // TODO Возможно нужна обработка если товара на складе нет
        public void GetGoods(Storage myStorage2)
        {
            Goods myTovar = myStorage2.GetTovar();
            //if (myTovar != null)
            //    myStorage2.AddTovar(myTovar);
        }
    }
}
