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
                                                .DataContext(this.DataContext)
                                                .Content(
                                                    new StackPanel()
                                                        .VerticalAlignment (VerticalAlignment.Center)
                                                        .HorizontalAlignment (HorizontalAlignment.Center)
                                                        .Orientation(Orientation.Vertical)
                                                               .Children(
                                                                   new Border()
                                                                   .Margin(5)
                                                                   .Background(Colors.LightGray)
                                                                   .Child(
                                                                       new TextBlock()
                                                                           .HorizontalAlignment(HorizontalAlignment.Center)
                                                                           .Text("Simple Calculator")
                                                                           .Foreground(Colors.Black)
                                                                       ),
                                                                   new TextBlock()
                                                                       .Text(() => vm.Total, x => $"Total: {x}")
                                                                       .HorizontalAlignment(HorizontalAlignment.Center),
                                                                   new TextBlock().Text(() => vm.X, x => $"Operand: {x}")
                                                                        .HorizontalAlignment(HorizontalAlignment.Center),
                                                                   new TextBlock()
                                                                        .Text(() => vm.SelectedArithmeticOP, x => $"Operator: {x}")
                                                                        .HorizontalAlignment(HorizontalAlignment.Center),
                                                                   new StackPanel()
                                                                       .Margin(7)
                                                                       .Orientation(Orientation.Horizontal)
                                                                       .Spacing(3)
                                                                       .Children(
                                                                           new Button()
                                                                               .Content("+")
                                                                               .CommandParameter('+')
                                                                               .Command(() => vm.AppendNumberWithOPCommand),
                                                                           new Button()
                                                                               .Content("-")
                                                                               .CommandParameter('-')
                                                                               .Command(() => vm.AppendNumberWithOPCommand),
                                                                           new Button()
                                                                               .Content("*")
                                                                               .CommandParameter('*')
                                                                               .Command(() => vm.AppendNumberWithOPCommand),
                                                                           new Button()
                                                                               .Content("/")
                                                                               .CommandParameter('/')
                                                                               .Command(() => vm.AppendNumberWithOPCommand),
                                                                           new Button()
                                                                               .Content("=")
                                                                               .CommandParameter('=')
                                                                               .Command(() => vm.AppendNumberWithOPCommand)
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
