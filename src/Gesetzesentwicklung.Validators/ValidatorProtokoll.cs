using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Validators
{
    public class ValidatorProtokoll
    {
        private List<string> entries = new List<string>();

        public IEnumerable<string> Entries
        {
            get
            {
               return entries.AsEnumerable();
            }
        }
        
        public void AddEntry(string message)
        {
            entries.Add(message);
        }

        public void AddEntries(IEnumerable<string> messages)
        {
            entries.AddRange(messages);
        }

    }
}
