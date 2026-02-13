using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using WindowSill.API;
using WindowSill.OutlookCalendar.Services;
using WindowSill.ScreenRecorder.ViewModels;

namespace WindowSill.OutlookCalendar;

[Export(typeof(ISill))]
[Name("WindowSill.OutlookCalendar")]
[Priority(Priority.Lowest)]
public sealed class OutlookCalendarSill : ISillListView, ISill
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
        _outlookCalendarVm = new OutlookCalendarVm(outlookService, this, settingsProvider);

        CreateViewList().ForgetSafely();
    }

    public string DisplayName => "/WindowSill.OutlookCalendar/Misc/DisplayName".GetLocalizedString();

    public IconElement CreateIcon()
         => new ImageIcon
         {
             Source = new SvgImageSource(new Uri(System.IO.Path.Combine(_pluginInfo.GetPluginContentDirectory(), "Assets", "outlook_calendar.svg")))
         };

    public SillView? PlaceholderView => null;
    public SillSettingsView[]? SettingsViews => null;

    public ObservableCollection<SillListViewItem> ViewList { get; } = new();

    public ValueTask OnActivatedAsync()
    {
        return ValueTask.CompletedTask;
    }

    public async Task CreateViewList()
    {
        await Task.Delay(10000);

        await ThreadHelper.RunOnUIThreadAsync(async () =>
        {
            ViewList.Clear();

            ViewList.Add(new SillListViewButtonItem('\xE787', new TextBlock().Margin(5).Text(_outlookCalendarVm.NextAppointmentLeftTime), _outlookCalendarVm.ExpandCommand));
        });
    }

    public ValueTask OnDeactivatedAsync()
    {
        return ValueTask.CompletedTask;
    }
}