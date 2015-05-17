using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gesetzesentwicklung.Shared;
using YamlDotNet.Serialization;
using System.Net.Mail;

namespace Gesetzesentwicklung.Models
{
    public abstract class FileSetting
    {
        [YamlIgnore]
        public virtual string FileSettingFilename { get; set; }
    }
}
