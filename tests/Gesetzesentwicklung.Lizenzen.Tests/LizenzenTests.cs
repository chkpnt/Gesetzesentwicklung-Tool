using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Lizenzen.Tests
{
    [TestFixture]
    public class LizenzenTests
    {
        private GenutzteLizenzen _lizenzen;

        [SetUp]
        public void SetUp()
        {
            _lizenzen = new GenutzteLizenzen();
        }

        [Test]
        public void Lizenzen_AusgabeDerGenutztenLizenzen()
        {
            var ausgabe = from lizenz in _lizenzen.Lizenzen
                          select string.Format(@"Projekt: {0}
Autor: {1}
Homepage: {2}
Lizenz:
{3}
", lizenz.Projekt, lizenz.Autor, lizenz.Homepage, lizenz.LizenzText);

            Console.WriteLine(string.Join(Environment.NewLine, ausgabe.ToArray()));
        }
    }
}
