using FortniteBot.Database;
using System.Resources;

namespace FortniteBot
{
    public class FortniteResourceManager
    {
        private readonly string DEFAULT_LANGUAGE = "EN";
        private readonly string GuildID;
        public string Language { get; set; }

        public FortniteResourceManager(string guildID)
        {
            GuildID  = guildID;
            Reload();
        }

        public void Reload()
        {
            var db = new Guilds(GuildID);
            Language = db.GetGuildLanguage().ToUpper();

            if (!File.Exists(Path.Combine("Resources", $"{Language}.resx")))
            {
                Language = DEFAULT_LANGUAGE;
            }
        }

        public string GV(string key)
        {
            ResourceManager rm = new($"FortniteBot.Resources.{Language}", typeof(FortniteBot).Assembly);
            var localizedString = rm.GetString(key);
            if (localizedString != null)
            {
                return localizedString;
            }
            return "";
        }
    }
}
