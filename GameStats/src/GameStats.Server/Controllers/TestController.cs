using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GameStats.Server.Models;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GameStats.Server.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        DatabaseContext context;

        public TestController()
        {
            context = new DatabaseContext(new Microsoft.EntityFrameworkCore.DbContextOptions<DatabaseContext>());
        }

        // GET: api/values
        [HttpGet("info")]
        public IActionResult Get()
        {
            return new ObjectResult(context.Servers.Include(x => x.Info)
                .ThenInclude(x => x.ServerInfoGameModes)
                .ThenInclude(x => x.GameMode)
                .Single(x => x.Endpoint == "8.8.8.8").Info);
        }

        [HttpGet("serverstats")]
        public IActionResult GetServerStats()
        {
            Models.Server server = context.Servers.Include(x => x.Matches)
                .ThenInclude(x => x.Scoreboard)
                .Single(x => x.Endpoint == "8.8.8.8");
            server.Stats = new ServerStats() { Server = server, Endpoint="8.8.8.8" };
            ServerStats stats = server.Stats;
            return new ObjectResult(stats);
        }

        [HttpGet("fill")]
        public IActionResult Fill()
        {
            ScoreboardItem[] items = new ScoreboardItem[]
            {
                new ScoreboardItem {Name="Sparin", Kills=20, Deaths=1, Frags=21 },
                new ScoreboardItem {Name="Bot", Kills=2, Deaths=18, Frags=2 },
                new ScoreboardItem {Name="Kenso", Kills=1, Deaths=6, Frags=7 }
            };
            Match[] matches = new Match[]
            {
                new Match {Map="DM-HelloWorld", FragLimit=20, Endpoint="8.8.8.8", GameMode="DM",
                    TimeLimit =40, Timestamp=DateTime.Now, Scoreboard= items,TimeElapsed= 23.56 }
            };

            Models.Server server = new Models.Server
            {
                Info = new ServerInfo { Name = "MockServer", Endpoint = "8.8.8.8", GameModes = new string[] { "DM", "TDM" } },
                //Matches = matches,
                Endpoint = "8.8.8.8"
            };

            context.Servers.Add(server);
            context.SaveChanges();
            context.Matches.AddRange(matches);
            context.SaveChanges();

            return new ObjectResult("OK");
        }
    }
}
