using Microsoft.UI.Xaml.Media.Imaging;
using System.ComponentModel.Composition;
using WindowSill.API;
using WindowSill.SimpleCalculator.Services;
using WindowSill.SimpleCalculator.UI;

namespace WindowSill.SimpleCalculator;

[Export(typeof(ISill))]
[Name("WindowSill.SimpleCalculator")]
[Priority(Priority.Lowest)]
public sealed class SimpleCalculatorSill : ISill, ISillSingleView
{
    private SimpleCalculatorVm _simpleCalculatorVm;
    private IPluginInfo _pluginInfo;
    private IProcessInteractionService _processInteraction;

    public SillView? View { get; private set; }

    [ImportingConstructor]
    public SimpleCalculatorSill(IPluginInfo pluginInfo, IProcessInteractionService processInteraction, ICalculatorService calculatorService)
    {
        _pluginInfo = pluginInfo;
        _processInteraction = processInteraction;
        _simpleCalculatorVm = new SimpleCalculatorVm(pluginInfo, processInteraction, calculatorService);

        View = _simpleCalculatorVm.CreateView();
        UpdateColorHeight();

        View.IsSillOrientationOrSizeChanged += (o, p) =>
        {
            UpdateColorHeight();
        };
    }

    private void UpdateColorHeight()
    {
        _simpleCalculatorVm?.ColorFontSize = View?.SillOrientationAndSize == SillOrientationAndSize.HorizontalSmall ? 10 : View?.SillOrientationAndSize == SillOrientationAndSize.HorizontalMedium ? 12 : 13;
        _simpleCalculatorVm?.ColorboxHeight = View?.SillOrientationAndSize == SillOrientationAndSize.HorizontalSmall ? 16 : View?.SillOrientationAndSize == SillOrientationAndSize.HorizontalMedium ? 18 : 18;
    }

    public string DisplayName => "/WindowSill.ColorPicker/Misc/DisplayName".GetLocalizedString();

    public IconElement CreateIcon()
         => new ImageIcon
         {
             Source = new SvgImageSource(new Uri(System.IO.Path.Combine(_pluginInfo.GetPluginContentDirectory(), "Assets", "colorpicker_logo.svg")))
         };

    public SillView? PlaceholderView => null;

    public SillSettingsView[]? SettingsViews => null;

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
        _simpleCalculatorVm = null;

        return ValueTask.CompletedTask;
    }
}
