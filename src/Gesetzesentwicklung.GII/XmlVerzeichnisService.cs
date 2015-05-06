using Gesetzesentwicklung.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Gesetzesentwicklung.GII
{
    public class XmlVerzeichnisService
    {
        private static readonly Uri XmlVerzeichnisUri = new Uri("http://www.gesetze-im-internet.de/gii-toc.xml");

        public async Task<Gesetzesverzeichnis> LadeVerzeichnisAsync()
        {
            using (var webclient = new WebClient())
            {
                var stream = await webclient.OpenReadTaskAsync(XmlVerzeichnisUri);
                var xmlVerzeichnis = ParseXml(stream);
                var modelConverter = new ModelConverter();
                return modelConverter.Convert(xmlVerzeichnis);
            }
        }

        private XmlVerzeichnis ParseXml(Stream stream)
        {
            var deserializer = new XmlSerializer(typeof(XmlVerzeichnis));
            var xmlReaderSetting = new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse };

            using (var xmlReader = XmlReader.Create(stream, xmlReaderSetting))
            {
                return deserializer.Deserialize(xmlReader) as XmlVerzeichnis;
            }
        }
    }
}
