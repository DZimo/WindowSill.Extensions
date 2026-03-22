using CommunityToolkit.Mvvm.Messaging;
using Windows.System;
using WindowSill.API;
using WindowSill.SimpleCalculator.Common;
using WindowSill.SimpleCalculator.Enums;
using WindowSill.SimpleCalculator.Models;

namespace WindowSill.SimpleCalculator.UI;

public sealed class SimpleCalculatorView : UserControl
{
    public SimpleCalculatorView(SimpleCalculatorVm calculatorVm)
    {
        KeyboardAccelerator keyboardAccelerator = new KeyboardAccelerator
        {
            Key = VirtualKey.Enter,
            Modifiers = VirtualKeyModifiers.None
        };
        keyboardAccelerator.Invoked += OnEnterPressed;
        base.KeyboardAccelerators.Add(keyboardAccelerator);
        base.KeyboardAcceleratorPlacementMode = KeyboardAcceleratorPlacementMode.Hidden;

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
                                           new SillListViewPopupItem(
                                            '\xE8EF',
                                            null,
                                            new SillPopupContent()
                                                .Name((o) =>
                                                {
                                                    o.Close();
                                                })
                                                .DataContext(this.DataContext)
                                                .Content(
                                                    new StackPanel()
                                                        .VerticalAlignment(VerticalAlignment.Center)
                                                        .HorizontalAlignment(HorizontalAlignment.Center)
                                                        .Orientation(Orientation.Vertical)
                                                               .Children(
                                                                   new Border()
                                                                   .Margin(5)
                                                                   .Background(Colors.LightGray)
                                                                   .Child(
                                                                       new TextBlock()
                                                                           .HorizontalAlignment(HorizontalAlignment.Center)
                                                                           .Text("/WindowSill.SimpleCalculator/Misc/DisplayNameSimple".GetLocalizedString())
                                                                           .Foreground(Colors.Black)
                                                                       ),
                                                                   new TextBlock()
                                                                       .Text(() => vm.Total, x => $"Total: {x}")
                                                                       .HorizontalAlignment(HorizontalAlignment.Center),
                                                                   new TextBlock()
                                                                       .Text(() => vm.X, x => $"Operand X: {x}")
                                                                       .HorizontalAlignment(HorizontalAlignment.Center),
                                                                   new TextBlock().Text(() => vm.SelectedNumber, x => $"Operand Y: {x}")
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
                                            ),
                                      new TextBox()
                                          .Name((o) =>
                                          {
                                              o.TextChanged += (s, e) =>
                                              {
                                                  SelectedNumberChanged(o);
                                                  o.UpdateTextBox();
                                              };
                                              o.GotFocus += (s, e) =>
                                              {
                                                  SelectedNumberFocused(o);
                                                  o.UpdateTextBox();
                                              };
                                          }
                                          )
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
                                          .MaxLength(20)
                                          .Text(x => x.Binding(() => vm.SelectedNumber).TwoWay().UpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged))
                                          .Padding(0),
                                      new StackPanel()
                                          .Margin(5, 0, 5, 0)
                                          .HorizontalAlignment(HorizontalAlignment.Center)
                                          .VerticalAlignment(VerticalAlignment.Center)
                                          .Padding(5, 0)
                                          .Children(
                                               new Border()
                                                        .Width(20)
                                                        .Height(20)
                                                        .CornerRadius(10)
                                                        .Background(Colors.Transparent)
                                                        .BorderThickness(2)
                                                        .BorderBrush(Colors.LightGray)
                                                        .HorizontalAlignment(HorizontalAlignment.Center)
                                                        .VerticalAlignment(VerticalAlignment.Top)
                                                        .Child(
                                                             new Grid()
                                                                    .VerticalAlignment(VerticalAlignment.Center)
                                                                    .Children(
                                                                        new TextBlock()
                                                                            .Foreground(Colors.Black)
                                                                            .Text(x => x.Binding(() => vm.SelectedArithmeticOP)
                                                                            .Converter(Converters.ArithmeticOpConverter))
                                                                            .HorizontalAlignment(HorizontalAlignment.Center)
                                                                            .VerticalAlignment(VerticalAlignment.Bottom)
                                                                            .TextAlignment(TextAlignment.Center)
                                                                            .Foreground(Colors.White)

                                  )))))
                      )
        ));
    }

    private void OnEnterPressed(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        WeakReferenceMessenger.Default.Send(new RequestNumberChanged(InterVmMessage.ExecutedRequested));
    }

    private void SelectedNumberChanged(TextBox o)
    {
        WeakReferenceMessenger.Default.Send(new RequestNumberChanged(InterVmMessage.SelectedNumberChanged));
    }

    private void SelectedNumberFocused(TextBox o)
    {
        WeakReferenceMessenger.Default.Send(new RequestNumberChanged(InterVmMessage.SelectedNumberFocused));
    }
}
