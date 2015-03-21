using Gesetzesentwicklung.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Validators
{
    public class CommitSettingValidator
    {
        private readonly IFileSystem _fileSystem;

        internal CommitSettingValidator(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public CommitSettingValidator() : this(fileSystem: new FileSystem())
        {
        }

        public bool IsValid(CommitSetting commitSetting, string parentDir, BranchesSettings branchSettings)
        {
            ValidatorProtokoll protokoll = new ValidatorProtokoll();
            return IsValid(commitSetting, parentDir, branchSettings, ref protokoll);
        }

        public bool IsValid(CommitSetting commitSetting, string parentDir, BranchesSettings branchSettings, ref ValidatorProtokoll protokoll)
        {
            return _fileSystem.DirectoryInfo.FromDirectoryName(Path.Combine(parentDir, commitSetting.Daten)).Exists;
        }
    }
}
