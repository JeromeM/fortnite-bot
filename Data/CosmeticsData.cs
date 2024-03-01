using FortniteBot.Shop;

namespace FortniteBot.Data.Cosmetics
{
    public class CosmeticsData
    {
        public Data Data { get; set; }
    }

    public class Data
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Rarity Rarity { get; set; }
        public Type Type { get; set; }
        public DateTime[] ShopHistory { get; set; }
        public  Introduction Introduction { get; set; }
        public Images Images { get; set; }
    }

    public class Introduction
    {
        public string Text { get; set; }
    }

    public class Rarity
    {
        public string DisplayValue { get; set; }
    }

    public class Type
    {
        public string Value { get; set; }
    }

    public class Images
    {

        public string Featured { get; set; }

    }
}