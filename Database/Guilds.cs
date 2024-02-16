using FortniteBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FortniteBot.Database
{
    public class Guilds
    {
        private readonly string DEFAULT_LANGUAGE = "EN";
        private readonly GuildContext db = new();
        private readonly string GuildID;

        public Guilds(string guildID)
        {
            GuildID = guildID;

            var optionsBuilder = new DbContextOptionsBuilder<GuildContext>();
            optionsBuilder.UseSqlite("Data Source=guilds.db");

            using var context = new GuildContext();
            context.Database.EnsureCreated();
        }

        public void AddGuild(Guild guild) {
            db.Add(guild);
            db.SaveChanges();
        }

        public bool GuildExists()
        {
            var guild = db.Guilds.Find(GuildID);
            if (guild == null)
            {
                return false;
            }
            return true;
        }

        public string GetGuildLanguage()
        {
            var guild = db.Guilds.Find(GuildID);
            if (guild == null)
            {
                guild = new Guild { 
                    GuildID = GuildID,
                    Language = DEFAULT_LANGUAGE
                };
                AddGuild(guild);

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
