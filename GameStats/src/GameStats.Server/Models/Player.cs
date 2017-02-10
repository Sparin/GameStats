using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStats.Server.Models
{
    public class Player
    {
        [JsonIgnore]
        public Match Match { get; set; }

        [JsonIgnore]
        public int MatchId { get; set; }

        [JsonIgnore]
        public int PlayerId { get; set; }

        public string Name { get; set; }
        public int Frags { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
    }
}
