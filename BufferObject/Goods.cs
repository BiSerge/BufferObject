using System;

namespace BufferObject
{
    [Serializable]
    public class Goods
    {
    	public string Name {set; get;}
    	//public int Cena {set; get;}
    	
    	 public Goods(string NameTovara)
    	{
    	 	Name = NameTovara;
    	 	//Cena = CenaTovara;
    	}
    }   
}
