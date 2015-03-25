using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Gesetzesentwicklung.Models;
using Gesetzesentwicklung.GII;

namespace Gesetzesentwicklung.GII.Tests
{
    [TestFixture]
    public class XmlVerzeichnisTests
    {
        private XmlSerializer _deserializer;

        private XmlVerzeichnis _verzeichnis;

        [SetUp]
        public void SetUp()
        {
            _deserializer = new XmlSerializer(typeof(XmlVerzeichnis));
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Gesetzesentwicklung.GII.Tests.Resources.gii-toc.xml"))
            {
                _verzeichnis = _deserializer.Deserialize(stream) as XmlVerzeichnis;
            }
        }

        [Test]
        public void GII_XmlVerzeichnis_Deserializer()
        {
            Assert.That(_verzeichnis.Normen, Has.Count.EqualTo(3));

            Assert.That(_verzeichnis.Normen.ElementAt(1).Titel, Is.EqualTo("Grundgesetz für die Bundesrepublik Deutschland"));
            Assert.That(_verzeichnis.Normen.ElementAt(1).Link, Is.EqualTo(new Uri("http://www.gesetze-im-internet.de/gg/xml.zip")));
        }
    }
}
