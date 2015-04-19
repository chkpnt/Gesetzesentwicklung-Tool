using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Models.Tests
{
    [TestFixture]
    public class CommitSettingsTests
    {
        private CommitSettings _commitSettings;
        private string _serializedCommitSettings;
        private IYamlStringParser _yamlStringParser;

        [SetUp]
        public void SetUp()
        {
            _commitSettings = new CommitSettings
            {
                Commits = new List<CommitSetting>
                {
                    new CommitSetting { Autor = "Foo Bar <foo@bar.net>", Datum = DateTime.Parse("01/01/2015") },
                    new CommitSetting { Autor = "Foo Bar <foo@bar.net>" , Beschreibung = 
@"Zeile 1
Zeile 2"}
                }
            };

            _serializedCommitSettings =
@"Commits:
- Autor: Foo Bar <foo@bar.net>
  Datum: 01.01.2015
- Autor: Foo Bar <foo@bar.net>
  Beschreibung: >-
    Zeile 1

    Zeile 2
";

            _yamlStringParser = new YamlStringParser();
        }

        [Test]
        public void Models_CommitSettings_Serialize()
        {
            var yaml = _yamlStringParser.ToYaml(_commitSettings);

            Assert.That(yaml, Is.EqualTo(_serializedCommitSettings));
        }

        [Test]
        public void Models_CommitSettings_Deserialize()
        {
            var settings = _yamlStringParser.FromYaml<CommitSettings>(_serializedCommitSettings);

            Assert.That(settings.Commits, Is.EquivalentTo(_commitSettings.Commits));
        }

        [Test]
        public void Models_CommitSettings_UebertrageFilename()
        {
            _commitSettings.FileSettingFilename = "bla";

            Assert.That(_commitSettings.Commits, Has.All.Matches<CommitSetting>(c => c.FileSettingFilename == "bla"));
        }

        [Test]
        public void Models_CommitSettings_UebertrageFilenameBeiExistierendemFilename()
        {
            _commitSettings.Commits.First().FileSettingFilename = "blub";
            _commitSettings.FileSettingFilename = "bla";

            Assert.That(_commitSettings.Commits.First().FileSettingFilename, Is.EqualTo("blub"));
            Assert.That(_commitSettings.Commits.Last().FileSettingFilename, Is.EqualTo("bla"));
        }
    }
}
