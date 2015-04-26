using Gesetzesentwicklung.Models;
using Gesetzesentwicklung.Validators;
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

            var commitSettings = ReadCommitSettings(sourceDirInfo);

            Repository.Init(destDirInfo.FullName);

            MakeCommits(sourceDirInfo, destDirInfo, commitSettings);
        }

        private void MakeCommits(DirectoryInfoBase sourceDirInfo, DirectoryInfoBase destDirInfo, CommitSettings commitSettings)
        {
            var firstCommit = commitSettings.Commits.First();
            using (var repo = new Repository(destDirInfo.FullName))
            {
                _fileSystem.File.WriteAllText(Path.Combine(destDirInfo.FullName, "README.md"), "bla");

                repo.Stage("README.md");

                Signature author = new Signature(firstCommit.Autor.DisplayName, firstCommit.Autor.Address, firstCommit.Datum);
                Signature committer = author;

                Commit c = repo.Commit(firstCommit.Beschreibung, author, committer);
            }
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

        internal CommitSettings ReadCommitSettings(DirectoryInfoBase sourceDirInfo)
        {
            var result = new CommitSettings { Commits = new List<CommitSetting>() };
            
            var yamlFiles = sourceDirInfo.GetFiles("*.yml", SearchOption.AllDirectories);

            var yamlFileParser = new YamlFileParser(_fileSystem);
            var validatorProtokoll = new ValidatorProtokoll();
            var validator = new CommitSettingValidator(_fileSystem);

            foreach (var commitSettingsYaml in yamlFiles)
            {
                var commitSettings = yamlFileParser.FromYaml<CommitSettings>(commitSettingsYaml.FullName);
                foreach (var commitSetting in commitSettings.Commits)
                {
                    validator.IsValid(commitSetting, commitSettingsYaml.DirectoryName, ref validatorProtokoll);
                }
                result.Commits.AddRange(commitSettings.Commits);
            }

            if (validatorProtokoll.Entries.Any())
            {
                var message = string.Join<string>(Environment.NewLine, validatorProtokoll.Entries.Select(e => e.Message));
                throw new ArgumentException(message);
            }

            result.Commits.Sort();

            return result;
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
