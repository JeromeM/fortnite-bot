using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FortniteBot.Models
{
    public class GuildContext : DbContext
    {
        public DbSet<Guild> Guilds { get; set; }
    
        public string DbPath { get; }

        public GuildContext()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            DbPath = Path.Join(path, "guilds.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }

    public class Guild
    {
        public string GuildID { get; set; }
        public string Language { get; set; }
    }

}