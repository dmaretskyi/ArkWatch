using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArkWatch.Models;

namespace ArkWatch.Storage
{
    public class HistoryDataCollection
    {
        public HistoryDataCollection(IEnumerable<HistoryData> recordings)
        {
            Recordings = new List<HistoryData>(recordings ?? throw new ArgumentNullException(nameof(recordings)));
        }

        public List<HistoryData> Recordings { get; }

        public HistoryData Find(string address)
        {
            return Recordings.FirstOrDefault(r => r.ServerAddress == address);
        }

        public HistoryData FindOrCreate(string address)
        {
            var recording = Recordings.FirstOrDefault(r => r.ServerAddress == address);
            if (recording == null)
            {
                recording = new HistoryData(address, Enumerable.Empty<HistoryRecord>());
                Recordings.Add(recording);
            }
            return recording;
        }
    }
}
