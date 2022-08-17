namespace DailyLog;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(MyPage), typeof(MyPage));
	}
}
