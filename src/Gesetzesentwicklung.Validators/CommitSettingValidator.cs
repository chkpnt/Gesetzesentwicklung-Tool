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
        private static string Message_VerzeichnisFehlt = "Verzeichnis fehlt: {0}";

        private readonly IFileSystem _fileSystem;

        public CommitSettingValidator() : this(fileSystem: new FileSystem()) { }

        public CommitSettingValidator(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public bool IsValid(CommitSetting commitSetting, string parentDir, BranchesSettings branchSettings)
        {
            ValidatorProtokoll protokoll = new ValidatorProtokoll();
            return IsValid(commitSetting, parentDir, branchSettings, ref protokoll);
        }

        public bool IsValid(CommitSetting commitSetting, string parentDir, BranchesSettings branchSettings, ref ValidatorProtokoll protokoll)
        {
            if (commitSetting.Daten == null)
            {
                return true;
            }

            var dir = _fileSystem.DirectoryInfo.FromDirectoryName(Path.Combine(parentDir, commitSetting.Daten));
            var exists = dir.Exists;

            if (!exists)
            {
                var message = string.Format(Message_VerzeichnisFehlt, dir.FullName);
                protokoll.AddEntry(message);
            }

            return exists;
        }
    }
}
