using CommunityToolkit.Mvvm.ComponentModel;
using WindowSill.API;
using WindowSill.OutlookCalendar.Models;

namespace WindowSill.OutlookCalendar.Settings
{
    public class SettingsViewModel : ObservableObject
    {
        private readonly ISettingsProvider _settingsProvider;

        public SettingsViewModel(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        public OfficeVersion SelectedOfficeVersion
        {
            get => _settingsProvider.GetSetting(Settings.SelectedOfficeVersion);
            set => _settingsProvider.SetSetting(Settings.SelectedOfficeVersion, value);
        }
    }
}
