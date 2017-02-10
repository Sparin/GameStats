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
        public Server Server { get; set; }

        [JsonIgnore]
        public DateTime Timestamp { get; set; }

        [JsonIgnore]
        public GameMode EFGameMode { get; set; }

        [JsonIgnore]
        public int MatchId { get; set; }        

        [JsonProperty(Order = -2)]
        public string Map { get; set; }
        public int FragLimit { get; set; }
        public int TimeLimit { get; set; }
        public double TimeElapsed { get; set; }
        public ICollection<Player> Scoreboard { get; set; }

        #region DB Ignored
        [JsonProperty(Order = -2)]
        public string GameMode { get { return EFGameMode.Name; } set { EFGameMode = new GameMode { Name = value }; } }
        #endregion
    }
}
