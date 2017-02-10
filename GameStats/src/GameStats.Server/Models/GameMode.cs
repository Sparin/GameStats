using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStats.Server.Models
{
    public class GameMode
    {
        [JsonIgnore]
        public ICollection<ServerInfoGameMode> Servers { get; set; }
        [JsonIgnore]
        public ICollection<Match> Matches { get; set; }

        [JsonProperty(PropertyName = "gameMode")]
        public string Name { get; set; }        
    }
}
