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

        internal YamlFileParser(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public YamlFileParser() : this(fileSystem: new FileSystem()) { }

        public T FromYaml<T>(string file)
        {
            var content = _fileSystem.File.ReadAllText(file, UTF8_Ohne_BOM);
            return YamlStringParser.FromYaml<T>(content);
        }
    }
}
