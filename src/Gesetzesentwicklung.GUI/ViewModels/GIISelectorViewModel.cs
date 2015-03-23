using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.GUI.ViewModels
{
    public class GIISelectorViewModel : PropertyChangedBase
    {
        private string _test;

        public string Test
        {
            get { return _test; }
            set
            {
                _test = value;
            }
        }
    }
}
