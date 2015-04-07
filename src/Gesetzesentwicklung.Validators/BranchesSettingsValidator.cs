using Gesetzesentwicklung.Models;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Validators
{
    public class BranchesSettingsValidator
    {
        private static string Message_InvalidBranch = "Ungültiger Branch, da sein Name in Konflikt zu einem anderen Branch steht: {0}";

        private readonly IFileSystem _fileSystem;

        internal BranchesSettingsValidator(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public BranchesSettingsValidator() : this(fileSystem: new FileSystem()) { }

        public bool IsValid(BranchesSettings branchSettings, string path, ref ValidatorProtokoll protokoll)
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
