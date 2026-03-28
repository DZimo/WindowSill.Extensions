using CommunityToolkit.WinUI.Controls;
using WindowSill.API;
using WindowSill.OutlookCalendar.Converters;
using WindowSill.OutlookCalendar.ViewModels;

namespace WindowSill.OutlookCalendar.Settings
{
    internal class SettingsView : UserControl
    {
        public SettingsView(ISettingsProvider settingsProvider, SettingsViewModel settingsVm, OutlookCalendarVm outlookCalendarVm)
        {
            this.DataContext(
                settingsVm,
                (view, viewModel) => view
                .Content(
                    new StackPanel()
                            .Spacing(2)
                            .Children(
                                new TextBlock()
                                    .Style(x => x.ThemeResource("BodyStrongTextBlockStyle"))
                                    .Margin(0, 0, 0, 8)
                                    .Text("/WindowSill.OutlookCalendar/Misc/General".GetLocalizedString()),
                                new SettingsCard()
                                    .Header("/WindowSill.OutlookCalendar/Misc/OfficeSettingsHeader".GetLocalizedString())
                                    .Description("/WindowSill.OutlookCalendar/Misc/OfficeVersionHeader".GetLocalizedString())
                                    .HeaderIcon(
                                        new FontIcon()
                                            .Glyph("\uE713"))
                                    .Content(
                                          new StackPanel()
                                          .Children(
                                                new ToggleSwitch()
                                                .IsOn(x => x.Binding(() => viewModel.SelectedOfficeVersion).Converter(new OfficeBoolConverter()).TwoWay()))),

           new UserControl()
               .DataContext(outlookCalendarVm, (view, vm) => view
                   .Content(
                       new StackPanel()
                         .Opacity(0.5)
                           .Children(
                               new SettingsCard()
                                   .Header("/WindowSill.OutlookCalendar/Misc/ConnectionHeader".GetLocalizedString())
                                   .Description("/WindowSill.OutlookCalendar/Misc/ConnectionDesc".GetLocalizedString())
                                   .HeaderIcon(
                                       new FontIcon()
                                           .Glyph("\uE716"))
                                           .Content(
                                             new StackPanel()
                                                   .Name(out var khra)
                                                   .Spacing(10)
                                                   .Children(
                                                       new ToggleSwitch()
                                                           .IsOn((x) => x.Binding(() => vm.IsLoggedIn))
                                                           .OnContent("/WindowSill.OutlookCalendar/Misc/Connected".GetLocalizedString())
                                                           .OffContent("/WindowSill.OutlookCalendar/Misc/Disconnected".GetLocalizedString())
                                                           .IsEnabled(false)
                                                   )
                                           ),
                               new SettingsCard()
                                   .Header("/WindowSill.OutlookCalendar/Misc/TimezoneHeader".GetLocalizedString())
                                   .Description("/WindowSill.OutlookCalendar/Misc/TimezoneDesc".GetLocalizedString())
                                   .HeaderIcon(
                                       new FontIcon()
                                           .Glyph("\uEC92"))
                                           .Content(
                                             new StackPanel()
                                                   .Spacing(10)
                                                   .Children(
                                                       new TextBlock()
                                                           .Text(TimeZoneInfo.Local.Id)
                                                   )
                                           )
                           )))))
            );
        }
    }
}
