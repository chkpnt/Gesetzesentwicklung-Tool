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
    public class CommitSettingTests
    {
        private CommitSetting _commitSetting;
        private string _serializedCommitSetting;
        private IYamlStringParser _yamlStringParser;

        [SetUp]
        public void SetUp()
        {
            _commitSetting = new CommitSetting
            {
                Autor = "Foo Bar <foo@bar.net>",
                Datum = DateTime.Parse("01/01/2015"),
                Beschreibung = "blabla",
                MergeInto = "Gesetze/GG/Bundesgesetzblatt"
            };

            _serializedCommitSetting =
@"MergeInto: Gesetze/GG/Bundesgesetzblatt
Autor: Foo Bar <foo@bar.net>
Datum: 01.01.2015
Beschreibung: blabla
";

            _yamlStringParser = new YamlStringParser();
        }

        [Test]
        public void Models_CommitSetting_Serialize()
        {
            var yaml = _yamlStringParser.ToYaml(_commitSetting);

            Assert.That(yaml, Is.EqualTo(_serializedCommitSetting));
        }

        [Test]
        public void Models_CommitSetting_Deserialize()
        {
            var setting = _yamlStringParser.FromYaml<CommitSetting>(_serializedCommitSetting);

            Assert.That(setting, Is.EqualTo(_commitSetting));
        }

        [Test]
        public void Models_CommitSetting_Order()
        {
            var commit_Oktober = new CommitSetting { Datum = DateTime.Parse("1960-10-08") };
            var commit_November = new CommitSetting { Datum = DateTime.Parse("1960-11-10") }; 
            var commit_Februar = new CommitSetting { Datum = DateTime.Parse("1960-02-04") };

            var unsortedCommits = new List<List<CommitSetting>>
            {
                new List<CommitSetting> { commit_Februar, commit_November, commit_Oktober },
                new List<CommitSetting> { commit_Oktober, commit_November, commit_Februar },
                new List<CommitSetting> { commit_Oktober, commit_Februar, commit_November },
                new List<CommitSetting> { commit_November, commit_Oktober, commit_Februar },
                new List<CommitSetting> { commit_November, commit_Februar, commit_Oktober }
            };
            var sortedCommits = new List<CommitSetting> { commit_Februar, commit_Oktober, commit_November };

            Assert.That(unsortedCommits, Has.All.Not.Ordered);
            Assert.That(sortedCommits, Is.Ordered);
        }

        [Test]
        public void Models_CommitSetting_OrderBeiNull()
        {
            var commitMitNull = new CommitSetting { };
            var commitOhneNull = new CommitSetting { Datum = DateTime.Parse("1960-11-10") };

            Assert.That(commitMitNull.Datum, Is.EqualTo(DateTime.MinValue));

            var liste1 = new List<CommitSetting> { commitMitNull, commitOhneNull };
            var liste2 = new List<CommitSetting> { commitOhneNull, commitMitNull };

            Assert.That(liste1, Is.Ordered);
            Assert.That(liste2, Is.Not.Ordered);
        }

        [Test]
        public void Models_CommitSetting_ToString()
        {
            var expected = "CommitSetting [Autor: Foo Bar <foo@bar.net>, " + 
                           "Datum: 01.01.2015, " +
                           "Beschreibung: \"blabla\", " + 
                           "BranchFrom: , " + 
                           "MergeInto: Gesetze/GG/Bundesgesetzblatt]";

            Assert.That(_commitSetting.ToString(), Is.EqualTo(expected));
        }
    }
}
