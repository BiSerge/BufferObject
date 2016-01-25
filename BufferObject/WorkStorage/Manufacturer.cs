using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BufferObject.WorkStorage
{
    public class Manufacturer
    {
        private Goods myTovar;

        public void PlaceStorage(Storage myStorage)
        {
            myStorage.AddTovar(myTovar = new Goods());
        }
    }
}
