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
    public class CommitSettingTests
    {
        private CommitSetting _commitSetting;
        private string _serializedCommitSetting;

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
Datum: 2015-01-01T00:00:00.0000000
Beschreibung: blabla
";
        }

        [Test]
        public void Models_CommitSetting_Serialize()
        {
            var yaml = Commons.ToYaml(_commitSetting);

            Assert.That(yaml, Is.EqualTo(_serializedCommitSetting));
        }

        [Test]
        public void Models_CommitSetting_Deserialize()
        {
            var setting = Commons.FromYaml<CommitSetting>(_serializedCommitSetting);

            Assert.That(setting, Is.EqualTo(_commitSetting));
        }
    }
}
