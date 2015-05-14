using Gesetzesentwicklung.Models;
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
    public class DirectoryInfoExtensionsTests
    {
        private IFileSystem _fileSystem;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\data\src\Gesetzesstand.yml", new MockFileData("bla") },
                { @"c:\data\src\Gesetzesstand\", new MockDirectoryData()},
                { @"c:\data\src\Gesetze\GG\Bundesgesetzblatt.yml", new MockFileData("") }
            });
        }

        [Test]
        public void Git_DirectoryInfoExtensionsTests_CopyTo()
        {
            var src = _fileSystem.DirectoryInfo.FromDirectoryName(@"c:\data\src");
            var dest = _fileSystem.DirectoryInfo.FromDirectoryName(@"c:\data\dest");

            src.CopyTo(dest);

            Assert.True(dest.Exists);
            Assert.That(dest.GetDirectories("*", SearchOption.AllDirectories).Count(), Is.EqualTo(3));
            Assert.True(_fileSystem.Directory.Exists(@"c:\data\dest\Gesetzesstand"));
            Assert.True(_fileSystem.Directory.Exists(@"c:\data\dest\Gesetze"));
            Assert.True(_fileSystem.Directory.Exists(@"c:\data\dest\Gesetze\GG"));
            Assert.That(dest.GetFiles("*", SearchOption.AllDirectories).Count(), Is.EqualTo(2));
            Assert.True(_fileSystem.File.Exists(@"c:\data\dest\Gesetzesstand.yml"));
            Assert.True(_fileSystem.File.Exists(@"c:\data\dest\Gesetze\GG\Bundesgesetzblatt.yml"));
        }
    }
}
