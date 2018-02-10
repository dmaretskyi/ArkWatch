using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArkWatch.Storage
{
    public class HistoryDataCollection
    {
        public HistoryDataCollection(IEnumerable<HistoryData> recordings)
        {
            Recordings = new List<HistoryData>(recordings ?? throw new ArgumentNullException(nameof(recordings)));
        }

        public List<HistoryData> Recordings { get; }

        public HistoryData FindByServerAddress(string address)
        {
            return Recordings.FirstOrDefault(r => r.ServerAddress == address);
        }
    }
}
