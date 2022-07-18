using DailyLog.ViewModels;

namespace DailyLog;

public partial class MainPage : ContentPage
{
	private readonly DailyLogViewModel _viewModel;

	public MainPage()
	{
		InitializeComponent();
		BindingContext = _viewModel = new DailyLogViewModel();
    }

	protected override async void OnAppearing()
	{
		await _viewModel.Initialize();
		base.OnAppearing();
	}
}

