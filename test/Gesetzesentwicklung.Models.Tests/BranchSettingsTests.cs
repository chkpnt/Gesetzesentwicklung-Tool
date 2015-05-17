using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Models.Tests
{
    [TestFixture]
    public class BranchSettingsTests
    {
        private BranchSettings _branchSettings;
        private string _serializedCommitSettings;
        private IYamlStringParser _yamlStringParser;

        [SetUp]
        public void SetUp()
        {
            _branchSettings = new BranchSettings
            {
                AutoMergeInto = "Gesetzesstand",
                Commits = new List<CommitSetting>
                {
                    new CommitSetting { _Autor = "Foo Bar <foo@bar.net>", _Datum = "01.01.2015" },
                    new CommitSetting { _Autor = "Foo Bar <foo@bar.net>", Beschreibung = 
@"Zeile 1
Zeile 2"}
                }
            };

            _serializedCommitSettings =
@"AutoMergeInto: Gesetzesstand
Commits:
- Datum: 01.01.2015
  Autor: '""Foo Bar"" <foo@bar.net>'
- Autor: '""Foo Bar"" <foo@bar.net>'
  Beschreibung: >-
    Zeile 1

    Zeile 2
";

            _yamlStringParser = new YamlStringParser();
        }

        [Test]
        public void Models_CommitSettings_Serialize()
        {
            var yaml = _yamlStringParser.ToYaml(_branchSettings);

            Assert.That(yaml, Is.EqualTo(_serializedCommitSettings));
        }

        [Test]
        public void Models_CommitSettings_Deserialize()
        {
            var settings = _yamlStringParser.FromYaml<BranchSettings>(_serializedCommitSettings);

            Assert.That(settings.Commits, Is.EquivalentTo(_branchSettings.Commits));
        }

        [Test]
        public void Models_CommitSettings_UebertrageFilename()
        {
            _branchSettings.FileSettingFilename = "bla";

            Assert.That(_branchSettings.Commits, Has.All.Matches<CommitSetting>(c => c.FileSettingFilename == "bla"));
        }

        [Test]
        public void Models_CommitSettings_UebertrageFilenameBeiExistierendemFilename()
        {
            _branchSettings.Commits.First().FileSettingFilename = "blub";
            _branchSettings.FileSettingFilename = "bla";

            Assert.That(_branchSettings.Commits.First().FileSettingFilename, Is.EqualTo("blub"));
            Assert.That(_branchSettings.Commits.Last().FileSettingFilename, Is.EqualTo("bla"));
        }
    }
}
