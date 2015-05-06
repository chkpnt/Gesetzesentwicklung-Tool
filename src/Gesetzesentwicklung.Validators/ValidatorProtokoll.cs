using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Validators
{
    public class ValidatorProtokoll
    {
        public class ProtokollEntry
        {
            public string Filename { get; private set; }
            public string Message { get; private set; }

            internal ProtokollEntry(string filename, string message)
            {
                Filename = filename;
                Message = message;
            }
        }

        private List<ProtokollEntry> entries = new List<ProtokollEntry>();

        public IEnumerable<ProtokollEntry> Entries => entries.AsEnumerable();

        public void AddEntry(string message)
        {
            AddEntry("", message);
        }

        public void AddEntry(string filename, string message)
        {
            entries.Add(new ProtokollEntry(filename, message));
        }

        public void AddEntries(IEnumerable<string> messages)
        {
            AddEntries("", messages);
        }

        public void AddEntries(string filename, IEnumerable<string> messages)
        {
            foreach (var message in messages)
            {
                entries.Add(new ProtokollEntry(filename, message));
            }
        }
    }
}
