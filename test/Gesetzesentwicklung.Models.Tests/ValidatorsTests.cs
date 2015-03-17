using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Models.Tests
{
    [TestFixture]
    public class ValidatorsTests
    {
        [Test]
        public void Models_Validators_InvalidBranchSettings()
        {
            var settings = new BranchesSettings
            {
                Branches = new Dictionary<string, BranchesSettings.BranchTyp>
                {
                    { "Gesetze", BranchesSettings.BranchTyp.Normal },
                    { "Gesetze/GG", BranchesSettings.BranchTyp.Normal }
                }
            };

            IEnumerable<string> validatorMessages;

            Assert.IsFalse(settings.IsValid(out validatorMessages));
            Assert.That(validatorMessages.Count(), Is.EqualTo(1));
            Assert.That(validatorMessages.ElementAt(0), Is.StringEnding("in Konflikt zu einem anderen Branch steht: Gesetze"));
        }

        [Test]
        public void Models_Validators_ValidBranchSettings()
        {
            var settings = new BranchesSettings
            {
                Branches = new Dictionary<string, BranchesSettings.BranchTyp>
                {
                    { "Gesetzesstand", BranchesSettings.BranchTyp.Normal },
                    { "Gesetze/GG", BranchesSettings.BranchTyp.Normal }
                }
            };

            IEnumerable<string> validatorMessages;

            Assert.IsTrue(settings.IsValid(out validatorMessages));
            Assert.IsEmpty(validatorMessages);
        }
    }
}
