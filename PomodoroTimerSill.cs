using CommunityToolkit.Diagnostics;

using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using Windows.System;

using WindowSill.API;
using WindowSill.PerfCounter.UI;
using WindowSill.PomodoroTimer.Services;

namespace WindowSill.PomodoroTimer;

[Export(typeof(ISill))]
[Name("WindowSill.PomodoroTimer")]
[Priority(Priority.Lowest)]
public sealed class PomodoroTimerSill : ISill, ISillSingleView
{
    private PomodoroTimerVm? pomodoroTimerVm;
    public SillView? View { get; private set; } = new SillView();

    [ImportingConstructor]
    public PomodoroTimerSill(ITimeHandlerService timeHandlerService, IPluginInfo pluginInfo)
    {
        pomodoroTimerVm = new PomodoroTimerVm(timeHandlerService, pluginInfo);
        View = pomodoroTimerVm.CreateView();
    }

    public string DisplayName => "/WindowSill.PomodoroTimer/Misc/DisplayName".GetLocalizedString();


    public IconElement CreateIcon()
        => new FontIcon
        {
            Glyph = "\uED56"
        };

    public ObservableCollection<SillListViewItem> ViewList
        => [
           new SillListViewButtonItem(
                '\uE710',
                "/WindowSill.PomodoroTimer/Misc/CommandTitle".GetLocalizedString(),
                OnCommandButtonClickAsync),
        ];

    public SillView? PlaceholderView => null;

    public SillSettingsView[]? SettingsViews => null;

    private async Task OnCommandButtonClickAsync()
    {

    }

    public ValueTask OnActivatedAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask OnDeactivatedAsync()
    {
        return ValueTask.CompletedTask;
    }
}
