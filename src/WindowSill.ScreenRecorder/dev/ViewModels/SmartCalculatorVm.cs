using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Recognizers.Text;
using NotepadBasedCalculator.Core;
using WindowSill.API;
using WindowSill.SimpleCalculator.UI;

namespace WindowSill.SimpleCalculator.ViewModels;

public partial class SmartCalculatorVm : ObservableObject
{
    public static SmartCalculatorVm Instance { get; private set; }

    [ObservableProperty]
    private int colorFontSize = 12;

    [ObservableProperty]
    private int colorboxHeight = 18;

    private string calculatedNumber = "";
    public string CalculatedNumber
    {
        get => calculatedNumber;
        set
        {
            if (calculatedNumber == value)
                return;

            calculatedNumber = value;
            OnPropertyChanged(nameof(calculatedNumber));
        }
    }

    public TextDocument textDocumentAPI = new TextDocument();

    private const string DefaultCulture = Culture.English;

    private ParserAndInterpreter parserAndInterpreter;

    public SmartCalculatorVm()
    {
        Instance = this;
    }

    public SillView CreateView()
    {
        return new SillView { Content = new SmartCalculatorView(this) };
    }
}