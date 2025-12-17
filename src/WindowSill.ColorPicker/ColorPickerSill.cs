using CommunityToolkit.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using WindowSill.API;
using WindowSill.ColorPicker.Services;
using WindowSill.ColorPicker.UI;

namespace WindowSill.ColorPicker;

[Export(typeof(ISill))]
[Name("WindowSill.ColorPicker")]
[Priority(Priority.Lowest)]
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

        View = _colorPickerVm.CreateView();

        //ViewList.Add(View);

        UpdateColorHeight();

        View.IsSillOrientationOrSizeChanged += (o, p) =>
        {
            UpdateColorHeight();
        };
    }

    private void UpdateColorHeight()
    {
        _colorPickerVm?.ColorFontSize = View?.SillOrientationAndSize == SillOrientationAndSize.HorizontalSmall ? 10 : View?.SillOrientationAndSize == SillOrientationAndSize.HorizontalMedium ? 12 : 13;
        _colorPickerVm?.ColorboxHeight = View?.SillOrientationAndSize == SillOrientationAndSize.HorizontalSmall ? 16 : View?.SillOrientationAndSize == SillOrientationAndSize.HorizontalMedium ? 18 : 18;
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
                "/WindowSill.Extension/Misc/CommandTitle".GetLocalizedString(),
                _colorPickerVm.GetColorCommand),

            new SillListViewButtonItem(
                '\xE8c8',
                "/WindowSill.Extension/Misc/CommandTitle".GetLocalizedString(),
                _colorPickerVm.CopyColorHexCommand),

            new SillListViewPopupItem('\xe790', null, null),

            new SillListViewButtonItem().DataContext(_colorPickerVm, (view, vm) => view.Content(
                new Border()
                    .Background(() => _colorPickerVm.SelectedColorBrush)
                    .Child(
                        new SillOrientedStackPanel()
                            .Background(() => _colorPickerVm.SelectedColorBrush)
                            .Children(
                                    new TextBox()
                                          .PlaceholderText("#FFFFFF")
                                          .PlaceholderForeground(Colors.Gray)
                                          .FontSize(x => x.Binding(() => _colorPickerVm.ColorFontSize).OneWay())
                                          .TextAlignment(TextAlignment.Center)
                                          .AcceptsReturn(false)
                                          .FontStretch(Windows.UI.Text.FontStretch.Expanded)
                                          .VerticalContentAlignment(VerticalAlignment.Center)
                                          .TextWrapping(TextWrapping.Wrap)
                                          .MinHeight(x => x.Binding(() => _colorPickerVm.ColorboxHeight).OneWay())
                                          .MaxWidth(75)
                                          .Width(75)
                                          .MaxLength(7)
                                          .Text(x => x.Binding(() => _colorPickerVm.SelectedColorHex).TwoWay())
                                          .Padding(0)
                                          .Margin(0)
                                          .BorderBrush(x => x.Binding(() => _colorPickerVm.SelectedColorBrush).OneWay())))
             )),
        ];

    private async Task OnCommandButtonClickAsync()
    {
        //throw new NotImplementedException();
    }

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
