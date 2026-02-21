using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using System.ComponentModel.Composition;
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

        public OfficeVersion IsNewerOfficeVersion { get; set; }

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

        public async void InitAllAppointments()
        {
            Appointments.Clear();

            if (IsNewerOfficeVersion == OfficeVersion.OfficeGraphql)
            {
                var events = await GraphClient.Me.Events.GetAsync(config =>
                {
                    config.QueryParameters.Orderby = new[] { "start/dateTime" };
                    config.QueryParameters.Top = 5;
                });

                if (events?.Value != null && events.Value.Any())
                {
                    Console.WriteLine("Next 5 appointments:");
                    foreach (var ev in events.Value)
                    {
                        Console.WriteLine($"Subject: {ev.Subject}");
                        Console.WriteLine($"Start : {ev.Start?.DateTime}");
                        Console.WriteLine($"End   : {ev.End?.DateTime}");
                        Console.WriteLine($"Location: {ev.Location?.DisplayName}");
                        Console.WriteLine("-----------------------------");
                    }
                }
            }
            else
            {
                Application? outlookApp;
                Outlook.Items? items = null;
                Outlook.NameSpace? ns = null;

                try
                {
                    outlookApp = new Application();

                    ns = outlookApp.GetNamespace("MAPI");

                    ns.Logon();

                    Outlook.MAPIFolder calendar = ns.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderCalendar);

                    items = calendar.Items;

                    items.IncludeRecurrences = true;

                    items.Sort("[Start]");
                }
                catch (Exception ex)
                {
                    ns?.Logoff();
                    outlookApp = null;
                    return;
                }

                finally
                {

                }

                if (items is null || ns is null)
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
    }
}
