using Discord.Commands;
using Discord.WebSocket;
using FortniteBot.Database;
using System.Reflection;
using FortniteBot.Helpers;
using Serilog;

namespace FortniteBot.Commands
{
    [Group("botnite")]
    public class MainModule : ModuleBase<SocketCommandContext>
    {
        private readonly string[] AvailableLanguages = ["FR", "EN"];

        [Command("help")]
        [Summary("Show BOT usage")]
        public async Task HelpCommand([Summary("More detailed help on a specific command")] string command = "")
        {
            var rm = new FortniteResourceManager(Context.Guild.Id.ToString());

            if (command != "")
            {
                object ret = "";

                Assembly assembly = Assembly.GetExecutingAssembly();
                var className = $"FortniteBot.Commands.{command.Capitalize()}Module";
                if (assembly != null)
                {
                    Type type = assembly.GetType(className) ?? throw new InvalidOperationException("Empty Assembly");

                    if (type != null)
                    {
                        object? instance = type != null ? Activator.CreateInstance(type) : null;
                        MethodInfo? method = type != null ? type.GetMethod("Usage") : null;
                        if (method != null)
                        {
                            ret = method.Invoke(instance, [rm]) ?? throw new Exception($"Can't invoke Usage method of class {className}");
                        }
                        else
                        {
                            Log.Error("La méthode n'a pas été trouvée !");
                        }
                    }
                    else
                    {
                        Log.Error("La classe n'a pas été trouvée !");
                    }
                    await ReplyAsync((string)ret);
                } else
                {
                    return;
                }
            }
            else
            {
                await ReplyAsync(
                    $"# List of commands\n" +
                    $"## _System_\n" +
                    $"- !botnite lang **lang** : Change the Bot language.\n" +
                    $"- !botnite help _command_ : This help. You can also pass any command in parameter to have more help.\n" +
                    $"## _Tools_\n" +
                    $"- !news : Show Fortnite news (Battle Royale & Save the World).\n" +
                    $"- !stats **accountName** _params_ : Show account stats.\n" +
                    $"## _Shop_\n" +
                    $"- !shop item : Show a random item available in the shop.\n" +
                    $"- !shop bundle : Show a random bundle from the shop.\n"
                );
            }
        }

        [Command("lang")]
        [Summary("Change language")]
        public async Task LangCommand([Summary("Sets the language of the bot")] string language = "")
        {
            var rm = new FortniteResourceManager(Context.Guild.Id.ToString());

            if (Context.User is SocketGuildUser user)
            {
                if (user.GuildPermissions.Administrator)
                {
                    foreach (string lang in AvailableLanguages)
                    {
                        if (lang.Equals(language, StringComparison.CurrentCultureIgnoreCase))
                        {
                            var db = new Guilds(Context.Guild.Id.ToString());
                            if (db.GuildExists())
                            {
                                db.UpdateGuild(lang);
                                rm.Reload();
                            }
                            else
                            {
                                db.AddGuild(new Models.Guild
                                {
                                    GuildID = Context.Guild.Id.ToString(),
                                    Language = lang,
                                });
                            }
                            await ReplyAsync($"{rm.GV("changelang1")} {lang}. {rm.GV("changelang2")}");
                            return;
                        }
                    }
                    await ReplyAsync($"{rm.GV("language")} {language} {rm.GV("langnotvalid")} {string.Join(", ", AvailableLanguages)}");
                }
                else
                {
                    await ReplyAsync($"{rm.GV("admincommand")}");
                }
            }
        }
    }
}
