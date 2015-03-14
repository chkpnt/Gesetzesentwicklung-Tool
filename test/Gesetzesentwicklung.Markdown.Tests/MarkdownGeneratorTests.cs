using Gesetzesentwicklung.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Markdown.Tests
{
    [TestFixture]
    class MarkdownGeneratorTests
    {
        private const string OutputFolder = "output";
        
        private MarkdownGenerator _generator;

        private Gesetz _gesetz;

        private GesetzesentwicklungSettings _settings;

        [SetUp]
        public void SetUp()
        {
            _gesetz = new Gesetz
            {
                Name = "GG",
                Artikel = new List<Artikel>
                {
                    neuerArtikel("Präambel", "Präambel-Text"),
                    neuerArtikel("I. Die Grundrechte", "Artikel 1",
@"(1) Absatz 1

(2) Absatz 2")
                }
            };

            _settings = new GesetzesentwicklungSettings
            {
                Autor = "Foo Bar <foo@example.net>",
                Datum = DateTime.Parse("01/01/2015 00:00:00"),
                Zeitzone = "CET",
                Beschreibung = @"Commit-Message

Letzte Zeile"
            };

            _generator = new MarkdownGenerator(_gesetz, _settings, OutputFolder);

            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }
        }

        [TearDown]
        public void TestDown()
        {
            Directory.Delete(OutputFolder, recursive: true);
        }

        [Test]
        public void testAufRichtigeVerzeichnisstruktur()
        {
            _generator.generate();

            Assert.IsTrue(Directory.Exists(Path.Combine(OutputFolder, "GG")));
            Assert.IsTrue(Directory.Exists(Path.Combine(OutputFolder, "GG", "I. Die Grundrechte")));
        }

        [Test]
        public void testProArtikelEineDatei()
        {
            _generator.generate();

            Assert.IsTrue(File.Exists(Path.Combine(OutputFolder, "GG", "Präambel.md")));
            Assert.IsTrue(File.Exists(Path.Combine(OutputFolder, "GG", "I. Die Grundrechte", "Artikel 1.md")));
        }

        [Test]
        public void testRichtigerInhaltInDenDateien()
        {
            _generator.generate();

            Assert.That(File.ReadAllText(Path.Combine(OutputFolder, "GG", "Präambel.md"), Encoding.UTF8),
                Is.EqualTo(
@"# Präambel

Präambel-Text"));
            Assert.That(File.ReadAllText(Path.Combine(OutputFolder, "GG", "I. Die Grundrechte", "Artikel 1.md"), Encoding.UTF8),
                Is.EqualTo(
@"# Artikel 1

(1) Absatz 1

(2) Absatz 2"));
        }

        [Test]
        public void testSettingsDatei()
        {
            _generator.generate();

            Assert.IsTrue(File.Exists(Path.Combine(OutputFolder, "GG", GesetzesentwicklungSettings.Filename)));
            Assert.That(File.ReadAllText(Path.Combine(OutputFolder, "GG", GesetzesentwicklungSettings.Filename), Encoding.UTF8),
                Is.EqualTo(
@"Autor: Foo Bar <foo@example.net>
Datum: 2015-01-01T00:00:00.0000000
Zeitzone: CET
Beschreibung: >-
  Commit-Message



  Letzte Zeile
"));
        }

        private Artikel neuerArtikel(string titel, string inhalt)
        {
            return neuerArtikel(null, titel, inhalt);
        }

        private Artikel neuerArtikel(string abschnitt, string name, string inhalt)
        {
            return new Artikel
            {
                Abschnitt = abschnitt,
                Name = name,
                Inhalt = inhalt
            };
        }
    }
}
