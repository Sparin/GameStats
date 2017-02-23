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
        private string connectionString { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            if (!Database.EnsureCreated())
                Database.Migrate();
        }
        public DatabaseContext(string connectionString) : base()
        {
            this.connectionString = connectionString;
            if (!Database.EnsureCreated())
                Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Primary keys
            modelBuilder.Entity<Server>().HasKey(x => x.Endpoint);
            modelBuilder.Entity<Match>().HasKey(x => new { x.Endpoint, x.Timestamp });
            modelBuilder.Entity<ScoreboardItem>().HasKey(x => new { x.Endpoint, x.Timestamp, x.Name });
            modelBuilder.Entity<ServerInfo>().HasKey(x => x.Endpoint);
            modelBuilder.Entity<GameMode>().HasKey(x => x.Name);
            modelBuilder.Entity<Player>().HasKey(x => x.Name);
            #endregion
            #region Relationships
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

            modelBuilder.Entity<Match>()
                .HasOne(x => x.EFGameMode)
                .WithMany(x => x.Matches)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Match>()
                .HasMany(x => x.Scoreboard)
                .WithOne(x => x.Match)
                .HasForeignKey(x => new { x.Endpoint, x.Timestamp })
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Player>()
                .HasMany(x => x.ScoreboardItems);

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
            #endregion
            #region Ignores
            modelBuilder.Entity<ServerInfo>().Ignore(x => x.GameModes);
            modelBuilder.Entity<Match>().Ignore(x => x.GameMode);
            modelBuilder.Entity<Player>().Ignore(x => x.Stats);
            modelBuilder.Entity<Server>().Ignore(x => x.Stats);
            #endregion
            #region Types of properties
            modelBuilder.Entity<Match>().Property(x => x.Timestamp).ForSqliteHasColumnType("NUMERIC");
            modelBuilder.Entity<ScoreboardItem>().Property(x => x.Timestamp).ForSqliteHasColumnType("NUMERIC");
            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (connectionString != null)
                optionsBuilder.UseSqlite(connectionString);
        }

        public DbSet<GameMode> GameModes { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<ScoreboardItem> ScoreboardItem { get; set; }
    }
}
