using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using CommandLine;

namespace GameStats.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Options options = new Options();
            var result = Parser.Default.ParseArguments<Options>(args).WithParsed(x => options = x);

            var host = new WebHostBuilder()
                .UseUrls(options.Prefixs.ToArray())
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
