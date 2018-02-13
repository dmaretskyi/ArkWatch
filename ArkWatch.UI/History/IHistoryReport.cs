using System.Collections.Generic;
using ArkWatch.Storage;
using LiveCharts;
using LiveCharts.Definitions.Charts;
using LiveCharts.Definitions.Series;

namespace ArkWatch.UI.History
{
    public interface IHistoryReport
    {
        ISeriesView CreateChart(HistoryData data);

        List<string> GetLabels();
    }
}