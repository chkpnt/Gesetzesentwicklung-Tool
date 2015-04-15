using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Shared.Tests
{
    [TestFixture]
    public class DateTimeExtensionsTests
    {
        [Test]
        public void Shared_DateTimeExtensions_ToEpoch()
        {
            Assert.That(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToEpoch(), Is.EqualTo(0));
            Assert.That(new DateTime(1970, 1, 2, 0, 0, 0, DateTimeKind.Utc).ToEpoch(), Is.EqualTo(86400));
            Assert.That(DateTime.ParseExact("01.01.1970 00:00:01 -01:00", "dd.MM.yyyy HH:mm:ss zzz", null).ToEpoch(), Is.EqualTo(3601));
        }
    }
}
