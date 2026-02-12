using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using WindowSill.API;
using WindowSill.ScreenRecorder.Settings;
using WindowSill.ScreenRecorder.ViewModels;
using Application = Microsoft.Office.Interop.Outlook.Application;
using Outlook = Microsoft.Office.Interop.Outlook;


namespace WindowSill.OutlookCalendar;

[Export(typeof(ISill))]
[Name("WindowSill.OutlookCalendar")]
[Priority(Priority.Lowest)]
[HideIconInSillListView]
public sealed class OutlookCalendar : ISillListView, ISill
{
    private OutlookCalendarVm _screenRecorderVm;
    private IPluginInfo _pluginInfo;
    private ISettingsProvider _settingsProvider;

    public SillView? View { get; private set; }

    [ImportingConstructor]
    public OutlookCalendar(IPluginInfo pluginInfo, ISettingsProvider settingsProvider)
    {
        _pluginInfo = pluginInfo;
        _settingsProvider = settingsProvider;
        //_screenRecorderVm = new ScreenRecorderVm(recorderService, this, settingsProvider);

        var outlookApp = new Application();
        Outlook.NameSpace ns = outlookApp.GetNamespace("MAPI");
        ns.Logon();

        Microsoft.Office.Interop.Outlook.MAPIFolder calendar = ns.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderCalendar);

        Microsoft.Office.Interop.Outlook.Items items = calendar.Items;

        items.IncludeRecurrences = true;

        items.Sort("[Start]");

        foreach (object item in items)
        {
            if (item is Microsoft.Office.Interop.Outlook.AppointmentItem appt)
            {
                Console.WriteLine($"Subject: {appt.Subject}");
                Console.WriteLine($"Start: {appt.Start}");
                Console.WriteLine($"End: {appt.End}");
                Console.WriteLine($"Location: {appt.Location}");
                Console.WriteLine("---------------------------");
            }
        }


        CreateViewList().ForgetSafely();
    }

    public string DisplayName => "/WindowSill.ScreenRecorder/Misc/DisplayName".GetLocalizedString();

    public IconElement CreateIcon()
         => new ImageIcon
         {
             Source = new SvgImageSource(new Uri(System.IO.Path.Combine(_pluginInfo.GetPluginContentDirectory(), "Assets", "screen_recorder.svg")))
         };

    public SillView? PlaceholderView => null;
    public SillSettingsView[]? SettingsViews =>
    [
        new SillSettingsView( DisplayName, new(() => new SettingsView(_settingsProvider)))
    ];

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

            ViewList.Add(new SillListViewButtonItem('\xE722', new TextBlock().Margin(5).Text("/WindowSill.ScreenRecorder/Misc/TakeSnapshotButton".GetLocalizedString()), _screenRecorderVm.Capture));

            ViewList.Add(new SillListViewButtonItem('\xE7C8', new TextBlock().Margin(5).Text("/WindowSill.ScreenRecorder/Misc/StartRecordButton".GetLocalizedString()), _screenRecorderVm.Record).Visibility(x => x.Binding(() => _screenRecorderVm.RecordButtonVisible)).DataContext(_screenRecorderVm));
            ViewList.Add(new SillListViewButtonItem('\xE71A', new StackPanel()
                                                                  .Orientation(Orientation.Vertical)
                                                                  .VerticalAlignment(VerticalAlignment.Center)
                                                                  .HorizontalAlignment(HorizontalAlignment.Center)
                                                                  .Children(
                                                                            new TextBlock().HorizontalAlignment(HorizontalAlignment.Center).Text(() => _screenRecorderVm.VideoTimeElapsed),
                                                                            new TextBlock().Margin(5).Text("/WindowSill.ScreenRecorder/Misc/StopRecordButton".GetLocalizedString())), _screenRecorderVm.Record).Visibility(x => x.Binding(() => _screenRecorderVm.RecordButtonInvisible)).DataContext(_screenRecorderVm));
        });
    }

    public ValueTask OnDeactivatedAsync()
    {
        return ValueTask.CompletedTask;
    }
}