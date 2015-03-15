using Gesetzesentwicklung.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Gesetzesentwicklung.Markdown
{
    public class MarkdownGenerator
    {
        private Gesetz _gesetz;
        private CommitSetting _settings;
        private DirectoryInfo _outputFolder;

        public MarkdownGenerator(Gesetz gesetz, CommitSetting settings, string outputFolder)
        {
            this._gesetz = gesetz;
            this._settings = settings;
            this._outputFolder = new DirectoryInfo(outputFolder);
        }

        public void generate()
        {
            buildDirectories();
            createFiles();
            createSettingFile();
        }

        private void buildDirectories()
        {
            _outputFolder.Create();

            foreach (var verzeichnis in getDirectories())
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

        private void createFiles()
        {
            foreach (var artikel in _gesetz.Artikel)
            {
                var dateiname = artikel.Name + ".md";

                if (artikel.Abschnitt == null)
                {
                    File.WriteAllText(
                        path: Path.Combine(_outputFolder.Name, _gesetz.Name, dateiname),
                        contents: formatMarkdown(artikel),
                        encoding: Encoding.UTF8
                    );
                }
                else
                {
                    File.WriteAllText(
                        path: Path.Combine(_outputFolder.Name, _gesetz.Name, artikel.Abschnitt, dateiname),
                        contents: formatMarkdown(artikel),
                        encoding: Encoding.UTF8
                    );
                }
            }
        }

        private string formatMarkdown(Artikel artikel)
        {
            return string.Format(
@"# {0}

{1}", artikel.Name, artikel.Inhalt);
        }

        private void createSettingFile()
        {
            using (TextWriter textWriter = new StreamWriter(
                path: Path.Combine(_outputFolder.Name, _gesetz.Name + ".yml"),
                append: false,
                encoding: Encoding.UTF8))
            {
                textWriter.NewLine = Environment.NewLine;
                new Serializer().Serialize(textWriter, _settings);
            }
        }
    }
}
