using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Gesetzesentwicklung.GII
{
    // XmlSerializer verlangt hier eine public-Visibility,
    // drum wenigstens ein internal-Konstruktor
    // Evtl. wechseln zu DataContractSerializer? 
    [XmlRoot("dokumente")]
    public class XmlGesetz
    {
        [XmlElement("norm")]
        public List<Norm> Normen { get; set; }

        public class Norm
        {
            [XmlElement("metadaten")]
            public Metadaten Metadaten { get; set; }

            [XmlElement("textdaten")]
            public Textdaten Textdaten { get; set; }
        }

        public class Metadaten
        {
            [XmlElement("jurabk")]
            public string Abkuerzung { get; set; }

            [XmlElement("enbez")]
            public string Bezeichnung { get; set; }

            [XmlElement("gliederungseinheit")]
            public Gliederungseinheit Gliederungseinheit { get; set; }
        }

        public class Gliederungseinheit
        {
            [XmlElement("gliederungsbez")]
            public string Bezeichnung { get; set; }

            [XmlElement("gliederungstitel")]
            public string Titel { get; set; }
        }

        public class Textdaten : IXmlSerializable
        {
            public string Text;

            public System.Xml.Schema.XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(System.Xml.XmlReader reader)
            {
                var doc = XDocument.Parse(reader.ReadOuterXml());

                var paragraphs = from p in doc.Descendants("P")
                                 select p.Value;

                this.Text = string.Join(Environment.NewLine + Environment.NewLine, paragraphs);
            }

            public void WriteXml(System.Xml.XmlWriter writer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
