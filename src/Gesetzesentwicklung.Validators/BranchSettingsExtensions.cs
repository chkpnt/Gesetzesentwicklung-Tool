using Gesetzesentwicklung.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Validators
{
    public static class BranchSettingsExtensions
    {
        private static string Message_InvalidBranch = "Ungültiger Branch, da sein Name in Konflikt zu einem anderen Branch steht: {0}";

        public static bool IsValid(this BranchesSettings branchSettings)
        {
            ValidatorProtokoll protokoll = new ValidatorProtokoll();
            return branchSettings.IsValid(ref protokoll);
        }

        public static bool IsValid(this BranchesSettings branchSettings, ref ValidatorProtokoll protokoll)
        {
            var branches = branchSettings.Branches.Keys;

            var invalidBranches = from branch1 in branches
                                  from branch2 in branches
                                  where branch2.StartsWith(branch1 + "/")
                                  select branch1;

            var validatorMessages = from branch in invalidBranches
                                    select string.Format(Message_InvalidBranch, branch);
            protokoll.AddEntries(validatorMessages);

            return !invalidBranches.Any();
        }
    }
}
