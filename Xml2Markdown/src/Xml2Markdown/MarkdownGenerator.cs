using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xml2Markdown
{
    class MarkdownGenerator
    {
        public string InputFile { get; private set; }
        public string OutputFolder { get; private set; }

        public MarkdownGenerator(string inputFile, string outputFolder)
        {
            this.InputFile = inputFile;
            this.OutputFolder = outputFolder;
        }

        internal void buildMarkdown()
        {
            throw new NotImplementedException();
        }
    }
}
