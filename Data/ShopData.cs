using FortniteBot;

namespace FortniteBot.Shop
{
    public class ShopData
    {
        public Data Data { get; set; }

    }

    public class Data
    {

        public string VbuckIcon { get; set; }
        public Featured Featured { get; set; }

    }


    public class Featured
    {
        public List<Entry> Entries { get; set; }
        public string Name { get; set; }

    }

    public class Entry
    {
        public int RegularPrice { get; set; }
        public int FinalPrice { get; set; }
        public Bundle Bundle { get; set; }
        public bool Giftable { get; set; }
    }

    public class Bundle
    {
        public string Info { get; set; }
        public string Name { get; set; }

        public string Image { get; set; }

        
    }
}