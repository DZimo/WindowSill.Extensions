using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.ComponentModel.Composition;
using WindowSill.API;
using WindowSill.OutlookCalendar.Common;
using WindowSill.OutlookCalendar.Enums;
using WindowSill.OutlookCalendar.Extensions;
using WindowSill.OutlookCalendar.Models;
using Application = Microsoft.Office.Interop.Outlook.Application;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace WindowSill.OutlookCalendar.Services
{
    [Export(typeof(IOutlookService))]

    public class OutlookService : IOutlookService
    {
        public bool IsAppointmentInitiated { get => Appointments.Count > 0; }

        public List<CalendarAppointmentVm> Appointments { get; set; } = new List<CalendarAppointmentVm>();

        public OfficeVersion IsNewerOfficeVersion { get; set; } = OfficeVersion.OfficeGraphql;

        public Outlook.NameSpace? OutlookNameSpace { get; set; }

        private GraphServiceClient? GraphClient { get; set; }

        public bool IsOutlookLogged { get; set; }

        public bool _isLoadingAppointments { get; set; }

        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        private ILogger _logger;
        public OutlookService()
        {
            _logger = this.Log();
        }

        public async Task InitAllAppointments()
        {
            try
            {

                if (await semaphoreSlim.WaitAsync(1000))
                    return;

                _isLoadingAppointments = true;

                Appointments.Clear();

                if (IsNewerOfficeVersion == OfficeVersion.OfficeGraphql)
                {
                    if (GraphClient is null)
                        return;

                    var startDateTime = DateTime.Now.ToString("o");
                    var endDateTime = DateTime.Now.AddMonths(1).ToString("o");

                    EventCollectionResponse? events = null;
                    try
                    {
                        events = await GraphClient.Me.CalendarView.GetAsync(config =>
                        {
                            config.QueryParameters.StartDateTime = startDateTime;
                            config.QueryParameters.EndDateTime = endDateTime;
                            config.QueryParameters.Orderby = new[] { "start/dateTime" };
                            config.QueryParameters.Top = 5;
                            config.Headers.Add("Prefer", $"outlook.timezone=\"{TimeZoneInfo.Local.Id}\"");
                        });
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error fetching calendar events from Microsoft Graph API.");
                    }
                    finally
                    {
                        _isLoadingAppointments = false;
                    }

                    if (events is null)
                        return;

                    if (events.Value != null && events.Value.Any())
                    {
                        foreach (var item in events.Value)
                        {
                            if (item.Start is null || item.End is null)
                                continue;

                            var start = item.Start.ToDateTimeExt();
                            var end = item.End.ToDateTimeExt();
                            var location = item.Location?.ToString() ?? string.Empty;

                            if (DateTime.Compare(DateTime.Now, item.Start.ToDateTimeExt()) < 0)
                                Appointments.Add(new CalendarAppointmentVm(item.Subject ?? string.Empty, start, end, location));
                        }
                    }

                    _isLoadingAppointments = false;
                }
                else
                {
                    Outlook.Items? items = null;

                    try
                    {
                        if (OutlookNameSpace is null)
                            return;

                        Outlook.MAPIFolder? calendar = OutlookNameSpace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderCalendar);

                        items = calendar.Items;

                        items.IncludeRecurrences = true;

                        items.Sort("[Start]");

                        calendar = null;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error fetching calendar events from Microsoft 2016 Graph API.");
                    }

                    if (items is null || OutlookNameSpace is null)
                        return;

                    //var filter = "[Start] >= '" + DateTime.Now.ToString("g") + "'";
                    //Outlook.Items filtereditems = items.Restrict(filter);

                    foreach (var item in items)
                    {
                        if (item is not Outlook.AppointmentItem appt)
                            continue;

                        if (DateTime.Compare(DateTime.Now, appt.Start) < 0)
                            Appointments.Add(new CalendarAppointmentVm(appt.Subject, appt.Start, appt.End, appt.Location));
                    }
                    items = null;
                }

            }
            finally
            {
                if (semaphoreSlim.CurrentCount == 0 && _isLoadingAppointments)
                {
                    semaphoreSlim.Release();
                    _isLoadingAppointments = false;
                }
            }
        }

        public CalendarAppointmentVm? FirstAppointment()
        {
            if (Appointments.Count > 0)
                return Appointments[0];

            return null;
        }

        public List<CalendarAppointmentVm> GetAllAppointments() =>
             Appointments;

        public async Task<string> InitLogin(string tenantID)
        {
            _logger.LogInformation("Trying to login into Outlook from Outlook Calendar Extension...");

            if (IsOutlookLogged)
                return "-";

            var username = "-";

            if (IsNewerOfficeVersion == OfficeVersion.OfficeGraphql)
            {
                if (GraphClient is not null)
                    return username;

                await Task.Run(async () =>
                {
                    try
                    {
                        GraphClient = new GraphServiceClient(new TokenCredentialMSAL(_logger));

                        var res = await GraphClient.Me.GetAsync();

                        if (res?.DisplayName is not null)
                        {
                            IsOutlookLogged = true;
                            username = res?.DisplayName;
                        }
                        else
                            IsOutlookLogged = false;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error while logging to Microsoft Graph API.");
                        IsOutlookLogged = false;
                    }
                });
            }
            else
            {
                await Task.Run(() =>
                {
                    var outlookApp = new Application();
                    OutlookNameSpace ??= outlookApp.GetNamespace("MAPI");

                    try
                    {
                        OutlookNameSpace.Logon();

                        if (OutlookNameSpace.CurrentUser.Name is null)
                        {
                            IsOutlookLogged = false;
                        }
                        else
                        {
                            username = OutlookNameSpace.CurrentUser.Name;
                            IsOutlookLogged = true;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error while logging to Microsoft 2016 Graph API.");
                        IsOutlookLogged = false;
                    }
                });
            }

            return username;
        }
    }
}
