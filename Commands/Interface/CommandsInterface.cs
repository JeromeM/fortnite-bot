namespace FortniteBot.Commands.Interface
{
    public interface ICommandsInterface
    {
        string ApiKey { get; }
        string URL { get; }

        public string Usage(FortniteResourceManager rm);
    }
}
