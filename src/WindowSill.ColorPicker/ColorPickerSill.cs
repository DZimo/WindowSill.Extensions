using CommunityToolkit.Diagnostics;
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
public sealed class ColorPickerSill : ISill, ISillSingleView
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

            new SillListViewPopupItem("test", null, null)
        ];

    private async Task OnCommandButtonClickAsync()
    {
        throw new NotImplementedException();
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
