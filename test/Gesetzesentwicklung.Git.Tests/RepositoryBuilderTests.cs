using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Git.Tests
{
    [TestFixture]
    public class RepositoryBuilderTests
    {
        private RepositoryBuilder _repositoryBuilder;
        private DirectoryInfo _sourceDir;
        private DirectoryInfo _destDir;

        [SetUp]
        public void SetUp()
        {
            Console.WriteLine("SetUp: " + TestContext.CurrentContext.Test.Name);
            _sourceDir = new DirectoryInfo("TestData");
            _destDir = new DirectoryInfo("Git-Repository");
            if (_destDir.Exists)
                Console.WriteLine("Verzeichnis existiert!");
            
        }

        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("TearDown: " + TestContext.CurrentContext.Test.Name);
            if (_destDir.Exists)
            {
                Console.WriteLine("TearDown: Verzeichnis löschen");
                _destDir.Delete(recursive: true);
            }
            Console.WriteLine();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException),
         ExpectedMessage=@"^Verzeichnis existiert nicht:.*\\TestDataNichtExistierend$",
         MatchType=MessageMatch.Regex)]
        public void NichtExistierendesQuellVerzeichnis()
        {
            _sourceDir = new DirectoryInfo("TestDataNichtExistierend");

            _repositoryBuilder = new RepositoryBuilder(_sourceDir, _destDir);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException),
         ExpectedMessage="Quell- und Zielverzeichnisse dürfen nicht gleich sein")]
        public void GleichesQuellUndZielverzeichnis()
        {
            var sourceDir = new DirectoryInfo("TestData");
            var destDir = new DirectoryInfo("TestData");

            _repositoryBuilder = new RepositoryBuilder(sourceDir, destDir);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException),
         ExpectedMessage=@"^Verzeichnis existiert schon:.*\\Git-Repository$",
         MatchType=MessageMatch.Regex)]
        public void ZielverzeichnisExistiertSchon()
        {            
            _destDir.Create();

            _repositoryBuilder = new RepositoryBuilder(_sourceDir, _destDir);
        }

        [Test]
        public void ZielIstEinGitRepository()
        {
            _repositoryBuilder = new RepositoryBuilder(_sourceDir, _destDir);
            _repositoryBuilder.build();

            Assert.That(_destDir.GetDirectories(".git", SearchOption.TopDirectoryOnly).Count(), Is.EqualTo(1));
        }
    }
}
