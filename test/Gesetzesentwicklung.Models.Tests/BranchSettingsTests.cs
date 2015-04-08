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
    public class BranchSettingsTests
    {
        private BranchesSettings _branchSettings;
        private string _serializedBranchSettings;
        private string _serializedBranchSettingsAlternative;
        private List<string> _yamlFiles;

        [SetUp]
        public void SetUp()
        {
            _branchSettings = new BranchesSettings
            {
                Branches = new Dictionary<string, BranchesSettings.BranchTyp>
                {
                    { "Gesetze/GG/Bundesgesetzblatt", BranchesSettings.BranchTyp.Normal },
                    { "Gesetze/GG/Änderung-1", BranchesSettings.BranchTyp.Feature },
                    { "Gesetze/GG/Änderung-2", BranchesSettings.BranchTyp.Feature }
                }
            };

            _yamlFiles = new List<string>
            {
                @"Gesetze\GG\Bundesgesetzblatt.yml",
                @"Gesetze\GG\Änderung-1.yml",
                @"Gesetze\GG\Änderung-2.yml"
            };

            _serializedBranchSettings =
@"Branches:
  Gesetze/GG/Bundesgesetzblatt: Normal
  Gesetze/GG/Änderung-1: Feature
  Gesetze/GG/Änderung-2: Feature
";
            _serializedBranchSettingsAlternative =
@"Branches:
  Gesetze/GG/Bundesgesetzblatt: Normal
  ""Gesetze/GG/Änderung-1"": Feature
  ""Gesetze/GG/Änderung-2"": Feature
";
        }

        [Test]
        public void Models_BranchSettings_Serialize()
        {
            var yaml = YamlStringParser.ToYaml(_branchSettings);

            Assert.That(yaml, Is.EqualTo(_serializedBranchSettings)
                            | Is.EqualTo(_serializedBranchSettingsAlternative));
        }

        [Test]
        public void Models_BranchSettings_Deserialize()
        {
            var settings = YamlStringParser.FromYaml<BranchesSettings>(_serializedBranchSettings);

            Assert.That(settings.Branches, Is.EquivalentTo(_branchSettings.Branches));
            Assert.That(settings.BranchesYamls, Is.EquivalentTo(_yamlFiles));
        }

        [Test]
        public void Models_BranchSettings_YamlFileAbleitung()
        {
            Assert.That(_branchSettings.BranchesYamls, Is.EquivalentTo(_yamlFiles));
        }
    }
}
