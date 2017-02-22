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
        DatabaseContext context;

        public ReportsController()
        {
            context = new DatabaseContext();
        }

        // GET: reports/recent-matches/<count>
        [HttpGet("recent-matches")]
        [HttpGet("recent-matches/{count}")]
        public IActionResult GetRecentMatches(int count = 50)
        {
                Match[] matches = context.Matches.Include(x => x.Scoreboard).OrderByDescending(x => x.Timestamp).Take(count).ToArray();
                return new ObjectResult(matches);
        }

        // GET: reports/best-players/<count>
        [HttpGet("best-players")]
        [HttpGet("best-players/{count}")]
        public IActionResult GetBestPlayers(int count = 50)
        {
            var items = context.ScoreboardItem.GroupBy(x => x.Name.ToLower())
                .Select(x => new
                {
                    Name = x.Key,
                    Items = x.Select(z => z),
                    Count = x.Count()
                })
                .Where(x => x.Count >= 10);

            var result = items.Select(x => new
            {
                Name = x.Name,
                KillToDeathRatio = (double)x.Items.Sum(z => z.Kills) / x.Items.Sum(z => z.Deaths)
            })
            .OrderByDescending(x => x.KillToDeathRatio)
            .Take(count);

            return new ObjectResult(result);
        }

        // GET: reports/popular-servers/<count>
        [HttpGet("popular-servers")]
        [HttpGet("popular-servers/{count}")]
        public IActionResult GetPopularServers(int count = 50)
        {
            var items = context.Servers.Include(x => x.Matches)
                .Include(x => x.Info)
                .Select(x => new
                {
                    Endpoint = x.Endpoint,
                    Name = x.Info.Name,
                    AverageMatchesPerDay = (double)x.Matches.Count/ x.Matches.GroupBy(z => z.Timestamp.Date).Count()
                })
                .OrderByDescending(x=>x.AverageMatchesPerDay);

            return new ObjectResult(items);
        }
    }
}
