using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using ArkWatch.Models;
using ArkWatch.Storage;
using NLog;

namespace ArkWatch.MonitorService
{
    public class Monitor
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private Timer _timer;
        private readonly IStorageProvider _storage;
        private readonly IHistoryStorageProvider _historyStorage;

        private bool recordRunning = false;

        public Monitor(IStorageProvider storage, IHistoryStorageProvider historyStorage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _historyStorage = historyStorage ?? throw new ArgumentNullException(nameof(historyStorage));

            CreateTimer();
        }

        private void CreateTimer()
        {
            _timer = new Timer() { AutoReset = true, Interval = TimeSpan.FromMinutes(10).TotalMilliseconds };
            _timer.Elapsed += (sender, args) => OnTimer();
        }

        private void OnTimer()
        {
            if(recordRunning) return;
            recordRunning = true;

            try
            {
                RecordData();
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            recordRunning = false;
        }

        private void RecordData()
        {
            var data = _storage.LoadData();
            var history = _historyStorage.LoadData();

            foreach (var server in data.Servers)
            {
                logger.Debug("Requesting {0}", server.Address);

                var info = ServerQuery.ServerQuery.Query(server.GetIpEndPoint()).Result;
                history.FindOrCreate(server.Address).Records
                    .Add(new HistoryRecord(DateTime.Now, info.Players.Select(p => p.Name)));

                var newPlayers = info.Players.Where(player => data.Players.All(p => p.Name != player.Name))
                    .Select(player => new Player(player.Name, ""));

                logger.Debug("Record {0}: {1} players", server.Address, info.Players.Count);

                foreach (var player in newPlayers)
                {
                    data.Players.Add(player);
                }
            }

            _storage.SaveData(data);
            _historyStorage.SaveData(history);
        }

        public void Start()
        {
            TryStartTimer();

            logger.Info("Service started");
            logger.Debug("Working dir: {0}", Directory.GetCurrentDirectory());
            logger.Debug("Record interval: {0}", Interval);
        }

        public void Pause()
        {
            TryStopTimer();
        }

        public void Continue()
        {
            TryStartTimer();
        }

        public void Stop()
        {
            TryStopTimer();
            logger.Info("Service stopped");
        }

        private void TryStartTimer()
        {
            if (_timer == null)
            {
                CreateTimer();

                logger.Error("Timer is null during start command");
            }

            try
            {
                _timer.Start();
            }
            catch (Exception e)
            {
                logger.Fatal(e);
            }
        }

        private void TryStopTimer()
        {
            try
            {
                _timer.Stop();
            }
            catch (Exception e)
            {
                logger.Fatal(e);
            }
        }

        public TimeSpan Interval
        {
            get => TimeSpan.FromMilliseconds(_timer.Interval);
            set => _timer.Interval = value.TotalMilliseconds;
        }
    }
}
