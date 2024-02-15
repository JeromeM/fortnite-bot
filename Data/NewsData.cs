namespace FortniteBot.News
{
    public class NewsData
    {
        public Data Data { get; set; }
    }

    public class Data
    {
        public BattleRoyale Br { get; set; }
        public SaveTheWorld Stw { get; set; }
    }

    public class BattleRoyale
    {
        public DateTime Date { get; set; }
        public string Image { get; set; }
        public List<Motd> Motds { get; set; }
        public List<Message> Messages { get; set; }
    }

    public class SaveTheWorld
    {
        public DateTime Date { get; set; }
        public string Image { get; set; }
        public List<Motd> Motds { get; set; }
        public List<Message> Messages { get; set; }
    }

    public class Motd
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Image { get; set; }
        public int SortingPriority { get; set; }
        public bool Hidden { get; set; }
    }

    public class Message
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Image { get; set; }
    }
}