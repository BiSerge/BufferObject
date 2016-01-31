using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BufferObject;

namespace BufferObjectTest
{
    [TestClass]
    public class StorageTest
    {
        [TestMethod]
        public void AddGoods_AddGoods_IncreasingTheNumberGoods() // При добавлении количество увеличивается на 1
        {
            // Организация
            int myMaxGoods = 10;
            Storage myStorage = new Storage(myMaxGoods, "SkladTest");
            Goods myTovar;
            int expected = myStorage.GetCount() + 1;

            // Действие
            myStorage.AddGoods(myTovar = new Goods());

            // Утверждение
            int actual = myStorage.GetCount();
            Assert.AreEqual(expected, actual, 0, "Количество товара не увеличивается!");
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void AddGoods_MoreThanTheMaximumAmount_QuantityDoesNotIncrease()     // При добавлении товара количество не должно привышать максимума
        {
            // Организация
            int myMaxGoods = 2;
            Storage myStorage = new Storage(myMaxGoods, "SkladTest");
            Goods myTovar;

            // Действие
            for (int i = 1; i < myMaxGoods + 2; i++)
                myStorage.AddGoods(myTovar = new Goods());

            // Утверждение
        }

        [TestMethod]
        public void GetGoods_GetTheGoods_TheResultingProductIsIndoors() // Получение товара товара
        {
            // Организация
            int myMaxGoods = 10;
            Storage myStorage = new Storage(myMaxGoods, "SkladTest");
            Goods expected = new Goods();

            // Действие
            myStorage.AddGoods(expected);

            // Утверждение
            Goods actual = myStorage.GetGoods();
            Assert.AreEqual(expected, actual, "Товар не получен!");
        }
    }
}
