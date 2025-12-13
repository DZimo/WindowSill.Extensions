using System.Numerics;
using WindowSill.API;
using WindowSill.SimpleCalculator.Services;

namespace WindowSill.SimpleCalculator.UI;

public sealed class SimpleCalculatorView : UserControl
{
    public SimpleCalculatorView(IPluginInfo pluginInfo, SimpleCalculatorVm calculatorVm)
    {
        this.DataContext(
          calculatorVm,
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
                                          .PlaceholderText("2 + 5")
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
                                          .Text(x => x.Binding(() => vm.SelectedNumber).TwoWay())
                                          .Padding(0),
                                      new StackPanel()
                                          .Width(7)
                                          .Margin(5, 0, 0, 0)
                                          .Children(
                                              new TextBlock()
                                                  .Text(x => x.Binding(() => vm.SelectedArithmeticOP))),
                                      new StackPanel()
                                      .Orientation(Orientation.Horizontal)
                                          .Children(
                                                new Button()
                                                   .Style(x => x.StaticResource("IconButton"))
                                                   .Content("\xE752")
                                                   .Command(x => x.Binding(() => vm.ExtendCalculatorCommand))
                                  ))
                      )
        )));
    }
}
