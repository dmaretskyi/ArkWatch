using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArkWatch.Models
{
    public class HistoryRecord
    {
        public HistoryRecord(DateTime time, IEnumerable<string> playersOnline)
        {
            Time = time;
            PlayersOnline = new List<string>(playersOnline ?? throw new ArgumentNullException(nameof(playersOnline)));
        }

        public DateTime Time { get; }

        public IEnumerable<string> PlayersOnline { get; }
    }
}
