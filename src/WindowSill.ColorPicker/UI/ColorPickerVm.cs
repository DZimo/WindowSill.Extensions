using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Drawing;
using Windows.ApplicationModel.DataTransfer;
using WindowSill.API;
using WindowSill.ColorPicker.Services;

namespace WindowSill.ColorPicker.UI;

public partial class ColorPickerVm : ObservableObject
{
    private readonly IPluginInfo _pluginInfo;
    private readonly IProcessInteractionService _processInteraction;
    private readonly IMouseService _mouseService;

    [ObservableProperty]
    private SolidColorBrush selectedColorBrush = new SolidColorBrush(Colors.IndianRed);

    private string selectedColorHex = "#FFFFFF";

    private bool exitRequested = true;

    public string SelectedColorHex
    {
        get => selectedColorHex;
        set
        {
            if (value.AsSpan().Length < 1)
                return;

            var res = string.Concat("#", value.AsSpan(1));

            selectedColorHex = res;
            object? newColor = null;

            try {
                newColor = new ColorConverter().ConvertFromString(selectedColorHex);
            }
            catch (Exception ex) { }

            if (newColor is not Color converted)
                return;

            SelectedColorBrush.Color = new Windows.UI.Color() { R = converted.R, G = converted.G, B = converted.B, A = 255  };
            OnPropertyChanged(nameof(SelectedColorHex));
            OnPropertyChanged(nameof(SelectedColorBrush));
        }
    }


    public static ColorPickerVm? Instance;

    public ColorPickerVm(IPluginInfo? pluginInfo, IProcessInteractionService processInteraction, IMouseService mouseService)
    {
        Guard.IsNotNull(pluginInfo, nameof(pluginInfo));
        Guard.IsNotNull(processInteraction, nameof(processInteraction));
        Guard.IsNotNull(mouseService, nameof(mouseService));

        _pluginInfo = pluginInfo;
        _processInteraction = processInteraction;
        _mouseService = mouseService;
        Instance = this;
    }

    public SillView CreateView()
    {
        return new SillView { Content = new ColorPickerView(_pluginInfo, this) };
    }

    [RelayCommand]
    private async Task CopyColorHex()
    {
        exitRequested = !exitRequested;

        var data = new DataPackage();
        data.SetText(new string(SelectedColorHex));
        Clipboard.SetContent(data);
    }

    [RelayCommand]
    private async Task GetColor()
    {
        exitRequested = !exitRequested;

        await Task.Run(async () =>
        {
            while (!exitRequested)
            {
                await Task.Delay(1);

                var hex = _mouseService.GetColorAtCursorNative();

                if (hex is "")
                    continue;

                await ThreadHelper.RunOnUIThreadAsync(() =>
                {
                      SelectedColorHex = hex;
                });
            }
        });
    }
}