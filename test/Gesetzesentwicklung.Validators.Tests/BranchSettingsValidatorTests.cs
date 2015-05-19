using Gesetzesentwicklung.Models;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Validators.Tests
{
    [TestFixture]
    public class BranchSettingsValidatorTests
    {
        private BranchSettingValidator _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new BranchSettingValidator();
        }

        [Test]
        [TestCaseSource(typeof(TestCaseSource_Valid))]
        public void Models_Validators_BranchSettings_Valid(List<BranchSettings> branchSettingsList)
        {
            var protokoll = new ValidatorProtokoll();
            var result = _classUnderTest.IsValid(branchSettingsList, ref protokoll);

            Assert.IsTrue(result);
            Assert.That(protokoll.Entries.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Models_Validators_BranchSettings_BranchGibtsNicht()
        {
            var branch_A = newBranchSettingWithSingleCommit("01.01.2000");
            branch_A.FileSettingFilename = "A.yml";
            branch_A.Branch = "A";

            var branch_B = newBranchSettingWithSingleCommit("02.01.2000", branchFrom: "GIBTSNICHT-B");
            branch_B.FileSettingFilename = "B.yml";
            branch_B.Branch = "B";

            var branch_C = newBranchSettingWithSingleCommit("03.01.2000", mergeInto: "GIBTSNICHT-C");
            branch_C.FileSettingFilename = "C.yml";
            branch_C.Branch = "C";

            var branchSettingsList = new List<BranchSettings> { branch_A, branch_B, branch_C };

            var protokoll = new ValidatorProtokoll();
            var result = _classUnderTest.IsValid(branchSettingsList, ref protokoll);

            Assert.IsFalse(result);
            Assert.That(protokoll.Entries.Count(), Is.EqualTo(2));
            Assert.That(protokoll.Entries.ElementAt(0).Message, Is.EqualTo(@"Von Branch oder Tag ""GIBTSNICHT-B"" kann nicht abgezweigt werden, da er nicht definiert oder zum Zeitpunkt 02.01.2000 noch nicht vorhanden ist"));
            Assert.That(protokoll.Entries.ElementAt(0).Filename, Is.EqualTo("B.yml"));
            Assert.That(protokoll.Entries.ElementAt(1).Message, Is.EqualTo(@"Für Merge benötigter Ziel-Branch ""GIBTSNICHT-C"" ist nicht definiert oder zum Zeitpunkt 03.01.2000 noch nicht vorhanden"));
            Assert.That(protokoll.Entries.ElementAt(1).Filename, Is.EqualTo("C.yml"));
        }

        [Test]
        public void Models_Validators_BranchSettings_BranchGibtsNochNicht()
        {
            var branch_A = newBranchSettingWithSingleCommit("03.01.2000");
            branch_A.FileSettingFilename = "A.yml";
            branch_A.Branch = "A";

            var branch_B = newBranchSettingWithSingleCommit("02.01.2000", branchFrom: "A");
            branch_B.FileSettingFilename = "B.yml";
            branch_B.Branch = "B";

            var branchSettingsList = new List<BranchSettings> { branch_A, branch_B };

            var protokoll = new ValidatorProtokoll();
            var result = _classUnderTest.IsValid(branchSettingsList, ref protokoll);

            Assert.IsFalse(result);
            Assert.That(protokoll.Entries.Count(), Is.EqualTo(1));
            Assert.That(protokoll.Entries.Single().Message, Is.EqualTo(@"Von Branch oder Tag ""A"" kann nicht abgezweigt werden, da er nicht definiert oder zum Zeitpunkt 02.01.2000 noch nicht vorhanden ist"));
            Assert.That(protokoll.Entries.Single().Filename, Is.EqualTo("B.yml"));
        }


        [Test]
        public void Models_Validators_BranchSettings_AutoMergeBranchGibtsUeberhauptNicht()
        {
            var branch_A = newBranchSettingWithSingleCommit("01.01.2000");
            branch_A.FileSettingFilename = "A.yml";
            branch_A.Branch = "A";

            var branch_B = newBranchSettingWithSingleCommit("02.01.2000");
            branch_B.AutoMergeInto = "GIBTSNICHT";
            branch_B.FileSettingFilename = "B.yml";
            branch_B.Branch = "B";

            var branchSettingsList = new List<BranchSettings> { branch_A, branch_B };

            var protokoll = new ValidatorProtokoll();
            var result = _classUnderTest.IsValid(branchSettingsList, ref protokoll);

            Assert.IsFalse(result);
            Assert.That(protokoll.Entries.Count(), Is.EqualTo(1));
            Assert.That(protokoll.Entries.ElementAt(0).Message, Is.EqualTo(@"Für Branch ""B"" definierter AutoMerge-Branch ""GIBTSNICHT"" ist nicht definiert"));
            Assert.That(protokoll.Entries.ElementAt(0).Filename, Is.EqualTo("B.yml"));
        }

        [Test]
        public void Models_Validators_BranchSettings_AutoMergeBranchGibtsNochNicht()
        {
            var branch_A = newBranchSettingWithSingleCommit("01.01.2000");
            branch_A.Commits.Add(newCommitSetting("03.01.2000", mergeInto: "B"));
            branch_A.FileSettingFilename = "A.yml";
            branch_A.Branch = "A";

            var branch_B = newBranchSettingWithSingleCommit("02.01.2000");
            branch_B.AutoMergeInto = "C";
            branch_B.FileSettingFilename = "B.yml";
            branch_B.Branch = "B";

            var branch_C = newBranchSettingWithSingleCommit("04.01.2000");
            branch_C.FileSettingFilename = "C.yml";
            branch_C.Branch = "C";

            var branchSettingsList = new List<BranchSettings> { branch_A, branch_B, branch_C };

            var protokoll = new ValidatorProtokoll();
            var result = _classUnderTest.IsValid(branchSettingsList, ref protokoll);

            Assert.IsFalse(result);
            Assert.That(protokoll.Entries.Count(), Is.EqualTo(1));
            Assert.That(protokoll.Entries.ElementAt(0).Message, Is.EqualTo(@"Branch ""A"" soll am 03.01.2000 nach ""B"" gemergt werden, für den ein AutoMerge-Branch ""C"" konfiguriert ist, der zu diesem Zeitpunkt nicht existiert"));
            Assert.That(protokoll.Entries.ElementAt(0).Filename, Is.EqualTo("A.yml"));
        }
        private BranchSettings newBranchSettingWithSingleCommit(string datum = null, string branchFrom = null, string mergeInto = null, string tag = null) => new BranchSettings
        {
            Commits = new List<CommitSetting>
            {
                newCommitSetting(datum, branchFrom, mergeInto, tag)
            }
        };

        private CommitSetting newCommitSetting(string datum = null, string branchFrom = null, string mergeInto = null, string tag = null) => new CommitSetting
        {
            _Datum = datum,
            BranchFrom = branchFrom,
            MergeInto = mergeInto,
            Tag = tag
        };
    }

    #region TestCaseSources
    public class TestCaseSource_Valid : IEnumerable
    {
        private IEnumerable<List<BranchSettings>> _testCases;

        public TestCaseSource_Valid()
        {
            _testCases = InitTestCase();
        }

        public IEnumerator GetEnumerator() => _testCases.GetEnumerator();

        private IEnumerable<List<BranchSettings>> InitTestCase()
        {
            var branches = new List<BranchSettings>();

            var branch_Gesetzesstand = new BranchSettings
            {
                Commits = new List<CommitSetting>
                {
                    new CommitSetting { _Datum = "01.01.2000", Tag = "INIT" }
                }
            };
            branch_Gesetzesstand.Branch = "Gesetzesstand";
            branches.Add(branch_Gesetzesstand);

            var branch_GG_Bundesgesetzblatt = new BranchSettings
            {
                // Jeder in diesen Branch gemergte Commit erzeugt
                // automatisch einen weiteren Merge-Commit:
                AutoMergeInto = "Gesetzesstand",

                Commits = new List<CommitSetting>
                {
                    // Zweigt von einem Tag ab:
                    new CommitSetting { _Datum = "02.01.2000", BranchFrom = "INIT" }
                }
            };
            branch_GG_Bundesgesetzblatt.Branch = "GG/Bundesgesetzblatt";
            branches.Add(branch_GG_Bundesgesetzblatt);

            var branch_GG_Aenderung_1 = new BranchSettings
            {
                Commits = new List<CommitSetting>
                {
                    // Zweigt von einem Branch ab:
                    new CommitSetting { _Datum = "03.01.2000", BranchFrom = "GG/Bundesgesetzblatt" },

                    // Mergt in einen Branch:
                    new CommitSetting { _Datum = "04.01.2000", MergeInto = "GG/Bundesgesetzblatt" }
                }
            };
            branch_GG_Bundesgesetzblatt.Branch = "GG/Änderung-1";
            branches.Add(branch_GG_Aenderung_1);


            yield return branches;
        }
    }
    #endregion
}
