using Caliburn.Micro;
using Gesetzesentwicklung.Lizenzen;
using Gesetzesentwicklung.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

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

            var lizenzenView = CollectionViewSource.GetDefaultView(Lizenzen);
            lizenzenView.SortDescriptions.Add(
                new SortDescription(PropertyNameSolver.Instance.Lizenz_Projekt, ListSortDirection.Ascending)
            );
        }

        public void OpenLizenz(Lizenz lizenz)
        {
            var windowManager = new WindowManager();
            windowManager.ShowDialog(new LizenzViewModel(lizenz));
        }


        public void NavigateTo(Lizenz lizenz)
        {
            Process.Start(new ProcessStartInfo(lizenz.Homepage.AbsoluteUri));
        }
    }
}
