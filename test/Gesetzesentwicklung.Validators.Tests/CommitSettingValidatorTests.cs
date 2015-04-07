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

            _classUnderTest = new CommitSettingValidator(_fileSystem);
        }

        [Test]
        public void Models_Validators_CommitSettings()
        {
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


            Assert.IsTrue(_classUnderTest.IsValid(commitSettingKorrekt, @"c:\data\GesetzesData\GG", _validBranchSettings));
            Assert.IsFalse(_classUnderTest.IsValid(commitSettingFalsch, @"c:\data\GesetzesData\GG", _validBranchSettings));
        }
    }
}
