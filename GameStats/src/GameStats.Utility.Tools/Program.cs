using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStats.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStats.Utility.Tools
{
    public class Program
    {
        private static DatabaseContext context = new DatabaseContext(new Microsoft.EntityFrameworkCore.DbContextOptions<DatabaseContext>());

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<AddOptions>(args)
                .WithParsed<AddOptions>(opts => AddEntityModel(opts));
        }

        private static void AddEntityModel(AddOptions opts)
        {
            switch (opts.Type.ToLower())
            {
                case "match":
                    int serversCount = context.Servers.Count();
                    if (serversCount == 0)
                    {
                        Console.WriteLine("Servers table is empty. Create random server? (y/n)");
                        if (Console.ReadLine() == "y")
                        {
                            context.Servers.Add(RandomModels.CreateRandomServer());
                            context.SaveChanges();
                            serversCount++;
                        }
                        else
                            Console.WriteLine("Operation was canceled, because Servers table is empty");
                    }

                    Random rnd = new Random();
                    List<Server.Models.Server> servers = context.Servers.Select(x => x)
                            .Include(x => x.Info)
                            .ThenInclude(x => x.ServerInfoGameModes)
                            .ThenInclude(x => x.GameMode)
                            .ToList();

                    throw new NotImplementedException();
                    //TODO: Generate a large number of Matches with unique primary keys

                    //object locker = new object();
                    //List<Task> awaitableTasks = new List<Task>();
                    //List<string> keys = context.Matches.Select(x =>  string.Format("{0}{1}", x.Endpoint, x.Timestamp.ToString())).ToList();
                    //while (opts.Count > 0 && serversCount != 0)
                    //{
                    //    awaitableTasks.Add(Task.Factory.StartNew(() =>
                    //    {
                    //        Match match = RandomModels.CreateRandomMatch(servers[rnd.Next(serversCount - 1)]);

                    //            context.Add(match);
                    //            context.Players.AddRange(match.Scoreboard.Select(x => new Player { Name = x.Name })
                    //                .Where(x => context.Players.Find(x.Name) == null));
                    //            opts.Count--;
                    //            Console.WriteLine("Match added to the database.\t{0} left", opts.Count);
                    //        };
                    //    }));

                    //}
                    //Task.WaitAll(awaitableTasks.ToArray());
                    //context.SaveChanges();

                    //List<Task> awaitableTasks = new List<Task>();
                    //List<Match> generatedMatches = new List<Match>();
                    //object e = new object();
                    //while (opts.Count > 0 && serversCount != 0)
                    //{
                    //    awaitableTasks.Add(Task.Factory.StartNew(() =>
                    //    {
                    //        Match match = RandomModels.CreateRandomMatch(servers[rnd.Next(serversCount - 1)]);
                    //        lock (e)
                    //        {
                    //            generatedMatches.Add(match);
                    //            opts.Count--;
                    //            //Console.WriteLine("Match added to the database.\t{0} left", opts.Count);
                    //        }
                    //    }));
                    //}
                    //Task.WaitAll(awaitableTasks.ToArray());
                    break;
                case "server":
                    while (opts.Count > 0)
                    {
                        //TODO: Generate a large number of Servers with unique primary keys
                        context.Add(RandomModels.CreateRandomServer());
                        context.SaveChanges();
                        opts.Count--;
                    }
                    break;
                default:
                    Console.WriteLine("Cannot recognize entity \"{0}\"", opts.Type);
                    break;
            }
        }
    }
}
