using Discord.Commands;
using Discord.WebSocket;
using FortniteBot.Database;

namespace FortniteBot.Commands
{
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        private string[] AvailableLanguages = ["FR", "EN"];

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
