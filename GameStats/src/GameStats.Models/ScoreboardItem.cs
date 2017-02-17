using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStats.Server.Models
{
    public class ScoreboardItem
    {        
        [JsonIgnore]
        public Match Match { get; set; }
        [JsonIgnore]
        public DateTime Timestamp { get; set; }
        [JsonIgnore]
        public string Endpoint { get; set; }
        [JsonIgnore]
        public Player Player { get; set; }

        public string Name { get; set; }
        public int Frags { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
    }
}
