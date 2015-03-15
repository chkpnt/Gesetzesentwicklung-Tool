using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Models
{
    public static class Validators
    {
        public static bool IsValid(this BranchesSettings branchSettings)
        {
            var branches = branchSettings.Branches.Keys;

            var invalidBranches = from branch1 in branches
                                  from branch2 in branches
                                  where branch2.StartsWith(branch1 + "/")
                                  select branch1;

            return !invalidBranches.Any();
        }
    }
}
