using Microsoft.UI.Xaml.Media.Imaging;
using WindowSill.API;
using WindowSill.OutlookCalendar.Models;
using WindowSill.ScreenRecorder.ViewModels;

namespace WindowSill.OutlookCalendar.ViewModels
{
    public sealed class OutlookCalendarView : UserControl
    {
        public OutlookCalendarView(OutlookCalendarVm outlookCalendarVm, IPluginInfo _pluginInfo)
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
                                 new SillListViewPopupItem(
                                     new StackPanel()
                                        .VerticalAlignment(VerticalAlignment.Center)
                                        .HorizontalAlignment(HorizontalAlignment.Center)
                                        .Orientation(Orientation.Horizontal)
                                        .Spacing(10)
                                        .Children(
                                             new ImageIcon()
                                                .MaxHeight(20)
                                                .Source(new SvgImageSource(new Uri(System.IO.Path.Combine(_pluginInfo.GetPluginContentDirectory(), "Assets", "outlook_calendar.svg")))),
                                             new TextBlock()
                                                 .TextAlignment(TextAlignment.Center)
                                                 .VerticalAlignment(VerticalAlignment.Center)
                                                 .Text(o => o.Binding(() => vm.NextAppointmentLeftTime))), null,
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
