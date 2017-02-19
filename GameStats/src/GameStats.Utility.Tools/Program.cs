using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStats.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using GameStats.Utility.Tools.Verbs;

namespace GameStats.Utility.Tools
{
    public class Program
    {        
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<AddOptions>(args)
                .WithParsed<AddOptions>(opts => Add.AddEntityModel(opts));
        }       
    }
}
