using Gesetzesentwicklung.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Gesetzesentwicklung.GII
{
    public class XmlGesetzService
    {
        public Gesetz ParseXml(Stream stream)
        {
            var deserializer = new XmlSerializer(typeof(XmlGesetz));
            var xmlReaderSetting = new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse };

            using (var xmlReader = XmlReader.Create(stream, xmlReaderSetting))
            {
                var xmlGesetz = deserializer.Deserialize(xmlReader) as XmlGesetz;
                var modelConverter = new ModelConverter();
                return modelConverter.Convert(xmlGesetz);
            }
        }
    }
}
