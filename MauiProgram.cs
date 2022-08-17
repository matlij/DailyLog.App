using AutoMapper;
using DailyLog.Models;
using DailyLog.ViewModels;

namespace DailyLog;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<MainPage>();
        builder.Services.AddScoped<DailyLogViewModel>();

        builder.Services.AddTransient<MyPage>();

        builder.Services.AddAutoMapper(typeof(MapperProfile));

		return builder.Build();
	}
}
