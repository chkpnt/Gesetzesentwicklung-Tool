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

        internal BranchesSettings ReadBranchesSettings(DirectoryInfoBase sourceDirInfo)
        {
            var branchesSettingsFileInfo = _fileSystem.FileInfo.FromFileName(Path.Combine(sourceDirInfo.FullName, "Branches.yml"));
            if (! branchesSettingsFileInfo.Exists)
            {
                var message = string.Format("Datei fehlt: {0}", branchesSettingsFileInfo.FullName);
                throw new ArgumentException(message);
            }

            var yamlFileParser = new YamlFileParser(_fileSystem);
            var branchesSettings = yamlFileParser.FromYaml<BranchesSettings>(branchesSettingsFileInfo.FullName);

            var validatorProtokoll = new ValidatorProtokoll();
            var validator = new BranchesSettingsValidator(_fileSystem);
            if (!validator.IsValid(branchesSettings, sourceDirInfo.FullName, ref validatorProtokoll))
            {
                var message = string.Join<string>(Environment.NewLine, validatorProtokoll.Entries);
                throw new ArgumentException(message);
            }
            return branchesSettings;
        }

        internal CommitSettings ReadCommitSettings(DirectoryInfoBase sourceDirInfo, BranchesSettings branchesSettings)
        {
            var result = new CommitSettings { Commits = new List<CommitSetting>() };
            
            var yamlFiles = from branchYamlFile in branchesSettings.BranchesYamls
                            select _fileSystem.FileInfo.FromFileName(Path.Combine(sourceDirInfo.FullName, branchYamlFile));

            var yamlFileParser = new YamlFileParser(_fileSystem);
            var validatorProtokoll = new ValidatorProtokoll();
            var validator = new CommitSettingValidator(_fileSystem);

            foreach (var commitSettingsYaml in yamlFiles)
            {
                var commitSettings = yamlFileParser.FromYaml<CommitSettings>(commitSettingsYaml.FullName);
                foreach (var commitSetting in commitSettings.Commits)
                {
                    validator.IsValid(commitSetting, commitSettingsYaml.DirectoryName, branchesSettings, ref validatorProtokoll);
                }
                result.Commits.AddRange(commitSettings.Commits);
            }

            if (validatorProtokoll.Entries.Any())
            {
                var message = string.Join<string>(Environment.NewLine, validatorProtokoll.Entries);
                throw new ArgumentException(message);
            }

            result.Commits.Sort();

            return result;
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
