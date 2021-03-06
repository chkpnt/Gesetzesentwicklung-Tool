﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Gesetzesentwicklung.Models
{
    public class YamlStringParser : IYamlStringParser
    {
        public T FromYaml<T>(string settings)
        {
            using (StringReader textReader = new StringReader(settings))
            {
                var deserializer = new Deserializer();
                return deserializer.Deserialize<T>(textReader);
            }
        }

        public string ToYaml(object o)
        {
            using (StringWriter textWriter = new StringWriter())
            {
                var serializer = new Serializer();
                serializer.Serialize(textWriter, o);
                return textWriter.ToString();
            }
        }
    }
}
