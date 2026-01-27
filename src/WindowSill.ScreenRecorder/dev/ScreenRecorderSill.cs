using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using WindowSill.API;
using WindowSill.ScreenRecorder.Services;
using WindowSill.ScreenRecorder.ViewModels;

namespace WindowSill.ScreenRecorder;

[Export(typeof(ISill))]
[Name("WindowSill.ScreenRecorder")]
[Priority(Priority.Lowest)]
public sealed class ScreenRecorderSill : ISillListView, ISillSingleView
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
        _screenRecorderVm = new ScreenRecorderVm(recorderService);
    }

    public string DisplayName => "/WindowSill.SimpleCalculator/Misc/DisplayName".GetLocalizedString();

    public IconElement CreateIcon()
         => new ImageIcon
         {
             Source = new SvgImageSource(new Uri(System.IO.Path.Combine(_pluginInfo.GetPluginContentDirectory(), "Assets", "calculator_logo.svg")))
         };

    public SillView? PlaceholderView => null;

    public SillSettingsView[]? SettingsViews => null;

    public ObservableCollection<SillListViewItem> ViewList => throw new NotImplementedException();

    private async Task OnCommandButtonClickAsync()
    {
        throw new NotImplementedException();
    }

    public async ValueTask OnActivatedAsync()
    {
        await ThreadHelper.RunOnUIThreadAsync(() =>
        {
            ViewList.Clear();

            var view = _screenRecorderVm.CreateView();
            var viewitem = new SillListViewButtonItem(view, null, DoNothing);
            ViewList.Add(viewitem);
        });
    }

    private async Task DoNothing()
    {
        return;
    }

    public ValueTask OnDeactivatedAsync()
    {
        return ValueTask.CompletedTask;
    }
}
