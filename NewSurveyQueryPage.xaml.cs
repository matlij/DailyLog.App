using DailyLog.ViewModels;

namespace DailyLog;

public partial class NewSurveyQueryPage : ContentPage
{
	public NewSurveyQueryPage(NewSurveyQueryViewModel viewModel)
	{
		BindingContext = viewModel;

        InitializeComponent();
	}
}