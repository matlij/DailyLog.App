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

	private void OnSaveClicked(object sender, EventArgs e)
	{
		SemanticScreenReader.Announce($"You have selected {_viewModel.Coffea.Selection}");
	}
}

