using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using WindowSill.API;
using WindowSill.PomodoroTimer.Models;
using WindowSill.PomodoroTimer.Services;

namespace WindowSill.PerfCounter.UI;

public partial class PomodoroTimerVm : ObservableObject
{
    private readonly IPluginInfo _pluginInfo;
    public readonly ITimeHandlerService _timeHandlerService;

    [ObservableProperty]
    private TimeManager timeManager = new();

    [ObservableProperty]
    private PomodoroType pomodoroType = PomodoroType.Short;

    public string MinutesLeft
    {
        get => $"{_timeHandlerService.GetMinutes(TimeManager):D2}";
    }
    public string SecondsLeft 
    {
        get => $"{_timeHandlerService.GetSeconds(TimeManager):D2}";
    }

    public string TimeLeft
    {
        get => $"{MinutesLeft}:{SecondsLeft}";
    }

    public PomodoroTimerVm(ITimeHandlerService timeHandlerService, IPluginInfo? pluginInfo)
    {
        Guard.IsNotNull(pluginInfo, nameof(pluginInfo));
        Guard.IsNotNull(timeHandlerService, nameof(timeHandlerService));

        _pluginInfo = pluginInfo;
        _timeHandlerService = timeHandlerService;

        _timeHandlerService.StartTimer(TimeManager, PomodoroType);
        _timeHandlerService.TimerReduced += OnTimerReduced;
    }

    private void OnTimerReduced(object? sender, TimeManager? e)
    {
        TimeManager.Seconds++;
        TimeManager.Minutes ++;

        ThreadHelper.RunOnUIThreadAsync(() =>
        {
            OnPropertyChanged(nameof(MinutesLeft));
            OnPropertyChanged(nameof(SecondsLeft));
            OnPropertyChanged(nameof(TimeLeft));
        });
    }

    public SillView CreateView()
    {
        return new SillView { Content = new PomodoroTimerView(_pluginInfo, this) };
    }
}