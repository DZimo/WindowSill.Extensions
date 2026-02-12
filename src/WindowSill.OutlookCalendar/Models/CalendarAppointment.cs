
namespace WindowSill.OutlookCalendar.Models
{
    public sealed class CalendarAppointment
    {
        public string Subject { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Location { get; set; }

        public CalendarAppointment(string subject, DateTime start, DateTime end, string location)
        {
            Subject = subject;
            Start = start;
            End = end;
            Location = location;
        }
    }
}
