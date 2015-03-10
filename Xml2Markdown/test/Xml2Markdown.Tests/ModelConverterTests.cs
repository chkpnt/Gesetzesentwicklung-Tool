using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xml2Markdown.Models;

namespace Xml2Markdown.Tests
{
    [TestFixture]
    class ModelConverterTests
    {
        ModelConverter _converter;

        XmlGesetz _xmlGesetz;

        [SetUp]
        public void setup()
        {
            _converter = new ModelConverter();

            _xmlGesetz = new XmlGesetz
            {
                Normen = new List<XmlGesetz.Norm>
                {
                    neuerGesetzesname("TEST"),
                    neuerArtikel("Präambel", "Präambel-Text"),
                    neueGliederungseinheit("I.", "Kapitel A"),
                    neuerArtikel("Art 1", "Kapitel A, Artikel 1"),
                    neuerArtikel("Art 2", "Kapitel A, Artikel 2"),
                    neueGliederungseinheit("II.", "Kapitel B"),
                    neuerArtikel("Art 3", "Kapitel B, Artikel 3")
                }
            };
        }

        [Test]
        public void testGesetzesnameVonErsterNorm()
        {
            var gesetz = _converter.Convert(_xmlGesetz);

            Assert.That(gesetz.Name, Is.EqualTo("TEST"));
        }

        [Test]
        public void testAbschnittsnameIstRichtigZusammengesetz()
        {
            var gesetz = _converter.Convert(_xmlGesetz);

            Assert.That(gesetz.Artikel.ElementAt(2).Abschnitt.Name, Is.EqualTo("I. Kapitel A"));
        }

        [Test]
        public void testArtikelStattArt()
        {
            var gesetz = _converter.Convert(_xmlGesetz);

            var artikelnamen = from artikel in gesetz.Artikel
                               where artikel.Name.StartsWith("Art")
                               select artikel.Name;

            Assert.That(artikelnamen, Is.All.StartsWith("Artikel"));
        }

        [Test]
        public void testArtikelInRichtigenAbschnitten()
        {
            var gesetz = _converter.Convert(_xmlGesetz);

            var artikelOhneAbschnitt = from artikel in gesetz.Artikel
                                       where artikel.Abschnitt == null
                                       select artikel;
            Assert.That(artikelOhneAbschnitt, Has.Exactly(1).Matches<Artikel>(a => a.Inhalt == "Präambel-Text"));

            var artikelInAbschnitt1 = from artikel in gesetz.Artikel
                                      where artikel.Abschnitt != null && artikel.Abschnitt.Name == "I. Kapitel A"
                                      select artikel;
            Assert.That(artikelInAbschnitt1, Has.Exactly(1).Matches<Artikel>(a => a.Inhalt == "Kapitel A, Artikel 1"));
            Assert.That(artikelInAbschnitt1, Has.Exactly(1).Matches<Artikel>(a => a.Inhalt == "Kapitel A, Artikel 2"));


            var artikelInAbschnitt2 = from artikel in gesetz.Artikel
                                      where artikel.Abschnitt != null && artikel.Abschnitt.Name == "II. Kapitel B"
                                      select artikel;
            Assert.That(artikelInAbschnitt2, Has.Exactly(1).Matches<Artikel>(a => a.Inhalt == "Kapitel B, Artikel 3"));
        }

        private XmlGesetz.Norm neuerGesetzesname(string abkuerzung)
        {
            return new XmlGesetz.Norm
            {
                Metadaten = new XmlGesetz.Metadaten
                {
                    Abkuerzung = abkuerzung,
                    Bezeichnung = null,
                    Gliederungseinheit = null
                },
                Textdaten = neuerText("")
            };
        }
        private XmlGesetz.Norm neueGliederungseinheit(string bezeichnung, string titel)
        {
            return new XmlGesetz.Norm
            {
                Metadaten = new XmlGesetz.Metadaten
                {
                    Abkuerzung = "egal",
                    Bezeichnung = null,
                    Gliederungseinheit = new XmlGesetz.Gliederungseinheit
                    {
                        Bezeichnung = bezeichnung,
                        Titel = titel
                    }
                },
                Textdaten = neuerText("")
            };
        }

        private XmlGesetz.Norm neuerArtikel(string bezeichnung, string inhalt)
        {
            return new XmlGesetz.Norm
            {
                Metadaten = new XmlGesetz.Metadaten
                { 
                    Abkuerzung = "egal",
                    Bezeichnung = bezeichnung,
                    Gliederungseinheit = null
                },
                Textdaten = neuerText(inhalt)
            };
        }

        private XmlGesetz.Textdaten neuerText(string inhalt)
        {
            return new XmlGesetz.Textdaten
            {
                Text =  inhalt
            };
        }
    }
}
