using Azure.Data.Tables;
using CommunityToolkit.Mvvm.ComponentModel;
using DailyLog.Models;
using DailyLog.Models.Constants;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;

namespace DailyLog.ViewModels
{
    public partial class ChartViewModel : ObservableObject
    {
        private readonly TableClient _logClient;
        private readonly TableClient _surveyClient;

        [ObservableProperty]
        public IEnumerable<ISeries> _series;

        [ObservableProperty]
        public ObservableCollection<SurveyDto> _survey = new();

        [ObservableProperty]
        public ObservableCollection<object> _selectedSurveyQueries = new();

        public ChartViewModel(TableClient<LogValueEntity> logClient, TableClient<SurveyEntity> surveyClient)
        {
            if (logClient is null)
            {
                throw new ArgumentNullException(nameof(logClient));
            }

            if (surveyClient is null)
            {
                throw new ArgumentNullException(nameof(surveyClient));
            }

            _logClient = logClient.Client;
            _surveyClient = surveyClient.Client;
        }

        public async Task Init()
        {
            Survey.Clear();
            Survey.Add(new SurveyDto { Name = SurveyConstants.Symptoms });

            var survey = _surveyClient.QueryAsync<SurveyEntity>(select: new[] { "RowKey" });
            await foreach (var item in survey)
            {
                if (item.RowKey != "Traning")
                {
                    Survey.Add(new SurveyDto { Name = item.RowKey });
                }
            }

            await UpdateChart(14);
        }

        public async Task UpdateChart(int daysOld)
        {
            var selected = _selectedSurveyQueries.Cast<SurveyDto>().ToList();
            if (!selected.Any())
            {
                return;
            }

            var startDate = DateTime.Now.AddDays(-daysOld);
            var result = _logClient.QueryAsync<LogValueEntity>(r => r.Timestamp > startDate.AddDays(-1));
            var entities = new List<LogValueEntity>();
            await foreach (var logEntity in result)
            {
                entities.Add(logEntity);
            }

            var series = new List<LineSeries<DateTimePoint>>();
            foreach (var entitiesByType in entities.Where(r => selected.Any(s => s.Name == r.RowKey)).GroupBy(e => e.RowKey))
            {
                var values = new List<DateTimePoint>();
                var dateCursor = startDate;
                for (int i = 0; i <= daysOld; i++)
                {
                    var value = entitiesByType.FirstOrDefault(e => ParseDate(e.PartitionKey).Day == dateCursor.Day)?.Selection ?? null;
                    values.Add(new DateTimePoint(dateCursor, value));
                    dateCursor = dateCursor.AddDays(1);
                }
                series.Add(new LineSeries<DateTimePoint>()
                {
                    Name = entitiesByType.Key,
                    Values = values,
                    Fill = null,
                    GeometrySize = entitiesByType.Key == Models.Constants.SurveyConstants.Symptoms ? 40 : 10
                });
            }

            Series = series;
        }
        private DateTime ParseDate(string date)
        {
            ReadOnlySpan<char> span = date;
            var year = int.Parse(span.Slice(0, 4));
            var month = int.Parse(span.Slice(4, 2));
            var day = int.Parse(span.Slice(6, 2));

            return new DateTime(year, month, day);
        }
    }

}
