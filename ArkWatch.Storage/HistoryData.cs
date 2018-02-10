using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArkWatch.Models;

namespace ArkWatch.Storage
{
    public class HistoryData
    {
        public HistoryData(string serverAddress, IEnumerable<HistoryRecord> records)
        {
            ServerAddress = serverAddress ?? throw new ArgumentNullException(nameof(serverAddress));
            Records = new List<HistoryRecord>(records ?? throw new ArgumentNullException(nameof(records)));
        }

        public string ServerAddress { get; }

        public List<HistoryRecord> Records { get; }
    }
}
