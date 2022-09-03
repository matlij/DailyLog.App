using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Azure.Data.Tables;
using CommunityToolkit.Mvvm.ComponentModel;
using DailyLog.Models;

namespace DailyLog.ViewModels
{
    public partial class ChartViewModel : ObservableObject
    {
        private readonly TableClient _client;

        [ObservableProperty]
        public IEnumerable<ISeries> _series;

        public ChartViewModel(TableClient<LogValueEntity> tableClient)
        {
            _client = tableClient.Client;
        }

        public async Task Init()
        {
            const int daysOld = 14;
            var startDate = DateTime.Now.AddDays(-daysOld);
            var result = _client.QueryAsync<LogValueEntity>(r => r.Timestamp > startDate);
            var entities = new List<LogValueEntity>();
            await foreach (var logEntity in result)
            {
                entities.Add(logEntity);
            }
            var series = new List<LineSeries<int>>();
            foreach (var entitiesByType in entities.Where(e => e.RowKey != "Traning").GroupBy(e => e.RowKey))
            {
                var values = new List<int>();
                var dateCursor = startDate;
                for (int i = 0; i <= daysOld; i++)
                {
                    var value = entitiesByType.FirstOrDefault(e => e.Timestamp.Value.Day == dateCursor.Day)?.Selection ?? 0;
                    values.Add(value);
                    dateCursor = dateCursor.AddDays(1);
                }
                series.Add(new LineSeries<int>()
                {
                    Name = entitiesByType.Key,
                    Values = values,
                    Fill = null,
                    
                });
            }

            Series = series;
        }
    }
}
