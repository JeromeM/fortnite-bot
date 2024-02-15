using Microsoft.Extensions.Configuration;

namespace FortniteBot.Helpers
{
    public static class ConfigurationHelper
    {
        public static string GetByName(string configKeyName)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            IConfigurationSection section = config.GetSection(configKeyName);

            if (section.Value == null)
            {
                return "";
            }

            return section.Value;
        }
    }
}