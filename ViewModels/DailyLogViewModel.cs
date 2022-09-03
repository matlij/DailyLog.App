using AutoMapper;
using Azure.Data.Tables;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyLog.Models;
using DailyLog.Pages;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;

namespace DailyLog.ViewModels
{

    public partial class DailyLogViewModel : ObservableObject
    {
        private readonly TableClient _logClient;
        private readonly TableClient _surveyClient;
        private readonly IMapper _mapper;

        public RadioButtonViewModel Symptoms { get; set; } = new RadioButtonViewModel { GroupName = "Symptoms" };
        public ObservableCollection<RadioButtonViewModel> RadioButtons { get; set; } = new ObservableCollection<RadioButtonViewModel>();

        [ObservableProperty]
        bool _isBusy;

        [RelayCommand]
        async Task Save()
        {
            IsBusy = true;

            try
            {
                var transactions = RadioButtons
                    .Select(r => new TableTransactionAction(TableTransactionActionType.UpsertReplace, CreateLogValueEntity(r)))
                    .ToList();
                transactions.Add(new TableTransactionAction(TableTransactionActionType.UpsertReplace, CreateLogValueEntity(Symptoms)));

                var result = await _logClient.SubmitTransactionAsync(transactions);
                if (result.Value.Any(r => r.IsError))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Nått gick fel. Statuskoder: {string.Join(", ", result.Value.Select(r => r.Status))}", "OK");
                }
                else
                {
                    //await Application.Current.MainPage.DisplayAlert("Sparat", $"Log för {entity.RowKey} sparat!", "OK");

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
        async Task NewSurveyQuery()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState(nameof(NewSurveyQueryPage)));
        }

        [RelayCommand]
        async Task OpenChartPage()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState(nameof(ChartPage)));
        }

        public DailyLogViewModel(IMapper mapper, TableClient<LogValueEntity> logClient, TableClient<SurveyEntity> surveyClient)
        {
            _mapper = mapper;
            _logClient = logClient.Client;
            _surveyClient = surveyClient.Client;
        }

        private static string GetDateNowString()
        {
            return DateTime.Now.ToString("yyyyMMdd");
        }

        public async Task Initialize()
        {
            IsBusy = true;

            var survey = _surveyClient.QueryAsync<SurveyEntity>(select: new[] { nameof(SurveyEntity.RowKey), nameof(SurveyEntity.CustomContent) });

            RadioButtons.Clear();
            await foreach (var surveyRow in survey)
            {
                var radioButton = new RadioButtonViewModel()
                {
                    GroupName = surveyRow.RowKey
                };
                if (!string.IsNullOrEmpty(surveyRow.CustomContent))
                {
                    radioButton.Content = JsonSerializer.Deserialize<string[]>(surveyRow.CustomContent);
                }
                radioButton.Selection = await GetSelection(surveyRow.RowKey);

                RadioButtons.Add(radioButton);
            }

            Symptoms.Selection = await GetSelection(nameof(Symptoms));
            
            IsBusy = false;
        }

        private async Task<string> GetSelection(string name)
        {
            try
            {
                var result = await _logClient.GetEntityAsync<LogValueEntity>(GetDateNowString(), name, select: new[] { nameof(LogValueEntity.Selection) });
                return result.Value.Selection.ToString();
            }
            catch (Exception)
            {
                Debug.WriteLine("Couldn't find existing survey data for: " + name);
            }

            return default;
        }

        private static LogValueEntity CreateLogValueEntity(RadioButtonViewModel r)
        {
            return new LogValueEntity()
            {
                PartitionKey = GetDateNowString(),
                RowKey = r.GroupName,
                Selection = Convert.ToInt32(r.Selection)
            };
        }
    }
}
