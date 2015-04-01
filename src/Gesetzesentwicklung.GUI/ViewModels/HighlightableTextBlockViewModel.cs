using Caliburn.Micro;
using Gesetzesentwicklung.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Gesetzesentwicklung.GUI.ViewModels
{
    public class HighlightableTextBlockViewModel : ViewAware
    {
        public Gesetzesverzeichnis.Norm Norm { get; private set; }

        public string NormTitel { get { return Norm.Titel; } }

        private TextBlock _textBlock;

        public HighlightableTextBlockViewModel(Gesetzesverzeichnis.Norm norm)
        {
            Norm = norm;
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);

            var frameworkElement = view as FrameworkElement;

            if (frameworkElement == null)
            {
                return;
            }

            _textBlock = frameworkElement.FindName("TextBlock") as TextBlock;
        }


        public void HighlightText(string filter)
        {
            if (_textBlock == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(filter))
            {
                _textBlock.Text = NormTitel;
                return;
            }

            _textBlock.Text = string.Empty;
            _textBlock.Inlines.Clear();

            var positionen = Regex.Matches(NormTitel, filter, RegexOptions.IgnoreCase)
                                  .Cast<Match>()
                                  .Select(m => m.Index);

            var runs = BuildRuns(positionen, filter.Length);
            _textBlock.Inlines.AddRange(runs);
        }

        private IEnumerable<Run> BuildRuns(IEnumerable<int> highlightPositions, int highlightLenght)
        {
            var currentPos = 0;

            foreach (var pos in highlightPositions)
            {
                if (currentPos < pos)
                {
                    yield return new Run(NormTitel.Substring(currentPos, pos - currentPos));
                }

                yield return new Run
                {
                    Text = NormTitel.Substring(pos, highlightLenght),
                    FontWeight = FontWeights.Bold
                };

                currentPos = pos + highlightLenght;
            }

            if (currentPos < NormTitel.Length)
            {
                yield return new Run(NormTitel.Substring(currentPos, NormTitel.Length - currentPos));
            }
        }

        public bool Contains(string GesetzesFilter)
        {
            return NormTitel.IndexOf(GesetzesFilter, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
