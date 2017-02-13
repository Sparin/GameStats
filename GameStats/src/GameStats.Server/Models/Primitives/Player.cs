using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStats.Server.Models
{
    public class Player
    {
        public string Name { get; set; }
        
        public PlayerStats Stats { get; set; }
    }
}
