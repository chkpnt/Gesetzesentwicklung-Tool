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
                { @"c:\data\GesetzesData\Branches.yml", new MockFileData(
@"Branches:
  Gesetzesstand: Normal
  Gesetze/GG/Bundesgesetzblatt: Normal
  Gesetze/GG/Änderung-1: Feature") },
                { @"c:\data\GesetzesData\Gesetzesstand.yml", new MockFileData(
@"Commits:
- Daten: Gesetzesstand
  Autor: Foo Bar <foo@example.net>
  Datum: 01.01.1980
  Beschreibung: |-
    1. Lesung

    Letzte Zeile") },
                { @"c:\data\GesetzesData\Gesetzesstand\", new MockDirectoryData()},
                { @"c:\data\GesetzesData\Gesetze\GG\Bundesgesetzblatt.yml", new MockFileData(
@"Commits:
- BranchFrom: Gesetzesstand
  Autor: Foo Bar <foo@example.net>
  Datum: 01.01.1981
  Beschreibung: init") },
                { @"c:\data\GesetzesData\Gesetze\GG\Änderung-1.yml", new MockFileData(
@"Commits:
- MergeInto: Gesetze/GG/Bundesgesetzblatt
  Daten: Änderung-1\Lesung-2
  Autor: Foo Bar <foo@example.net>
  Datum: 10.04.1982
  Beschreibung: |-
    2. Lesung

    Letzte Zeile
- BranchFrom: Gesetze/GG/Bundesgesetzblatt
  Daten: Änderung-1\Lesung-1
  Autor: Foo Bar <foo@example.net>
  Datum: 01.01.1982
  Beschreibung: |-
    1. Lesung

    Letzte Zeile") },
                { @"c:\data\GesetzesData\Gesetze\GG\Änderung-1\Lesung-1\", new MockDirectoryData()},
                { @"c:\data\GesetzesData\Gesetze\GG\Änderung-1\Lesung-2\", new MockDirectoryData()},
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

        [Test]
        [ExpectedException(typeof(ArgumentException),
         ExpectedMessage=@"Datei fehlt: c:\data\GesetzesData\Branches.yml")]
        public void Git_BranchesSettingsDateiFehlt()
        {
            _fileSystem.File.Delete(@"c:\data\GesetzesData\Branches.yml");

            var sourceDirInfo = _fileSystem.DirectoryInfo.FromDirectoryName(_sourceDir);

            _classUnderTest.ReadBranchesSettings(sourceDirInfo);
        }

        [Test]
        public void Git_BranchesSettingsDateiKorrektEingelesen()
        {
            var sourceDirInfo = _fileSystem.DirectoryInfo.FromDirectoryName(_sourceDir);

            var branchSettings = _classUnderTest.ReadBranchesSettings(sourceDirInfo);

            Assert.That(branchSettings.Branches.Count(), Is.EqualTo(3));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException),
         ExpectedMessage = @"Yaml-Datei fehlt: c:\data\GesetzesData\Gesetze\GG\Änderung-1.yml")]
        public void Git_BranchesSettingsDatei_CommitSettingsYamlFehlt()
        {
            _fileSystem.File.Delete(@"c:\data\GesetzesData\Gesetze\GG\Änderung-1.yml");

            var sourceDirInfo = _fileSystem.DirectoryInfo.FromDirectoryName(_sourceDir);

            _classUnderTest.ReadBranchesSettings(sourceDirInfo);
        }

        [Test]
        public void Git_CommitSettingsDateienKorrektEingelesen()
        {
            var sourceDirInfo = _fileSystem.DirectoryInfo.FromDirectoryName(_sourceDir);
            var branchesSettings = _classUnderTest.ReadBranchesSettings(sourceDirInfo);

            var commitSettings = _classUnderTest.ReadCommitSettings(sourceDirInfo, branchesSettings);

            Assert.That(commitSettings.Commits.Count(), Is.EqualTo(4));
            Assert.That(commitSettings.Commits.First()._Datum, Is.EqualTo("01.01.1980"));
            Assert.That(commitSettings.Commits.Last()._Datum, Is.EqualTo("10.04.1982"));
        }
    }
}
