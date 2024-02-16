using Microsoft.EntityFrameworkCore;

namespace FortniteBot.Models
{
    public class GuildContext : DbContext
    {
        public DbSet<Guild> Guilds { get; set; }
    
        public string DbPath { get; }

        public GuildContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "guilds.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }

    public class Guild
    {
        public int GuildID { get; set; }
        public string Language { get; set; }
    }

}