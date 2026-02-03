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

            try
            {
                TimeDisplayMode = _settingsProvider.GetSetting(Settings.DisplayMode);
            }
            catch (ArgumentException ex)
            {
                _settingsProvider.SetSetting(Settings.DisplayMode, TimeDisplayMode.TimeLeft);
                TimeDisplayMode = _settingsProvider.GetSetting(Settings.DisplayMode);
            }
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
        public int ShortBreakDuration
        {
            get => _settingsProvider.GetSetting(Settings.ShortBreakDuration);
            set
            {
                if (ShortBreakDuration == value)
                    return;

                _settingsProvider.SetSetting(Settings.ShortBreakDuration, value);

                OnPropertyChanged(nameof(ShortBreakDuration));
            }
        }

        public int LongBreakDuration
        {
            get => _settingsProvider.GetSetting(Settings.LongBreakDuration);
            set
            {
                if (LongBreakDuration == value)
                    return;

                _settingsProvider.SetSetting(Settings.LongBreakDuration, value);

                OnPropertyChanged(nameof(LongBreakDuration));
            }
        }
    }
}
