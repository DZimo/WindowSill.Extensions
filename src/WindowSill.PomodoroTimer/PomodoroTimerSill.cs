using CommunityToolkit.Diagnostics;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Windows.System;
using WindowSill.API;
using WindowSill.PomodoroTimer.Services;
using WindowSill.PomodoroTimer.Settings;
using WindowSill.PomodoroTimer.UI;

namespace WindowSill.PomodoroTimer;

[Export(typeof(ISill))]
[Name("WindowSill.PomodoroTimer")]
[Priority(Priority.Lowest)]
public sealed class PomodoroTimerSill : ISill, ISillSingleView
{
    private PomodoroTimerVm? pomodoroTimerVm;
    private IPluginInfo _pluginInfo;
    private ISettingsProvider _settingsProvider;

    public SillView? View { get; private set; }

    [ImportingConstructor]
    public PomodoroTimerSill(ITimeHandlerService timeHandlerService, IPluginInfo pluginInfo, ISettingsProvider settingsProvider)
    {
        _pluginInfo = pluginInfo;
        _settingsProvider = settingsProvider;

        pomodoroTimerVm = new PomodoroTimerVm(timeHandlerService, pluginInfo);

        View = pomodoroTimerVm.CreateView();
        UpdateColorHeight();

        View.IsSillOrientationOrSizeChanged += (o, p) =>
        {
            UpdateColorHeight();
        };
    }

    private void UpdateColorHeight()
    {
        pomodoroTimerVm?.ProgressHeight = View?.SillOrientationAndSize == SillOrientationAndSize.HorizontalSmall ? 2 : View?.SillOrientationAndSize == SillOrientationAndSize.HorizontalMedium ? 4 : 6;
        pomodoroTimerVm?.ProgressWidthDefault = View?.SillOrientationAndSize == SillOrientationAndSize.HorizontalSmall ? 14 : View?.SillOrientationAndSize == SillOrientationAndSize.HorizontalMedium ? 15 : 20;
    }

    public string DisplayName => "/WindowSill.PomodoroTimer/Misc/DisplayName".GetLocalizedString();

    public IconElement CreateIcon()
         => new ImageIcon
         {
             Source = new SvgImageSource(new Uri(System.IO.Path.Combine(_pluginInfo.GetPluginContentDirectory(), "Assets", "pomodoro_logo.svg")))
         };

    public SillView? PlaceholderView => null;


    public SillSettingsView[]? SettingsViews =>
        [
        new SillSettingsView(
            DisplayName,
            new(() => new SettingsView(_settingsProvider)))
        ];


    public ValueTask OnActivatedAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask OnDeactivatedAsync()
    {
        View = null;
        pomodoroTimerVm = null;

        return ValueTask.CompletedTask;
    }
}
