using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Git
{
    internal static class DirectoryInfoExtensions
    {
        // Vielleicht lieber ins Shared-Projekt. Aber dann bekomme ich
        // dort eine Abhängigkeit zu System.IO.Abstractions hinein...
        public static void CopyTo(this DirectoryInfoBase source, DirectoryInfoBase target)
        {
            if (!target.Exists)
            {
                target.Create();
            }

            foreach (var fileInfo in source.GetFiles())
            {
                fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), true);
            }

            foreach (var sourceSubDir in source.GetDirectories())
            {
                var targetSubDir = target.CreateSubdirectory(sourceSubDir.Name);
                sourceSubDir.CopyTo(targetSubDir);
            }
        }
    }
}
