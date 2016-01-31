namespace BufferObject.WorkStorage
{
    public class Manufacturer   // Производитель
    {
        private Goods myTovar;

        public void PlaceStorage(Storage Storage)
        {
            Storage.AddGoods(myTovar = new Goods());
        }
    }
}
