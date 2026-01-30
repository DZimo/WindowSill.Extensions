using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using WindowSill.API;

namespace WindowSill.ScreenRecorder.Settings
{
    internal sealed class SettingsVm : ObservableObject
    {
        ISettingsProvider _settingsProvider;

        public SettingsVm(ISettingsProvider settingsProvider)
        {
            Guard.IsNotNull(settingsProvider);

            _settingsProvider = settingsProvider;
        }

        public string ScreenshotSavePath
        {
            get => _settingsProvider.GetSetting(Settings.ScreenshotSavePath);
            set
            {
                _settingsProvider.SetSetting(Settings.ScreenshotSavePath, value);
                OnPropertyChanged(nameof(SettingsVm.ScreenshotSavePath));
            }
        }
    }
}
