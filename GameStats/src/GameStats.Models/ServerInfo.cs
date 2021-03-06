﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStats.Server.Models
{
    public class ServerInfo
    {
        [JsonIgnore]
        public ICollection<ServerInfoGameMode> ServerInfoGameModes { get; set; }

        [JsonIgnore]
        public Server Server { get; set; }
        
        [JsonIgnore]
        public string Endpoint { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        #region DB Ignored
        [JsonProperty(Required = Required.Always)]
        public ICollection<string> GameModes
        {
            get
            {
                if (ServerInfoGameModes == null)
                    return null;
                return ServerInfoGameModes.Select(x => x.GameMode.Name).ToList();
            }
            set
            {
                ServerInfoGameModes = value.Select(x => new ServerInfoGameMode { GameMode = new GameMode { Name = x } }).ToList();
            }
        }
        #endregion
    }
}
