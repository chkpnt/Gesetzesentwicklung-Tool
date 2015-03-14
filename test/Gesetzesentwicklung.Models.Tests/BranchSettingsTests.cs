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
    public class BranchSettingsTests
    {
        BranchesSettings _branchSettings;
        string _serializedBranchSettings;

        [SetUp]
        public void SetUp()
        {
            _branchSettings = new BranchesSettings
            {
                Branches = new Dictionary<string, BranchesSettings.BranchTyp>
                {
                    { "Bundesgesetzblatt", BranchesSettings.BranchTyp.Normal },
                    { "1. Lesung", BranchesSettings.BranchTyp.Feature },
                    { "2. Lesung", BranchesSettings.BranchTyp.Feature }
                }
            };

            _serializedBranchSettings =
@"Branches:
  Bundesgesetzblatt: Normal
  1. Lesung: Feature
  2. Lesung: Feature
";
        }

        [Test]
        public void Models_SerializeBranchSettings()
        {
            var yaml = toYaml(_branchSettings);

            Assert.That(yaml, Is.EqualTo(_serializedBranchSettings));
        }

        [Test]
        public void Models_DeserializeBranchSettings()
        {
            var settings = fromYaml(_serializedBranchSettings);

            Assert.That(settings.Branches, Is.EquivalentTo(_branchSettings.Branches));
        }

        private string toYaml(BranchesSettings settings)
        {
            using (StringWriter textWriter = new StringWriter())
            {
                var serializer = new Serializer();
                serializer.Serialize(textWriter, settings);
                return textWriter.ToString();
            }
        }

        private BranchesSettings fromYaml(string settings)
        {
            using (StringReader textReader = new StringReader(settings))
            {
                var deserializer = new Deserializer();
                return deserializer.Deserialize<BranchesSettings>(textReader);
            }
        }
    }
}
