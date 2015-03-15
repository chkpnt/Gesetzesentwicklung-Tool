using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Shared.Tests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void Shared_StringExtensions_ToLiteral()
        {
            var str = @"Zeile 1
Zeile 2.";

            Assert.That(str.ToLiteral(), Is.EqualTo(@"""Zeile 1\r\nZeile 2."""));
        }
    }
}
