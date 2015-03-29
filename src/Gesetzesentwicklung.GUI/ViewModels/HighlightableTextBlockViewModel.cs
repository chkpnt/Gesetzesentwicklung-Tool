using Caliburn.Micro;
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
    class HighlightableTextBlockViewModel : ViewAware
    {
        private readonly string _text;

        public string Text { get { return _text; } }

        private TextBlock _textBlock;

        public HighlightableTextBlockViewModel(string text)
        {
            _text = text;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);


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
                _textBlock.Text = Text;
                return;
            }

            _textBlock.Text = string.Empty;
            _textBlock.Inlines.Clear();

            var positionen = Regex.Matches(Text, filter, RegexOptions.IgnoreCase)
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
                    yield return new Run(Text.Substring(currentPos, pos - currentPos));
                }

                yield return new Run
                {
                    Text = Text.Substring(pos, highlightLenght),
                    FontWeight = FontWeights.Bold
                };

                currentPos = pos + highlightLenght;
            }

            if (currentPos < Text.Length)
            {
                yield return new Run(Text.Substring(currentPos, Text.Length - currentPos));
            }
        }

        public bool Contains(string GesetzesFilter)
        {
            return Text.IndexOf(GesetzesFilter, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
