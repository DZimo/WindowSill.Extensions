using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using System.ComponentModel.Composition;
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

        public async Task InitAllAppointments()
        {
            Appointments.Clear();

            if (IsNewerOfficeVersion == OfficeVersion.OfficeGraphql)
            {
                if (GraphClient is null)
                    return;

                var startDateTime = DateTime.Now.ToString("o");
                var endDateTime = DateTime.Now.AddMonths(1).ToString("o");

                var events = await GraphClient.Me.CalendarView.GetAsync(config =>
                {
                    config.QueryParameters.StartDateTime = startDateTime;
                    config.QueryParameters.EndDateTime = endDateTime;
                    config.QueryParameters.Orderby = new[] { "start/dateTime" };
                    config.QueryParameters.Top = 5;
                    config.Headers.Add("Prefer", $"outlook.timezone=\"{TimeZoneInfo.Local.Id}\"");
                });

                if (events?.Value != null && events.Value.Any())
                {
                    foreach (var item in events.Value)
                    {
                        if (item.Start is null || item.End is null)
                            continue;

                        var start = item.Start.ToDateTime();
                        var end = item.End.ToDateTime();

                        if (DateTime.Compare(DateTime.Now, item.Start.ToDateTime()) < 0)
                            Appointments.Add(new CalendarAppointmentVm(item.Subject ?? string.Empty, start, end, item.Location.ToString() ?? string.Empty));
                    }
                }
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
        }

        public CalendarAppointmentVm? FirstAppointment()
        {
            if (Appointments.Count > 0)
                return Appointments[0];

            return null;
        }

        public List<CalendarAppointmentVm> GetAllAppointments()
        {
            return Appointments;
        }

        static async Task PreAuthenticateAsync(DeviceCodeCredential credential, SettingsAzure settings)
        {
            if (!string.IsNullOrEmpty(""))
            {
                var tokenContext = new TokenRequestContext(settings.GraphUserScopes ??
                    new[] { "https://graph.microsoft.com/.default" });
                var authRecord = await credential.AuthenticateAsync(tokenContext);

                if (authRecord != null)
                {
                    using var cacheStream = new FileStream(settings.AuthRecordCachePath, FileMode.Create, FileAccess.Write);
                    await authRecord.SerializeAsync(cacheStream);
                }
            }
        }

        public void InitLogin()
        {
            IsNewerOfficeVersion = OfficeVersion.OfficeGraphql;
            if (IsNewerOfficeVersion == OfficeVersion.OfficeGraphql)
            {
                if (GraphClient is not null)
                    return;

                var credential = new InteractiveBrowserCredential(
                    new InteractiveBrowserCredentialOptions
                    {
                        ClientId = "3c62448e-650a-497a-b43c-35f9db069e4f",
                        TenantId = "common"
                    });

                GraphClient = new GraphServiceClient(credential, new[] { "Calendars.Read" });
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
