using GameStats.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStats.Utility.Tools
{
    //TODO: Code Review
    //TODO: Add more models
    public static class RandomModels
    {
        private static Random randomizer = new Random();
        private static string[] _gameModes = { "DM", "TDM", "FFA", "CTF", "DE", "CS", "QUS", "SAS", "FBL", "QTF" };


        /// <summary>
        /// Creates random server
        /// </summary>        
        public static Server.Models.Server CreateRandomServer()
        {
            string endpoint;
            if (randomizer.Next(0, 1) == 0)
                endpoint = String.Format("{0}.{1}.{2}.{3}-{4}", randomizer.Next(0, 255), randomizer.Next(0, 255), randomizer.Next(0, 255), randomizer.Next(0, 255), randomizer.Next(0, 65535));
            else
                endpoint = String.Format("mockServer{0}-{1}", randomizer.Next(0, 10000), randomizer.Next(0, 65535));

            List<string> gameModes = new List<string>();
            int gameModesCount = randomizer.Next(1, 10);
            for (int i = 0; i < gameModesCount; i = gameModes.Count)
            {
                int index = randomizer.Next(0, 9);
                if (!gameModes.Contains(_gameModes[index]))
                    gameModes.Add(_gameModes[index]);
            }

            Server.Models.Server result = new Server.Models.Server
            {
                Endpoint = endpoint,
                Info = new ServerInfo
                {
                    Endpoint = endpoint,
                    Name = "Mock Server",
                    GameModes = gameModes
                }
            };

            return result;
        }

        /// <summary>
        /// Creates random match for server 
        /// </summary>
        /// <param name="server">Server is required for relationships of entities and for gamemodes conformity</param>
        public static Match CreateRandomMatch(Server.Models.Server server)
        {
            Match result = new Match
            {
                Endpoint = server.Endpoint,
                FragLimit = randomizer.Next(100),
                TimeLimit = randomizer.Next(100),
                TimeElapsed = randomizer.NextDouble(),
                Map = String.Format("MockMap-{0}", randomizer.Next(50)),
                GameMode = server.Info.GameModes.ToArray()[randomizer.Next(server.Info.GameModes.Count - 1)],
                Timestamp = DateTime.Now.AddSeconds(randomizer.Next(-604800, 604800)),
            };
            result.Scoreboard = CreateRandomScoreboard(result);
            return result;
        }

        /// <summary>
        /// Creates random scoreboard's item for match
        /// </summary>
        /// <param name="match">Match is required for FragLimit conformity</param>
        public static ScoreboardItem CreateRandomScoreboardItem(Match match)
        {
            return new ScoreboardItem
            {
                Name = String.Format("mock player {0}", randomizer.Next(1000000)),
                Frags = randomizer.Next(match.FragLimit),
                Kills = randomizer.Next(match.FragLimit),
                Deaths = randomizer.Next(match.FragLimit)
            };
        }

        /// <summary>
        /// Creates random scoreboard for match
        /// </summary>
        /// <param name="match">Match required for FragLimit conformity</param>
        public static ICollection<ScoreboardItem> CreateRandomScoreboard(Match match)
        {
            int count = randomizer.Next(50);
            List<ScoreboardItem> items = new List<ScoreboardItem>();
            for (; count > 0; count--)
            {
                ScoreboardItem item = CreateRandomScoreboardItem(match);
                bool toAdd = true;
                foreach (ScoreboardItem checkItem in items)
                    if (checkItem.Name == item.Name)
                        toAdd = false;
                if (toAdd)
                    items.Add(item);
                else
                    count++;
            }
            return items.ToArray();
        }
    }
}
