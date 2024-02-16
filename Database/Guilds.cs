using FortniteBot.Models;

namespace FortniteBot.Database
{
    public class Guilds(string guildID)
    {
        private readonly string DEFAULT_LANGUAGE = "EN";
        private readonly GuildContext db = new();
        private readonly string GuildID = guildID;

        public void AddGuild(Guild guild) {
            db.Add(guild);
            db.SaveChanges();
        }

        public string GetGuildLanguage()
        {
            var guild = db.Guilds.Find(GuildID);
            if (guild == null)
            {
                return DEFAULT_LANGUAGE;
            }
            return guild.Language;
        }

        public void UpdateGuild(string language)
        {

            var guild = db.Guilds.Find(GuildID);
            if (guild == null) { return; }

            guild.Language = language;
            db.SaveChanges();
        }
    }
}
