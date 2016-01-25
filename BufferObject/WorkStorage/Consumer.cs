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
        public bool GetGoods(Storage myStorage2)
        {
            if (myStorage2.GetTovar() != null)
                return true;
            else
                return false;
        }
    }
}
