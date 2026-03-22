using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.ComponentModel.Composition;
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

        public async Task InitAllAppointments()
        {
            if (_isLoadingAppointments)
                return;

            _isLoadingAppointments = true;

            await semaphoreSlim.WaitAsync(1);

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
                catch
                {
                    return;
                }

                finally
                {

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
            semaphoreSlim.Release();
        }

        public CalendarAppointmentVm? FirstAppointment()
        {
            if (Appointments.Count > 0)
                return Appointments[0];

            return null;
        }

        public List<CalendarAppointmentVm> GetAllAppointments() =>
             Appointments;

        public async Task InitLogin(string tenantID)
        {
            if (IsNewerOfficeVersion == OfficeVersion.OfficeGraphql)
            {
                if (GraphClient is not null)
                    return;

                GraphClient = new GraphServiceClient(new TokenCredentialMSAL());
            }
            else
            {
                var outlookApp = new Application();
                OutlookNameSpace = outlookApp.GetNamespace("MAPI");
                OutlookNameSpace.Logon();
            }
        }
    }
}
