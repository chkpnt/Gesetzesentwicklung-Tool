using Gesetzesentwicklung.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Markdown
{
    public class MarkdownGenerator
    {
        private Gesetz _gesetz;
        private DirectoryInfo _outputFolder;

        public MarkdownGenerator(Gesetz gesetz, string outputFolder)
        {
            this._gesetz = gesetz;
            this._outputFolder = new DirectoryInfo(outputFolder);
        }

        public void buildMarkdown()
        {
            buildDirectories();
        }

        private void buildDirectories()
        {
            _outputFolder.Create();

            foreach(var verzeichnis in getDirectories())
            {
                _outputFolder.CreateSubdirectory(verzeichnis);
            }
        }

        private IEnumerable<string> getDirectories()
        {
            var abschnitte = (from artikel in _gesetz.Artikel
                              where artikel.Abschnitt != null
                              select artikel.Abschnitt).Distinct();

            foreach(var abschnitt in abschnitte)
            {
                yield return Path.Combine(_gesetz.Name, abschnitt);
            }
        }
    }
}
