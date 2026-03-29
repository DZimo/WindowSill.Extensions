using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Media.Imaging;
using System.ComponentModel.Composition;
using WindowSill.API;
using WindowSill.OutlookCalendar.Services;
using WindowSill.OutlookCalendar.Settings;
using WindowSill.OutlookCalendar.ViewModels;
using WindowSill.OutlookCalendar.Views;

namespace WindowSill.OutlookCalendar;

[Export(typeof(ISill))]
[Name("OutlookCalendar")]
public sealed class OutlookCalendarSill : ISillFirstTimeSetup, ISillSingleView
{
    private OutlookCalendarVm _outlookCalendarVm;
    private readonly IPluginInfo _pluginInfo;
    private readonly ISettingsProvider _settingsProvider;
    private readonly ILogger _logger;
    private SettingsViewModel _settingsViewModel;
    private bool _isSillLoaded;
    public SillView? View { get; private set; }

    [ImportingConstructor]
    public OutlookCalendarSill(IPluginInfo pluginInfo, ISettingsProvider settingsProvider, IOutlookService outlookService)
    {
        _logger = this.Log();

        _logger.LogInformation("Test Only - Windowsill OutlookCalendar SILL Called");

        if (_isSillLoaded)
            return;

        _isSillLoaded = true;

        _pluginInfo = pluginInfo;
        _settingsProvider = settingsProvider;

        _outlookCalendarVm = new OutlookCalendarVm(outlookService, settingsProvider, this);
        View = _outlookCalendarVm.CreateView(_outlookCalendarVm, _pluginInfo);
        View.Visibility = Visibility.Collapsed;
        _settingsViewModel = new SettingsViewModel(_settingsProvider, _outlookCalendarVm);
    }

    public string DisplayName => "/WindowSill.OutlookCalendar/Misc/DisplayName".GetLocalizedString();

    public IconElement CreateIcon()
         => new ImageIcon
         {
             Source = new SvgImageSource(new Uri(System.IO.Path.Combine(_pluginInfo.GetPluginContentDirectory(), "Assets", "outlook_calendar.svg")))
         };

    public SillView? PlaceholderView => null;
    public SillSettingsView[]? SettingsViews =>
        [new SillSettingsView(DisplayName, new(() => new SettingsView(_settingsProvider, _settingsViewModel, _outlookCalendarVm)))];

    public ValueTask OnActivatedAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask OnDeactivatedAsync()
    {
        //View = null;
        //_outlookCalendarVm = null;
        //_outlookCalendarVm.CleanUp();
        return ValueTask.CompletedTask;
    }

    public IFirstTimeSetupContributor[] GetFirstTimeSetupContributors()
    {
        return [new OutlookCalendarFirstTimeContributor(_settingsViewModel)];
    }
}