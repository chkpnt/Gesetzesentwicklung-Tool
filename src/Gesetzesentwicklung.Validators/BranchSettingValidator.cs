using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gesetzesentwicklung.Models;

namespace Gesetzesentwicklung.Validators
{
    public class BranchSettingValidator
    {
        public bool IsValid(List<BranchSettings> branchSettingsList, ref ValidatorProtokoll protokoll)
        {
            var commits = from branchSettings in branchSettingsList
                          from commitSetting in branchSettings.Commits
                          orderby commitSetting ascending
                          select commitSetting;

            var knownAutoMergeSettings = branchSettingsList
                .Where(b => b.AutoMergeInto != null)
                .ToDictionary(b => b.Branch, b => b.AutoMergeInto);

            var knownBranchesSoFar = new HashSet<string> { null };
            var knownTagsSoFar = new HashSet<string> { null };

            var valid = true;

            foreach (var commit in commits)
            {
                valid &= IsValid_BranchFrom(commit, knownBranchesSoFar, knownTagsSoFar, ref protokoll);
                valid &= IsValid_MergeInto(commit, knownBranchesSoFar, ref protokoll);
                valid &= IsValid_AutoMergeCommits(commit, knownAutoMergeSettings, knownBranchesSoFar, ref protokoll);

                knownBranchesSoFar.Add(commit.Branch);
                knownTagsSoFar.Add(commit.Tag);
            }

            valid &= IsValid_AutoMergeBranches(branchSettingsList, knownBranchesSoFar, ref protokoll);

            return valid;
        }

        private bool IsValid_BranchFrom(CommitSetting commit, HashSet<string> knownBranchesSoFar, HashSet<string> knownTagsSoFar, ref ValidatorProtokoll protokoll)
        {
            if (!knownBranchesSoFar.Contains(commit.BranchFrom) &&
                !knownTagsSoFar.Contains(commit.BranchFrom))
            {
                protokoll.AddEntry(
                    filename: commit.FileSettingFilename,
                    message: $@"Von Branch oder Tag ""{commit.BranchFrom}"" kann nicht abgezweigt werden, da er nicht definiert oder zum Zeitpunkt {commit._Datum} noch nicht vorhanden ist"
                );
                return false;
            }

            return true;
        }


        private bool IsValid_MergeInto(CommitSetting commit, HashSet<string> knownBranchesSoFar, ref ValidatorProtokoll protokoll)
        {
            if (!knownBranchesSoFar.Contains(commit.MergeInto))
            {
                protokoll.AddEntry(
                    filename: commit.FileSettingFilename,
                    message: $@"Für Merge benötigter Ziel-Branch ""{commit.MergeInto}"" ist nicht definiert oder zum Zeitpunkt {commit._Datum} noch nicht vorhanden"
                );
                return false;
            }

            return true;
        }


        private bool IsValid_AutoMergeCommits(CommitSetting commit, Dictionary<string, string> knownAutoMergeSettings, HashSet<string> knownBranchesSoFar, ref ValidatorProtokoll protokoll)
        {
            if (commit.MergeInto != null)
            {
                string autoMergeBranchDestination;
                knownAutoMergeSettings.TryGetValue(commit.MergeInto, out autoMergeBranchDestination);

                if (!knownBranchesSoFar.Contains(autoMergeBranchDestination))
                {
                    protokoll.AddEntry(
                        filename: commit.FileSettingFilename,
                        message: $@"Branch ""{commit.Branch}"" soll am {commit._Datum} nach ""{commit.MergeInto}"" gemergt werden, für den ein AutoMerge-Branch ""{autoMergeBranchDestination}"" konfiguriert ist, der zu diesem Zeitpunkt nicht existiert"
                    );
                    return false;
                }
            }

            return true;
        }

        private bool IsValid_AutoMergeBranches(List<BranchSettings> branchSettingsList, HashSet<string> knownBranchesSoFar, ref ValidatorProtokoll protokoll)
        {
            foreach (var branchSetting in branchSettingsList)
            {
                if (!knownBranchesSoFar.Contains(branchSetting.AutoMergeInto))
                {
                    protokoll.AddEntry(
                        filename: branchSetting.FileSettingFilename,
                        message: $@"Für Branch ""{branchSetting.Branch}"" definierter AutoMerge-Branch ""{branchSetting.AutoMergeInto}"" ist nicht definiert"
                    );
                    return false;
                }
            }

            return true;
        }
    }
}
