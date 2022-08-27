using Azure.Data.Tables;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyLog.Models;

namespace DailyLog.ViewModels
{
    public partial class NewSurveyQueryViewModel : ObservableObject
    {
        private readonly TableClient _surveyClient;

        public NewSurveyQueryViewModel(TableClient<SurveyEntity> surveyClient)
        {
            _surveyClient = surveyClient.Client;
        }

        [ObservableProperty]
        string _input;

        [RelayCommand]
        async Task AddQuery()
        {
            try
            {
                var result = await _surveyClient.AddEntityAsync(new SurveyEntity { PartitionKey = "Key", RowKey = _input });
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Nått gick fel. Error: {e.Message}", "OK");
            }
        }
    }
}
