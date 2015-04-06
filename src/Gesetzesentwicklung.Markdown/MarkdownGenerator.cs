using Gesetzesentwicklung.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Gesetzesentwicklung.Markdown
{
    public class MarkdownGenerator
    {
        private static readonly Encoding UTF8_Ohne_BOM = new UTF8Encoding(false, false);

        private IFileSystem _fileSystem;


        internal MarkdownGenerator(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public MarkdownGenerator() : this(fileSystem: new FileSystem()) { }

        public void generate(Gesetz gesetz, CommitSetting settings, string outputFolder)
        {
            var buildFolder = _fileSystem.DirectoryInfo.FromDirectoryName(outputFolder);

            buildDirectories(gesetz, buildFolder);
            createFiles(gesetz, buildFolder);
            createSettingFile(settings, gesetz, buildFolder);
        }

        private void buildDirectories(Gesetz gesetz, DirectoryInfoBase buildFolder)
        {
            buildFolder.Create();

            foreach (var verzeichnis in getDirectories(gesetz))
            {
                buildFolder.CreateSubdirectory(verzeichnis);
            }
        }

        private IEnumerable<string> getDirectories(Gesetz gesetz)
        {
            var abschnitte = (from artikel in gesetz.Artikel
                              where artikel.Abschnitt != null
                              select artikel.Abschnitt).Distinct();

            foreach(var abschnitt in abschnitte)
            {
                yield return Path.Combine(gesetz.Name, abschnitt);
            }
        }

        private void createFiles(Gesetz gesetz, DirectoryInfoBase buildFolder)
        {
            foreach (var artikel in gesetz.Artikel)
            {
                var dateiname = artikel.Name + ".md";

                if (artikel.Abschnitt == null)
                {
                    _fileSystem.File.WriteAllText(
                        path: Path.Combine(buildFolder.FullName, gesetz.Name, dateiname),
                        contents: formatMarkdown(artikel),
                        encoding: UTF8_Ohne_BOM
                    );
                }
                else
                {
                    _fileSystem.File.WriteAllText(
                        path: Path.Combine(buildFolder.FullName, gesetz.Name, artikel.Abschnitt, dateiname),
                        contents: formatMarkdown(artikel),
                        encoding: UTF8_Ohne_BOM
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

        private void createSettingFile(CommitSetting settings, Gesetz gesetz, DirectoryInfoBase buildFolder)
        {
            using (TextWriter textWriter = new StringWriter())
            {
                textWriter.NewLine = Environment.NewLine;
                new Serializer().Serialize(textWriter, settings);

                _fileSystem.File.WriteAllText(
                    path: Path.Combine(buildFolder.FullName, gesetz.Name + ".yml"),
                    contents: textWriter.ToString(),
                    encoding: UTF8_Ohne_BOM
                );
            }
        }
    }
}
