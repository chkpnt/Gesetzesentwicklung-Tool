using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Models.Tests
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

            IEnumerable<string> validatorMessages;

            Assert.IsFalse(settings.IsValid(out validatorMessages));
            Assert.That(validatorMessages.Count(), Is.EqualTo(1));
            Assert.That(validatorMessages.ElementAt(0), Is.StringEnding("in Konflikt zu einem anderen Branch steht: Gesetze"));
        }

        [Test]
        public void Models_Validators_ValidBranchSettings()
        {    
            IEnumerable<string> validatorMessages;

            Assert.IsTrue(_validBranchSettings.IsValid(out validatorMessages));
            Assert.IsEmpty(validatorMessages);
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
