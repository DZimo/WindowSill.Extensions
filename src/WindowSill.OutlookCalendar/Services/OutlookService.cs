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

        public List<CalendarAppointment> Appointments { get; set; } = new List<CalendarAppointment>();

        public void InitAllAppointments()
        {
            var outlookApp = new Application();
            Outlook.NameSpace ns = outlookApp.GetNamespace("MAPI");
            ns.Logon();

            Outlook.MAPIFolder calendar = ns.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderCalendar);

            Outlook.Items items = calendar.Items;

            items.IncludeRecurrences = true;

            items.Sort("[Start]");

            foreach (object item in items)
            {
                if (item is Outlook.AppointmentItem appt)
                {
                    Console.WriteLine($"Subject: {appt.Subject}");
                    Console.WriteLine($"Start: {appt.Start}");
                    Console.WriteLine($"End: {appt.End}");
                    Console.WriteLine($"Location: {appt.Location}");
                    Console.WriteLine("---------------------------");
                }
            }
        }

        public CalendarAppointment FirstAppointment()
        {
            throw new NotImplementedException();
        }

        public List<CalendarAppointment> GetAllAppointments()
        {
            return Appointments;
        }
    }
}
