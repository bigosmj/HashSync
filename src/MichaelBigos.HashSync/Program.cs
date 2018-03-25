using MichaelBigos.HashSync.Business;
using MichaelBigos.HashSync.Models;
using MichaelBigos.HashSync.Services;
using System;
using System.IO.Abstractions;

namespace MichaelBigos.HashSync
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleService consoleService = new ConsoleService();
            IFileSystem fileSystem = new FileSystem() as IFileSystem;
            FileSyncLogic logic = new FileSyncLogic(fileSystem);
            InputValidationService inputValidationService = new InputValidationService(consoleService, fileSystem);

            CommandLineArgumentInput input = consoleService.ParseArguments(args);
            if (!inputValidationService.Validate(input))
                return;

            if (input.operation == "help")
            {
                consoleService.PrintHelp();
            }
            else
            {
                logic.SyncFolders(input);
            }
        }
    }
}
