using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Drawing;
using Windows.ApplicationModel.DataTransfer;
using WindowSill.API;
using WindowSill.ColorPicker.Services;
using Color = Windows.UI.Color;

namespace WindowSill.ColorPicker.UI;

public partial class ColorPickerVm : ObservableObject
{
    private readonly IPluginInfo _pluginInfo;
    private readonly IProcessInteractionService _processInteraction;
    private readonly IMouseService _mouseService;

    [ObservableProperty]
    private SolidColorBrush selectedColorBrush = new SolidColorBrush(Colors.IndianRed);

    [ObservableProperty]
    private int colorFontSize = 12;

    [ObservableProperty]
    private int colorboxHeight = 18;

    private bool exitRequested = true;

    private Color selectedColorWinUI = Colors.White;

    public Color SelectedColorWinUI
    {
        get => selectedColorWinUI;
        set
        {
            selectedColorWinUI = value;
            SelectedColorHex = _mouseService.ColorToHEX(selectedColorWinUI);
            OnPropertyChanged(nameof(SelectedColorWinUI));
        }
    }

    private string selectedColorHex = "#FFFFFF";

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

            try
            {
                newColor = new ColorConverter().ConvertFromString(selectedColorHex);
            }
            catch (Exception ex) { }

            if (newColor is not System.Drawing.Color converted)
                return;

            SelectedColorBrush.Color = new Windows.UI.Color() { R = converted.R, G = converted.G, B = converted.B, A = 255 };
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

        _mouseService.MouseExited += (s, e) =>
        {
            exitRequested = true;
            _mouseService.EndHook();
        };
    }

    public SillView CreateView()
    {
        return new SillView { Content = new ColorPickerView(_pluginInfo, this) };
    }

    [RelayCommand]
    public async Task CopyColorHex()
    {
        exitRequested = !exitRequested;

        var data = new DataPackage();
        data.SetText(new string(SelectedColorHex));
        Clipboard.SetContent(data);
    }

    [RelayCommand]
    public async Task GetColor()
    {
        _mouseService.BeginHook();

        exitRequested = false;
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