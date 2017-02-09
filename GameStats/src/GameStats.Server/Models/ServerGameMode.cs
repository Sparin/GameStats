using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStats.Server.Models
{
    public class ServerGameMode
    {
        public GameMode GameMode { get; set; }
        public string GameModeId { get; set; }

        public Server Server { get; set; }
        public string Endpoint { get; set; }
    }
}
