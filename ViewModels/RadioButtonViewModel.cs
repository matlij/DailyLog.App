using CommunityToolkit.Mvvm.ComponentModel;

namespace DailyLog.ViewModels
{
    public partial class RadioButtonViewModel : ObservableObject
    {
        public string[] Content { get; set; } = new string[] { "0", "1", "2", "3", "4" };

        [ObservableProperty]
        string _groupName;

        [ObservableProperty]
        object _selection;
    }
}
