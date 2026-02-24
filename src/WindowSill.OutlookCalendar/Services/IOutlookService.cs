using WindowSill.OutlookCalendar.Models;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace WindowSill.OutlookCalendar.Services
{
    public interface IOutlookService
    {
        public bool IsAppointmentInitiated { get; }

        public bool IsOutlookLogged { get; }

        public OfficeVersion IsNewerOfficeVersion { get; set; }

        public List<CalendarAppointmentVm> Appointments { get; set; }

        public Outlook.NameSpace? OutlookNameSpace { get; set; }

        public Task InitAllAppointments();

        public List<CalendarAppointmentVm> GetAllAppointments();

        public CalendarAppointmentVm? FirstAppointment();

        public void InitLogin();

    }
}
