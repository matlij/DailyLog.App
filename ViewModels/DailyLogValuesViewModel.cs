using CommunityToolkit.Mvvm.ComponentModel;

namespace DailyLog.ViewModels
{
    public partial class DailyLogValuesViewModel : ObservableObject
    {
        [ObservableProperty]
        string _health;

        [ObservableProperty]
        string _traning;

        [ObservableProperty]
        string _coffea;

        [ObservableProperty]
        string _sauna;
    }
}
