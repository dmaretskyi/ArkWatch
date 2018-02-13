using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArkWatch.Storage;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using Microsoft.Win32.SafeHandles;

namespace ArkWatch.UI.History
{
    public class OnlineHeatMapReport : IHistoryReport
    {
        private const int IntervalsPerDay = 12;
        private const int DaysPerWeek = 7;

        private readonly Func<string, bool> predicate;

        public OnlineHeatMapReport(Func<string, bool> predicate = null) 
        {
            this.predicate = predicate ?? (_ => true);
        }

        private int GetIntervalIndex(DateTime date)
        {
            var seconds = date.TimeOfDay.TotalSeconds;
            int idx = (int)Math.Floor(seconds / (TimeSpan.FromDays(1).TotalSeconds / IntervalsPerDay));
            return idx;
        }

        private int GetDayIndex(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Sunday: return 6;
                default: return (int) day - 1;
            }
        }

        private int[,] CountPlayers(HistoryData data)
        {
            var playersOnline = new HashSet<string>[DaysPerWeek, IntervalsPerDay];
            for (int day = 0; day < DaysPerWeek; day++)
            {
                for (int interval = 0; interval < IntervalsPerDay; interval++)
                {
                    playersOnline[day, interval] = new HashSet<string>();
                }
            }

            foreach (var record in data.Records)
            {
                int day = GetDayIndex(record.Time.DayOfWeek);
                int interval = GetIntervalIndex(record.Time);

                foreach (var player in record.PlayersOnline.Where(predicate))
                {
                    playersOnline[day, interval].Add(player);
                }
            }

            var playerCounts = new int[DaysPerWeek, IntervalsPerDay];

            for (int day = 0; day < DaysPerWeek; day++)
            {
                for (int interval = 0; interval < IntervalsPerDay; interval++)
                {
                    playerCounts[day, interval] = playersOnline[day, interval].Count;
                }
            }

            return playerCounts;
        }   

        public ISeriesView CreateChart(HistoryData data)
        {
            var counts = CountPlayers(data);
            var values = new ChartValues<HeatPoint>();

            for (int day = 0; day < DaysPerWeek; day++)
            {
                for (int interval = 0; interval < IntervalsPerDay; interval++)
                {
                    values.Add(new HeatPoint(interval, day, counts[day, interval]));
                }
            }

            return new HeatSeries {Values = values, DataLabels = true};
        }

        public List<string> GetLabels()
        {
            var secondsPerInterval = TimeSpan.FromDays(1).TotalSeconds / IntervalsPerDay;

            return Enumerable.Range(0, IntervalsPerDay)
                .Select(i =>
                    $"{TimeSpan.FromSeconds(secondsPerInterval * i).ToString("hh\\:mm")}-{TimeSpan.FromSeconds(secondsPerInterval * (i + 1)).ToString("hh\\:mm")}")
                .ToList();
        }
    }
}
