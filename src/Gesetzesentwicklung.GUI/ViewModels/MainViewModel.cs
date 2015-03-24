using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.GUI.ViewModels
{
    class MainViewModel : PropertyChangedBase
    {
        private string _windowTitle = "Default Title";

        private GIISelectorViewModel _giiSelectorView;

        public MainViewModel()
        {
            _giiSelectorView = new GIISelectorViewModel();
        }

        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                _windowTitle = value;
                NotifyOfPropertyChange(() => WindowTitle);
            }
        }

        public GIISelectorViewModel GIISelectorView
        {
            get { return _giiSelectorView; }
            set
            {
                _giiSelectorView = value;
                NotifyOfPropertyChange(() => GIISelectorView);
            }
        }

        public void OpenAbout()
        {
            var windowManager = new WindowManager();
            windowManager.ShowDialog(new AboutViewModel());
        }
    }
}
