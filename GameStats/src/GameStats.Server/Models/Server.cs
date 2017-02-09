using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStats.Server.Models
{
    public class Server
    {
        public string Name { get; set; }
        public ICollection<string> GameModes { get { return ServerGameModes.Select(x => x.GameMode.Name).ToList(); } }

        [JsonIgnore]
        public ICollection<ServerGameMode> ServerGameModes { get; set; }
        [JsonIgnore]
        public string Endpoint { get; set; }
    }
}
