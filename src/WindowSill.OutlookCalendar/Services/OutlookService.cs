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

        public void InitAllAppointments()
        {
            Appointments.Clear();
            Outlook.Items? items = null;
            Outlook.NameSpace? ns = null;

            try
            {
                var outlookApp = new Application();

                ns = outlookApp.GetNamespace("MAPI");
                ns.Logon();

                Outlook.MAPIFolder calendar = ns.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderCalendar);

                items = calendar.Items;

                items.IncludeRecurrences = true;

                items.Sort("[Start]");
            }
            catch (Exception ex)
            {
                return;
            }

            finally
            {
                ns?.Logoff();
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
    }
}
