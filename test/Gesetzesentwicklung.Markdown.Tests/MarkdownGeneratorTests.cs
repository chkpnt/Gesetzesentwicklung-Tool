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
        MarkdownGenerator _generator;

        Gesetz _gesetz;

        const string OutputFolder = "output";

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

            _generator = new MarkdownGenerator(_gesetz, OutputFolder);

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
            _generator.buildMarkdown();

            Assert.IsTrue(Directory.Exists(Path.Combine(OutputFolder, "GG")));
            Assert.IsTrue(Directory.Exists(Path.Combine(OutputFolder, "GG", "I. Die Grundrechte")));
        }

        [Test]
        public void testProArtikelEineDatei()
        {
            _generator.buildMarkdown();

            Assert.IsTrue(File.Exists(Path.Combine(OutputFolder, "GG", "Präambel.md")));
            Assert.IsTrue(File.Exists(Path.Combine(OutputFolder, "GG", "I. Die Grundrechte", "Artikel 1.md")));
        }

        [Test]
        public void testRichtigerInhaltInDenDateien()
        {
            _generator.buildMarkdown();

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
