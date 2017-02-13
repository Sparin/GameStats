using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GameStats.Server.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Server>().HasKey(x => x.Endpoint);
            modelBuilder.Entity<Server>()
                .HasMany(x => x.Matches)
                .WithOne(x => x.Server);
            modelBuilder.Entity<Server>()
                .HasOne(x => x.Stats)
                .WithOne(x => x.Server)
                .HasForeignKey<ServerStats>(x => x.Endpoint);
            modelBuilder.Entity<Server>()
                .HasOne(x => x.Info)
                .WithOne(x => x.Server)
                .HasForeignKey<ServerInfo>(x => x.Endpoint);

            modelBuilder.Entity<Match>().HasKey(x => new { x.Endpoint, x.Timestamp });
            modelBuilder.Entity<Match>()
                .HasOne(x => x.Server)
                .WithMany(x => x.Matches);
            modelBuilder.Entity<Match>()
                .HasOne(x => x.EFGameMode)
                .WithMany(x => x.Matches);
            modelBuilder.Entity<Match>()
                .HasMany(x => x.Scoreboard)
                .WithOne(x => x.Match);

            modelBuilder.Entity<ScoreboardItem>().HasKey(x => new { x.Endpoint, x.Timestamp, x.Name });
            //modelBuilder.Entity<ScoreboardItem>()
            //    .HasOne(x => x.Player)
            //    .WithMany(x => x.Stats.ScoreboardItems);
            modelBuilder.Entity<ScoreboardItem>()
                .HasOne(x => x.Match)
                .WithMany(x => x.Scoreboard);

            modelBuilder.Entity<ServerInfo>().HasKey(x => x.Endpoint);
            modelBuilder.Entity<ServerInfo>()
                .HasOne(x => x.Server)
                .WithOne(x => x.Info)
                .HasForeignKey<Server>(x => x.Endpoint);
            modelBuilder.Entity<ServerInfo>().Ignore(x => x.GameModes);

            modelBuilder.Entity<Player>().HasKey(x => x.Name);
            modelBuilder.Entity<Player>()
                .HasOne(x => x.Stats)
                .WithOne(x => x.Player)
                .HasForeignKey<PlayerStats>(x => x.Name);

            modelBuilder.Entity<GameMode>().HasKey(x => x.Name);

            modelBuilder.Entity<PlayerStats>().HasKey(x => x.Name);
            modelBuilder.Entity<PlayerStats>()
                .HasOne(x => x.Player)
                .WithOne(x => x.Stats)
                .HasForeignKey<Player>(x => x.Name);
            //modelBuilder.Entity<PlayerStats>()
            //    .HasMany(x => x.ScoreboardItems)
            //    .WithOne(x => x.Player.Stats);

            modelBuilder.Entity<ServerStats>().HasKey(x => x.Endpoint);
            modelBuilder.Entity<ServerStats>()
                .HasOne(x => x.Server)
                .WithOne(x => x.Stats)
                .HasForeignKey<Server>(x => x.Endpoint);

            modelBuilder.Entity<ServerInfoGameMode>()
                .HasKey(x => new { x.Name, x.Endpoint });
            modelBuilder.Entity<ServerInfoGameMode>()
                .HasOne(gm => gm.Info)
                .WithMany(s => s.ServerInfoGameModes)
                .HasForeignKey(gmr => gmr.Endpoint);
            modelBuilder.Entity<ServerInfoGameMode>()
                .HasOne(s => s.GameMode)
                .WithMany(gm => gm.Servers)
                .HasForeignKey(gmr => gmr.Name);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./GameStats.Storage.db");
        }

        public DbSet<GameMode> GameModes { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Server> Servers { get; set; }
    }
}
