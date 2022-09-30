using DailyLog.ViewModels;

namespace DailyLog.Pages;

public partial class ChartPage : ContentPage
{
	private readonly ChartViewModel _viewModel;

	public ChartPage(ChartViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}

	protected override async void OnAppearing()
	{
		await _viewModel.Init();

        base.OnAppearing();
	}

	private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (_viewModel != null)
		{
	        await _viewModel.UpdateChart(14);
		}
	}
}