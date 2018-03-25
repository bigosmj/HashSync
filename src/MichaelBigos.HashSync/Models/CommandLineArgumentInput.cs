using System;
using System.Collections.Generic;
using System.Text;

namespace MichaelBigos.HashSync.Models
{
    public class CommandLineArgumentInput
    {
        public string operation { get; set; }
        public string sourcePath { get; set; }
        public string targetPath { get; set; }
        public Dictionary<string, List<string>> options { get; set; }

        public CommandLineArgumentInput()
        {
            options = new Dictionary<string, List<string>>();
        }
    }
}
