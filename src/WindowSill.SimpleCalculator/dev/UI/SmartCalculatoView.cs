using Windows.System;
using WindowSill.API;
using WindowSill.SimpleCalculator.ViewModels;

namespace WindowSill.SimpleCalculator.UI;

public sealed class SmartCalculatorView : UserControl
{
    public SmartCalculatorView(SmartCalculatorVm calculatorVm)
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
                                      new TextBlock()
                                          .FontSize(x => x.Binding(() => vm.ColorFontSize).OneWay())
                                          .TextAlignment(TextAlignment.Center)
                                          .FontStretch(Windows.UI.Text.FontStretch.Expanded)
                                          .TextWrapping(TextWrapping.Wrap)
                                          .MinHeight(x => x.Binding(() => vm.ColorboxHeight).OneWay())
                                          .MaxWidth(75)
                                          .Width(75)
                                          .Text(x => x.Binding(() => vm.CalculatedNumber).TwoWay().UpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged))
                                          .Padding(0)
                                  ))
                      )
        ));
    }

    private void OnEnterPressed(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        SimpleCalculatorVm.Instance?.AppendNumberWithOPCommand.Execute('=');
    }

    private void SelectedNumberChanged()
    {
        SimpleCalculatorVm.Instance?.NumberTextboxChanging();
    }

    private void SelectedNumberFocused()
    {
        SimpleCalculatorVm.Instance?.NumberTextboxFocused();
    }
}
