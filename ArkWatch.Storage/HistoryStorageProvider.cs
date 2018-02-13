using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ArkWatch.Models;

namespace ArkWatch.Storage
{
    public class HistoryStorageProvider : IHistoryStorageProvider
    {
        private const char Separator = ',';

        public string Path { get; }

        public HistoryStorageProvider(string filePath)
        {
            Path = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public HistoryDataCollection LoadData()
        {
            if (!Directory.Exists(Path))
            {
                SaveData(new HistoryDataCollection(Enumerable.Empty<HistoryData>()));
            }

            var files = Directory.GetFiles(Path);

            var recordings = files
                .Where(file => file.Substring(file.Length - 4, 4) == ".csv")
                .Select(file => LoadDataFromFile(file));

            return new HistoryDataCollection(recordings);
        }

        private HistoryData LoadDataFromFile(string file)
        {
            var filename = file.Split('\\').Last();
            var address = filename.Substring(0, filename.Length - 4).Replace("_", ":");
            var records = File.ReadAllLines(file)
                .Select(line =>
                {
                    var parts = line.Split(Separator);
                    var time = DateTime.Parse(parts[0]);
                    return new HistoryRecord(time, parts.Skip(1));
                });
            return new HistoryData(address, records);
        }

        public void SaveData(HistoryDataCollection data)
        {
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }

            foreach (var dataRecording in data.Recordings)
            {
                SaveDataFile(System.IO.Path.Combine(Path, $"{dataRecording.ServerAddress.Replace(":", "_")}.csv"), dataRecording);
            }
        }

        private void SaveDataFile(string path, HistoryData data)
        {
            var lines = data.Records.Select(rec =>
                $"{rec.Time},{string.Join(Separator.ToString(), rec.PlayersOnline)}");
            File.WriteAllLines(path, lines);
        }
    }
}
