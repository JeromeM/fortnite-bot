using Discord.Commands;

namespace FortniteBot
{
    public class StatsModule : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [Summary("Echoes a message.")]
        public async Task SayAsync([Remainder][Summary("The text to echo")] string echo)
        {
            Console.WriteLine("TOTO");
            await ReplyAsync(echo);
        }
    }
}