using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BufferObject.WorkStorage
{
    public class Manufacturer
    {
        //private string myNameGoods;
        //private Storage myStorage;

        //public Manufacturer(string myNameGoods, Storage myStorage)
        //{
        //    this.myNameGoods = myNameGoods;
        //    this.myStorage = myStorage;
        //}

        public void PlaceStorage(string myNameGoods, Storage myStorage)
        {
            myStorage.AddTovar(CreateGoods(myNameGoods));
        }

        private Goods CreateGoods(string myNameGoods)
        {
            Goods myTovar = new Goods(myNameGoods);
            return myTovar;
        }
    }
}
