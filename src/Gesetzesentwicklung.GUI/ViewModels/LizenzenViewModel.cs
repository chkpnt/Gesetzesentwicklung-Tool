using Caliburn.Micro;
using Gesetzesentwicklung.Lizenzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.GUI.ViewModels
{
    class LizenzenViewModel : PropertyChangedBase
    {
        private List<GenutzteLizenzen> _lizenzen;

        public List<GenutzteLizenzen> Lizenzen
        {
            get { return _lizenzen; }
            set
            {
                _lizenzen = value;
                NotifyOfPropertyChange(() => Lizenzen);
            }
        }
    }
}
