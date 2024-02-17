using Discord;
using FortniteBot;

namespace FortniteBot.Shop
{
    public class ShopData
    {
        public Data Data { get; set; }

    }

    public class Data
    {
        public Featured Featured { get; set; }

    }


    public class Featured
    {
        public List<Entry> Entries { get; set; }

    }

    public class Entry
    {
        public int RegularPrice { get; set; }
        public int FinalPrice { get; set; }
        public Bundle Bundle { get; set; }
        public bool Giftable { get; set; }
        public List<Item> Items { get; set; }

    }

    public class Bundle
    {
        public string Info { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }


    }


    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Rarity Rarity { get; set; }
        public Images Images {  get; set; }
        public Introduction Introduction { get; set; }
    }

    public class Rarity
    {
        public string DisplayValue{ get; set; }

       

    }
    public class Images
    {
        public string Featured { get; set; }

    }
    public class Introduction
    {
        public string Text { get; set; }
    }

}