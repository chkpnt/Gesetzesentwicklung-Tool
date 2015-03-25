using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Gesetzesentwicklung.GII
{
    [XmlRoot("items")]
    public class XmlVerzeichnis
    {
        [XmlElement("item")]
        public List<Norm> Normen { get; set; }

        public class Norm
        {
            [XmlElement("title")]
            public string Titel { get; set; }

            [XmlIgnore()]
            public Uri Link { get; set; }

            [XmlElement("link")]
            public string _Link
            {
                get { return Link.ToString(); }
                set { Link = new Uri(value); }
            }
        }
    }
}
