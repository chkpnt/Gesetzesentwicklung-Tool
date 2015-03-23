using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Gesetzesentwicklung.Lizenzen
{
    public class GenutzteLizenzen
    {
        public IEnumerable<Lizenz> Lizenzen { get; private set; }

        public GenutzteLizenzen()
        {
            Lizenzen = GetLizenzenFromResources();
        }

        private IEnumerable<Lizenz> GetLizenzenFromResources()
        {
            var deserializer = new Deserializer();
            var lizenzResourcen = from resource in Assembly.GetExecutingAssembly().GetManifestResourceNames()
                                  where resource.StartsWith("Gesetzesentwicklung.Lizenzen.Lizenzen") && resource.EndsWith(".yaml")
                                  select resource;

            foreach (var lizenzResource in lizenzResourcen)
            {
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(lizenzResource))
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        yield return deserializer.Deserialize<Lizenz>(streamReader);
                    }
                }
            }
        }
    }
}
