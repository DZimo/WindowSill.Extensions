using Microsoft.UI.Xaml.Media.Imaging;
using System.ComponentModel.Composition;
using WindowSill.API;
using WindowSill.OutlookCalendar.Services;
using WindowSill.OutlookCalendar.Settings;
using WindowSill.OutlookCalendar.Views;
using WindowSill.ScreenRecorder.ViewModels;

namespace WindowSill.OutlookCalendar;

[Export(typeof(ISill))]
[Name("WindowSill.OutlookCalendar")]
[Priority(Priority.Lowest)]
public sealed class OutlookCalendarSill : ISillActivatedByDefault, ISillFirstTimeSetup, ISillSingleView
{
    private OutlookCalendarVm _outlookCalendarVm;
    private IPluginInfo _pluginInfo;
    private ISettingsProvider _settingsProvider;
    private SettingsViewModel _settingsViewModel;
    public SillView? View { get; private set; }

    private string authRecordCachePath = "./auth-record.bin";

    [ImportingConstructor]
    public OutlookCalendarSill(IPluginInfo pluginInfo, ISettingsProvider settingsProvider, IOutlookService outlookService)
    {
        _pluginInfo = pluginInfo;
        _settingsProvider = settingsProvider;
        _outlookCalendarVm = new OutlookCalendarVm(outlookService, settingsProvider, this);
        View = _outlookCalendarVm.CreateView(_outlookCalendarVm, _pluginInfo);
        View.Visibility = Visibility.Collapsed;
        _settingsViewModel = new SettingsViewModel(_settingsProvider);
    }

    public string DisplayName => "/WindowSill.OutlookCalendar/Misc/DisplayName".GetLocalizedString();

    public IconElement CreateIcon()
         => new ImageIcon
         {
             Source = new SvgImageSource(new Uri(System.IO.Path.Combine(_pluginInfo.GetPluginContentDirectory(), "Assets", "outlook_calendar.svg")))
         };

    public SillView? PlaceholderView => null;
    public SillSettingsView[]? SettingsViews =>
        [new SillSettingsView(DisplayName, new(() => new SettingsView(_settingsProvider, _settingsViewModel)))];

    public ValueTask OnActivatedAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask OnDeactivatedAsync()
    {
        _outlookCalendarVm.CleanUp();
        return ValueTask.CompletedTask;
    }

    public IFirstTimeSetupContributor[] GetFirstTimeSetupContributors()
    {
        return [new OutlookCalendarFirstTimeContributor(_settingsViewModel)];
    }
}