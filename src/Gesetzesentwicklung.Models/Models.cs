using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Models
{
    public class Gesetz
    {
        public string Name { get; set; }
        public IEnumerable<Artikel> Artikel { get; set; }
    }

    public class Artikel
    {
        public string Abschnitt { get; set; }
        public string Name { get; set; }
        public string Inhalt { get; set; }
    }

    public class Abschnitt
    {
        public string Name { get; set; }
    }

    public class Gesetzesverzeichnis
    {
        public IEnumerable<Norm> Normen { get; set; }

        public class Norm
        {
            public string Titel { get; set; }
            public Uri Link { get; set; }
        }
    }
}
