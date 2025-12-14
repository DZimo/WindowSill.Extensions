using Microsoft.UI.Xaml.Controls;
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
                                          .Name((o) =>
                                          {
                                              o.TextChanged += (s, e) =>
                                              {
                                                  UpdateNumberText();
                                              };
                                          })
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
                                          .Text(x => x.Binding(() => vm.SelectedNumber).TwoWay().UpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged))
                                          .Padding(0),
                                      new StackPanel()
                                          .Margin(5, 0, 5, 0)
                                          .Padding(5, 0)
                                          .Children(
                                              new TextBlock()
                                                  .Text(x => x.Binding(() => vm.SelectedArithmeticOP)
                                                  .Converter(Converters.ArithmeticOpConverter))
                                                  .VerticalAlignment(VerticalAlignment.Center)
                                                  .HorizontalAlignment(HorizontalAlignment.Center)),
                                      new SillListViewPopupItem(
                                            '\xF0ad',
                                            null,
                                            new SillPopupContent()
                                            .Content(
                                                    new StackPanel()
                                                        .VerticalAlignment (VerticalAlignment.Center)
                                                        .HorizontalAlignment (HorizontalAlignment.Center)
                                                        .Orientation(Orientation.Vertical)
                                                               .Children(
                                                                   new TextBlock()
                                                                       .HorizontalAlignment(HorizontalAlignment.Center)
                                                                       .Text("Extra params"),
                                                                   new StackPanel()
                                                                       .Orientation(Orientation.Horizontal)
                                                                       .Children(
                                                                       new Button()
                                                                           .Content("+"),
                                                                       new Button()
                                                                           .Content("-"),
                                                                       new Button()
                                                                           .Content("*"),
                                                                       new Button()
                                                                           .Content("/"),
                                                                       new Button()
                                                                           .Content("=")
                                                                       )
                                                      
                                                               ))
                                            )
                                  ))
                      )
        ));
    }

    private void UpdateNumberText()
    {
        SimpleCalculatorVm.Instance?.NumberTextboxChanging();
    }
}
