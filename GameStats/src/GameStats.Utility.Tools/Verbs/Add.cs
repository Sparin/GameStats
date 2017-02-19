using GameStats.Server.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace GameStats.Utility.Tools.Verbs
{
    public class Add
    {
        private static DatabaseContext context = new DatabaseContext(new DbContextOptions<DatabaseContext>());

        /// <summary>
        /// Adds random entity model to the database 
        /// </summary>
        /// <param name="opts">Add options parameters</param>
        internal static void AddEntityModel(AddOptions opts)
        {
            if (opts.Count > 10000)
            {
                Console.WriteLine("Too many elements trying to add. Maximum equal 10000");
                return;
            }

            switch (opts.Type.ToLower())
            {
                case "match":
                    if (opts.ServerIndex < 0)
                        AddRandomMatches(opts);
                    else
                        Console.WriteLine("-s --ServerIndex is required");
                    break;
                case "server":
                    AddRandomServers(opts);
                    break;
                default:
                    Console.WriteLine("Cannot recognize entity \"{0}\"", opts.Type);
                    break;
            }
        }

        /// <summary>
        /// Adds random matches to the database
        /// </summary>
        /// <param name="opts">Add options parameters</param>
        internal static void AddRandomMatches(AddOptions opts)
        {
            if (context.Servers.Count() <= opts.ServerIndex)
            {
                Console.WriteLine("ServerIndex cannot be greater or equal server count");
                return;
            }

            Random rnd = new Random();
            List<Server.Models.Server> servers = context.Servers.Select(x => x)
                    .Include(x => x.Info)
                    .ThenInclude(x => x.ServerInfoGameModes)
                    .ThenInclude(x => x.GameMode)
                    .ToList();
            context.Players.Select(x => x);
            List<string> playersServer = context.Players.Select(x => x.Name).ToList();

            object locker = new object();
            //List<Task> awaitableTask = new List<Task>();
            ConcurrentBag<string> keys = new ConcurrentBag<string>();
            foreach (string key in context.Matches.Select(x => string.Format("{0}-{1}", x.Endpoint, x.Timestamp)))
                keys.Add(key);

            //TODO: Fix the UNIQUE Constraint problem. Some Matches cannot check correctly keys and duplicate another match
            for (int i = opts.Count; i > 0; i--)
            //awaitableTask.Add(Task.Factory.StartNew(() =>
            {
                Match match = RandomModels.CreateRandomMatch(servers[opts.ServerIndex]);
                while (keys.Contains(string.Format("{0}-{1}", match.Endpoint, match.Timestamp)))
                    match = RandomModels.CreateRandomMatch(servers[opts.ServerIndex]);

                lock (locker)
                {
                    keys.Add(string.Format("{0}-{1}", match.Endpoint, match.Timestamp));
                    string[] names = match.Scoreboard.Select(x => x.Name).ToArray();
                    List<Player> players = names
                        .Except(playersServer)
                        .Select(x => new Player { Name = x }).ToList();

                    context.Matches.Add(match);
                    context.Players.AddRange(players);
                    playersServer.AddRange(names);
                    Console.WriteLine("Match #{0} with key {1}-{2} added", context.Matches.Local.Count, match.Endpoint, match.Timestamp);
                }

            }//));
            //Task.WaitAll(awaitableTask.ToArray());
            Console.Write("Trying to save changes to the database... ");
            context.SaveChanges();
            Console.WriteLine("saved");
        }

        /// <summary>
        /// Adds random servers to the database
        /// </summary>
        /// <param name="opts">Add options parameters</param>
        internal static void AddRandomServers(AddOptions opts)
        {
            object locker = new object();
            List<Task> awaitableTasks = new List<Task>();
            ConcurrentBag<string> keys = new ConcurrentBag<string>();
            foreach (string key in context.Servers.Select(x => x.Endpoint))
                keys.Add(key);

            for (int i = opts.Count; i > 0; i--)
                awaitableTasks.Add(Task.Factory.StartNew(() =>
                {
                    Server.Models.Server server = RandomModels.CreateRandomServer();
                    while (keys.Contains(server.Endpoint))
                        server = RandomModels.CreateRandomServer();
                    keys.Add(server.Endpoint);
                    lock (locker)
                    {
                        context.Add(server);
                        Console.WriteLine("Server #{0} with endpoint {1} added", context.Servers.Local.Count, server.Endpoint);
                    }
                }));

            Task.WaitAll(awaitableTasks.ToArray());
            Console.Write("Trying to save changes to the database... ");
            context.SaveChanges();
            Console.WriteLine("saved");
        }
    }
}
