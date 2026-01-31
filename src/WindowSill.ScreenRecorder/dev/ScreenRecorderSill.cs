using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using WindowSill.API;
using WindowSill.ScreenRecorder.Services;
using WindowSill.ScreenRecorder.Settings;
using WindowSill.ScreenRecorder.ViewModels;

namespace WindowSill.ScreenRecorder;

[Export(typeof(ISill))]
[Name("WindowSill.ScreenRecorder")]
[Priority(Priority.Lowest)]
public sealed class ScreenRecorderSill : ISillListView, ISill
{
    private ScreenRecorderVm _screenRecorderVm;
    private IPluginInfo _pluginInfo;
    private IProcessInteractionService _processInteraction;
    private ISettingsProvider _settingsProvider;

    public SillView? View { get; private set; }

    [ImportingConstructor]
    public ScreenRecorderSill(IPluginInfo pluginInfo, IProcessInteractionService processInteraction, IRecorderService recorderService, ISettingsProvider settingsProvider)
    {
        _pluginInfo = pluginInfo;
        _processInteraction = processInteraction;
        _settingsProvider = settingsProvider;
        _screenRecorderVm = new ScreenRecorderVm(recorderService, this, settingsProvider);
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
    new SillSettingsView(
            DisplayName,
            new(() => new SettingsView(_settingsProvider)))
    ];

    public ObservableCollection<SillListViewItem> ViewList => [
            new SillListViewButtonItem('\xE722', new TextBlock().Margin(5).Text("/WindowSill.ScreenRecorder/Misc/DisplayName".GetLocalizedString()), _screenRecorderVm.Capture),
            new SillListViewButtonItem('\xE7C8', new TextBlock().Margin(5).Text("/WindowSill.ScreenRecorder/Misc/DisplayName".GetLocalizedString()), _screenRecorderVm.Capture)
        ];

    public ValueTask OnActivatedAsync()
    {
        //await ThreadHelper.RunOnUIThreadAsync(() =>
        //{
        //    var test = new SillListViewButtonItem('\xF7EE', new TextBlock().Margin(5).Text("/WindowSill.ScreenRecorder/Misc/DisplayName".GetLocalizedString()), _screenRecorderVm.Capture);
        //    ViewList.Add(test);
        //    ViewList.Add(new SillListViewButtonItem('\xE7C8', new TextBlock().Margin(5).Text("/WindowSill.ScreenRecorder/Misc/DisplayName".GetLocalizedString()), _screenRecorderVm.Capture));
        //});
        return ValueTask.CompletedTask;
    }

    public ValueTask OnDeactivatedAsync()
    {
        return ValueTask.CompletedTask;
    }
}
