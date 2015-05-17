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

            var commitSettings = ReadBranchSettings(sourceDirInfo);

            Repository.Init(destDirInfo.FullName);

            MakeCommits(sourceDirInfo, destDirInfo, commitSettings);
        }

        private void MakeCommits(DirectoryInfoBase sourceDirInfo, DirectoryInfoBase destDirInfo, BranchSettings branchSettings)
        {
            using (var repo = new Repository(destDirInfo.FullName))
            {
                var firstCommit = true;
                foreach (var commit in branchSettings.Commits)
                {
                    var branchName = DeriveBranchName(commit.FileSettingFilename, sourceDirInfo);
                    if (firstCommit)
                    {
                        InitialBranch(repo, branchName);
                    }
                    else
                    {
                        Checkout(repo, branchName, commit.BranchFrom);
                    }

                    if (commit.Ziel != null)
                    {
                        CleanUpPathInRepository(repo, commit.Ziel);
                        UpdateRepository(repo, commit, destDirInfo);
                    }

                    Commit(repo, commit);

                    if (!string.IsNullOrEmpty(commit.Tag))
                    {
                        repo.ApplyTag(commit.Tag);
                    }

                    firstCommit = false;
                }
            }
        }

        private void Commit(Repository repo, CommitSetting commit)
        {
            var authorSignature = new Signature(commit.Autor.DisplayName, commit.Autor.Address, commit.Datum);
            var committerSignature = authorSignature;

            repo.Commit(commit.Beschreibung, authorSignature, committerSignature);
        }

        private void UpdateRepository(Repository repo, CommitSetting commit, DirectoryInfoBase destDirInfo)
        {
            var datenQuelle = _fileSystem.DirectoryInfo.FromDirectoryName(Path.Combine(Path.GetDirectoryName(commit.FileSettingFilename), commit.Daten));
            var datenZiel = _fileSystem.DirectoryInfo.FromDirectoryName(Path.Combine(destDirInfo.FullName, commit.Ziel));
            datenQuelle.CopyTo(datenZiel);

            if (commit.Ziel == "")
            {
                repo.Stage("*");
            }
            else
            {
                repo.Stage(commit.Ziel);
            }
        }

        private void Checkout(Repository repo, string branchName, string branchFrom)
        {
            var branch = repo.Branches[branchName];

            var head = repo.Head;

            if (branch == null)
            {
                if (branchFrom == null)
                {
                    repo.CreateBranch(branchName);
                }
                else
                {
                    repo.CreateBranch(branchName, branchFrom);
                }
            }

            repo.Checkout(branchName);
        }

        private void InitialBranch(Repository repo, string branchName)
        {
            repo.Refs.UpdateTarget("HEAD", $"refs/heads/{branchName}");
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

        internal BranchSettings ReadBranchSettings(DirectoryInfoBase sourceDirInfo)
        {
            var result = new BranchSettings { Commits = new List<CommitSetting>() };

            var yamlFiles = sourceDirInfo.GetFiles("*.yml", SearchOption.AllDirectories);

            var yamlFileParser = new YamlFileParser(_fileSystem);
            var validatorProtokoll = new ValidatorProtokoll();
            var validator = new CommitSettingValidator(_fileSystem);

            foreach (var branchSettingsFile in yamlFiles)
            {
                var branchSettings = yamlFileParser.FromYaml<BranchSettings>(branchSettingsFile.FullName);
                foreach (var commitSetting in branchSettings.Commits)
                {
                    commitSetting.Branch = DeriveBranchName(branchSettingsFile.FullName, sourceDirInfo);
                    validator.IsValid(commitSetting, branchSettingsFile.DirectoryName, ref validatorProtokoll);
                }
                result.Commits.AddRange(branchSettings.Commits);
            }

            if (validatorProtokoll.Entries.Any())
            {
                var message = string.Join<string>(Environment.NewLine, validatorProtokoll.Entries.Select(e => e.Message));
                throw new ArgumentException(message);
            }

            result.Commits.Sort();

            return result;
        }

        internal string DeriveBranchName(string fileSettingFilename, DirectoryInfoBase sourceDirInfo)
        {
            if (string.IsNullOrEmpty(fileSettingFilename))
            {
                throw new ArgumentException("Branch kann nicht abgeleitet werden: Fehlender Dateiname");
            }

            var letzterTeil = Path.GetFileNameWithoutExtension(fileSettingFilename);
            var fileSettingDirectory = Path.GetDirectoryName(fileSettingFilename);

            if (!fileSettingDirectory.StartsWith(sourceDirInfo.FullName))
            {
                throw new ArgumentException($"Branch kann nicht abgeleitet werden: CommitSetting-Datei muss unterhalb von {sourceDirInfo.FullName} liegen");
            }

            var davor = fileSettingDirectory
                .Replace(sourceDirInfo.FullName, "")
                .Replace(@"\", "/")
                .TrimStart('/');

            return davor == "" ? letzterTeil : $"{davor}/{letzterTeil}";
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
