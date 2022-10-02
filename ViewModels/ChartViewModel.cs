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
        public ObservableCollection<ISeries> _series = new();

        public Axis[] XAxes { get; set; } =
        {
                new Axis
                {
                    Labeler = value => new DateTime((long) value).ToString("MMMM dd"),
                    LabelsRotation = 15,
        
                    // when using a date time type, let the library know your unit 
                    UnitWidth = TimeSpan.FromDays(1).Ticks, 
        
                    // if the difference between our points is in hours then we would:
                    // UnitWidth = TimeSpan.FromHours(1).Ticks,
        
                    // since all the months and years have a different number of days
                    // we can use the average, it would not cause any visible error in the user interface
                    // Months: TimeSpan.FromDays(30.4375).Ticks
                    // Years: TimeSpan.FromDays(365.25).Ticks
        
                    // The MinStep property forces the separator to be greater than 1 day.
                    MinStep = TimeSpan.FromDays(1).Ticks
                }
            };

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

            var symptoms = new SurveyDto { Name = SurveyConstants.Symptoms };
            SelectedSurveyQueries.Add(symptoms);
            Survey.Add(symptoms);

            var survey = _surveyClient.QueryAsync<SurveyEntity>(select: new[] { "RowKey" });
            await foreach (var item in survey)
            {
                if (item.RowKey != "Traning")
                {
                    Survey.Add(new SurveyDto { Name = item.RowKey });
                }
            }

            await UpdateChart(28);
        }

        public async Task UpdateChart(int daysOld)
        {
            var selectedQueries = SelectedSurveyQueries.Cast<SurveyDto>();
            RemoveUnselectedChartLines(selectedQueries);

            var selectedNew = selectedQueries.Where(s => !Series.Any(c => c.Name == s.Name));
            if (!selectedNew.Any())
            {
                return;
            }

            var logRowsGroupedByQuery = (await GetLog(daysOld))
                .Where(r => selectedNew.Any(s => s.Name == r.RowKey))
                .GroupBy(e => e.RowKey);
            foreach (var logGroup in logRowsGroupedByQuery)
            {
                Series.Add(new LineSeries<DateTimePoint>()
                {
                    Name = logGroup.Key,
                    Values = GetChartSeriesValues(daysOld, DateTime.Now.AddDays(-daysOld), logGroup),
                    Fill = null
                });
            }
        }

        private async Task<List<LogValueEntity>> GetLog(int fromDays)
        {
            var filter = string.Empty;
            for (int i = 0; i < fromDays; i++)
            {
                if (i > 0)
                {
                    filter += " or ";
                }
                filter += $"PartitionKey eq '{DateTime.Now.AddDays(-i):yyyyMMdd}'";
            }

            var result = _logClient.QueryAsync<LogValueEntity>(filter: filter);
            var entities = new List<LogValueEntity>();
            await foreach (var logEntity in result)
            {
                entities.Add(logEntity);
            }

            return entities;
        }

        private static List<DateTimePoint> GetChartSeriesValues(int daysOld, DateTime startDate, IGrouping<string, LogValueEntity> entitiesByType)
        {
            var values = new List<DateTimePoint>();
            var dateCursor = startDate;
            for (int i = 0; i <= daysOld; i++)
            {
                var value = entitiesByType.FirstOrDefault(e => ParseDate(e.PartitionKey).Day == dateCursor.Day)?.Selection ?? null;
                values.Add(new DateTimePoint(dateCursor, value));
                dateCursor = dateCursor.AddDays(1);
            }

            return values;
        }

        private void RemoveUnselectedChartLines(IEnumerable<SurveyDto> selectedQueries)
        {
            Series
                .Where(c => !selectedQueries.Any(s => s.Name == c.Name))
                .ToList()
                .ForEach(s => Series.Remove(s));
        }

        private static DateTime ParseDate(string date)
        {
            ReadOnlySpan<char> span = date;
            var year = int.Parse(span.Slice(0, 4));
            var month = int.Parse(span.Slice(4, 2));
            var day = int.Parse(span.Slice(6, 2));

            return new DateTime(year, month, day);
        }
    }

}
