using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStats.Server.Models
{
    public class Server
    {   
        [JsonIgnore]
        public ICollection<Match> Matches { get; set; }
        [JsonIgnore]
        public ServerStats Stats { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Endpoint { get; set; }
        [JsonProperty(Required = Required.Always)]
        public ServerInfo Info { get; set; }
    }
}
