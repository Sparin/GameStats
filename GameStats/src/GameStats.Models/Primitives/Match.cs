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
        public string Endpoint { get; set; }

        [JsonIgnore]
        public DateTime Timestamp { get; set; }

        [JsonIgnore]
        public GameMode EFGameMode { get; set; }

        [JsonProperty(Order = -2, Required = Required.Always)]
        public string Map { get; set; }
        [JsonProperty(Required = Required.Always)]
        public int FragLimit { get; set; }
        [JsonProperty(Required = Required.Always)]
        public int TimeLimit { get; set; }
        [JsonProperty(Required = Required.Always)]
        public double TimeElapsed { get; set; }
        [JsonProperty(Required = Required.Always)]
        public ICollection<ScoreboardItem> Scoreboard { get; set; }

        #region DB Ignored
        [JsonProperty(Order = -2, Required = Required.Always)]
        public string GameMode { get { return EFGameMode.Name; } set { EFGameMode = new GameMode { Name = value }; } }
        #endregion
    }
}
