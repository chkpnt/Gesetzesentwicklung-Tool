using Gesetzesentwicklung.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Validators.Tests
{
    [TestFixture]
    public class ValidatorsTests
    {
        private BranchesSettings _validBranchSettings;

        [SetUp]
        public void SetUp()
        {
            _validBranchSettings = new BranchesSettings
            {
                Branches = new Dictionary<string, BranchesSettings.BranchTyp>
                {
                    { "Gesetzesstand", BranchesSettings.BranchTyp.Normal },
                    { "Gesetze/GG", BranchesSettings.BranchTyp.Normal }
                }
            };
        }

        [Test]
        public void Models_Validators_InvalidBranchSettings()
        {
            var settings = new BranchesSettings
            {
                Branches = new Dictionary<string, BranchesSettings.BranchTyp>
                {
                    { "Gesetze", BranchesSettings.BranchTyp.Normal },
                    { "Gesetze/GG", BranchesSettings.BranchTyp.Normal }
                }
            };

            ValidatorProtokoll protokoll = new ValidatorProtokoll();

            Assert.IsFalse(settings.IsValid(ref protokoll));
            Assert.That(protokoll.Entries.Count(), Is.EqualTo(1));
            Assert.That(protokoll.Entries.ElementAt(0), Is.StringEnding("in Konflikt zu einem anderen Branch steht: Gesetze"));
        }

        [Test]
        public void Models_Validators_ValidBranchSettings()
        {
            ValidatorProtokoll protokoll = new ValidatorProtokoll();

            Assert.IsTrue(_validBranchSettings.IsValid(ref protokoll));
            Assert.IsEmpty(protokoll.Entries);
        }

        [Test]
        public void Models_Validators_CommitSettings()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { @"c:\data\Gesetze\GG\Änderung-1.yml", new MockFileData("") },
                    { @"c:\data\Gesetze\GG\Änderung-1\Lesung-1\", new MockDirectoryData()}
                });

            var validator = new CommitSettingValidator(fileSystem);

            var commitSettingKorrekt = new CommitSetting
            {
                Autor = "Foo Bar <foo@bar.net>",
                BranchFrom = "Gesetze/GG",
                Daten = @"Änderung-1\Lesung-1"
            };

            var commitSettingFalsch = new CommitSetting
            {
                Autor = "Foo Bar <foo@bar.net>",
                BranchFrom = "Gesetze/GG",
                Daten = @"Änderung-1\Lesung-3"
            };


            Assert.IsTrue(validator.IsValid(commitSettingKorrekt, @"c:\data\Gesetze\GG", _validBranchSettings));
            Assert.IsFalse(validator.IsValid(commitSettingFalsch, @"c:\data\Gesetze\GG", _validBranchSettings));
        }
    }
}
