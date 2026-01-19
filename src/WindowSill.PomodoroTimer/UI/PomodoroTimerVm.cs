using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WindowSill.API;
using WindowSill.PomodoroTimer.Models;
using WindowSill.PomodoroTimer.Services;
using WindowSill.PomodoroTimer.Settings;

namespace WindowSill.PomodoroTimer.UI;

public partial class PomodoroTimerVm : ObservableObject
{
    private readonly IPluginInfo _pluginInfo;
    private readonly ISettingsProvider _settingsProvider;
    public readonly ITimeHandlerService _timeHandlerService;

    [ObservableProperty]
    private TimeManager timeManager = new();

    [ObservableProperty]
    private PomodoroType pomodoroType = PomodoroType.Short;

    private TimeDisplayMode timeDisplayMode;

    public TimeDisplayMode TimeDisplayMode
    {
        get => _settingsProvider.GetSetting(Settings.Settings.DisplayMode);
        set
        {
            timeDisplayMode = value;
            _settingsProvider.SetSetting<TimeDisplayMode>(Settings.Settings.DisplayMode, value);
            OnPropertyChanged(nameof(TimeDisplayMode));
            OnPropertyChanged(nameof(SettingsVm.TimeDisplayMode));
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PomodoroStopped))]
    [NotifyCanExecuteChangedFor(nameof(StartPomodoroCommand))]
    [NotifyCanExecuteChangedFor(nameof(StopPomodoroCommand))]
    private bool pomodoroStarted;

    [ObservableProperty]
    private SolidColorBrush pomodoroColor = new SolidColorBrush(Colors.IndianRed);

    [ObservableProperty]
    private double progressHeight = 5;

    [ObservableProperty]
    private double progressWidth = 30;

    public double progressWidthDefault;

    public double ProgressWidthDefault
    {
        get => progressWidthDefault;
        set
        {
            progressWidthDefault = value;
            ProgressWidth = value;
            OnProgressWidthChanged(ProgressWidth);
            OnPropertyChanged(nameof(ProgressWidth));
        }
    }

    public static PomodoroTimerVm? Instance;

    private bool PomodoroStopped
    {
        get => !PomodoroStarted;
    }

    public string MinutesLeft
    {
        get => $"{_timeHandlerService.GetMinutes(TimeManager):D2}";
    }
    public string SecondsLeft
    {
        get => $"{_timeHandlerService.GetSeconds(TimeManager):D2}";
    }

    public string MinutesSpent
    {
        get => field;
        set => field = value;
    }

    public string SecondsSpent = "00";

    public string TimeLeft
    {
        get
        {
            return TimeDisplayMode is TimeDisplayMode.TimeSpent ? $"{MinutesLeft}:{SecondsLeft}" : MinutesSpent is not "" ? $"{MinutesSpent:D2}:{SecondsSpent:D2}" : $"{PomodoroDuration:D2}:{00:D2}";
        }
    }

    private int PomodoroDuration => _timeHandlerService.GetTimeFromType(PomodoroType);

    public PomodoroTimerVm(ITimeHandlerService timeHandlerService, IPluginInfo? pluginInfo, ISettingsProvider settingsProvider)
    {
        Guard.IsNotNull(pluginInfo, nameof(pluginInfo));
        Guard.IsNotNull(timeHandlerService, nameof(timeHandlerService));

        _pluginInfo = pluginInfo;
        _timeHandlerService = timeHandlerService;
        _settingsProvider = settingsProvider;

        TimeDisplayMode = settingsProvider.GetSetting<TimeDisplayMode>(Settings.Settings.DisplayMode);
        MinutesSpent = PomodoroDuration.ToString();
        Instance = this;
    }

    public SillView CreateView()
    {
        return new SillView { Content = new PomodoroTimerView(_pluginInfo, this) };
    }

    [RelayCommand(CanExecute = nameof(PomodoroStopped))]
    private void StartPomodoro()
    {
        PomodoroStarted = true;

        _timeHandlerService.StartTimer(TimeManager, PomodoroType);
        _timeHandlerService.TimerReduced += OnTimerReduced;
        _timeHandlerService.TimerFinished += OnTimerFinished;
    }

    private void OnTimerFinished(object? sender, TimeManager? e)
    {
        ReserTimersVm();

        _timeHandlerService.ChangeTime(TimeManager, PomodoroType);
    }

    [RelayCommand(CanExecute = nameof(PomodoroStarted))]
    private void StopPomodoro()
    {
        PomodoroStarted = false;

        _timeHandlerService.ResetTimer(TimeManager, PomodoroType);
        _timeHandlerService.TimerReduced -= OnTimerReduced;
        _timeHandlerService.TimerFinished -= OnTimerFinished;

        ReserTimersVm();
    }

    private void OnTimerReduced(object? sender, TimeManager? e)
    {
        TimeManager.Seconds++;
        TimeManager.Minutes = TimeManager.Seconds / 60;

        var time = _timeHandlerService.GetTimeFromBreak(TimeManager.IsBreakTime, PomodoroType) * 60;

        ThreadHelper.RunOnUIThreadAsync(() =>
        {
            var remaining = time - TimeManager.Seconds;
            SecondsSpent = (remaining % 60).ToString();
            MinutesSpent = (remaining / 60).ToString();

            var progressRatio = (double)remaining / time;

            if (progressRatio > 0)
                ProgressWidth = progressWidthDefault * progressRatio;

            OnPropertyChanged(nameof(MinutesLeft));
            OnPropertyChanged(nameof(SecondsLeft));
            OnPropertyChanged(nameof(TimeLeft));
            OnPropertyChanged(nameof(ProgressWidth));
        });
    }

    private void ReserTimersVm()
    {
        ThreadHelper.RunOnUIThreadAsync(() =>
        {
            TimeManager.Seconds = 0;
            TimeManager.Minutes = 0;

            OnPropertyChanged(nameof(MinutesLeft));
            OnPropertyChanged(nameof(SecondsLeft));
            OnPropertyChanged(nameof(TimeLeft));
        });
    }

    [RelayCommand]
    public void ChangePomodoroType()
    {
        PomodoroType = PomodoroType switch
        {
            PomodoroType.Short => PomodoroType.Long,
            PomodoroType.Long => PomodoroType.Short,
            _ => PomodoroType.Short,
        };

        StopPomodoro();
    }

    [RelayCommand]
    public void ChangeTimeDisplay()
    {
        ThreadHelper.RunOnUIThreadAsync(() =>
        {
            TimeDisplayMode = TimeDisplayMode is TimeDisplayMode.TimeSpent ? TimeDisplayMode.TimeLeft : TimeDisplayMode.TimeSpent;
            OnPropertyChanged(nameof(MinutesLeft));
            OnPropertyChanged(nameof(SecondsLeft));
            OnPropertyChanged(nameof(TimeLeft));
        });
    }
}