using System.Numerics;
using WindowSill.API;

namespace WindowSill.ColorPicker.UI;

public sealed class ColorPickerView : UserControl
{
    public ColorPickerView(IPluginInfo pluginInfo, ColorPickerVm pomodoroVm)
    {
        this.DataContext(
          pomodoroVm,
          (view, vm) => view
          .Content(
              new Grid()
                  .VerticalAlignment(VerticalAlignment.Top)
                  .Children(
                      new SillOrientedStackPanel()
                          .Spacing(1)
                          .Children(
                              new StackPanel()
                                  .Children(
                                      new TextBox()
                                          .VerticalAlignment(VerticalAlignment.Top)
                                          .PlaceholderText("#FFFFFF")
                                          .PlaceholderForeground(Colors.Gray)
                                          .FontSize(12)
                                          .Height(17)
                                          .MinHeight(0)
                                          .MaxLength(7)
                                          .Margin(0, 0, 0, 3)
                                          .Text(x => x.Binding(() => vm.SelectedColorHex).TwoWay())
                                          .BorderBrush(x => x.Binding(() => vm.SelectedColorBrush).OneWay()),
                                      new StackPanel()
                                      .Orientation(Orientation.Horizontal)
                                          .Children(
                                                new Button()
                                                   .Style(x => x.StaticResource("IconButton"))
                                                   .Content("\xEf3c"),
                                                new Button()
                                                   .Style(x => x.StaticResource("IconButton"))
                                                   .Content("\xE8c8")
                                  ))
                      )
        )));
    }
}
