using Caliburn.Micro;
using Gesetzesentwicklung.GL;
using Gesetzesentwicklung.Shared;
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
    public class GesetzeImInternetViewModel : PropertyChangedBase
    {
        private string _gesetzesFilter;

        private HighlightableTextBlockViewModel _selectedHighlightableTextBlockViewModel;

        private ICollectionView _gesetzeImInternetView;

        private GesetzeImInternetService _gesetzesService;

        public string GesetzesFilter
        {
            get { return _gesetzesFilter; }
            set
            {
                _gesetzesFilter = value;
                NotifyOfPropertyChange(() => GesetzesFilter);

                _gesetzeImInternetView.Refresh();
            }
        }

        public List<HighlightableTextBlockViewModel> GesetzeImInternet { get; private set; }

        public HighlightableTextBlockViewModel SelectedGesetzeImInternet
        {
            get { return _selectedHighlightableTextBlockViewModel; }
            set
            {
                _selectedHighlightableTextBlockViewModel = value;
                NotifyOfPropertyChange(() => SelectedGesetzeImInternet);
                NotifyOfPropertyChange(() => IsItemSelected);
            }
        }

        public bool IsItemSelected => SelectedGesetzeImInternet != null;


        public GesetzeImInternetViewModel()
        {
            _gesetzesService =  new GesetzeImInternetService();
            _gesetzesFilter = "";

            GesetzeImInternet = new List<HighlightableTextBlockViewModel>();
            _gesetzeImInternetView = CollectionViewSource.GetDefaultView(GesetzeImInternet);
            _gesetzeImInternetView.Filter = new Predicate<object>(GesetzeImInternet_FilterUndHighlighter);
            _gesetzeImInternetView.SortDescriptions.Add(
                new SortDescription(PropertyNameSolver.Instance.HighlightableTextBlockViewModel_NormTitle, ListSortDirection.Ascending)
            );

            Task.Run(() =>
            {
                var verzeichnis = _gesetzesService.GetGesetzesverzeichnisAsync().Result;
                var normen = from norm in verzeichnis.Normen
                             select new HighlightableTextBlockViewModel(norm);

                Execute.OnUIThread(() =>
                {
                    GesetzeImInternet.Clear();
                    GesetzeImInternet.AddRange(normen);
                    _gesetzeImInternetView.Refresh();
                });
            });
        }

        private bool GesetzeImInternet_FilterUndHighlighter(object o)
        {
            var highlightableTextBlock = o as HighlightableTextBlockViewModel;
            var showNorm = highlightableTextBlock.Contains(GesetzesFilter);
            if (showNorm)
            {
                highlightableTextBlock.HighlightText(GesetzesFilter);
            }
            return showNorm;
        }

        public void GenerateMarkdown()
        {
            if (SelectedGesetzeImInternet == null)
            {
                return;
            }

            //Task.Run(() =>
            //{
                _gesetzesService.GenerateMarkdownAsync(SelectedGesetzeImInternet.Norm);
            //});
        }

    }
}
