using AutoMapper;
using DailyLog.Models;
using DailyLog.ViewModels;

namespace DailyLog;

public class MapperProfile : Profile
{
	public MapperProfile()
	{
        CreateMap<LogValueEntity, RadioButtonViewModel>();
        CreateMap<RadioButtonViewModel, LogValueEntity>();
    }
}