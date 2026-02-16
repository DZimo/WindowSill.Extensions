using WindowSill.OutlookCalendar.Models;

namespace WindowSill.OutlookCalendar.Services
{
    public interface IOutlookService
    {
        public bool IsAppointmentInitiated { get; }

        public List<CalendarAppointmentVm> Appointments { get; set; }

        public void InitAllAppointments();

        public List<CalendarAppointmentVm> GetAllAppointments();

        public CalendarAppointmentVm? FirstAppointment();

    }
}
