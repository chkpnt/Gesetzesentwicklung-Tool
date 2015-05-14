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
                if (firstCommit.Ziel != null) {
                    CleanUpPathInRepository(repo, firstCommit.Ziel);
                    var datenQuelle = _fileSystem.DirectoryInfo.FromDirectoryName(Path.Combine(Path.GetDirectoryName(firstCommit.FileSettingFilename), firstCommit.Daten));
                    var datenZiel = _fileSystem.DirectoryInfo.FromDirectoryName(Path.Combine(destDirInfo.FullName, firstCommit.Ziel));
                    datenQuelle.CopyTo(datenZiel);

                    if (firstCommit.Ziel == "")
                    {
                        repo.Stage("*");
                    }
                    else
                    {
                        repo.Stage(firstCommit.Ziel);
                    }
                }

                var authorSignature = new Signature(firstCommit.Autor.DisplayName, firstCommit.Autor.Address, firstCommit.Datum);
                var committerSignature = authorSignature;

                repo.Commit(firstCommit.Beschreibung, authorSignature, committerSignature);

                if (! string.IsNullOrEmpty(firstCommit.Tag))
                {
                    repo.ApplyTag(firstCommit.Tag);
                }
            }
        }

        private void CleanUpPathInRepository(Repository repo, string ziel)
        {
            if (ziel == "")
            {
                ziel = "*";
            }

            repo.Remove(ziel);
        }

        internal void TestAssertions(DirectoryInfoBase sourceDirInfo, DirectoryInfoBase destDirInfo, bool overwrite = false)
        {
            if (!sourceDirInfo.Exists)
            {
                var message = $"Verzeichnis existiert nicht: {sourceDirInfo.FullName}";
                throw new ArgumentException(message);
            }

            if (SameDirectory(sourceDirInfo.FullName, destDirInfo.FullName))
            {
                throw new ArgumentException("Quell- und Zielverzeichnisse dürfen nicht gleich sein");
            }

            if (overwrite == false && destDirInfo.Exists)
            {
                var message = $"Verzeichnis existiert schon: {destDirInfo.FullName}";
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
