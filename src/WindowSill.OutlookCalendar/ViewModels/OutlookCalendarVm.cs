using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WindowSill.API;
using WindowSill.OutlookCalendar.Models;
using WindowSill.OutlookCalendar.Services;
using WindowSill.OutlookCalendar.Settings;
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

    [ObservableProperty]
    private ObservableCollection<CalendarAppointmentVm> allAppointments;

    private Timer recordTimer;

    private int appointmentCheckTime = 2;

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
        _view = sillView;

        _ = HandleCalendarService();
    }

    private async Task HandleCalendarService()
    {
        _outlookService.IsNewerOfficeVersion = _settingsProvider.GetSetting(Settings.SelectedOfficeVersion);
        _outlookService.InitLogin();

        await Task.Delay(TimeSpan.FromSeconds(5));

        await FetchAppointmentsOnUI();

        recordTimer = new(TimeSpan.FromMinutes(appointmentCheckTime));
        recordTimer.Start();
        recordTimer.Elapsed += RecordTimer_Elapsed;
    }

    private async void RecordTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        await FetchAppointmentsOnUI();
    }

    private async Task FetchAppointmentsOnUI()
    {
        await ThreadHelper.RunOnUIThreadAsync(() =>
        {
            FetchAppointments();
        });
    }

    [RelayCommand]
    public Task Expand()
    {
        return Task.CompletedTask;
    }

    private async Task FetchAppointments()
    {
        await _outlookService.InitAllAppointments();
        AllAppointments = new(_outlookService.GetAllAppointments());

        if (_outlookService.FirstAppointment() is null)
        {
            _view.View.Visibility = Visibility.Collapsed;
            return;
        }

        var left = _outlookService.FirstAppointment()?.Start - DateTime.Now;

        if (left is null)
        {
            _view.View.Visibility = Visibility.Collapsed;
            return;
        }

        var subject = _outlookService.FirstAppointment().Subject ?? "Meeting";
        var canShow = left.Value.TotalMinutes < 30;

        _view.View.Visibility = canShow ? Visibility.Visible : Visibility.Collapsed;
        NextAppointmentLeftTime = canShow ? $"{Math.Round(left.Value.TotalMinutes).ToString()}m - {subject}" : "No meeting";
    }

    public SillView CreateView(OutlookCalendarVm calendarVm, IPluginInfo _pluginInfo)
    {
        return new SillView { Content = new OutlookCalendarView(calendarVm, _pluginInfo), DataContext = calendarVm };
    }

    public void CleanUp()
    {
        _outlookService.OutlookNameSpace?.Logoff();
    }
}