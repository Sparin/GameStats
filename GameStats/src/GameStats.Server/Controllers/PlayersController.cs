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
    [Route("[controller]")]
    public class PlayersController : Controller
    {
        // GET: api/values
        [HttpGet("{name}/stats")]
        public IActionResult GetPlayersStats(string name)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                Player player = context.Players.Include(x => x.ScoreboardItems)
                    .ThenInclude(x => x.Match)
                    .ThenInclude(x => x.Server)
                    .Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();
                if (player == null)
                    return NotFound();
                player.ScoreboardItems = context.ScoreboardItem.Include(x=>x.Match)
                    .Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();
                player.Stats = new PlayerStats { ScoreboardItems = player.ScoreboardItems, Player = player };

                return new ObjectResult(player.Stats);                
            }
        }
    }
}
