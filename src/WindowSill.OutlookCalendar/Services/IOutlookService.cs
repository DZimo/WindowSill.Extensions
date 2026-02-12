using WindowSill.OutlookCalendar.Models;

namespace WindowSill.OutlookCalendar.Services
{
    public interface IOutlookService
    {
        public bool IsAppointmentInitiated { get; }

        public List<CalendarAppointment> Appointments { get; set; }

        public void InitAllAppointments();

        public List<CalendarAppointment> GetAllAppointments();

        public CalendarAppointment FirstAppointment();

    }
}
