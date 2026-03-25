using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System.Collections.ObjectModel;
using WindowSill.API;
using WindowSill.OutlookCalendar.Models;
using WindowSill.OutlookCalendar.Services;
using WindowSill.OutlookCalendar.Views;
using Timer = System.Timers.Timer;

namespace WindowSill.OutlookCalendar.ViewModels;

public partial class OutlookCalendarVm : ObservableObject
{
    public static OutlookCalendarVm Instance { get; private set; }

    [ObservableProperty]
    private Visibility appointmentVisibility = Visibility.Collapsed;

    [ObservableProperty]
    private string nextAppointmentLeftTime = string.Empty;

    [ObservableProperty]
    private string userName = "-";

    [ObservableProperty]
    private bool foundAppointment;

    [ObservableProperty]
    private ObservableCollection<CalendarAppointmentVm> allAppointments;

    private Timer recordTimer;

    private int appointmentCheckTime = 2;

    [ObservableProperty]
    private char recordGlyph = '\xE714';

    private readonly IOutlookService _outlookService;
    private readonly ISettingsProvider _settingsProvider;
    private readonly ILogger _logger;
    private ISillSingleView _view;

    private OutlookCalendar.Enums.AccountType selectedAccountType;

    public event EventHandler InitCalendarService;
    private string tenantID = "common";
    private static IPublicClientApplication _clientApp;

    public OutlookCalendarVm(IOutlookService outlookService, ISettingsProvider settingsProvider, ISillSingleView sillView)
    {
        Instance = this;
        _outlookService = outlookService;
        _settingsProvider = settingsProvider;
        _view = sillView;

        _logger = this.Log();

        selectedAccountType = _settingsProvider.GetSetting(WindowSill.OutlookCalendar.Settings.Settings.SelectedAccountType);

        _ = HandleCalendarService();
    }

    private async Task HandleCalendarService()
    {
        _outlookService.IsNewerOfficeVersion = _settingsProvider.GetSetting(WindowSill.OutlookCalendar.Settings.Settings.SelectedOfficeVersion);

        var usertemp = await _outlookService.InitLogin(tenantID);
        await Task.Delay(TimeSpan.FromSeconds(5));

        recordTimer = new(TimeSpan.FromMinutes(appointmentCheckTime));
        recordTimer.Start();
        recordTimer.Elapsed += RecordTimer_Elapsed;

        if (_outlookService.IsOutlookLogged)
            _logger.LogInformation("Logged into Outlook successfully.");
        else
        {
            _logger.LogWarning("Failed to log into Outlook.");
            return;
        }

        await FetchAppointmentsOnUI(usertemp);
    }

    private async void RecordTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        //Debug.WriteLine("CALLLLLLLLED 1111111111");
        var usertemp = "-";
        if (!_outlookService.IsOutlookLogged)
            usertemp = await _outlookService.InitLogin(tenantID);

        if (!_outlookService.IsOutlookLogged)
            return;

        await FetchAppointmentsOnUI(usertemp);
    }

    private async Task FetchAppointmentsOnUI(string foundUsername)
    {
        await Task.Run(async () =>
        {
            await _outlookService.InitAllAppointments().ConfigureAwait(false);
        }).ConfigureAwait(false);

        await ThreadHelper.RunOnUIThreadAsync(async () =>
        {
            UserName = foundUsername;
            await FetchAppointments();
        });
    }

    [RelayCommand]
    public Task Expand()
    {
        return Task.CompletedTask;
    }

    private async Task FetchAppointments()
    {
        AllAppointments = new(_outlookService.GetAllAppointments().Distinct().Take(5));

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

        var subject = _outlookService.FirstAppointment()?.Subject ?? "Meeting";
        var canShow = left.Value.TotalMinutes < 30 && left.Value.TotalMinutes > 0;

        _view.View.Visibility = canShow ? Visibility.Visible : Visibility.Collapsed;
        _view.View.UpdateLayout();

        var res = subject.Length > 10 ? $"{subject.Substring(0, 10)}.." : subject;
        NextAppointmentLeftTime = canShow ? $"{Math.Round(left.Value.TotalMinutes).ToString()}m - {res}" : "No meeting";

        if (left.Value.TotalMinutes < appointmentCheckTime)
        {
            var txt = "/WindowSill.OutlookCalendar/Misc/UpcomingMeetingDesc".GetLocalizedString().Replace("{subject}", subject).Replace("{minutes}", Math.Round(left.Value.TotalMinutes).ToString());
            ShowNotification("/WindowSill.OutlookCalendar/Misc/UpcomingMeetingHeader".GetLocalizedString(), txt);
        }
    }

    public SillView CreateView(OutlookCalendarVm calendarVm, IPluginInfo _pluginInfo)
    {
        return new SillView { Content = new OutlookCalendarView(calendarVm, _pluginInfo), DataContext = calendarVm };
    }

    public void CleanUp()
    {
        recordTimer.Elapsed -= RecordTimer_Elapsed;
        _outlookService.OutlookNameSpace?.Logoff();
    }

    public void ShowNotification(string title, string message)
    {
        var notification = new AppNotificationBuilder()
            .AddText(title)
            .AddText(message);

        AppNotificationManager.Default.Show(notification.BuildNotification());
    }
}