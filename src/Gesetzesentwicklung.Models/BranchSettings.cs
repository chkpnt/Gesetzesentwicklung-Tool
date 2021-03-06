﻿using System;
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

        private string _branch;

        // Derzeit abgeleitet über den Dateinamen
        [YamlIgnore]
        public string Branch
        {
            get { return _branch; }
            set
            {
                _branch = value;
                foreach (var commit in Commits.Where(c => c.Branch == null))
                {
                    commit.Branch = _branch;
                }
            }
        }

        [YamlMember(Order = 0)]
        public string AutoMergeInto { get; set; }

        [YamlMember(Order = 1)]
        public List<CommitSetting> Commits { get; set; }
    }
}
