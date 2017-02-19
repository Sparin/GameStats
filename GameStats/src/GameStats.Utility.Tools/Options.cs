using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStats.Utility.Tools
{

    [Verb("add", HelpText = "Add specific model type with random values to the database")]
    class AddOptions
    {
        [Option('t', "type", Required = true, HelpText = "Type of entity for adding (match, server)")]
        public string Type { get; set; }

        [Option('c', "count", Default = 1, HelpText = "Number of elements")]
        public int Count { get; set; }

        [Option('s', "serverIndex", Default = -1, HelpText = "Server's index for match. Required for match")]
        public int ServerIndex { get; set; }
    }
}
