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

namespace Gesetzesentwicklung.Tests
{
    [TestFixture]
    public class DeserializerTests
    {
        private XmlSerializer _deserializer;

        private XmlGesetz _gesetz;

        [SetUp]
        public void Setup()
        {
            _deserializer = new XmlSerializer(typeof(XmlGesetz));
            Assembly.GetExecutingAssembly().GetManifestResourceNames().ToList().ForEach(l => Console.WriteLine(l));
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xml2Markdown.Tests.Resources.demo.xml"))
            {
                _gesetz = _deserializer.Deserialize(stream) as XmlGesetz;
            }
        }

        [Test]
        public void GII_Deserializer()
        {
            Assert.That(_gesetz.Normen, Has.Count.EqualTo(214));
            
            Assert.That(_gesetz.Normen.First().Metadaten.Abkuerzung, Is.EqualTo("GG"));
            Assert.That(_gesetz.Normen.First().Metadaten.Gliederungseinheit, Is.Null);
            Assert.That(_gesetz.Normen.First().Metadaten.Bezeichnung, Is.Null);

            Assert.That(_gesetz.Normen.ElementAt(1).Metadaten.Abkuerzung, Is.EqualTo("GG"));
            Assert.That(_gesetz.Normen.ElementAt(1).Metadaten.Gliederungseinheit, Is.Null);
            Assert.That(_gesetz.Normen.ElementAt(1).Metadaten.Bezeichnung, Is.EqualTo("Eingangsformel"));
            Assert.That(_gesetz.Normen.ElementAt(1).Textdaten.Text, Is.StringStarting("Der Parlamentarische Rat"));

            Assert.That(_gesetz.Normen.ElementAt(4).Metadaten.Abkuerzung, Is.EqualTo("GG"));
            Assert.That(_gesetz.Normen.ElementAt(4).Metadaten.Gliederungseinheit, Is.Null);
            Assert.That(_gesetz.Normen.ElementAt(4).Metadaten.Bezeichnung, Is.EqualTo("Art 1"));
            Assert.That(_gesetz.Normen.ElementAt(4).Textdaten.Text, Is.EqualTo(
@"(1) Die Würde des Menschen ist unantastbar. Sie zu achten und zu schützen ist Verpflichtung aller staatlichen Gewalt.

(2) Das Deutsche Volk bekennt sich darum zu unverletzlichen und unveräußerlichen Menschenrechten als Grundlage jeder menschlichen Gemeinschaft, des Friedens und der Gerechtigkeit in der Welt.

(3) Die nachfolgenden Grundrechte binden Gesetzgebung, vollziehende Gewalt und Rechtsprechung als unmittelbar geltendes Recht."));
        }
    }
}
