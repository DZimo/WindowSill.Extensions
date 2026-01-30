using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using WindowSill.API;
using WindowSill.PomodoroTimer.Models;

namespace WindowSill.PomodoroTimer.Settings
{
    public sealed partial class SettingsVm : ObservableObject
    {
        ISettingsProvider _settingsProvider;

        public static SettingsVm? Instance;

        public SettingsVm(ISettingsProvider settingsProvider)
        {
            Guard.IsNotNull(settingsProvider);

            _settingsProvider = settingsProvider;
            TimeDisplayMode = _settingsProvider.GetSetting(Settings.DisplayMode);
            Instance = this;
        }

        public TimeDisplayMode TimeDisplayMode
        {
            get => _settingsProvider.GetSetting(Settings.DisplayMode);
            set
            {
                if (TimeDisplayMode == value)
                    return;

                _settingsProvider.SetSetting(Settings.DisplayMode, value);

                OnPropertyChanged(nameof(TimeDisplayMode));
                WeakReferenceMessenger.Default.Send("");
            }
        }
    }
}
