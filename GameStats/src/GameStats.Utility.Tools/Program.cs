using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStats.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using GameStats.Utility.Tools.Verbs;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace GameStats.Utility.Tools
{
    public class Program
    {
        static public IConfigurationRoot Configuration { get; set; }
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
            DatabaseContext.ConnectionString = Configuration["connectionString"] ?? @"Filename .\GameStats.Storage.db";

            Parser.Default.ParseArguments<AddOptions>(args)
                .WithParsed<AddOptions>(opts => Add.AddEntityModel(opts));
        }
    }
}
