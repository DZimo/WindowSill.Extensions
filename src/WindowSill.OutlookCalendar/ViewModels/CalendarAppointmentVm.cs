using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Graph.Models;

namespace WindowSill.OutlookCalendar.Models
{
    public partial class CalendarAppointmentVm : ObservableObject, IEquatable<CalendarAppointmentVm>
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
        public CalendarAppointmentVm(string? subject, DateTimeTimeZone? start, DateTimeTimeZone? end, string? location)
        {
            Subject = subject ?? "";
            Location = location ?? "";

            if (start.ToString() is not null && end.ToString() is not null)
            {
                DateTime parsedDate;

                Start = DateTime.TryParse(start?.ToString(), out parsedDate) ? parsedDate : DateTime.Now;
                End = DateTime.TryParse(end?.ToString(), out parsedDate) ? parsedDate : DateTime.Now;
                return;
            }

            Start = DateTime.Now;
            End = DateTime.Now;
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 23 + Start.GetHashCode();
            hash = hash * 23 + End.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return $"{Subject} at {Start:t}";
        }

        public bool Equals(CalendarAppointmentVm? other)
        {
            if (other is null || this is null)
                return false;

            if (Object.ReferenceEquals(this, other))
                return true;

            return this.Start == other.Start && this.Subject == other.Subject;
        }
    }
}
