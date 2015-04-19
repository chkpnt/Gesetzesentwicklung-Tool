using Gesetzesentwicklung.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Git
{
    public class YamlFileParser
    {
        private static readonly Encoding UTF8_Ohne_BOM = new UTF8Encoding(false, false);

        private readonly IFileSystem _fileSystem;

        private readonly IYamlStringParser _yamlStringParser;

        internal YamlFileParser(IFileSystem fileSystem, IYamlStringParser yamlStringParser)
        {
            _fileSystem = fileSystem;
            _yamlStringParser = yamlStringParser;
        }

        internal YamlFileParser(IFileSystem fileSystem)
            : this(fileSystem: fileSystem,
                   yamlStringParser: new YamlStringParser()) { }

        public YamlFileParser()
            : this(fileSystem: new FileSystem(),
                   yamlStringParser: new YamlStringParser()) { }

        public T FromYaml<T>(string file) where T : FileSetting
        {
            var content = _fileSystem.File.ReadAllText(file, UTF8_Ohne_BOM);
            var setting = _yamlStringParser.FromYaml<T>(content);
            setting.FileSettingFilename = file;
            return setting;
        }
    }
}
