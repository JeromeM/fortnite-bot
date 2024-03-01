namespace FortniteBot.Data.Code
{
    public class CodeData
    {
        public Data Data { get; set; }
    }
    public class Data
    {
        public string Code { get; set; }
        public Account Account  { get; set; }
        public string Statues { get; set; }
    }
    public class Account
    {
        public string Name { get; set; }
    }
}
