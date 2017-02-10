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
            modelBuilder.Entity<Server>()
                .HasOne(x => x.Info)
                .WithOne(x => x.Server)
                .HasForeignKey<ServerInfo>(x => x.Endpoint);
            modelBuilder.Entity<Server>()
                .HasKey(x => x.Endpoint);

            modelBuilder.Entity<GameMode>()
                .HasKey(x => x.Name);

            modelBuilder.Entity<Match>()
                .HasOne(x => x.Server)
                .WithMany(x => x.Matches);
            modelBuilder.Entity<Match>()
                .HasMany(x => x.Scoreboard)
                .WithOne(x => x.Match)
                .HasForeignKey(x => x.MatchId);

            modelBuilder.Entity<Player>()
                .HasOne(x => x.Match)
                .WithMany(x => x.Scoreboard)
                .HasForeignKey(x => x.MatchId);

            modelBuilder.Entity<ServerInfo>()
                .Ignore(x => x.GameModes)
                .HasKey(x => x.Endpoint);
            modelBuilder.Entity<ServerInfo>()
                .HasOne(x => x.Server)
                .WithOne(x => x.Info)
                .HasForeignKey<ServerInfo>(x => x.Endpoint);

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
