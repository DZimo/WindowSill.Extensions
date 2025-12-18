using Microsoft.UI.Xaml.Media.Imaging;
using System.ComponentModel.Composition;
using WindowSill.API;
using WindowSill.CurrentTabs.Services;
using WindowSill.CurrentTabs.UI;

namespace WindowSill.CurrentTabs;

[Export(typeof(ISill))]
[Name("WindowSill.CurrentTabs")]
[Priority(Priority.Lowest)]
public sealed class CurrentTabsSill : ISill, ISillSingleView
{
    private CurrentTabsVm _currentTabsVm;
    private IPluginInfo _pluginInfo;
    private IProcessInteractionService _processInteraction;
    private ISettingsProvider _settingsProvider;

    public SillView? View { get; private set; }

    [ImportingConstructor]
    public CurrentTabsSill(IPluginInfo pluginInfo, IProcessInteractionService processInteraction, ITabService tabService, ISettingsProvider settingsProvider)
    {
        _pluginInfo = pluginInfo;
        _processInteraction = processInteraction;
        _settingsProvider = settingsProvider;
        _currentTabsVm = new CurrentTabsVm(settingsProvider, processInteraction);

        View = _currentTabsVm.CreateView();
        UpdateColorHeight();

        View.IsSillOrientationOrSizeChanged += (o, p) =>
        {
            UpdateColorHeight();
        };
    }

    private void UpdateColorHeight()
    {
    }

    public string DisplayName => "/WindowSill.CurrentTabs/Misc/DisplayName".GetLocalizedString();

    public IconElement CreateIcon()
         => new ImageIcon
         {
             Source = new SvgImageSource(new Uri(System.IO.Path.Combine(_pluginInfo.GetPluginContentDirectory(), "Assets", "current_tabs_logo.svg")))
         };

    public SillView? PlaceholderView => null;

    public SillSettingsView[]? SettingsViews => null;

    public ValueTask OnActivatedAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask OnDeactivatedAsync()
    {
        View = null;
        _currentTabsVm = null;

        return ValueTask.CompletedTask;
    }
}
