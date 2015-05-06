using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesetzesentwicklung.Shared
{
    public static class DateTimeExtensions
    {
        private readonly static DateTime Epoch0 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToEpoch(this DateTime date) => Convert.ToInt64((date.ToUniversalTime() - Epoch0).TotalSeconds);
    }
}
