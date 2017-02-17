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
        [Option('t', Required = true)]
        public string Type { get; set; }

        [Option('c', Default = 1)]
        public int Count { get; set; }
    }
}
