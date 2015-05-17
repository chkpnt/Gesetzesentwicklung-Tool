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
    public class RepositoryBuilderInternalTests
    {
        private DirectoryInfoBase _srcDir = new DirectoryInfo(@"c:\data\src");

        private RepositoryBuilder _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            var fileSystem = new MockFileSystem();
            _classUnderTest = new RepositoryBuilder(fileSystem);
        }

        [TestCase(null)]
        [TestCase("")]
        [ExpectedException(typeof(ArgumentException),
         ExpectedMessage = "Branch kann nicht abgeleitet werden: Fehlender Dateiname")]
        public void Git_RepositoryBuilder_DeriveBranchName_Fehler(string filename)
        {
            _classUnderTest.DeriveBranchName(null, _srcDir);
        }

        [TestCase(@"c:\data\foobar\Gesetzesstand.yml")]
        [ExpectedException(typeof(ArgumentException),
         ExpectedMessage = "Branch kann nicht abgeleitet werden:",
         MatchType = MessageMatch.StartsWith)]
        public void Git_RepositoryBuilder_DeriveBranchName_SettingsDateiNichtUnterhalbDesSrcDirs(string filename)
        {
            _classUnderTest.DeriveBranchName(filename, _srcDir);
        }

        [TestCase(@"c:\data\src\Gesetzesstand.yml", Result = "Gesetzesstand")]
        [TestCase(@"c:\data\src\GG\Blub\Änderung-1.yml", Result = "GG/Blub/Änderung-1")]
        public string Git_RepositoryBuilder_DeriveBranchName(string filename) => _classUnderTest.DeriveBranchName(filename, _srcDir);
    }
}
