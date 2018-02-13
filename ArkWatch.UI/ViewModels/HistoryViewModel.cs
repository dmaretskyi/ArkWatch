using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using ArkWatch.Models;
using ArkWatch.Storage;
using ArkWatch.UI.History;
using LiveCharts;
using LiveCharts.Definitions.Charts;
using LiveCharts.Definitions.Series;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ArkWatch.UI.ViewModels
{
    public class HistoryViewModel : ReactiveObject
    {
        private readonly IStorageProvider storage;

        private readonly IHistoryStorageProvider historyStorage;

        public IReadOnlyList<Server> Servers { get; }

        [Reactive]
        public Server SelectedServer { get; set; }

        private HistoryData SelectedData { [ObservableAsProperty] get; }

        public SeriesCollection Chart { [ObservableAsProperty] get; }

        public List<string> Labels { get; }

        public HistoryViewModel()
        {
            storage = new JsonStorageProvider("storage.json");
            historyStorage = new HistoryStorageProvider("history");

            var storageData = storage.LoadData();
            var history = historyStorage.LoadData();

            Servers = new List<Server>(storageData.Servers);

            this.WhenAnyValue(x => x.SelectedServer)
                .Where(server => server != null)
                .Select(server => history.Find(server.Address))
                .ToPropertyEx(this, x => x.SelectedData);

            var report = new OnlineHeatMapReport(null);

            this.WhenAnyValue(x => x.SelectedData)
                .Where(data => data != null)
                .Select(data => new SeriesCollection{report.CreateChart(data)})
                .ToPropertyEx(this, x => x.Chart);

            Labels = report.GetLabels();
        }
    }
}