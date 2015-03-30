using Gesetzesentwicklung.GUI.ViewModels;
using Gesetzesentwicklung.Lizenzen;
using Gesetzesentwicklung.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.GUI
{
    class PropertyNameSolver
    {
        public string HighlightableTextBlockViewModel_NormTitle { get; private set; }
        public string Lizenz_Projekt { get; private set; }

        public static readonly PropertyNameSolver Instance = new PropertyNameSolver();

        private PropertyNameSolver()
        {
            HighlightableTextBlockViewModel_NormTitle = ReflectOn<HighlightableTextBlockViewModel>.GetProperty<string>(m => m.NormTitel).Name;
            Lizenz_Projekt = ReflectOn<Lizenz>.GetProperty<string>(m => m.Projekt).Name;
        }
    }
}
