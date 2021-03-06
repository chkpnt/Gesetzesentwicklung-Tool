﻿using NUnit.Framework;
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
        public async void GII_VerzeichnisLaderAsync()
        {
            var lader = new XmlVerzeichnisService();
            var gesetzesVerzeichnis = await lader.LadeVerzeichnisAsync();
            Assert.That(gesetzesVerzeichnis.Normen.Count(), Is.GreaterThan(0));
        }
    }
}
