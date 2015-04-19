using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Models
{
    public interface IYamlStringParser
    {
        T FromYaml<T>(string settings);
        string ToYaml(object o);
    }
}
