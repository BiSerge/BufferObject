﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BufferObject;

namespace BufferObjectTest
{
    [TestClass]
    public class StorageTest
    {
        [TestMethod]
        public void AddGoods_IncreasingTheNumberGoods()
        {
            // arrange
            int myMaxGoods = 10;
            Storage myStorage = new Storage(myMaxGoods, "SkladTest");
            Goods myTovar;
            int expected = myStorage.GetCount() + 1;

            // act
            myStorage.AddGoods(myTovar = new Goods());

            // assert
            int actual = myStorage.GetCount();
            Assert.AreEqual(expected, actual, 0, "Товар не добавляется на склад!");
        }

        [TestMethod]
        public void GetGoods_GetGoods()
        {
            // arrange
            int myMaxGoods = 10;
            Storage myStorage = new Storage(myMaxGoods, "SkladTest");
            Goods expected = new Goods();
            //Goods expected = myTovar;

            // act
            myStorage.AddGoods(expected);

            // assert
            Goods actual = myStorage.GetGoods();
            Assert.AreEqual(expected, actual, "Товар не получен!");
        }
    }
}