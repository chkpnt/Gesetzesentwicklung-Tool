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

namespace Gesetzesentwicklung.Validators.Tests
{
    [TestFixture]
    public class CommitSettingValidatorTests
    {
        private BranchesSettings _validBranchSettings;

        private CommitSetting _validCommitSetting;

        private IFileSystem _fileSystem;

        private CommitSettingValidator _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { @"c:\data\GesetzesData\GG\Änderung-1.yml", new MockFileData("") },
                    { @"c:\data\GesetzesData\GG\Änderung-1\Lesung-1\", new MockDirectoryData()}
                });

            _validCommitSetting = new CommitSetting
            {
                Autor = "Foo Bar <foo@bar.net>",
                BranchFrom = "Gesetze/GG",
                Daten = @"Änderung-1\Lesung-1"
            };

            _validBranchSettings = new BranchesSettings();

            _classUnderTest = new CommitSettingValidator(_fileSystem);
        }

        [Test]
        public void Models_Validators_CommitSetting()
        {
            var commitSettingKorrektOhneDaten = new CommitSetting
            {
                Autor = "Foo Bar <foo@bar.net>",
                BranchFrom = "Gesetze/GG"
            };

            var commitSettingFalsch = new CommitSetting
            {
                Autor = "Foo Bar <foo@bar.net>",
                BranchFrom = "Gesetze/GG",
                Daten = @"Änderung-1\Lesung-3"
            };


            Assert.IsTrue(_classUnderTest.IsValid(_validCommitSetting, @"c:\data\GesetzesData\GG", _validBranchSettings));
            Assert.IsTrue(_classUnderTest.IsValid(commitSettingKorrektOhneDaten, @"c:\data\GesetzesData\GG", _validBranchSettings));
            Assert.IsFalse(_classUnderTest.IsValid(commitSettingFalsch, @"c:\data\GesetzesData\GG", _validBranchSettings));
        }

        [Test]
        public void Models_Validators_CommitSetting_VerzeichnisFehlt()
        {
            _fileSystem.Directory.Delete(@"c:\data\GesetzesData\GG\Änderung-1\Lesung-1\");

            ValidatorProtokoll protokoll = new ValidatorProtokoll();
            var result = _classUnderTest.IsValid(_validCommitSetting, @"c:\data\GesetzesData\GG", _validBranchSettings, ref protokoll);

            Assert.IsFalse(result);
            Assert.That(protokoll.Entries.Count(), Is.EqualTo(1));
            Assert.That(protokoll.Entries.First(), Is.EqualTo(@"Verzeichnis fehlt: c:\data\GesetzesData\GG\Änderung-1\Lesung-1"));
        }
    }
}
