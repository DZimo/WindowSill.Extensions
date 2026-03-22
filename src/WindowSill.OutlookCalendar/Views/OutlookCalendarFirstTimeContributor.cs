using System.ComponentModel;
using WindowSill.API;
using WindowSill.OutlookCalendar.Settings;

namespace WindowSill.OutlookCalendar.Views;

public sealed class OutlookCalendarFirstTimeContributor : UserControl, IFirstTimeSetupContributor
{
    public bool CanContinue => true;

    public event PropertyChangedEventHandler? PropertyChanged;

    private SettingsViewModel _settingsViewModel;

    public OutlookCalendarFirstTimeContributor(SettingsViewModel settingsViewModel)
    {
        _settingsViewModel = settingsViewModel;
    }

    public FrameworkElement GetView()
    {
        return new OutlookCalendarFirstTimeView(_settingsViewModel);
    }
}
