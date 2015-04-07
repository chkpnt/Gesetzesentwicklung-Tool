using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Git.Tests
{
    [TestFixture]
    [Category("WithFSAccess")]
    public class RepositoryBuilderTests
    {
        private DirectoryInfoBase _sourceDirInfo;
        private DirectoryInfoBase _destDirInfo;

        private IFileSystem _fileSystem;

        private RepositoryBuilder _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new FileSystem();

            _sourceDirInfo = _fileSystem.DirectoryInfo.FromDirectoryName("TestData");
            _destDirInfo = _fileSystem.DirectoryInfo.FromDirectoryName("TestRepository");

            _classUnderTest = new RepositoryBuilder();
        }

        [TearDown]
        public void TearDown()
        {
            if (_destDirInfo.Exists)
            {
                _destDirInfo.Delete(recursive: true);
            }
        }

        [Test]
        public void Git_ZielIstEinGitRepository()
        {
            _classUnderTest.Build(_sourceDirInfo.FullName, _destDirInfo.FullName);

            Assert.That(_destDirInfo.GetDirectories(".git", SearchOption.TopDirectoryOnly).Count(), Is.EqualTo(1));
        }
    }
}
