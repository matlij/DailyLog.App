using DailyLog.Models;
using DailyLog.Pages;
using DailyLog.ViewModels;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace DailyLog;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
            .UseSkiaSharp(true)
			.UseMauiApp<App>()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        var cs = "DefaultEndpointsProtocol=https;AccountName=dailylogstorage;AccountKey=SbalUV4skdA3fxsfIond8qA6VCGWDtF8UVTTtB5hIZGN0jpHzmTgEL9zb+pl0YNkTTPateLz9atx+AStHzSS9g==;EndpointSuffix=core.windows.net";
        builder.Services.AddScoped(s => new TableClient<SurveyEntity>(cs, "Survey"));
        builder.Services.AddScoped(s => new TableClient<LogValueEntity>(cs, "DailyLog"));

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddScoped<DailyLogViewModel>();

        builder.Services.AddTransient<NewSurveyQueryPage>();
        builder.Services.AddTransient<NewSurveyQueryViewModel>();

        builder.Services.AddTransient<ChartPage>();
        builder.Services.AddTransient<ChartViewModel>();

        builder.Services.AddAutoMapper(typeof(MapperProfile));

		return builder.Build();
	}
}