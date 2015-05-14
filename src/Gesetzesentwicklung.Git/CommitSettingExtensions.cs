using Gesetzesentwicklung.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Git
{
    internal static class CommitSettingExtensions
    {
        public static string DerivedBranchName(this CommitSetting commitSetting, DirectoryInfoBase sourceDirInfo)
        {
            if (string.IsNullOrEmpty(commitSetting.FileSettingFilename))
            {
                throw new ArgumentException("Branch kann nicht abgeleitet werden: Fehlender Dateiname");
            }

            var letzterTeil = Path.GetFileNameWithoutExtension(commitSetting.FileSettingFilename);
            var fileSettingDirectory = Path.GetDirectoryName(commitSetting.FileSettingFilename);

            if (! fileSettingDirectory.StartsWith(sourceDirInfo.FullName))
            {
                throw new ArgumentException($"Branch kann nicht abgeleitet werden: CommitSetting-Datei muss unterhalb von {sourceDirInfo.FullName} liegen");
            }

            var davor = fileSettingDirectory
                .Replace(sourceDirInfo.FullName, "")
                .Replace(@"\", "/")
                .TrimStart('/');

            return davor == "" ? letzterTeil : $"{davor}/{letzterTeil}";
        }
    }
}
