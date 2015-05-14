using Gesetzesentwicklung.Models;
using Gesetzesentwicklung.Shared;
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

        public bool IsValid(CommitSetting commitSetting, string parentDir)
        {
            var protokoll = new ValidatorProtokoll();
            return IsValid(commitSetting, parentDir, ref protokoll);
        }

        public bool IsValid(CommitSetting commitSetting, string parentDir, ref ValidatorProtokoll protokoll)
        {
            var valid = true;
            valid &= IsValid_Daten(commitSetting, parentDir, ref protokoll);
            valid &= IsValid_Ziel(commitSetting, parentDir, ref protokoll);
            valid &= IsValid_Datum(commitSetting, parentDir, ref protokoll);
            return valid;
        }
        private bool IsValid_Daten(CommitSetting commitSetting, string parentDir, ref ValidatorProtokoll protokoll)
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

        private bool IsValid_Ziel(CommitSetting commitSetting, string parentDir, ref ValidatorProtokoll protokoll)
        {
            if (commitSetting.Ziel == null)
            {
                if (commitSetting.Daten == null)
                {
                    return true;
                }
                else
                {
                    protokoll.AddEntry("Kein Ziel angegeben, wohl aber Daten");
                    return false;
                }
            }

            return true;
        }


        // TODO: Herausnehmen, sobald Git gefixt ist...
        private bool IsValid_Datum(CommitSetting commitSetting, string parentDir, ref ValidatorProtokoll protokoll)
        {
            if (commitSetting.Datum.Equals(default(DateTime)))
            {
                protokoll.AddEntry("Datum fehlt");
                return false;
            }

            if (commitSetting.Datum.ToEpoch() < 100000000)
            {
                protokoll.AddEntry("Minimales Datum ist der 04.03.1973");
                return false;
            }

            return true;
        }
    }
}
