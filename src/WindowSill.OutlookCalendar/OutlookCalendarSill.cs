using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using WindowSill.API;
using WindowSill.ScreenRecorder.ViewModels;


namespace WindowSill.OutlookCalendar;

[Export(typeof(ISill))]
[Name("WindowSill.OutlookCalendar")]
[Priority(Priority.Lowest)]
[HideIconInSillListView]
public sealed class OutlookCalendarSill : ISillListView, ISill
{
    private OutlookCalendarVm _screenRecorderVm;
    private IPluginInfo _pluginInfo;
    private ISettingsProvider _settingsProvider;

    public SillView? View { get; private set; }

    [ImportingConstructor]
    public OutlookCalendarSill(IPluginInfo pluginInfo, ISettingsProvider settingsProvider)
    {
        _pluginInfo = pluginInfo;
        _settingsProvider = settingsProvider;
        //_screenRecorderVm = new ScreenRecorderVm(recorderService, this, settingsProvider);

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
        await ThreadHelper.RunOnUIThreadAsync(() =>
        {
            ViewList.Clear();
        });
    }

    public ValueTask OnDeactivatedAsync()
    {
        return ValueTask.CompletedTask;
    }
}