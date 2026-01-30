using System.Collections.ObjectModel;
using WindowSill.API;
using WindowSill.ScreenRecorder.ViewModels;

namespace WindowSill.ScreenRecorder.Views;

public sealed class ScreenRecorderView : UserControl
{
    private ScreenRecorderVm _screenRecorderVm;
    public ScreenRecorderView(ScreenRecorderVm screenRecorderVm)
    {
        _screenRecorderVm = screenRecorderVm;
        ObservableCollection<SillListViewItem> testList = new();
        testList.Add(StartRecordingButton());
        testList.Add(StopRecordingButton());

        //this.DataContext(
        //  screenRecorderVm,
        //  (view, vm) => view
        //  .Content(
        //      new Grid()
        //          .VerticalAlignment(VerticalAlignment.Top)
        //          .Children(
        //              new SillOrientedStackPanel()
        //                  .Spacing(1)
        //                  .Children(
        //                      new StackPanel()
        //                          .Orientation(Orientation.Horizontal)
        //                          .Children(
        //                              new TextBlock()
        //                                  .FontSize(x => x.Binding(() => vm.ColorFontSize).OneWay())
        //                                  .TextAlignment(TextAlignment.Center)
        //                                  .FontStretch(Windows.UI.Text.FontStretch.Expanded)
        //                                  .TextWrapping(TextWrapping.Wrap)
        //                                  .MinHeight(x => x.Binding(() => vm.ColorboxHeight).OneWay())
        //                                  .MaxWidth(75)
        //                                  .Width(75)
        //                                  .Text(x => x.Binding(() => vm.CalculatedNumber).TwoWay().UpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged))
        //                                  .Padding(0)
        //                          ))
        //              )
        //));
    }

    private SillListViewButtonItem StartRecordingButton()
    {
        return new SillListViewButtonItem('\xEf3c', new TextBlock().Margin(5).Text("/WindowSill.ColorPicker/Misc/GrabColor".GetLocalizedString()), _screenRecorderVm.TestVm);
    }

    private SillListViewButtonItem StopRecordingButton()
    {
        return new SillListViewButtonItem('\xEf3c', new TextBlock().Margin(5).Text("/WindowSill.ColorPicker/Misc/GrabColor".GetLocalizedString()), _screenRecorderVm.TestVm);
    }
}
