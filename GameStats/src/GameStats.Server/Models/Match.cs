using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStats.Server.Models
{
    public class Match
    {
        [JsonIgnore]
        public DateTime Timestamp { internal get; set; }
        [JsonIgnore]
        public string Endpoint { internal get; set; }

        public string Map { get; set; }
        public GameMode GameMode { get; set; }
        public int FragLimit { get; set; }
        public int TimeLimit { get; set; }
        public double TimeElapsed { get; set; }
        public ICollection<Player> Scoreboard { get; set; }
    }
}
