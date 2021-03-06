﻿using NUnit.Framework;
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
                _Ziel = "/",
                MergeInto = "Gesetze/GG/Bundesgesetzblatt",
                Tag = "GitTag",
                _Autor = "Foo Bar <foo@bar.net>",
                _Datum = "01.01.2015",
                Beschreibung = "blabla"
            };

            _serializedCommitSetting =
@"Ziel: /
MergeInto: Gesetze/GG/Bundesgesetzblatt
Tag: GitTag
Datum: 01.01.2015
Autor: '""Foo Bar"" <foo@bar.net>'
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
        public void Models_CommitSetting_Mapping_Ziel()
        {
            var commitSetting = new CommitSetting();

            Assert.That(commitSetting.Ziel, Is.Null);
            commitSetting._Ziel = "/";
            Assert.That(commitSetting.Ziel, Is.EqualTo(""));
            commitSetting._Ziel = "/GG/Foobar";
            Assert.That(commitSetting.Ziel, Is.EqualTo(@"GG\Foobar"));

            commitSetting.Ziel = null;
            Assert.That(commitSetting._Ziel, Is.Null);
            commitSetting.Ziel = "";
            Assert.That(commitSetting._Ziel, Is.EqualTo("/"));
            commitSetting.Ziel = @"GG\Foobar";
            Assert.That(commitSetting._Ziel, Is.EqualTo("/GG/Foobar"));
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
            var commitMitNull = new CommitSetting();
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
            var expected = "CommitSetting [Autor: \"Foo Bar\" <foo@bar.net>, " +
                           "Datum: 01.01.2015, " +
                           "Beschreibung: \"blabla\", " +
                           "BranchFrom: , " +
                           "MergeInto: Gesetze/GG/Bundesgesetzblatt]";

            Assert.That(_commitSetting.ToString(), Is.EqualTo(expected));
        }
    }
}
