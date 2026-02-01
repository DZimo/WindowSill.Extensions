using WindowSill.API;
using WindowSill.ScreenRecorder.ViewModels;

namespace WindowSill.ScreenRecorder.Views;

public sealed class ScreenRecorderView : UserControl
{
    private ScreenRecorderVm _screenRecorderVm;
    public ScreenRecorderView(ScreenRecorderVm screenRecorderVm)
    {
        _screenRecorderVm = screenRecorderVm;
    }

    public SillListViewButtonItem StartRecordingButton()
    {
        return new SillListViewButtonItem('\xE722', new TextBlock().Margin(5).Text("/WindowSill.ScreenRecorder/Misc/DisplayName".GetLocalizedString()), _screenRecorderVm.Capture);
    }

    public SillListViewButtonItem StopRecordingButton()
    {
        return new SillListViewButtonItem('\xE7C8', new TextBlock().Margin(5).Text("/WindowSill.ScreenRecorder/Misc/DisplayName".GetLocalizedString()), _screenRecorderVm.Capture);
    }
}
