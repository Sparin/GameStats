using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStats.Server
{
    public class Options
    {
        [Option('p', "prefix", Default = new string[] { "http://+:80/" }, HelpText = "Sets prefix for the server")]
        public IEnumerable<string> Prefixs { get; set; }
    }
}
