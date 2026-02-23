using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Graph;
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
    private List<CalendarAppointmentVm> allAppointments;

    private Timer recordTimer;

    private int appointmentCheckTime = 5;

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

    private void FetchAppointments()
    {
        _outlookService.InitAllAppointments();
        AllAppointments = _outlookService.GetAllAppointments();

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

    private GraphServiceClient _graphClient;
    private GraphServiceClient GraphClient
    {
        get
        {
            if (_graphClient == null)
            {
                var credential = new Azure.Identity.InteractiveBrowserCredential(
                    new Azure.Identity.InteractiveBrowserCredentialOptions
                    {
                        ClientId = "3c62448e-650a-497a-b43c-35f9db069e4f",
                        TenantId = "common"
                    });

                _graphClient = new Microsoft.Graph.GraphServiceClient(
                    credential,
                    new[] { "Calendars.Read" });
            }
            return _graphClient;
        }
    }
}