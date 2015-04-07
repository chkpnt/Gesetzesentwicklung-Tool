using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Git.Tests
{
    [TestFixture]
    public class RepositoryBuilderVorbedingungenTests
    {
        private string _sourceDir = @"c:\data\GesetzesData\";
        private string _destDir = @"c:\data\GesetzesRepo\";

        private IFileSystem _fileSystem;

        private RepositoryBuilder _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { @"c:\data\GesetzesData\GG\Änderung-1.yml", new MockFileData("") },
                    { @"c:\data\GesetzesData\GG\Änderung-1\Lesung-1\", new MockDirectoryData()}
                });

            _classUnderTest = new RepositoryBuilder(_fileSystem);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException),
         ExpectedMessage=@"^Verzeichnis existiert nicht: c:\\data\\gibtsNicht$",
         MatchType=MessageMatch.Regex)]
        public void Git_NichtExistierendesQuellVerzeichnis()
        {
            var sourceDirInfo = _fileSystem.DirectoryInfo.FromDirectoryName(@"c:\data\gibtsNicht\");
            var destDirInfo = _fileSystem.DirectoryInfo.FromDirectoryName(_destDir);

            _classUnderTest.TestAssertions(sourceDirInfo, destDirInfo);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException),
         ExpectedMessage="Quell- und Zielverzeichnisse dürfen nicht gleich sein")]
        public void Git_GleichesQuellUndZielverzeichnis()
        {
            var sourceDirInfo = _fileSystem.DirectoryInfo.FromDirectoryName(_sourceDir);

            _classUnderTest.TestAssertions(sourceDirInfo, sourceDirInfo);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException),
         ExpectedMessage=@"^Verzeichnis existiert schon: c:\\data\\GesetzesRepo$",
         MatchType=MessageMatch.Regex)]
        public void Git_ZielverzeichnisExistiertSchon()
        {
            var sourceDirInfo = _fileSystem.DirectoryInfo.FromDirectoryName(_sourceDir);
            var destDirInfo = _fileSystem.DirectoryInfo.FromDirectoryName(_destDir);
            
            destDirInfo.Create();

            _classUnderTest.TestAssertions(sourceDirInfo, destDirInfo);
        }
    }
}
