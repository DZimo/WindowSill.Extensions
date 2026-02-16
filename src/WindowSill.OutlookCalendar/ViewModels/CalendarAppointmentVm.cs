
using CommunityToolkit.Mvvm.ComponentModel;

namespace WindowSill.OutlookCalendar.Models
{
    public partial class CalendarAppointmentVm : ObservableObject
    {
        [ObservableProperty]
        public string subject;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Location { get; set; }

        public CalendarAppointmentVm(string subject, DateTime start, DateTime end, string location)
        {
            Subject = subject;
            Start = start;
            End = end;
            Location = location;
        }
    }
}
