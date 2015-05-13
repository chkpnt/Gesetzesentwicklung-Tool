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
                Daten = @"Änderung-1\Lesung-1",
                _Ziel = "/GG",
                BranchFrom = "Gesetze/GG",
                _Autor = "Foo Bar <foo@bar.net>",
                _Datum = "01.01.1990"
            };

            _classUnderTest = new CommitSettingValidator(_fileSystem);
        }

        [Test]
        public void Models_Validators_CommitSetting()
        {
            var commitSettingKorrektOhneDaten = new CommitSetting
            {
                BranchFrom = "Gesetze/GG",
                _Autor = "Foo Bar <foo@bar.net>",
                _Datum = "01.01.1990"
            };

            var commitSettingFalsch = new CommitSetting
            {
                Daten = @"Änderung-1\Lesung-3",
                BranchFrom = "Gesetze/GG",
                _Autor = "Foo Bar <foo@bar.net>",
                _Datum = "01.01.1990"
            };


            Assert.IsTrue(_classUnderTest.IsValid(_validCommitSetting, @"c:\data\GesetzesData\GG"));
            Assert.IsTrue(_classUnderTest.IsValid(commitSettingKorrektOhneDaten, @"c:\data\GesetzesData\GG"));
            Assert.IsFalse(_classUnderTest.IsValid(commitSettingFalsch, @"c:\data\GesetzesData\GG"));
        }

        [Test]
        public void Models_Validators_CommitSetting_VerzeichnisFehlt()
        {
            _fileSystem.Directory.Delete(@"c:\data\GesetzesData\GG\Änderung-1\Lesung-1\");

            var protokoll = new ValidatorProtokoll();
            var result = _classUnderTest.IsValid(_validCommitSetting, @"c:\data\GesetzesData\GG", ref protokoll);

            Assert.IsFalse(result);
            Assert.That(protokoll.Entries.Count(), Is.EqualTo(1));
            Assert.That(protokoll.Entries.First().Message, Is.EqualTo(@"Verzeichnis fehlt: c:\data\GesetzesData\GG\Änderung-1\Lesung-1"));
        }

        [Test]
        public void Models_Validators_CommitSetting_DatumFehlt()
        {
            var commitSettingOhneDatum = new CommitSetting
            {
                Daten = @"Änderung-1\Lesung-1",
                _Ziel = "/GG",
                BranchFrom = "Gesetze/GG",
                _Autor = "Foo Bar <foo@bar.net>"
            };

            var protokoll = new ValidatorProtokoll();
            var result = _classUnderTest.IsValid(commitSettingOhneDatum, @"c:\data\GesetzesData\GG", ref protokoll);

            Assert.IsFalse(result);
            Assert.That(protokoll.Entries.Count(), Is.EqualTo(1));
            Assert.That(protokoll.Entries.First().Message, Is.EqualTo("Datum fehlt"));
        }

        [Test]
        public void Models_Validators_CommitSetting_ZuFrueh()
        {
            var commitSettingZuFrueh = new CommitSetting
            {
                Daten = @"Änderung-1\Lesung-1",
                _Ziel = "/GG",
                BranchFrom = "Gesetze/GG",
                _Autor = "Foo Bar <foo@bar.net>",
                _Datum = "03.03.1973"
            };

            var protokoll = new ValidatorProtokoll();
            var result = _classUnderTest.IsValid(commitSettingZuFrueh, @"c:\data\GesetzesData\GG", ref protokoll);

            Assert.IsFalse(result);
            Assert.That(protokoll.Entries.Count(), Is.EqualTo(1));
            Assert.That(protokoll.Entries.First().Message, Is.EqualTo("Minimales Datum ist der 04.03.1973"));
        }

        [Test]
        public void Models_Validators_CommitSetting_ZielFehlt()
        {
            var commitSettingOhneZiel = new CommitSetting
            {
                Daten = @"Änderung-1\Lesung-1",
                BranchFrom = "Gesetze/GG",
                _Autor = "Foo Bar <foo@bar.net>",
                _Datum = "01.01.1990"
            };

            var protokoll = new ValidatorProtokoll();
            var result = _classUnderTest.IsValid(commitSettingOhneZiel, @"c:\data\GesetzesData\GG", ref protokoll);

            Assert.IsFalse(result);
            Assert.That(protokoll.Entries.Count(), Is.EqualTo(1));
            Assert.That(protokoll.Entries.First().Message, Is.EqualTo("Kein Ziel angegeben, wohl aber Daten"));
        }

        [Test]
        public void Models_Validators_CommitSetting_ZielFalschesFormat()
        {
            var commitSettingFalschesZiel = new CommitSetting
            {
                Daten = @"Änderung-1\Lesung-1",
                _Ziel = "GG",
                BranchFrom = "Gesetze/GG",
                _Autor = "Foo Bar <foo@bar.net>",
                _Datum = "01.01.1990"
            };

            var protokoll = new ValidatorProtokoll();
            var result = _classUnderTest.IsValid(commitSettingFalschesZiel, @"c:\data\GesetzesData\GG", ref protokoll);

            Assert.IsFalse(result);
            Assert.That(protokoll.Entries.Count(), Is.EqualTo(1));
            Assert.That(protokoll.Entries.First().Message, Is.EqualTo(@"Ziel muss mit ""/"" anfangen"));
        }
    }
}
