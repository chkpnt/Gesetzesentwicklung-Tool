using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Gesetzesentwicklung.GII
{
    // XmlSerializer verlangt hier eine public-Visibility,
    // drum wenigstens ein internal-Konstruktor
    // Evtl. wechseln zu DataContractSerializer? 
    [XmlRoot("items")]
    public class XmlVerzeichnis
    {
        [XmlElement("item")]
        public List<Norm> Normen { get; set; }

        public class Norm
        {
            [XmlElement("title")]
            public string Titel { get; set; }

            [XmlElement("link")]
            public string Link { get; set; }
        }
    }
}
