using Caliburn.Micro;
using Gesetzesentwicklung.Lizenzen;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.GUI.ViewModels
{
    class AboutViewModel
    {
        public ObservableCollection<Lizenz> Lizenzen { get; private set; }

        public AboutViewModel()
        {
            Lizenzen = new ObservableCollection<Lizenz>();

            var genutzteLizenzen = new GenutzteLizenzen();
            foreach (Lizenz lizenz in genutzteLizenzen.Lizenzen)
            {
                Lizenzen.Add(lizenz);
            }
        }

        public void OpenLizenz(Lizenz lizenz)
        {
            var windowManager = new WindowManager();
            windowManager.ShowDialog(new LizenzViewModel(lizenz));
        }
    }
}
