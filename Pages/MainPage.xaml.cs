using DailyLog.ViewModels;

namespace DailyLog;

public partial class MainPage : ContentPage
{
	private readonly DailyLogViewModel _viewModel;

	public MainPage(DailyLogViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
    }

	protected override async void OnAppearing()
	{
		await _viewModel.Initialize();
		base.OnAppearing();
	}

	private async void DatePicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (_viewModel != null && _viewModel.Date > _viewModel.DateMinValue)
		{
	        await _viewModel.UpdateSelection();
		}
    }
}

