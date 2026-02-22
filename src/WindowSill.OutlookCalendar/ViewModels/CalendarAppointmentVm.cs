using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Graph.Models;

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
    }
}
