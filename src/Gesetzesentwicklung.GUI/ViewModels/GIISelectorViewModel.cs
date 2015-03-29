using Caliburn.Micro;
using Gesetzesentwicklung.GII;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Gesetzesentwicklung.GUI.ViewModels
{
    public class GIISelectorViewModel : PropertyChangedBase
    {
        private string _gesetzesFilter;

        public string GesetzesFilter
        {
            get { return _gesetzesFilter; }
            set
            {
                _gesetzesFilter = value;
                NotifyOfPropertyChange(() => GesetzesFilter);

                GesetzeImInternet.Refresh();
            }
        }

        public ListCollectionView GesetzeImInternet { get; private set; }

        public GIISelectorViewModel()
        {
            _gesetzesFilter = "";
            //_gesetzeImInternet = new CollectionViewSource();
            //_gesetzeImInternet.Source = new List<string> { "bla", "blub " };
            //_gesetzeImInternet.View.Refresh();
            //Refresh();

            Task.Run(() =>
            {
                var verzeichnisLader = new VerzeichnisLader();
                var xmlVerzeichnis = verzeichnisLader.LadeVerzeichnis().Result;
                var normen = from norm in xmlVerzeichnis.Normen
                             select new HighlightableTextBlockViewModel(norm.Titel);

                Execute.OnUIThread(() =>
                {
                    GesetzeImInternet = new ListCollectionView(normen.ToList());
                    GesetzeImInternet.Filter = o =>
                        {
                            var s = o as HighlightableTextBlockViewModel;
                            var showNorm = s.Contains(GesetzesFilter);
                            if (showNorm)
                            {
                                s.HighlightText(GesetzesFilter);
                            }
                            return showNorm;
                        };
                    GesetzeImInternet.SortDescriptions.Add(new SortDescription("Text", ListSortDirection.Ascending));
                    NotifyOfPropertyChange(() => GesetzeImInternet);
                });
            });
        }

    }
}
