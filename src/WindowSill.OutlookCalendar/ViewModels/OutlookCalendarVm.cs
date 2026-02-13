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
    private string nextAppointmentLeftTime = string.Empty;

    private Timer recordTimer = new();

    private int elapsedSeconds = 0;

    private int appointmentCheckTime = 5000;

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
        recordTimer.Interval = appointmentCheckTime;
        recordTimer.Start();
        recordTimer.Elapsed += RecordTimer_Elapsed;
    }

    private async void RecordTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _outlookService.InitAllAppointments();
        await ThreadHelper.RunOnUIThreadAsync(() =>
        {
            if (_outlookService.FirstAppointment() is null)
                return;

            var left = _outlookService.FirstAppointment()?.Start - DateTime.Now;

            if (left is null)
                return;

            var subject = _outlookService.FirstAppointment().Subject;

            NextAppointmentLeftTime = left.Value.TotalMinutes < 30 ? $"{Math.Round(left.Value.TotalMinutes).ToString()}m / {subject}" : "No meeting";
        });
    }


    [RelayCommand]
    public Task Expand()
    {
        return Task.CompletedTask;
    }

}