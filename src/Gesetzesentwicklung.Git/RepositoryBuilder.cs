using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Git
{
    public class RepositoryBuilder
    {
        private DirectoryInfo _sourceDir;
        private DirectoryInfo _destDir;

        public RepositoryBuilder(DirectoryInfo _sourceDir, DirectoryInfo _destDir, bool overwrite = false)
        {
            if (!_sourceDir.Exists)
            {
                var message = string.Format("Verzeichnis existiert nicht: {0}", _sourceDir.FullName);
                throw new ArgumentException(message);
            }

            if (SameDirectory(_sourceDir.FullName, _destDir.FullName))
            {
                throw new ArgumentException("Quell- und Zielverzeichnisse dürfen nicht gleich sein");
            }

            if (overwrite == false && _destDir.Exists)
            {
                var message = string.Format("Verzeichnis existiert schon: {0}", _destDir.FullName);
                throw new ArgumentException(message);
            }

            this._sourceDir = _sourceDir;
            this._destDir = _destDir;

        }

        public void build()
        {
            cleanUpDestDir();

            IEnumerable<string> branches;
            inspectSourceDir(out branches);

            Repository.Init(_destDir.FullName);


        }

        private void inspectSourceDir(out IEnumerable<string> branches)
        {
            branches = from dir in _sourceDir.GetDirectories()
                       where !dir.Name.StartsWith(".")
                       select dir.Name;
        }

        private void cleanUpDestDir()
        {
            if (_destDir.Exists)
            {
                _destDir.Delete(recursive: true);
            }

            _destDir.Create();
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
