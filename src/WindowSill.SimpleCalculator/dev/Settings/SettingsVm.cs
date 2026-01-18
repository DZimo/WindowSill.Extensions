using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using WindowSill.API;

namespace WindowSill.SimpleCalculator.Settings
{
    internal sealed partial class SettingsVm : ObservableObject
    {
        ISettingsProvider _settingsProvider;

        public SettingsVm(ISettingsProvider settingsProvider)
        {
            Guard.IsNotNull(settingsProvider);

            _settingsProvider = settingsProvider;
        }

        public bool IsAutoOpen
        {
            get => _settingsProvider.GetSetting(Settings.AutoPopup);
            set => _settingsProvider.SetSetting(Settings.AutoPopup, value);
        }

        public bool IsAutoPaste
        {
            get => _settingsProvider.GetSetting(Settings.AutoCopyPaste);
            set => _settingsProvider.SetSetting(Settings.AutoCopyPaste, value);
        }
    }
}
