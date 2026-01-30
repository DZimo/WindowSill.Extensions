using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using WindowSill.API;
using WindowSill.PomodoroTimer.Models;
using WindowSill.PomodoroTimer.Services;
using WindowSill.PomodoroTimer.Settings;

namespace WindowSill.PomodoroTimer.UI;

public partial class PomodoroTimerVm : ObservableObject, IRecipient<string>
{
    private readonly IPluginInfo _pluginInfo;
    private readonly ISettingsProvider _settingsProvider;
    public readonly ITimeHandlerService _timeHandlerService;

    [ObservableProperty]
    private TimeManager timeManager = new();

    [ObservableProperty]
    private PomodoroType pomodoroType = PomodoroType.Short;

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

    public string TimeToDisplay
    {
        get
        {
            return SettingsVm.Instance?.TimeDisplayMode is TimeDisplayMode.TimeLeft ? $"{MinutesLeft}:{SecondsLeft}" : MinutesSpent is not "" ? $"{MinutesSpent:D2}:{SecondsSpent:D2}" : $"{PomodoroDuration:D2}:{00:D2}";
        }
    }

    private int PomodoroDuration => _timeHandlerService.GetTimeFromType(PomodoroType);

    public PomodoroTimerVm(ITimeHandlerService timeHandlerService, IPluginInfo? pluginInfo, ISettingsProvider settingsProvider, SettingsVm settingsVm)
    {
        Guard.IsNotNull(pluginInfo, nameof(pluginInfo));
        Guard.IsNotNull(timeHandlerService, nameof(timeHandlerService));

        _pluginInfo = pluginInfo;
        _timeHandlerService = timeHandlerService;
        _settingsProvider = settingsProvider;

        var tes = _settingsProvider.GetSetting(Settings.Settings.DisplayMode);
        MinutesSpent = PomodoroDuration.ToString();
        Instance = this;
        WeakReferenceMessenger.Default.RegisterAll(this);
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
            OnPropertyChanged(nameof(TimeToDisplay));
            OnPropertyChanged(nameof(ProgressWidth));
        });
    }

    private void ReserTimersVm()
    {
        ThreadHelper.RunOnUIThreadAsync(() =>
        {
            TimeManager.Seconds = 0;
            TimeManager.Minutes = 0;
            MinutesSpent = "";

            OnPropertyChanged(nameof(PomodoroType));
            OnPropertyChanged(nameof(MinutesLeft));
            OnPropertyChanged(nameof(SecondsLeft));
            OnPropertyChanged(nameof(TimeToDisplay));
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
            SettingsVm.Instance?.TimeDisplayMode = SettingsVm.Instance?.TimeDisplayMode is TimeDisplayMode.TimeElapsed ? TimeDisplayMode.TimeLeft : TimeDisplayMode.TimeElapsed;
        });
        NotifyTimeLeftChanged();
    }

    public void NotifyTimeLeftChanged()
    {
        ThreadHelper.RunOnUIThreadAsync(() =>
        {
            OnPropertyChanged(nameof(this.MinutesLeft));
            OnPropertyChanged(nameof(this.SecondsLeft));
            OnPropertyChanged(nameof(this.TimeToDisplay));
        });
    }

    public void Receive(string message) =>
        NotifyTimeLeftChanged();
}