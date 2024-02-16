using FortniteBot.Database;
using System.Resources;

namespace FortniteBot
{
    public class FortniteResourceManager(string guildID)
    {
        private readonly string DEFAULT_LANGUAGE = "EN";
        private readonly string GuildID = guildID;

        public string GV(string key)
        {
            var db = new Guilds(GuildID);

            ResourceManager rm = new($"FortniteBot.Resources.{DEFAULT_LANGUAGE}", typeof(FortniteBot).Assembly);
            var localizedString = rm.GetString(key);
            if (localizedString != null)
            {
                return localizedString;
            }
            return "";
        }
    }
}
