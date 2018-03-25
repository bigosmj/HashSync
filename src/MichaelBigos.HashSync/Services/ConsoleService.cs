using MichaelBigos.HashSync.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MichaelBigos.HashSync.Services
{
    public class ConsoleService
    {
        public void PrintHelp()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            StringBuilder help = new StringBuilder();
            help.Append($"HashSync v");
            help.AppendLine(version);
            help.AppendLine("=============================================================================");
            help.AppendLine("\r\nHashSync is a mirroring file copier, in the same vein as robocopy. It");
            help.AppendLine("specializes in only copying files who's contents are actually different, ");
            help.AppendLine("overcoming the situation of file dates being different even though the contents");
            help.AppendLine("are the same.");

            Console.WriteLine(help.ToString());

            PrintUsage();
        }

        public void PrintUsage()
        {
            StringBuilder usage = new StringBuilder();

            usage.AppendLine("Usage:");
            usage.AppendLine("HashSync [source] [target] [options]");
            usage.AppendLine("\r\nsource\tThe full path of the source folder");
            usage.AppendLine("target\tThe full path of the target folder");
            usage.AppendLine("\r\nOptions:");
            usage.AppendLine("None presently");

            Console.WriteLine(usage.ToString());
        }

        public CommandLineArgumentInput ParseArguments(string[] args)
        {
            CommandLineArgumentInput parsedArguments = new CommandLineArgumentInput();

            if (args.Length >= 1 && args[0] == "/?")
            {
                parsedArguments.operation = "help";
            }            
            else if (args.Length >= 2)
            {
                parsedArguments.sourcePath = args[0];
                parsedArguments.targetPath = args[1];
            }

            int idx = 2;
            string thisOption = string.Empty;
            List<string> optionValues = new List<string>();
            string thisValue, lastOption;

            while (idx < args.Length)
            {
                thisValue = args[idx];
                if (thisValue.Substring(0,1) == "/")
                {
                    thisOption = thisValue;
                    parsedArguments.options.Add(thisOption, new List<string>());
                    continue;
                }
                else if (thisOption != string.Empty)
                {
                    parsedArguments.options[thisOption].Add(thisValue);
                }
                else
                {
                    Console.WriteLine($"Argument ignored: {thisValue}");
                }

                idx++;
            }

            return parsedArguments;
        }
    }
}
