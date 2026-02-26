using CommunityToolkit.Mvvm.ComponentModel;
using WindowSill.API;
using WindowSill.OutlookCalendar.Enums;

namespace WindowSill.OutlookCalendar.Settings
{
    public partial class SettingsViewModel : ObservableObject
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

        public AccountType SelectedAccountType
        {
            get => _settingsProvider.GetSetting(Settings.SelectedAccountType);
            set => _settingsProvider.SetSetting(Settings.SelectedAccountType, value);
        }
    }
}
