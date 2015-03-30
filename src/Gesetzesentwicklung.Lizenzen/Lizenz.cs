using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Lizenzen
{
    public class Lizenz
    {
        public string Projekt { get; set; }
        public string Autor { get; set; }
        public Uri Homepage { get; set; }
        public string LizenzAbk { get; set; }
        public string LizenzText { get; set; }
    }
}
