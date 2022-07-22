using Azure;
using Azure.Data.Tables;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyLog.Models;
using System.Diagnostics;

namespace DailyLog.ViewModels
{
    public partial class RadioButtonViewModel : ObservableObject
    {
        [ObservableProperty]
        string _groupName;

        [ObservableProperty]
        object _selection;
    }

    public partial class DailyLogViewModel : ObservableObject
    {
        private const string PartitionKey = "Key";
        private readonly TableClient _tableClient;

        public RadioButtonViewModel Health { get; } = new RadioButtonViewModel() { GroupName = "Health" };
        public RadioButtonViewModel Training { get; } = new RadioButtonViewModel() { GroupName = "Training" };
        public RadioButtonViewModel Coffea { get; } = new RadioButtonViewModel() { GroupName = "Coffea" };
        public RadioButtonViewModel Sauna { get; } = new RadioButtonViewModel() { GroupName = "Sauna" };

        [ObservableProperty]
        bool _isBusy;

        [RelayCommand]
        async Task Save()
        {
            IsBusy = true;

            try
            {
                var entity = new LogEntity
                {
                    PartitionKey = PartitionKey,
                    RowKey = RowKey(),
                    Training = (TraningType)int.Parse(Training.Selection.ToString()),
                    Coffea = int.Parse(Coffea.Selection.ToString()),
                    Sauna = int.Parse(Sauna.Selection.ToString()),
                };
                var result = await _tableClient.UpsertEntityAsync(entity);
                if (result.IsError)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Nått gick fel. Statuskod: {result.Status}", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Sparat", $"Log för {entity.RowKey} sparat!", "OK");

                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Nått gick fel. Error: {e.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        Task UpdateLog()
        {
            return Initialize();
        }

        public DailyLogViewModel()
        {
            _tableClient = new TableClient("DefaultEndpointsProtocol=https;AccountName=dailylogstorage;AccountKey=SbalUV4skdA3fxsfIond8qA6VCGWDtF8UVTTtB5hIZGN0jpHzmTgEL9zb+pl0YNkTTPateLz9atx+AStHzSS9g==;EndpointSuffix=core.windows.net", "DailyLog");
            _tableClient.CreateIfNotExists();
        }

        private static string RowKey()
        {
            return DateTime.Now.ToString("yyyyMMdd");
        }

        public async Task Initialize()
        {
            IsBusy = true;

            var result = await GetTodaysLog();
            Health.Selection = result.Health.ToString();
            Coffea.Selection = result.Coffea.ToString();
            Sauna.Selection = result.Sauna.ToString();
            Training.Selection = ((int)result.Training).ToString();

            IsBusy = false;
        }

        private async Task<LogEntity> GetTodaysLog()
        {
            try
            {
                var result = await _tableClient.GetEntityAsync<LogEntity>(PartitionKey, RowKey());
                return result;
            }
            catch (Exception e)
            {
                Trace.WriteLine( $"Misslyckades med att hämta log. Error: {e.Message}");
            }

            return new LogEntity();
        }
    }
}
