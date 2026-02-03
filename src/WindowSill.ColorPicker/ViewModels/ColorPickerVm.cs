using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Helpers;
using System.Drawing;
using Windows.ApplicationModel.DataTransfer;
using WindowSill.API;
using WindowSill.ColorPicker.Enum;
using WindowSill.ColorPicker.Model;
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
    private double selectedColorThickness = 0.5;

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
            if (selectedColorWinUI == value)
                return;

            selectedColorWinUI = value;
            SelectedColorHex = _mouseService.ColorToHEX(selectedColorWinUI);
            SelectedColorHSV = selectedColorWinUI.ToHsv();
            SelectedColorHSL = selectedColorWinUI.ToHsl();

            CombinedColor.H = Math.Round(SelectedColorHSV.H);
            CombinedColor.S = Math.Round(SelectedColorHSV.S);
            CombinedColor.V = Math.Round(SelectedColorHSV.V);
            CombinedColor.A = Math.Round(SelectedColorHSV.A);

            CombinedColor.HL = Math.Round(SelectedColorHSL.H);
            CombinedColor.SL = Math.Round(SelectedColorHSL.S);
            CombinedColor.L = Math.Round(SelectedColorHSL.L);
            CombinedColor.AL = Math.Round(SelectedColorHSL.A);

            OnPropertyChanged(nameof(SelectedColorWinUI));
            OnPropertyChanged(nameof(CombinedColor));
        }
    }

    private CombinedColor combinedColor = new() { H = 156, S = 0, V = 100, HL = 156, SL = 156, L = 100 };

    public CombinedColor CombinedColor
    {
        get => combinedColor;
        set
        {
            if (combinedColor.Equals(value))
                return;

            combinedColor = value;

            OnPropertyChanged(nameof(CombinedColor));
        }
    }

    private HsvColor selectedColorHSV = Colors.White.ToHsv();

    public HsvColor SelectedColorHSV
    {
        get => selectedColorHSV;
        set
        {
            if (selectedColorHSV.Equals(value))
                return;

            selectedColorHSV = value;

            OnPropertyChanged(nameof(SelectedColorHSV));
        }
    }

    private HslColor selectedColorHSL = Colors.White.ToHsl();

    public HslColor SelectedColorHSL
    {
        get => selectedColorHSL;
        set
        {
            if (selectedColorHSL.Equals(value))
                return;

            selectedColorHSL = value;

            OnPropertyChanged(nameof(SelectedColorHSL));
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
            SelectedColorWinUI = new Windows.UI.Color() { R = converted.R, G = converted.G, B = converted.B, A = 255 };
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
    public async Task CopyColorAny(ColorTypes colorTypes)
    {
        var data = new DataPackage();

        switch (colorTypes)
        {
            case ColorTypes.HEX:
                data.SetText(new string(SelectedColorHex));
                break;
            case ColorTypes.RGB:
                data.SetText($"RGB({SelectedColorWinUI.R}, {SelectedColorWinUI.G}, {SelectedColorWinUI.B})");
                break;
            case ColorTypes.HSV:
                data.SetText($"HSV({Math.Round(SelectedColorHSV.H)}, {Math.Round(SelectedColorHSV.S)}%, {Math.Round(SelectedColorHSV.V)}%)");
                break;
            case ColorTypes.HSL:
                data.SetText($"HSL({Math.Round(SelectedColorHSL.H)}, {Math.Round(SelectedColorHSL.S)}%, {Math.Round(SelectedColorHSL.L)}%)");
                break;
        }

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