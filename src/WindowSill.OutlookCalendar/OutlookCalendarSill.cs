using Microsoft.UI.Xaml.Media.Imaging;
using System.ComponentModel.Composition;
using WindowSill.API;
using WindowSill.OutlookCalendar.Services;
using WindowSill.ScreenRecorder.ViewModels;

namespace WindowSill.OutlookCalendar;

[Export(typeof(ISill))]
[Name("WindowSill.OutlookCalendar")]
[Priority(Priority.Lowest)]
public sealed class OutlookCalendarSill : ISillSingleView, ISill
{
    private OutlookCalendarVm _outlookCalendarVm;
    private IPluginInfo _pluginInfo;
    private ISettingsProvider _settingsProvider;

    public SillView? View { get; private set; }

    [ImportingConstructor]
    public OutlookCalendarSill(IPluginInfo pluginInfo, ISettingsProvider settingsProvider, IOutlookService outlookService)
    {
        _pluginInfo = pluginInfo;
        _settingsProvider = settingsProvider;
        _outlookCalendarVm = new OutlookCalendarVm(outlookService, settingsProvider, this);
        View = _outlookCalendarVm.CreateView();
        View.Visibility = Visibility.Collapsed;
    }

    public string DisplayName => "/WindowSill.OutlookCalendar/Misc/DisplayName".GetLocalizedString();

    public IconElement CreateIcon()
         => new ImageIcon
         {
             Source = new SvgImageSource(new Uri(System.IO.Path.Combine(_pluginInfo.GetPluginContentDirectory(), "Assets", "outlook_calendar.svg")))
         };

    public SillView? PlaceholderView => null;
    public SillSettingsView[]? SettingsViews => null;

    public ValueTask OnActivatedAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask OnDeactivatedAsync()
    {
        return ValueTask.CompletedTask;
    }
}