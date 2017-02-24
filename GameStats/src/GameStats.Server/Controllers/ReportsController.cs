using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GameStats.Server.Models;
using Microsoft.EntityFrameworkCore;

//TODO: Do some stress-test on response time
//TODO: Write a LocalCounters for this reports
namespace GameStats.Server.Controllers
{
    [Route("[controller]")]
    public class ReportsController : Controller
    {
        private readonly DatabaseContext context;

        public ReportsController(DatabaseContext context)
        {
            this.context = context;
        }

        // GET: reports/recent-matches/<count>
        [HttpGet("recent-matches")]
        [HttpGet("recent-matches/{count}")]
        public IActionResult GetRecentMatches(int count = 5)
        {
            if (count < 0) count = 0;
            if (count > 50) count = 50;
            return new ObjectResult(context.Matches.Include(x => x.Scoreboard)
                .Include(x => x.EFGameMode)
                .OrderByDescending(x => x.Timestamp)
                .Take(count));
        }

        // GET: reports/best-players/<count>
        [HttpGet("best-players")]
        [HttpGet("best-players/{count}")]
        public IActionResult GetBestPlayers(int count = 5)
        {
            if (count < 0) count = 0;
            if (count > 50) count = 50;
            var items = context.ScoreboardItem.GroupBy(x => x.Name.ToLower())
                .Where(x => x.Count() >= 10 && x.Sum(y => y.Deaths) != 0);

            var result = items.Select(x => new
            {
                Name = x.Key,
                KillToDeathRatio = (double)x.Sum(z => z.Kills) / x.Sum(z => z.Deaths)
            })
            .OrderByDescending(x => x.KillToDeathRatio)
            .Take(count);

            return new ObjectResult(result);
        }
        
        // GET: reports/popular-servers/<count>
        [HttpGet("popular-servers")]
        [HttpGet("popular-servers/{count}")]
        public IActionResult GetPopularServers(int count = 5)
        {
            if (count < 0) count = 0;
            if (count > 50) count = 50;
            var items = context.Servers.Include(x => x.Matches)
                .Include(x => x.Info)
                .Select(x => new
                {
                    Endpoint = x.Endpoint,
                    Name = x.Info.Name,
                    AverageMatchesPerDay = (double)x.Matches.Count / x.Matches.GroupBy(z => z.Timestamp.Date).Count()
                })
                .OrderByDescending(x => x.AverageMatchesPerDay);

            return new ObjectResult(items);
        }
    }
}
