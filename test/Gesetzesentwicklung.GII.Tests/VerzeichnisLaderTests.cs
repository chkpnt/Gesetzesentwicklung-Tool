using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.GII.Tests
{
    [TestFixture]
    public class VerzeichnisLaderTests
    {
        [Test]
        [Category("WithExternalResource")]
        public async void GII_VerzeichnisLader()
        {
            var lader = new VerzeichnisLader();
            var xmlVerzeichnis = await lader.LadeVerzeichnis();
            Assert.That(xmlVerzeichnis.Normen, Has.Count.GreaterThan(0));
        }
    }
}
