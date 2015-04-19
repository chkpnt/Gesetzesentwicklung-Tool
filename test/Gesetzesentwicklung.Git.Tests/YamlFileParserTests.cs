using Gesetzesentwicklung.Models;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Git.Tests
{
    [TestFixture]
    public class YamlFileParserTests
    {
        private IFileSystem _fileSystem;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\data\datei.yml", new MockFileData("") }
            });
        }

        [Test]
        public void Git_YamlFileParser_SetztFilenameInObjekt()
        {
            var yamlStringParserMock = Substitute.For<IYamlStringParser>();
            yamlStringParserMock.FromYaml<FileSettingFake>(Arg.Any<string>()).Returns(new FileSettingFake());

            var classUnderTest = new YamlFileParser(_fileSystem, yamlStringParserMock);
            var fileSetting = classUnderTest.FromYaml<FileSettingFake>(@"c:\data\datei.yml");

            Assert.That(fileSetting.FileSettingFilename, Is.EqualTo(@"c:\data\datei.yml"));
        }

        public class FileSettingFake : FileSetting {}
    }
}
