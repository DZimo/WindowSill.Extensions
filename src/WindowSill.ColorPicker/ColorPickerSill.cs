using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Windows.UI;
using WindowSill.API;
using WindowSill.ColorPicker.Enum;
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

            new SillListViewPopupItem('\xe', null, new SillPopupContent()
                .Background(Color.FromArgb(140, 0, 0, 0))
                .DataContext(_colorPickerVm)
                .Content( new SillOrientedStackPanel()
                           .Children(
                                new StackPanel()
                                    .Spacing(4)
                                    .VerticalAlignment(VerticalAlignment.Center)
                                    .HorizontalAlignment(HorizontalAlignment.Center)
                                    .MinWidth(375)
                                    .Width(375)
                                    .MaxWidth(375)
                                    .Margin(1)
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
                                        .Color(x => x.Binding(() => _colorPickerVm.SelectedColorWinUI).TwoWay()),
                                    new StackPanel()
                                        .Orientation(Orientation.Horizontal)
                                        .HorizontalAlignment(HorizontalAlignment.Center)
                                        .VerticalAlignment(VerticalAlignment.Center)
                                        .Spacing(1)
                                        .Children(
                                                 new Button()
                                                    .Command(_colorPickerVm.CopyColorAnyCommand)
                                                    .CommandParameter(ColorTypes.RGB)
                                                    .Content(
                                                        new StackPanel()
                                                            .Orientation(Orientation.Horizontal)
                                                            .Children(
                                                                new TextBlock()
                                                                .Text("RGB: "),
                                                                new TextBlock()
                                                                .Text(() => _colorPickerVm.SelectedColorWinUI.R, R => $"({R},"),
                                                                new TextBlock()
                                                                .Text(() => _colorPickerVm.SelectedColorWinUI.G, G => $"{G},"),
                                                                new TextBlock()
                                                                .Text(() => _colorPickerVm.SelectedColorWinUI.B, B => $"{B})"))
                                                    ),

                                                new Button()
                                                    .Command(_colorPickerVm.CopyColorAnyCommand)
                                                    .CommandParameter(ColorTypes.HSV)
                                                    .Content(
                                                        new StackPanel()
                                                            .Orientation(Orientation.Horizontal)
                                                            .Children(
                                                                new TextBlock()
                                                                .Text("HSV: "),
                                                                new TextBlock()
                                                                .Text(() => _colorPickerVm.CombinedColor.H, H => $"({H},"),
                                                                new TextBlock()
                                                                .Text(() => _colorPickerVm.CombinedColor.S, S => $"{S},"),
                                                                 new TextBlock()
                                                                .Text(() => _colorPickerVm.CombinedColor.V, V => $"{V})"))
                                                    ),

                                                new Button()
                                                    .Command(_colorPickerVm.CopyColorAnyCommand)
                                                    .CommandParameter(ColorTypes.HSL)
                                                    .Content(
                                                        new StackPanel()
                                                            .Orientation(Orientation.Horizontal)
                                                            .Children(
                                                                new TextBlock()
                                                                .Text("HSL: "),
                                                                new TextBlock()
                                                                .Text(() => _colorPickerVm.CombinedColor.HL, HL => $"({HL},"),
                                                                new TextBlock()
                                                                .Text(() => _colorPickerVm.CombinedColor.SL, SL => $"{SL},"),
                                                                 new TextBlock()
                                                                .Text(() => _colorPickerVm.CombinedColor.SL, L => $"{L})")
                                                            )
                                                    )
                                        )
                                )
                           )
                )).Background(Colors.Transparent).DataContext(_colorPickerVm, (view, vm) => view.Content(
                ColorPickerView.ColorPickerGrid(vm)
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
