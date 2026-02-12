using WindowSill.API;
using WindowSill.ScreenRecorder.ViewModels;

namespace WindowSill.OutlookCalendar.Views;

public sealed class OutlookCalendarView : UserControl
{
    private OutlookCalendarVm _outlookCalendarVm;
    public OutlookCalendarView(OutlookCalendarVm outlookCalendarVm)
    {
        _outlookCalendarVm = outlookCalendarVm;
    }

    public SillListViewButtonItem ExtendAppointments()
    {
        return new SillListViewButtonItem('\xE722', new TextBlock().Margin(5).Text("/WindowSill.OutlookCalendar/Misc/DisplayName".GetLocalizedString()), _outlookCalendarVm.Expand);
    }
}
