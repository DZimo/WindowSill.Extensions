using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WindowSill.API;
using WindowSill.OutlookCalendar.Services;
using Timer = System.Timers.Timer;

namespace WindowSill.ScreenRecorder.ViewModels;

public partial class OutlookCalendarVm : ObservableObject
{
    public static OutlookCalendarVm Instance { get; private set; }

    [ObservableProperty]
    private int colorFontSize = 12;

    [ObservableProperty]
    private int colorboxHeight = 18;

    [ObservableProperty]
    private Visibility recordButtonVisible = Visibility.Visible;


    [ObservableProperty]
    private Visibility recordButtonInvisible = Visibility.Collapsed;

    [ObservableProperty]
    private string videoTimeElapsed = string.Empty;

    private Timer recordTimer = new();

    private int elapsedSeconds = 0;


    [ObservableProperty]
    private char recordGlyph = '\xE714';

    private string selectedScreenshotName = string.Empty;

    private IOutlookService _outlookService;
    private ISillListView _view;
    private ISettingsProvider _settingsProvider;

    public OutlookCalendarVm(IOutlookService outlookService, ISillListView view, ISettingsProvider settingsProvider)
    {
        Instance = this;
        _outlookService = outlookService;
        _view = view;

        _settingsProvider = settingsProvider;
        recordTimer.Interval = 1000;
        recordTimer.Start();
        recordTimer.Elapsed += RecordTimer_Elapsed;
    }

    private async void RecordTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        await ThreadHelper.RunOnUIThreadAsync(() =>
        {
            VideoTimeElapsed = $"{(elapsedSeconds / 60):D2}:{(elapsedSeconds % 60):D2}";
        });
        elapsedSeconds++;
    }


    [RelayCommand]
    public Task Expand()
    {
        return Task.CompletedTask;
    }

}