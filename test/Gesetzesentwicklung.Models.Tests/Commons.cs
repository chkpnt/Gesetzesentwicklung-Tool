using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Gesetzesentwicklung.Models.Tests
{
    class Commons
    {
        internal static string ToYaml(object settings)
        {
            using (StringWriter textWriter = new StringWriter())
            {
                var serializer = new Serializer();
                serializer.Serialize(textWriter, settings);
                return textWriter.ToString();
            }
        }

        internal static T FromYaml<T>(string settings)
        {
            using (StringReader textReader = new StringReader(settings))
            {
                var deserializer = new Deserializer();
                return deserializer.Deserialize<T>(textReader);
            }
        }
    }
}
