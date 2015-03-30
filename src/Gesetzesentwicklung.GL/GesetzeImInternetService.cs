using Gesetzesentwicklung.GII;
using Gesetzesentwicklung.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.GL
{
    public class GesetzeImInternetService
    {
        public async Task<Gesetzesverzeichnis> GetGesetzesverzeichnis()
        {
            var verzeichnisLader = new VerzeichnisLader();
            return await verzeichnisLader.LadeVerzeichnis();
        }
    }
}
