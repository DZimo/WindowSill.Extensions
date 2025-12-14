using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic;
using WindowSill.API;
using WindowSill.SimpleCalculator.Enums;
using WindowSill.SimpleCalculator.Services;

namespace WindowSill.SimpleCalculator.UI;

public partial class SimpleCalculatorVm : ObservableObject
{
    private readonly IPluginInfo _pluginInfo;
    private readonly IProcessInteractionService _processInteraction;
    private readonly ICalculatorService _calculatorService;

    [ObservableProperty]
    private ArithmeticOperator selectedArithmeticOP = ArithmeticOperator.None;

    private ArithmeticOperator lastArithmeticOP = ArithmeticOperator.None;

    [ObservableProperty]
    private float total = 0;

    [ObservableProperty]
    private float x = 0;

    [ObservableProperty]
    private float y = 0;

    public event EventHandler testEvent;

    private bool numberUpdated = true;

    private string selectedNumber = "";
    public string SelectedNumber
    {
        get => selectedNumber;
        set 
        {
            if (selectedNumber == value)
                return;

            selectedNumber = value;
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
        SelectedNumber = "99+";
    }

    public void NumberTextboxChanging()
    {
        char[] buffer = new char[selectedNumber.Length];
        var span = buffer.AsSpan();
        selectedNumber.AsSpan().CopyTo(span);

        var op = _calculatorService.GetArithmeticOperator(span);

        if (op is ArithmeticOperator.None)
            return;

        SelectedArithmeticOP = op;

        X = _calculatorService.GetNumberX(span, _calculatorService.ArithmeticOperatorToString(SelectedArithmeticOP).ToString().AsSpan());

        Total = Total == 0 ? X : SelectedArithmeticOP is ArithmeticOperator.Equal ? _calculatorService.CalculateTotal(X, Total, lastArithmeticOP) : X;

        lastArithmeticOP = op;
        SelectedNumber = Total > 0 && lastArithmeticOP is ArithmeticOperator.Equal ? Total.ToString() : SelectedNumber = "";
    }

    [RelayCommand]
    private void AppendNumberWithOP(char op) => 
        SelectedNumber += op;
}