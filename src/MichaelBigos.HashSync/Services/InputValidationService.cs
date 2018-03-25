using MichaelBigos.HashSync.Models;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text;

namespace MichaelBigos.HashSync.Services
{
    public class InputValidationService : IInputValidationService
    {
        private readonly ConsoleService console;
        private readonly IFileSystem fileSystem;

        public InputValidationService(ConsoleService console, IFileSystem fileSystem)
        {
            this.console = console;
            this.fileSystem = fileSystem;
        }

        public bool Validate(CommandLineArgumentInput input)
        {
            bool ret = false;
            if (input.operation == "help") 
            {
                ret = true;  //suspend any other validation
            }
            else if (string.IsNullOrEmpty(input.sourcePath))
            {
                Console.WriteLine("Error: Source Path is missing");
            }
            else if (string.IsNullOrEmpty(input.targetPath))
            {
                Console.WriteLine("Error: Target Path is missing");
            }
            else if (!fileSystem.Directory.Exists(input.sourcePath))
            {
                Console.WriteLine("Error: The Source Path does not exist");
            }
            else
            {
                ret = true;
            }

            if (!ret)
            {
                console.PrintUsage();
            }

            return ret;
        }
    }
}
