using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GameStats.Server.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GameStats.Server.Controllers
{
    [Route("[controller]")]
    public class ServersController : Controller
    {
        private readonly DatabaseContext context;

        public ServersController(DatabaseContext context)
        {
            this.context = context;
        }

        // GET: servers/<endpoint>/info
        [HttpGet("{endpoint}/info")]
        public IActionResult GetServerInfo(string endpoint)
        {
            Models.Server result = context.Servers.Include(x => x.Info)
                .ThenInclude(x => x.ServerInfoGameModes)
                .ThenInclude(x => x.GameMode)
                .Where(x => x.Endpoint == endpoint)
                .FirstOrDefault();
            if (result == null)
                return NotFound();
            else
                return new ObjectResult(result.Info);
        }

        // PUT: servers/<endpoint>/info
        [HttpPut("{endpoint}/info")]
        public IActionResult PutServerInfo(string endpoint, [FromBody]ServerInfo info)
        {
            if (info == null)
                return BadRequest();
            Models.Server server = context.Servers.Include(x => x.Info).ThenInclude(x => x.ServerInfoGameModes).Where(x => x.Endpoint == endpoint).FirstOrDefault();
            if (server != null)
            { context.Remove(server); context.SaveChanges(); }

            info.Endpoint = endpoint;
            List<ServerInfoGameMode> gameModes = new List<ServerInfoGameMode>();
            //Get existing gamemodes
            gameModes.AddRange(context.GameModes.Select(x => new ServerInfoGameMode
            {
                Info = info,
                Endpoint = endpoint,
                GameMode = x,
                Name = x.Name
            }).Where(x => info.GameModes.Contains(x.Name)));
            //Create new gamemodes;
            gameModes.AddRange(info.GameModes.Except(gameModes.Select(x => x.Name)).Select(x => new ServerInfoGameMode
            {
                Info = info,
                Endpoint = endpoint,
                GameMode = new GameMode { Name = x },
                Name = x
            }));

            context.Servers.Add(new Models.Server { Endpoint = endpoint, Info = info });
            context.SaveChanges();
            return Ok();
        }

        // GET: servers/<endpoint>/matches/<timestamp>
        [HttpGet("{endpoint}/matches/{timestamp}")]
        public IActionResult GetMatch(string endpoint, DateTime timestamp)
        {
            Match match = context.Matches.Include(x => x.Scoreboard).Include(x=>x.EFGameMode).Where(x => x.Timestamp == timestamp.ToUniversalTime() && x.Endpoint == endpoint).FirstOrDefault();
            if (match != null)
                return new ObjectResult(match);
            else
                return NotFound();
        }

        // PUT: servers/<endpoint>/matches/<timestamp>
        [HttpPut("{endpoint}/matches/{timestamp}")]
        public IActionResult PutMatch(string endpoint, DateTime timestamp, [FromBody]Match match)
        {
            if (match == null || context.Servers.Find(endpoint) == null)
                return BadRequest();
            if (context.Matches.Where(x => x.Timestamp == timestamp.ToUniversalTime() && x.Endpoint == endpoint).FirstOrDefault() != null)
                return StatusCode(409);

            match.Endpoint = endpoint;
            match.Timestamp = timestamp.ToUniversalTime();
            match.EFGameMode = context.GameModes.Find(match.GameMode) ?? new GameMode { Name = match.GameMode };

            context.Players.AddRange(match.Scoreboard.Select(x => new Player { Name = x.Name }).
                            Where(x => context.Players.Where(z => z.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() == null));
            context.Matches.Add(match);
            context.SaveChanges();

            return Ok();
        }

        // GET: servers/info
        [HttpGet("info")]
        public IActionResult GetServers()
        {
            return new ObjectResult(context.ServerInfo.Select(x=>x)
                .Include(x => x.ServerInfoGameModes)
                .ThenInclude(x => x.GameMode));
        }

        // GET: servers/<endpoint>/stats
        [HttpGet("{endpoint}/stats")]
        public IActionResult GetServerStats(string endpoint)
        {
            Models.Server server = context.Servers
                .Include(x => x.Matches).ThenInclude(x => x.Scoreboard)
                .Include(x => x.Matches).ThenInclude(x => x.EFGameMode)
                .Where(x => x.Endpoint == endpoint)
                .FirstOrDefault();

            if (server == null)
                return NotFound();
            else
            {
                server.Stats = new ServerStats { Server = server };
                return new ObjectResult(server.Stats);
            }
        }
    }
}
