using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Git
{
    public class RepositoryBuilder
    {
        private IFileSystem _fileSystem;

        internal RepositoryBuilder(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public RepositoryBuilder() : this(fileSystem: new FileSystem()) { }

        // Leider kann ich LibGit2Sharp kein System.IO.Abstractions unterjubeln...
        public void Build(string sourceDir, string destDir, bool overwrite = false)
        {
            var sourceDirInfo = _fileSystem.DirectoryInfo.FromDirectoryName(sourceDir);
            var destDirInfo = _fileSystem.DirectoryInfo.FromDirectoryName(destDir);

            TestAssertions(sourceDirInfo, destDirInfo, overwrite);

            CleanUpDir(destDirInfo);

            IEnumerable<string> branches;
            InspectSourceDir(sourceDirInfo, out branches);

            Repository.Init(destDirInfo.FullName);
        }

        internal void TestAssertions(DirectoryInfoBase sourceDirInfo, DirectoryInfoBase destDirInfo, bool overwrite = false)
        {
            if (!sourceDirInfo.Exists)
            {
                var message = string.Format("Verzeichnis existiert nicht: {0}", sourceDirInfo.FullName);
                throw new ArgumentException(message);
            }

            if (SameDirectory(sourceDirInfo.FullName, destDirInfo.FullName))
            {
                throw new ArgumentException("Quell- und Zielverzeichnisse dürfen nicht gleich sein");
            }

            if (overwrite == false && destDirInfo.Exists)
            {
                var message = string.Format("Verzeichnis existiert schon: {0}", destDirInfo.FullName);
                throw new ArgumentException(message);
            }
        }

        private void InspectSourceDir(DirectoryInfoBase sourceDirInfo, out IEnumerable<string> branches)
        {
            branches = from dir in sourceDirInfo.GetDirectories()
                       where !dir.Name.StartsWith(".")
                       select dir.Name;
        }

        private void CleanUpDir(DirectoryInfoBase dirInfo)
        {
            if (dirInfo.Exists)
            {
                dirInfo.Delete(recursive: true);
            }

            dirInfo.Create();
        }

        static private bool SameDirectory(string path1, string path2)
        {
            var comp = string.Compare(
                    System.IO.Path.GetFullPath(path1).TrimEnd('\\'),
                    System.IO.Path.GetFullPath(path2).TrimEnd('\\'),
                    StringComparison.InvariantCultureIgnoreCase);

            return comp == 0;
        }    
    }
}
