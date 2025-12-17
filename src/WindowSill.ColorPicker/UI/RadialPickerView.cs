using WindowSill.API;
using Picker = Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Text;

namespace WindowSill.ColorPicker.UI
{
    public sealed class RadialPickerView : SillPopupContent
    {
        public RadialPickerView(ColorPickerVm colorPickerVm) 
        {
            this.DataContext(
                colorPickerVm,
                (view, vm) => view
                   .Content(
                       new Grid()
                           .Children(
                                new StackPanel()
                                .Spacing(4)
                                .VerticalAlignment(VerticalAlignment.Center)
                                .HorizontalAlignment(HorizontalAlignment.Center)
                                .Margin(5)
                                .Children(
                                    new TextBlock()
                                        .VerticalAlignment(VerticalAlignment.Center)
                                        .HorizontalAlignment(HorizontalAlignment.Center)
                                        .FontWeight(FontWeights.Bold)
                                        .Text("Color Picker"),
                                    new Picker.ColorPicker()
                                        .HorizontalContentAlignment(HorizontalAlignment.Center)
                                        .VerticalAlignment(VerticalAlignment.Center)
                                        .HorizontalAlignment(HorizontalAlignment.Center)
                                        .Margin(5)
                                        .IsColorPreviewVisible(true)
                                        .IsColorChannelTextInputVisible(false)
                                        .IsHexInputVisible(false)
                                        .ColorSpectrumShape(ColorSpectrumShape.Ring)
                                        .Color(x => x.Binding(() => vm.SelectedColorWinUI).TwoWay())
                                    ))));
            }
    }
}
