using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GameStats.Server.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base() { }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Server>().HasKey(x => x.Endpoint);
            modelBuilder.Entity<Server>()
                .HasMany(x => x.Matches)
                .WithOne(x => x.Server)
                .HasForeignKey(x => x.Endpoint)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Server>()
                .HasOne(x => x.Info)
                .WithOne(x => x.Server)
                .HasForeignKey<ServerInfo>(x => x.Endpoint)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Match>().HasKey(x => new { x.Endpoint, x.Timestamp });
            modelBuilder.Entity<Match>()
                .HasOne(x => x.EFGameMode)
                .WithMany(x => x.Matches)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Match>()
                .HasMany(x => x.Scoreboard)
                .WithOne(x => x.Match)
                .HasForeignKey(x => new { x.Endpoint, x.Timestamp })
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ScoreboardItem>().HasKey(x => new { x.Endpoint, x.Timestamp, x.Name });

            modelBuilder.Entity<ServerInfo>().HasKey(x => x.Endpoint);
            modelBuilder.Entity<ServerInfo>().Ignore(x => x.GameModes);

            modelBuilder.Entity<Player>().HasKey(x => x.Name);
            modelBuilder.Entity<Player>()
                .HasMany(x => x.ScoreboardItems);

            modelBuilder.Entity<GameMode>().HasKey(x => x.Name);

            modelBuilder.Entity<ServerInfoGameMode>()
                .HasKey(x => new { x.Name, x.Endpoint });
            modelBuilder.Entity<ServerInfoGameMode>()
                .HasOne(gm => gm.Info)
                .WithMany(s => s.ServerInfoGameModes)
                .HasForeignKey(gmr => gmr.Endpoint);
            modelBuilder.Entity<ServerInfoGameMode>()
                .HasOne(s => s.GameMode)
                .WithMany(gm => gm.Servers)
                .HasForeignKey(gmr => gmr.Name)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Player>().Ignore(x => x.Stats);
            modelBuilder.Entity<Server>().Ignore(x => x.Stats);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Filename=./GameStats.Storage.db");
        }

        public DbSet<GameMode> GameModes { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<ScoreboardItem> ScoreboardItem { get; set; }
    }
}
