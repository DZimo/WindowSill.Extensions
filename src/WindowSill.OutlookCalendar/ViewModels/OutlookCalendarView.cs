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
                  new Grid()
                      .Children(
                          new SillOrientedStackPanel()
                              .VerticalAlignment(VerticalAlignment.Center)
                              .HorizontalAlignment(HorizontalAlignment.Center)
                              .Spacing(1)
                              .Children(
                                 new TextBlock()
                                       .Text(o => o.Binding(() => vm.NextAppointmentLeftTime)))
                        )));
        }
    }
}
