using System.Resources;

namespace FortniteBot
{
    public class FortniteResourceManager
    {
        private static readonly string DEFAULT_LANGUAGE = "en";

        public static string GV(string key)
        {
            ResourceManager rm = new ResourceManager(DEFAULT_LANGUAGE, typeof(FortniteBot).Assembly);
            var localizedString = rm.GetString(key);
            if (localizedString != null)
            {
                return localizedString;
            }
            return "";
        }
    }
}
