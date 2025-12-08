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
                                  .Orientation(Orientation.Horizontal)
                                  .Children(
                                      new TextBox()
                                          .PlaceholderText("#FFFFFF")
                                          .PlaceholderForeground(Colors.Gray)
                                          .FontSize(12)
                                          .TextAlignment(TextAlignment.Center)
                                          .AcceptsReturn(false)
                                          .FontStretch(Windows.UI.Text.FontStretch.Expanded)
                                          .VerticalContentAlignment(VerticalAlignment.Center)
                                          .TextWrapping(TextWrapping.NoWrap)
                                          .MinHeight(18)
                                          .MaxWidth(75)
                                          .Width(75)
                                          .MaxLength(7)
                                          .Text(x => x.Binding(() => vm.SelectedColorHex).TwoWay())
                                          .Padding(0)
                                          .Margin(0)
                                          .BorderBrush(x => x.Binding(() => vm.SelectedColorBrush).OneWay()),
                                      new StackPanel()
                                          .Width(7)
                                          .Margin(5, 0, 0, 0)
                                          .Background(x => x.Binding(() => vm.SelectedColorBrush).OneWay()),
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
