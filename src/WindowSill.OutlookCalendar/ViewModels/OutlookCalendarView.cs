using WindowSill.API;
using WindowSill.ScreenRecorder.ViewModels;

namespace WindowSill.OutlookCalendar.ViewModels
{
    public sealed class OutlookCalendarView : UserControl
    {
        public OutlookCalendarView(OutlookCalendarVm outlookCalendarVm)
        {
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
                                            .Children(
                                                new TextBlock().Text("/WindowSill.OutlookCalendar/Misc/AppointmentListTitle".GetLocalizedString()).HorizontalTextAlignment(TextAlignment.Center),
                                                new ListView()
                                                   .ItemsSource(o => o.Binding(() => vm.AllAppointments))
                                                   .ItemTemplate(() =>
                                                   new StackPanel().Children(new TextBlock().Text(x => x.Binding("Subject")))))))
                                 //new ListView()
                                 //    .DataContext(vm)
                                 //    .ItemsSource(o => o.Binding(() => vm.AllAppointments))
                                 //    .ItemTemplateSelector<CalendarAppointmentVm>((calendar, selector) => selector
                                 //        .Default(() => new TextBlock().Text(() => calendar.Subject))))))
                                 )
                       ));
        }
    }
}
