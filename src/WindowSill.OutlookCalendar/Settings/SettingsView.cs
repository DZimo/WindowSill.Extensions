using CommunityToolkit.WinUI.Controls;
using WindowSill.API;
using WindowSill.OutlookCalendar.Converters;

namespace WindowSill.OutlookCalendar.Settings
{
    internal class SettingsView : UserControl
    {
        public SettingsView(ISettingsProvider settingsProvider, SettingsViewModel settingsVm)
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
                                new SettingsCard()
                                    .Header("/WindowSill.OutlookCalendar/Misc/AccountTypeHeader".GetLocalizedString())
                                    .Description("/WindowSill.OutlookCalendar/Misc/AccountTypeDesc".GetLocalizedString())
                                    .HeaderIcon(
                                        new FontIcon()
                                            .Glyph("\uE716"))
                                    .Content(
                                          new StackPanel()
                                      .Children(
                                                new ToggleSwitch()
                                                .IsOn(x => x.Binding(() => viewModel.SelectedAccountType).Converter(new AccountBoolConverter()).TwoWay())))
                            )
                )
            );
        }
    }
}
