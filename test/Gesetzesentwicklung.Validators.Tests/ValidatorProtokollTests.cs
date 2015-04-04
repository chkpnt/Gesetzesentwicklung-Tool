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

            var expected = new List<string> { "Ein Eintrag", "foo", "bar" };
            Assert.That(_classUnderTest.Entries, Is.EquivalentTo(expected));
        }
    }
}
