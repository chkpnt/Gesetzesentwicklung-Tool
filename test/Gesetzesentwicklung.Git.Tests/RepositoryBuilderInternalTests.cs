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
        private DirectoryInfoBase _sourceDirInfo;
        private DirectoryInfoBase _destDirInfo;

        private IFileSystem _fileSystem;

        private RepositoryBuilder _classUnderTest;
        
        [SetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\data\GesetzesData\Gesetzesstand.yml", new MockFileData(
@"Commits:
- Daten: Gesetzesstand
  Ziel: /
  Autor: Foo Bar <foo@example.net>
  Datum: 01.01.1980
  Beschreibung: |-
    1. Lesung

    Letzte Zeile") },
                { @"c:\data\GesetzesData\Gesetzesstand\", new MockDirectoryData()},
                { @"c:\data\GesetzesData\Gesetze\GG\Bundesgesetzblatt.yml", new MockFileData(
@"AutoMergeInto: Gesetzesstand
Commits:
- BranchFrom: Gesetzesstand
  Autor: Foo Bar <foo@example.net>
  Datum: 01.01.1981
  Beschreibung: init") },
                { @"c:\data\GesetzesData\Gesetze\GG\Änderung-1.yml", new MockFileData(
@"Commits:
- Daten: Änderung-1\Lesung-2
  Ziel: /GG
  MergeInto: Gesetze/GG/Bundesgesetzblatt
  Autor: Foo Bar <foo@example.net>
  Datum: 10.04.1982
  Beschreibung: |-
    2. Lesung

    Letzte Zeile
- Daten: Änderung-1\Lesung-1
  Ziel: /GG
  BranchFrom: Gesetze/GG/Bundesgesetzblatt
  Autor: Foo Bar <foo@example.net>
  Datum: 01.01.1982
  Beschreibung: |-
    1. Lesung

    Letzte Zeile") },
                { @"c:\data\GesetzesData\Gesetze\GG\Änderung-1\Lesung-1\", new MockDirectoryData()},
                { @"c:\data\GesetzesData\Gesetze\GG\Änderung-1\Lesung-2\", new MockDirectoryData()},
            });

            _sourceDirInfo = _fileSystem.DirectoryInfo.FromDirectoryName(@"c:\data\GesetzesData\");
            _destDirInfo = _fileSystem.DirectoryInfo.FromDirectoryName(@"c:\data\GesetzesRepo\");

            _classUnderTest = new RepositoryBuilder(_fileSystem);
        }

        [TestCase(null)]
        [TestCase("")]
        [ExpectedException(typeof(ArgumentException),
         ExpectedMessage = "Branch kann nicht abgeleitet werden: Fehlender Dateiname")]
        public void Git_RepositoryBuilder_DeriveBranchName_Fehler(string filename)
        {
            _classUnderTest.DeriveBranchName(null, _sourceDirInfo);
        }

        [TestCase(@"c:\data\foobar\Gesetzesstand.yml")]
        [ExpectedException(typeof(ArgumentException),
         ExpectedMessage = "Branch kann nicht abgeleitet werden:",
         MatchType = MessageMatch.StartsWith)]
        public void Git_RepositoryBuilder_DeriveBranchName_SettingsDateiNichtUnterhalbDesSrcDirs(string filename)
        {
            _classUnderTest.DeriveBranchName(filename, _sourceDirInfo);
        }

        [TestCase(@"c:\data\GesetzesData\Gesetzesstand.yml", Result = "Gesetzesstand")]
        [TestCase(@"c:\data\GesetzesData\GG\Blub\Änderung-1.yml", Result = "GG/Blub/Änderung-1")]
        public string Git_RepositoryBuilder_DeriveBranchName(string filename) => _classUnderTest.DeriveBranchName(filename, _sourceDirInfo);


        [Test]
        [ExpectedException(typeof(ArgumentException),
         ExpectedMessage = @"^Verzeichnis existiert nicht: c:\\data\\gibtsNicht$",
         MatchType = MessageMatch.Regex)]
        public void Git_RepositoryBuilder_TestAssertions_NichtExistierendesQuellVerzeichnis()
        {
            var sourceDirInfo = _fileSystem.DirectoryInfo.FromDirectoryName(@"c:\data\gibtsNicht\");

            _classUnderTest.TestAssertions(sourceDirInfo, _destDirInfo);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException),
         ExpectedMessage = "Quell- und Zielverzeichnisse dürfen nicht gleich sein")]
        public void Git_RepositoryBuilder_TestAssertions_GleichesQuellUndZielverzeichnis()
        {
            _classUnderTest.TestAssertions(_sourceDirInfo, _sourceDirInfo);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException),
         ExpectedMessage = @"^Verzeichnis existiert schon: c:\\data\\GesetzesRepo$",
         MatchType = MessageMatch.Regex)]
        public void Git_RepositoryBuilder_TestAssertions_ZielverzeichnisExistiertSchon()
        {
            _destDirInfo.Create();

            _classUnderTest.TestAssertions(_sourceDirInfo, _destDirInfo);
        }

        [Test]
        public void Git_RepositoryBuilder_ReadCommitSettings_CommitSettingsDateienKorrektEingelesen()
        {
            var commitSettings = _classUnderTest.ReadBranchSettings(_sourceDirInfo);

            Assert.That(commitSettings.Commits.Count(), Is.EqualTo(4));
            Assert.That(commitSettings.Commits.First()._Datum, Is.EqualTo("01.01.1980"));
            Assert.That(commitSettings.Commits.First().Branch, Is.EqualTo("Gesetzesstand"));
            Assert.That(commitSettings.Commits.Last()._Datum, Is.EqualTo("10.04.1982"));
            Assert.That(commitSettings.Commits.Last().Branch, Is.EqualTo("Gesetze/GG/Änderung-1"));
        }
    }
}
