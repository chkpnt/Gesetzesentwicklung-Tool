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
    public class BranchesSettingsValidatorTests
    {
        private BranchesSettings _validBranchSettings;

        private IFileSystem _fileSystem;

        private BranchesSettingsValidator _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _validBranchSettings = new BranchesSettings
            {
                Branches = new Dictionary<string, BranchesSettings.BranchTyp>
                {
                    { "Gesetzesstand", BranchesSettings.BranchTyp.Normal },
                    { "Gesetze/GG/Bundesgesetzblatt", BranchesSettings.BranchTyp.Normal },
                    { "Gesetze/GG/Änderung-1", BranchesSettings.BranchTyp.Feature }
                }
            };

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { @"c:\GesetzesData\Gesetzesstand.yml", new MockFileData("") },
                    { @"c:\GesetzesData\Gesetze\GG\Bundesgesetzblatt.yml", new MockFileData("") },
                    { @"c:\GesetzesData\Gesetze\GG\Änderung-1.yml", new MockFileData("")}
                });

            _classUnderTest = new BranchesSettingsValidator(_fileSystem);
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

            Assert.IsFalse(_classUnderTest.IsValid(settings, @"c:\GesetzesData\", ref protokoll));
            Assert.That(protokoll.Entries.Count(), Is.EqualTo(1));
            Assert.That(protokoll.Entries.ElementAt(0), Is.StringEnding("in Konflikt zu einem anderen Branch steht: Gesetze"));
        }

        [Test]
        public void Models_Validators_YamlsNichtDa()
        {
            _fileSystem.File.Delete(@"c:\GesetzesData\Gesetzesstand.yml");
            _fileSystem.File.Delete(@"c:\GesetzesData\Gesetze\GG\Änderung-1.yml");

            ValidatorProtokoll protokoll = new ValidatorProtokoll();

            Assert.IsFalse(_classUnderTest.IsValid(_validBranchSettings, @"c:\GesetzesData\", ref protokoll));
            Assert.That(protokoll.Entries.Count(), Is.EqualTo(2));
            Assert.That(protokoll.Entries.ElementAt(0), Is.EqualTo(@"Yaml-Datei fehlt: c:\GesetzesData\Gesetzesstand.yml"));
            Assert.That(protokoll.Entries.ElementAt(1), Is.EqualTo(@"Yaml-Datei fehlt: c:\GesetzesData\Gesetze\GG\Änderung-1.yml"));
        }

        [Test]
        public void Models_Validators_ValidBranchSettings()
        {
            ValidatorProtokoll protokoll = new ValidatorProtokoll();

            Assert.IsTrue(_classUnderTest.IsValid(_validBranchSettings, @"c:\GesetzesData\", ref protokoll));
            Assert.IsEmpty(protokoll.Entries);
        }
    }
}
