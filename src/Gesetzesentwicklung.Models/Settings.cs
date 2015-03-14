using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Models
{
    public class GesetzesentwicklungSettings
    {
        public static string Filename = "gesetzesentwicklung.yml";

        public string Autor { get; set; }
        public DateTime Datum { get; set; }
        public string Zeitzone { get; set; }

        // [YamlMember(ScalarStyle = ScalarStyle.Literal)]
        public string Beschreibung { get; set; }
    }

    public class BranchesSettings
    {
        public enum BranchTyp
        {
            Feature,
            Normal
        }

        public Dictionary<string, BranchTyp> Branches { get; set; }
    }
}
