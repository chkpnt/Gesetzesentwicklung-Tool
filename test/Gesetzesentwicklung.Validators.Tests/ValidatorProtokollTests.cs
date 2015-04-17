using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Validators.Tests
{
    [TestFixture]
    public class ValidatorProtokollTests
    {
        private ValidatorProtokoll _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new ValidatorProtokoll();
        }

        [Test]
        public void Validator_ValidatorProtokoll()
        {
            _classUnderTest.AddEntry("Ein Eintrag");
            _classUnderTest.AddEntries(new List<string> { "foo", "bar" });
            _classUnderTest.AddEntry("c:\tmp", "a");
            _classUnderTest.AddEntries("d:\tmp", new List<string> { "b" });

            var expectedMessage = new List<string> { "Ein Eintrag", "foo", "bar", "a", "b" };
            Assert.That(_classUnderTest.Entries.Select(e => e.Message), Is.EquivalentTo(expectedMessage));

            var expectedFilenames = new List<string> { "", "", "", "c:\tmp", "d:\tmp" };
        }
    }
}
