using Caliburn.Micro;
using Gesetzesentwicklung.Lizenzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.GUI.ViewModels
{
    class LizenzViewModel
    {
        public string Projekt { get; private set; }
        public string LizenzText { get; private set; }

        public LizenzViewModel(Lizenz lizenz)
        {
            Projekt = lizenz.Projekt;
            LizenzText = lizenz.LizenzText;
        }
    }
}
