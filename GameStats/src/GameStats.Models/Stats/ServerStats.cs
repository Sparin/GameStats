using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStats.Server.Models
{
    public class ServerStats
    {
        [JsonIgnore]
        public Server Server { get; set; }
        [JsonIgnore]
        public string Endpoint { get; set; }

        public int TotalMatchesPlayed
        {
            get
            {
                return Server.Matches.Count;
            }
        }
        public int MaximumMatchesPerDay
        {
            get
            {
                return Server.Matches.GroupBy(x => x.Timestamp.Date)
                    .Select(x => x.Count())
                    .Max();
            }
        }
        public double AverageMatchesPerDay
        {
            get
            {
                return (double)TotalMatchesPlayed / Server.Matches.GroupBy(x => x.Timestamp.Date).Count();
            }
        }
        public int MaximumPopulation
        {
            get
            {
                return Server.Matches.Select(x => x.Scoreboard.Count).Max();
            }
        }
        public double AveragePopulation
        {
            get
            {                
                return (double)Server.Matches.Sum(x=>x.Scoreboard.Count) / TotalMatchesPlayed;
            }
        }
        public ICollection<string> Top5GameModes
        {
            get
            {
                return Server.Matches.GroupBy(x => x.GameMode)
                    .Select(x => new { GameMode = x.Key, Count = x.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .Select(x => x.GameMode)
                    .ToList();
            }
        }
        public ICollection<string> Top5Maps
        {
            get
            {
                return Server.Matches.GroupBy(x => x.Map)
                    .Select(x => new { Map = x.Key, Count = x.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .Select(x => x.Map)
                    .ToList();
            }
        }
    }
}
