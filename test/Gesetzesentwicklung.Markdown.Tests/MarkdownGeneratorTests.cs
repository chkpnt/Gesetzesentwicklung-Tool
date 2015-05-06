using Gesetzesentwicklung.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Markdown.Tests
{
    [TestFixture]
    class MarkdownGeneratorTests
    {
        private const string OutputFolder = @"c:\data\GesetzeImInternet\output\";
        
        private MarkdownGenerator _generator;

        private Gesetz _gesetz;

        private CommitSetting _settings;

        private IFileSystem _fileSystem;

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

            _settings = new CommitSetting
            {
                _Autor = "Foo Bar <foo@example.net>",
                _Datum = "01.01.2015",
                Beschreibung = @"Commit-Message

Letzte Zeile"
            };

            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { @"c:\data\GesetzeImInternet\", new MockDirectoryData()}
                });
            _generator = new MarkdownGenerator(_fileSystem);
        }

        [Test]
        public void Markdown_RichtigeVerzeichnisstruktur()
        {
            _generator.generate(_gesetz, _settings, OutputFolder);

            Assert.IsTrue(_fileSystem.DirectoryInfo.FromDirectoryName(Path.Combine(OutputFolder, "GG")).Exists);
            Assert.IsTrue(_fileSystem.DirectoryInfo.FromDirectoryName(Path.Combine(OutputFolder, "GG", "I. Die Grundrechte")).Exists);
        }

        [Test]
        public void Markdown_ProArtikelEineDatei()
        {
            _generator.generate(_gesetz, _settings, OutputFolder);

            Assert.IsTrue(_fileSystem.FileInfo.FromFileName(Path.Combine(OutputFolder, "GG", "Präambel.md")).Exists);
            Assert.IsTrue(_fileSystem.FileInfo.FromFileName(Path.Combine(OutputFolder, "GG", "I. Die Grundrechte", "Artikel 1.md")).Exists);
        }

        [Test]
        public void Markdown_RichtigerInhaltInDenDateien()
        {
            _generator.generate(_gesetz, _settings, OutputFolder);

            Assert.That(_fileSystem.File.ReadAllText(Path.Combine(OutputFolder, "GG", "Präambel.md"), Encoding.UTF8),
                Is.EqualTo(
@"# Präambel

Präambel-Text"));
            Assert.That(_fileSystem.File.ReadAllText(Path.Combine(OutputFolder, "GG", "I. Die Grundrechte", "Artikel 1.md"), Encoding.UTF8),
                Is.EqualTo(
@"# Artikel 1

(1) Absatz 1

(2) Absatz 2"));
        }

        [Test]
        public void Markdown_SettingsDatei()
        {
            _generator.generate(_gesetz, _settings, OutputFolder);

            Assert.IsTrue(_fileSystem.File.Exists(Path.Combine(OutputFolder, "GG.yml")));
            Assert.That(_fileSystem.File.ReadAllText(Path.Combine(OutputFolder, "GG.yml"), Encoding.UTF8),
                Is.EqualTo(
@"Autor: '""Foo Bar"" <foo@example.net>'
Datum: 01.01.2015
Beschreibung: >-
  Commit-Message


  Letzte Zeile
"));
        }

        private Artikel neuerArtikel(string titel, string inhalt) => neuerArtikel(null, titel, inhalt);

        private Artikel neuerArtikel(string abschnitt, string name, string inhalt) => new Artikel
        {
            Abschnitt = abschnitt,
            Name = name,
            Inhalt = inhalt
        };
    }
}
