using WindowSill.API;
using WindowSill.OutlookCalendar.Models;
using WindowSill.ScreenRecorder.ViewModels;

namespace WindowSill.OutlookCalendar.ViewModels
{
    public sealed class OutlookCalendarView : UserControl
    {
        public OutlookCalendarView(OutlookCalendarVm outlookCalendarVm)
        {
            var subject = nameof(CalendarAppointmentVm.Subject);
            var start = nameof(CalendarAppointmentVm.Start);

            this.DataContext(
              outlookCalendarVm,
              (view, vm) => view
              .Content(
                       new SillOrientedStackPanel()
                              .VerticalAlignment(VerticalAlignment.Center)
                              .HorizontalAlignment(HorizontalAlignment.Center)
                              .Spacing(1)
                              .Children(
                                 new SillListViewPopupItem(new TextBlock().Text(o => o.Binding(() => vm.NextAppointmentLeftTime)), null,
                                     new SillPopupContent()
                                        .DataContext(outlookCalendarVm)
                                        .Content(
                                             new StackPanel()
                                             .VerticalAlignment(VerticalAlignment.Center)
                                             .HorizontalAlignment(HorizontalAlignment.Center)
                                             .Spacing(1)
                                                .Children(
                                                    new TextBlock().Text("/WindowSill.OutlookCalendar/Misc/AppointmentListTitle".GetLocalizedString()).HorizontalTextAlignment(TextAlignment.Center).Margin(4),
                                                    new ListView()
                                                       .ItemsSource(o => o.Binding(() => vm.AllAppointments))
                                                       .ItemTemplate(() =>
                                                       new StackPanel()
                                                       .Orientation(Orientation.Horizontal)
                                                       .Spacing(4)
                                                       .Children(
                                                           new TextBlock().Text(x => x.Binding(subject)),
                                                           new TextBlock().Text(x => x.Binding(start))
                                                        )
                                                )
                                            )
                                         )
                                        )
                                 )
                       ));
        }
    }
}
