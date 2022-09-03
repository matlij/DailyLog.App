using DailyLog.Pages;

namespace DailyLog;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(NewSurveyQueryPage), typeof(NewSurveyQueryPage));
        Routing.RegisterRoute(nameof(ChartPage), typeof(ChartPage));
    }
}
