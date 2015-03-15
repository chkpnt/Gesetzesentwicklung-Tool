using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Gesetzesentwicklung.Models.Tests
{
    [TestFixture]
    public class CommitSettingsTests
    {
        private CommitSettings _commitSettings;
        private string _serializedCommitSettings;

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
  Datum: 2015-01-01T00:00:00.0000000
- Autor: Foo Bar <foo@bar.net>
  Beschreibung: >-
    Zeile 1

    Zeile 2
";
        }

        [Test]
        public void Models_CommitSettings_Serialize()
        {
            var yaml = Commons.ToYaml(_commitSettings);

            Assert.That(yaml, Is.EqualTo(_serializedCommitSettings));
        }

        [Test]
        public void Models_CommitSettings_Deserialize()
        {
            var settings = Commons.FromYaml<CommitSettings>(_serializedCommitSettings);

            Assert.That(settings.Commits, Is.EquivalentTo(_commitSettings.Commits));
        }
    }
}
