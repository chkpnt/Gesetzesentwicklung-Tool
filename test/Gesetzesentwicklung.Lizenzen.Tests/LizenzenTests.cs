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
                          select $@"Projekt: {lizenz.Projekt}
Autor: {lizenz.Autor}
Homepage: {lizenz.Homepage}
Lizenz:
{lizenz.LizenzText}
";

            Console.WriteLine(string.Join(Environment.NewLine, ausgabe.ToArray()));
        }
    }
}
