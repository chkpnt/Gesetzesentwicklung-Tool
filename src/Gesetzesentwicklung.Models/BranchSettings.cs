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
    public class BranchSettings : FileSetting
    {
        private string _fileSettingFilename;

        public override string FileSettingFilename
        {
            get { return _fileSettingFilename; }
            set
            {
                _fileSettingFilename = value;
                foreach (var commit in Commits.Where(c => c.FileSettingFilename == null))
                {
                    commit.FileSettingFilename = _fileSettingFilename;
                }
            }
        }

        public List<CommitSetting> Commits { get; set; }
    }
}
