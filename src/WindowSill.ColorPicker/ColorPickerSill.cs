using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using WindowSill.API;
using WindowSill.ColorPicker.Services;
using WindowSill.ColorPicker.UI;
using Picker = Microsoft.UI.Xaml.Controls;

namespace WindowSill.ColorPicker;

[Export(typeof(ISill))]
[Name("WindowSill.ColorPicker")]
[Priority(Priority.Lowest)]
[HideIconInSillListView]
public sealed class ColorPickerSill : ISill, ISillListView
{
    private ColorPickerVm _colorPickerVm;
    private IPluginInfo _pluginInfo;
    private IProcessInteractionService _processInteraction;

    public SillView? View { get; private set; }

    [ImportingConstructor]
    public ColorPickerSill(IPluginInfo pluginInfo, IProcessInteractionService processInteraction, IMouseService mouseService)
    {
        _pluginInfo = pluginInfo;
        _processInteraction = processInteraction;
        _colorPickerVm = new ColorPickerVm(pluginInfo, processInteraction, mouseService);

        UpdateColorHeight();

        ViewList[0].IsSillOrientationOrSizeChanged += (o, p) =>
        {
            UpdateColorHeight();
        };
    }

    private void UpdateColorHeight()
    {
        var isSmall = ViewList[0]?.SillOrientationAndSize == SillOrientationAndSize.HorizontalSmall;
        var isMedium = ViewList[0]?.SillOrientationAndSize == SillOrientationAndSize.HorizontalMedium;

        _colorPickerVm?.ColorFontSize = isSmall ? 10 : isMedium ? 12 : 13;
        _colorPickerVm?.ColorboxHeight = isSmall ? 16 : isMedium ? 18 : 18;
        _colorPickerVm?.SelectedColorThickness = isSmall ? 0 : isMedium ? 0.5 : 1;
    }

    public string DisplayName => "/WindowSill.ColorPicker/Misc/DisplayName".GetLocalizedString();

    public IconElement CreateIcon()
         => new ImageIcon
         {
             Source = new SvgImageSource(new Uri(System.IO.Path.Combine(_pluginInfo.GetPluginContentDirectory(), "Assets", "colorpicker_logo.svg")))
         };

    public SillView? PlaceholderView => null;

    public SillSettingsView[]? SettingsViews => null;

    public ObservableCollection<SillListViewItem> ViewList
        => [
            new SillListViewButtonItem(
                '\xEf3c',
                new TextBlock().Margin(5).Text("/WindowSill.ColorPicker/Misc/GrabColor".GetLocalizedString()),
                _colorPickerVm.GetColor),

            new SillListViewButtonItem(
                '\xE8c8',
                new TextBlock().Margin(5).Text("/WindowSill.ColorPicker/Misc/CopyColor".GetLocalizedString()),
                _colorPickerVm.CopyColorHex),

            new SillListViewPopupItem('\xe', null, new SillPopupContent().ToolTipService(toolTip:  "/WindowSill.ColorPicker/Misc/CommandTitle".GetLocalizedString()).DataContext(_colorPickerVm).Content( new SillOrientedStackPanel()
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
                                        .Color(x => x.Binding(() => _colorPickerVm.SelectedColorWinUI).TwoWay())
                                    ))))
                    .Background(Colors.Transparent)
                    .DataContext(_colorPickerVm, (view, vm) => view.Content(
                new Grid()
                    .Children(
                        new SillOrientedStackPanel()
                            .Children(
                                  new StackPanel()
                                      .Orientation(Orientation.Horizontal)
                                      .Spacing(1)
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
                                              .Content("\xe790")
                            )
                    )
             )),
        ];

    public ValueTask OnActivatedAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask OnDeactivatedAsync()
    {
        View = null;
        _colorPickerVm = null;

        return ValueTask.CompletedTask;
    }
}
