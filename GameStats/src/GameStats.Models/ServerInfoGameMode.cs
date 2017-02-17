using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStats.Server.Models
{
    //Hasn't DBSet
    public class ServerInfoGameMode
    {
        public GameMode GameMode { get; set; }
        public string Name { get; set; }

        public ServerInfo Info { get; set; }
        public string Endpoint { get; set; }
    }
}
