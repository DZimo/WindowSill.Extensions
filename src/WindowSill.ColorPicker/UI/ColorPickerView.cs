using WindowSill.API;

namespace WindowSill.ColorPicker.UI;

public sealed class ColorPickerView : UserControl
{
    public ColorPickerView(IPluginInfo pluginInfo, ColorPickerVm colorPickerVm)
    {
        this.DataContext(
          colorPickerVm,
          (view, vm) => view
          .Content(
              new Grid()
                  .VerticalAlignment(VerticalAlignment.Top)
                  .Children(
                      new SillOrientedStackPanel()
                          .Children(
                              new StackPanel()
                                  .Orientation(Orientation.Horizontal)
                                  .Spacing(4)
                                  .Children(
                                      new TextBox()
                                          .PlaceholderText("#FFFFFF")
                                          .PlaceholderForeground(Colors.Gray)
                                          .FontSize(x => x.Binding(() => vm.ColorFontSize).OneWay())
                                          .TextAlignment(TextAlignment.Center)
                                          .AcceptsReturn(false)
                                          .FontStretch(Windows.UI.Text.FontStretch.Expanded)
                                          .VerticalContentAlignment(VerticalAlignment.Center)
                                          .TextWrapping(TextWrapping.Wrap)
                                          .MinHeight(x => x.Binding(() => vm.ColorboxHeight).OneWay())
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
                                                   .Content("\xEf3c")
                                                   .Command(x => x.Binding(() => vm.GetColorCommand)),
                                                new Button()
                                                   .Style(x => x.StaticResource("IconButton"))
                                                   .Content("\xE8c8")
                                                   .Command(x => x.Binding(() => vm.CopyColorHexCommand))
                                  ))
                      )
        )));
    }

    public static Grid ColorPickerGrid(ColorPickerVm vm) => new Grid()
                    .Children(
                        new SillOrientedStackPanel()
                            .Children(
                                  new StackPanel()
                                      .Orientation(Orientation.Horizontal)
                                      .Spacing(1)
                                      .VerticalAlignment(VerticalAlignment.Center)
                                      .Children(
                                        new StackPanel()
                                              .Width(7)
                                              .Margin(5, 0, 5, 0)
                                              .Background(x => x.Binding(() => vm.SelectedColorBrush).OneWay()),
                                        new Border()
                                              .BorderBrush(x => x.Binding(() => vm.SelectedColorBrush).OneWay())
                                              .BorderThickness(x => x.Binding(() => vm.SelectedColorThickness).OneWay().Convert(o => new Thickness(o)))
                                              .CornerRadius(2)
                                              .Padding(0)
                                              .Margin(0, 0, 0, 2)
                                              .Child(
                                                 new TextBox()
                                                      .PlaceholderText("#FFFFFF")
                                                      .PlaceholderForeground(Colors.Gray)
                                                      .FontSize(x => x.Binding(() => vm.ColorFontSize).OneWay())
                                                      .TextAlignment(TextAlignment.Center)
                                                      .AcceptsReturn(false)
                                                      .FontStretch(Windows.UI.Text.FontStretch.Expanded)
                                                      .VerticalContentAlignment(VerticalAlignment.Center)
                                                      .TextWrapping(TextWrapping.Wrap)
                                                      .MinHeight(x => x.Binding(() => vm.ColorboxHeight).OneWay())
                                                      .MaxWidth(75)
                                                      .Width(75)
                                                      .MaxLength(7)
                                                      .Text(x => x.Binding(() => vm.SelectedColorHex).TwoWay())
                                                      .Padding(0))),
                                      new Button()
                                              .Style(x => x.StaticResource("IconButton"))
                                              .IsHitTestVisible(false)
                                              .VerticalAlignment(VerticalAlignment.Center)
                                              .VerticalContentAlignment(VerticalAlignment.Center)
                                              .Content("\xe790")));
}
