using CommunityToolkit.WinUI.Controls;
using WindowSill.API;
using WindowSill.OutlookCalendar.Settings;
using WindowSill.ScreenRecorder.ViewModels;

namespace WindowSill.OutlookCalendar.Views;

public sealed class OutlookCalendarFirstTimeView : UserControl
{
    private OutlookCalendarVm _outlookCalendarVm;
    public OutlookCalendarFirstTimeView(SettingsViewModel settingsViewModel)
    {
        this.Content(
         new StackPanel()
             .Spacing(16)
             .Children(
                 new TextBlock()
                     .TextWrapping(TextWrapping.WrapWholeWords)
                     .Text("/WindowSill.OutlookCalendar/Misc/FirstTimeHeadline".GetLocalizedString()),

                 new SettingsCard()
                     .Header("/WindowSill.OutlookCalendar/Misc/OfficeVersionHeader".GetLocalizedString())
                     .HeaderIcon(
                         new FontIcon()
                             .Glyph("\uE713")
                     )
                     .IsClickEnabled(true)
                     .Content(
                              new StackPanel()
                                  .Children(
                                            new ToggleSwitch()
                                            .IsOn(x => x.Binding(() => settingsViewModel.SelectedOfficeVersion))
             )
     )));
    }

    public SillListViewButtonItem ExtendAppointments()
    {
        return new SillListViewButtonItem('\xE722', new TextBlock().Margin(5).Text("/WindowSill.OutlookCalendar/Misc/DisplayName".GetLocalizedString()), _outlookCalendarVm.Expand);
    }
}
