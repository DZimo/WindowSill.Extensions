using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WindowSill.API;
using WindowSill.SimpleCalculator.Enums;
using WindowSill.SimpleCalculator.Services;

namespace WindowSill.SimpleCalculator.UI;

public partial class SimpleCalculatorVm : ObservableObject
{
    private readonly IPluginInfo _pluginInfo;
    private readonly IProcessInteractionService _processInteraction;
    private readonly ICalculatorService _calculatorService;

    //[ObservableProperty]
    //private string selectedArithmeticOP = "";

    [ObservableProperty]
    private ArithmeticOperator selectedArithmeticOP;

    private string selectedNumber;
    public string SelectedNumber
    {
        get => selectedNumber;
        set 
        { 
            selectedNumber = value;
            var span = selectedNumber.AsSpan();
            SelectedArithmeticOP = _calculatorService.GetArithmeticOperator(span);
            OnPropertyChanged(nameof(SelectedNumber));
        }
    }


    [ObservableProperty]
    private int colorFontSize = 12;

    [ObservableProperty]
    private int colorboxHeight = 18;

    public static SimpleCalculatorVm? Instance;

    public SimpleCalculatorVm(IPluginInfo? pluginInfo, IProcessInteractionService processInteraction, ICalculatorService calculatorService)
    {
        Guard.IsNotNull(pluginInfo, nameof(pluginInfo));
        Guard.IsNotNull(processInteraction, nameof(processInteraction));
        Guard.IsNotNull(calculatorService, nameof(calculatorService));

        _pluginInfo = pluginInfo;
        _processInteraction = processInteraction;
        _calculatorService = calculatorService;
        Instance = this;
    }

    public SillView CreateView()
    {
        return new SillView { Content = new SimpleCalculatorView(_pluginInfo, this) };
    }

    [RelayCommand]
    public void ExtendCalculator()
    {
    }
}