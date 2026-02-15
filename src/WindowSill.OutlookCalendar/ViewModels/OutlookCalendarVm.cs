using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WindowSill.API;
using WindowSill.OutlookCalendar.Services;
using WindowSill.OutlookCalendar.ViewModels;
using Timer = System.Timers.Timer;

namespace WindowSill.ScreenRecorder.ViewModels;

public partial class OutlookCalendarVm : ObservableObject
{
    public static OutlookCalendarVm Instance { get; private set; }

    [ObservableProperty]
    private Visibility appointmentVisibility = Visibility.Collapsed;

    [ObservableProperty]
    private string nextAppointmentLeftTime = string.Empty;

    [ObservableProperty]
    private bool foundAppointment;


    private Timer recordTimer = new();

    private int elapsedSeconds = 0;

    private int appointmentCheckTime = 5000;

    [ObservableProperty]
    private char recordGlyph = '\xE714';

    private string selectedScreenshotName = string.Empty;

    private IOutlookService _outlookService;
    private ISillSingleView _view;
    private ISettingsProvider _settingsProvider;

    public OutlookCalendarVm(IOutlookService outlookService, ISettingsProvider settingsProvider, ISillSingleView sillView)
    {
        Instance = this;
        _outlookService = outlookService;

        _settingsProvider = settingsProvider;
        recordTimer.Interval = appointmentCheckTime;
        recordTimer.Start();
        recordTimer.Elapsed += RecordTimer_Elapsed;
        _view = sillView;
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

            var subject = _outlookService.FirstAppointment().Subject ?? "Meeting";
            var canShow = left.Value.TotalMinutes < 30;

            _view.View.Visibility = canShow ? Visibility.Visible : Visibility.Collapsed;
            NextAppointmentLeftTime = canShow ? $"{Math.Round(left.Value.TotalMinutes).ToString()}m - {subject}" : "No meeting";
        });
    }


    [RelayCommand]
    public Task Expand()
    {
        return Task.CompletedTask;
    }

    public SillView CreateView()
    {
        return new SillView { Content = new OutlookCalendarView(this) };
    }
}