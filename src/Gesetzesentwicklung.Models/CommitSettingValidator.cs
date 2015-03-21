using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Models
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
            IEnumerable<string> notNeededValidatorMessages;
            return IsValid(commitSetting, parentDir, branchSettings, out notNeededValidatorMessages);
        }

        public bool IsValid(CommitSetting commitSetting, string parentDir, BranchesSettings branchSettings, out IEnumerable<string> validatorMessages)
        {
            validatorMessages = Enumerable.Empty<string>();

            return _fileSystem.DirectoryInfo.FromDirectoryName(Path.Combine(parentDir, commitSetting.Daten)).Exists;
        }
    }
}
