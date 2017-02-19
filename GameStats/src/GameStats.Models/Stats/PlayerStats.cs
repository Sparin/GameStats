using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStats.Server.Models
{
    public class PlayerStats
    {
        [JsonIgnore]
        public Player Player { get; set; }
        [JsonIgnore]
        public string Name { get; set; }
        [JsonIgnore]
        public ICollection<ScoreboardItem> ScoreboardItems { get; set; }

        public int TotalMatchesPlayed
        {
            get
            {
                return ScoreboardItems.Count;
            }
        }
        public int TotalMatchesWon
        {
            get
            {
                return ScoreboardItems.Select(x => x.Match.Scoreboard)
                    .Where(x => ScoreboardItems.Contains(x.First()))
                    .Count();
            }
        }
        public string FavouriteServer
        {
            get
            {
                return ScoreboardItems.Select(x => x.Match)
                    .GroupBy(x => x.Server)
                    .OrderByDescending(x => x.Count())
                    .First().Key.Endpoint;
            }
        } //Server's Endpoint
        public int UniqueServers
        {
            get
            {
                return ScoreboardItems.Select(x => x.Match)
                    .GroupBy(x => x.Server)
                    .Count();
            }
        }
        public string FavouriteGameMode
        {
            get
            {
                return ScoreboardItems.Select(x => x.Match)
                    .GroupBy(x => x.GameMode)
                    .OrderByDescending(x => x.Count())
                    .First().Key;
            }
        }
        public double AverageScoreboardPercent
        {
            get
            {
                int count = ScoreboardItems.Select(x => x.Match).Count();
                int percentSumm = ScoreboardItems.Select(x => x.Match.Scoreboard)
                    .Sum(x => x.ToList().FindIndex(z => z.Player == Player) / (x.Count - 1) * 100);
                return percentSumm / count;
            }
        } 
        public int MaximumMatchesPerDay
        {
            get
            {
                return ScoreboardItems.Select(x => x.Match)
                    .GroupBy(x => x.Timestamp.Date)
                    .Select(x => x.Count())
                    .Max();
            }
        }
        public double AverageMatchesPerDay
        {
            get
            {
                return ScoreboardItems.Count() / ScoreboardItems.Select(x => x.Match)
                                                                        .GroupBy(x => x.Timestamp.Date)
                                                                        .Count();
            }
        }
        public DateTime LastMatchPlayed
        {
            get
            {
                return ScoreboardItems.Select(x => x.Match)
                    .OrderByDescending(x => x.Timestamp)
                    .First().Timestamp;
            }
        }
        public double KillToDeathRatio
        {
            get
            { return ScoreboardItems.Sum(x => x.Kills) / ScoreboardItems.Sum(x => x.Deaths); }
        }
    }
}
