using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using ArkWatch.Models;
using ArkWatch.Storage;

namespace ArkWatch.MonitorService
{
    public class Monitor
    {
        private readonly Timer _timer;
        private readonly IStorageProvider _storage;
        private readonly IHistoryStorageProvider _historyStorage;

        private bool recordRunning = false;

        public Monitor(IStorageProvider storage, IHistoryStorageProvider historyStorage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _historyStorage = historyStorage ?? throw new ArgumentNullException(nameof(historyStorage));

            _timer = new Timer() {AutoReset = true, Interval = TimeSpan.FromMinutes(10).TotalMilliseconds};
            _timer.Elapsed += (sender, args) => RecordData();
        }


        private void RecordData()
        {
            if(recordRunning) return;
            recordRunning = true;

            try
            {
                var data = _storage.LoadData();
                var history = _historyStorage.LoadData();

                foreach (var server in data.Servers)
                {
                    Console.WriteLine($"[{DateTime.Now}] Requesting {server.Address}");

                    var info = ServerQuery.ServerQuery.Query(server.GetIpEndPoint()).Result;
                    history.FindOrCreate(server.Address).Records
                        .Add(new HistoryRecord(DateTime.Now, info.Players.Select(p => p.Name)));

                    var newPlayers = info.Players.Where(player => data.Players.All(p => p.Name != player.Name))
                        .Select(player => new Player(player.Name, ""));

                    Console.WriteLine($"[{DateTime.Now}] Record {server.Address}: {info.Players.Count} players");

                    foreach (var player in newPlayers)
                    {
                        data.Players.Add(player);
                    }
                }

                _storage.SaveData(data);
                _historyStorage.SaveData(history);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            recordRunning = false;
        }

        public void Start()
        {
            _timer.Start();

            Console.WriteLine("Service started");
            Console.WriteLine($"Working dir: {Directory.GetCurrentDirectory()}");
            Console.WriteLine($"Record interval: {Interval}");
        }

        public void Stop()
        {
            _timer.Stop();
            Console.WriteLine("Service stopped");
        }

        public TimeSpan Interval
        {
            get => TimeSpan.FromMilliseconds(_timer.Interval);
            set => _timer.Interval = value.TotalMilliseconds;
        }
    }
}
