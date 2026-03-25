using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media.Imaging;
using WindowSill.API;
using WindowSill.OutlookCalendar.Models;
using WindowSill.OutlookCalendar.ViewModels;

namespace WindowSill.OutlookCalendar.Views
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
                                                             new Border()
                                                                .BorderThickness(0, 0, 0, 1)
                                                                .BorderBrush(new SolidColorBrush(Colors.Gray))
                                                                .Padding(0, 0, 0, 4)
                                                                .Child(
                                                                 new StackPanel()
                                                                 .Children(
                                                                     new StackPanel()
                                                                        .Orientation(Orientation.Horizontal)
                                                                        .HorizontalAlignment(HorizontalAlignment.Center)
                                                                        .Children(
                                                                             new TextBlock().Text("/WindowSill.OutlookCalendar/Misc/AppointmentListTitle".GetLocalizedString()).HorizontalTextAlignment(TextAlignment.Center).Margin(4),
                                                                             new FontIcon().Glyph("\uE787")
                                                                        ),
                                                                     new TextBlock()
                                                                        .Text(o => o.Binding(() => vm.UserName))
                                                                        .FontSize(10)
                                                                        .FontWeight(FontWeights.SemiBold)
                                                                        .HorizontalTextAlignment(TextAlignment.Center)
                                                                        )),
                                                            new ListView()
                                                               .ItemsSource(o => o.Binding(() => vm.AllAppointments))
                                                               .ItemTemplate(() =>
                                                               new StackPanel()
                                                               .Orientation(Orientation.Horizontal)
                                                               .VerticalAlignment(VerticalAlignment.Center)
                                                               .HorizontalAlignment(HorizontalAlignment.Center)
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
