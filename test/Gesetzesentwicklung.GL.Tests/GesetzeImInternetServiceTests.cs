using Gesetzesentwicklung.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.GL.Tests
{
    [TestFixture]
    public class GesetzeImInternetServiceTests
    {
        private GesetzeImInternetService _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new GesetzeImInternetService();
        }

        [Test]
        [Category("WithExternalResource")]
        public async void GL_GesetzeImInternetService_DownloadNormAsync()
        {
            var norm = new Gesetzesverzeichnis.Norm
            {
                Titel = "Grundgesetz für die Bundesrepublik Deutschland",
                Link = new Uri("http://www.gesetze-im-internet.de/gg/xml.zip")
            };

            var gesetz = await _classUnderTest.LadeGesetzAusNormZipAsync(norm);

            Assert.That(gesetz.Name, Is.EqualTo("GG"));
            Assert.That(gesetz.Artikel.Count(), Is.EqualTo(199));
        }
    }
}
