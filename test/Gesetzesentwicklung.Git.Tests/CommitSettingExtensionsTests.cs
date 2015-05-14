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
    public class CommitSettingsExtensionsTests
    {
        private DirectoryInfoBase _srcDir = new DirectoryInfo(@"c:\data\src");

        [Test]
        [ExpectedException(typeof(ArgumentException),
         ExpectedMessage = "Branch kann nicht abgeleitet werden: Fehlender Dateiname")]
        public void Git_CommitSettingsExtensionsTests_Fehler()
        {
            var commitSetting = new CommitSetting();
            commitSetting.DerivedBranchName(_srcDir);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException),
         ExpectedMessage = "Branch kann nicht abgeleitet werden:", 
         MatchType = MessageMatch.StartsWith)]
        public void Git_CommitSettingsExtensionsTests_SettingsDateiNichtUnterhalbDesSrcDirs()
        {
            var commitSetting = new CommitSetting { FileSettingFilename = @"c:\data\foobar\Gesetzesstand.yml" };
            Assert.That(commitSetting.DerivedBranchName(_srcDir), Is.EqualTo("Gesetzesstand"));
        }

        [Test]
        public void Git_CommitSettingsExtensionsTests_DerivedBranchName()
        {
            var commitSetting = new CommitSetting { FileSettingFilename = @"c:\data\src\Gesetzesstand.yml" };
            Assert.That(commitSetting.DerivedBranchName(_srcDir), Is.EqualTo("Gesetzesstand"));

            commitSetting = new CommitSetting { FileSettingFilename = @"c:\data\src\GG\Blub\Änderung-1.yml" };
            Assert.That(commitSetting.DerivedBranchName(_srcDir), Is.EqualTo("GG/Blub/Änderung-1"));
        }
    }
}
