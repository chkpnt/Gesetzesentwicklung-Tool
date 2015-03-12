using Gesetzesentwicklung.GII;
using Gesetzesentwicklung.Markdown;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3 || HelpRequired(args[1]))
            {
                DisplayHelp();
                Environment.Exit(0);
            }

            var inputFile = args[1];
            var outputFolder = args[2];

            if (!File.Exists(inputFile))
            {
                DisplayHelp();
                Console.WriteLine(string.Format("File not found: {0}", inputFile));
                Environment.Exit(1);
            }

            var generator = new MarkdownGenerator(inputFile, outputFolder);
            generator.buildMarkdown();
        }

        private static void DisplayHelp()
        {
            Console.WriteLine(@"
Xml2Markdown.exe <filename> <output-folder>
");
        }

        private static bool HelpRequired(string param)
        {
            return param == "-h" || param == "--help" || param == "/?";
        }
    }
}
