using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var files = Directory.GetFiles(Path);

            var recordings = files
                .Where(file => file.Substring(file.Length - 4, 4) == ".csv")
                .Select(file =>
                {
                    var address = file.Substring(0, file.Length - 4);
                    var records = File.ReadAllLines(System.IO.Path.Combine(Path, file))
                        .Select(line =>
                        {
                            var parts = line.Split(Separator);
                            var time = DateTime.Parse(parts[0]);
                            return new HistoryRecord(time, parts.Skip(1));
                        });
                    return new HistoryData(address, records);
                });

            

            return new HistoryDataCollection(recordings);
        }

        public void SaveData(HistoryDataCollection data)
        {
            foreach (var dataRecording in data.Recordings)
            {
                SaveDataFile(System.IO.Path.Combine(Path, $"{dataRecording.ServerAddress}.csv"), dataRecording);
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
