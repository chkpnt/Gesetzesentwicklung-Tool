using LibGit2Sharp;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Git.Tests
{
    [TestFixture]
    [Category("WithFSAccess")]
    public class RepositoryBuilderTests
    {
        private DirectoryInfoBase _sourceDirInfo;
        private DirectoryInfoBase _destDirInfo;

        private IFileSystem _fileSystem;

        private RepositoryBuilder _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new FileSystem();

            _sourceDirInfo = _fileSystem.DirectoryInfo.FromDirectoryName("TestData");
            _destDirInfo = _fileSystem.DirectoryInfo.FromDirectoryName("TestRepo");

            _classUnderTest = new RepositoryBuilder();

            DeleteDestDir();
        }

        [TearDown]
        public void TearDown()
        {
            DeleteDestDir();
        }

        private void DeleteDestDir()
        {
            _destDirInfo.Refresh();
            if (_destDirInfo.Exists)
            {
                // Manche Dateien sind ReadOnly?!
                foreach (var file in _destDirInfo.EnumerateFiles("*", SearchOption.AllDirectories))
                {
                    file.Attributes = FileAttributes.Normal;
                }

                _destDirInfo.Delete(recursive: true);
            }
        }

        [Test]
        public void Git_RepositoryKorrektErzeugt()
        {
            _classUnderTest.Build(_sourceDirInfo.FullName, _destDirInfo.FullName);

            AssertThat_DestDirIstEinRepository();
            AssertThat_ErsterCommitKorrekt();
            AssertThat_ErsterCommitHatInitTag();
            AssertThat_ZweiterCommitBranchtAbVomInitTag();
        }

        private void AssertThat_ZweiterCommitBranchtAbVomInitTag()
        {
            using (var repo = new Repository(_destDirInfo.FullName))
            {
                var filter = new CommitFilter { SortBy = CommitSortStrategies.Reverse };
                var secondCommit = repo.Commits.QueryBy(filter).ElementAt(1);
                Assert.That(secondCommit.MessageShort, Is.StringStarting("<C2>"));

                var commitDesInitTags = (Commit) repo.Tags["init"].Target;
                Assert.That(secondCommit.Parents, Has.Member(commitDesInitTags));
            }
        }

        private void AssertThat_DestDirIstEinRepository()
        {
            Assert.That(_destDirInfo.GetDirectories(".git", SearchOption.TopDirectoryOnly).Count(), Is.EqualTo(1));
            Assert.IsTrue(Repository.IsValid(_destDirInfo.FullName));
        }

        private void AssertThat_ErsterCommitKorrekt()
        {
            using (var repo = new Repository(_destDirInfo.FullName))
            {
                var filter = new CommitFilter { SortBy = CommitSortStrategies.Reverse };
                var firstCommit = repo.Commits.QueryBy(filter).First();
                Assert.That(firstCommit.MessageShort, Is.StringStarting("<C1>"));
                Assert.That(firstCommit.Author.Name, Is.EqualTo("Foo Bar"));
                Assert.That(firstCommit.Author.Email, Is.EqualTo("foo@example.net"));
                Assert.That(firstCommit.Author.When, Is.EqualTo(DateTimeOffset.ParseExact("01.01.1980", "dd.MM.yyyy", null)));
                Assert.That(ListBranchesContainingCommit(repo, firstCommit.Sha).Select(b => b.Name) , Contains.Item("Gesetzesstand"));
            }
        }

        private void AssertThat_ErsterCommitHatInitTag()
        {
            using (var repo = new Repository(_destDirInfo.FullName))
            {
                var filter = new CommitFilter { SortBy = CommitSortStrategies.Reverse };
                var firstCommit = repo.Commits.QueryBy(filter).First();

                var tag = repo.Tags["init"];
                Assert.That((Commit)tag.Target, Is.EqualTo(firstCommit));
            }
        }

        private IEnumerable<Branch> ListBranchesContainingCommit(Repository repo, string commitSha)
        {
            var localHeads = repo.Refs.Where(r => r.IsLocalBranch());

            var commit = repo.Lookup<Commit>(commitSha);
            var localHeadsContainingTheCommit = repo.Refs.ReachableFrom(localHeads, new[] { commit });

            return localHeadsContainingTheCommit
                .Select(branchRef => repo.Branches[branchRef.CanonicalName]);
        }
    }
}
