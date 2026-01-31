using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Recognizers.Text;
using NotepadBasedCalculator.Api;
using NotepadBasedCalculator.Core;
using NotepadBasedCalculator.Core.Mef;
using Spectre.Console;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using WindowSill.API;
using WindowSill.SimpleCalculator.Enums;
using WindowSill.SimpleCalculator.Models;
using WindowSill.SimpleCalculator.Services;

namespace WindowSill.SimpleCalculator.UI;

public partial class SimpleCalculatorVm : ObservableObject, IRecipient<RequestNumberChanged>
{
    private readonly ISettingsProvider _settingsProvider;
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

    [ObservableProperty]
    private bool autoPopupOpen;

    [ObservableProperty]
    private bool autoCopyPaste;

    public SillListViewItem test;

    public static SimpleCalculatorVm? Instance;

    public TextDocument textDocumentAPI = new TextDocument();

    private const string DefaultCulture = Culture.English;

    public ParserAndInterpreter parserAndInterpreter;

    public SimpleCalculatorVm(ISettingsProvider settingsProvider, IProcessInteractionService processInteraction, ICalculatorService calculatorService)
    {
        Guard.IsNotNull(settingsProvider, nameof(settingsProvider));
        Guard.IsNotNull(processInteraction, nameof(processInteraction));
        Guard.IsNotNull(calculatorService, nameof(calculatorService));

        _settingsProvider = settingsProvider;
        _processInteraction = processInteraction;
        _calculatorService = calculatorService;
        Instance = this;

        AutoPopupOpen = _settingsProvider.GetSetting<bool>(Settings.Settings.AutoPopup);
        AutoCopyPaste = _settingsProvider.GetSetting<bool>(Settings.Settings.AutoCopyPaste);

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var mefComposer = new MefComposer(new[] { typeof(SimpleCalculatorVm).Assembly });
        ParserAndInterpreterFactory parserAndInterpreterFactory = mefComposer.ExportProvider.GetExport<ParserAndInterpreterFactory>();
        parserAndInterpreter = parserAndInterpreterFactory.CreateInstance(DefaultCulture, textDocumentAPI);
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public SillView CreateView()
    {
        return new SillView { Content = new SimpleCalculatorView(this) };
    }

    [RelayCommand]
    public void ExtendCalculator()
    {
        test.StartBringIntoView();
    }

    private async Task testLine()
    {
        var lastString = SelectedNumber;
        while (true)
        {
            await Task.Delay(1);

            if (lastString == SelectedNumber)
                continue;

            IReadOnlyList<ParserAndInterpreterResultLine>? results =
                     await AnsiConsole
                     .Status()
                     .AutoRefresh(true)
                     .Spinner(Spinner.Known.Dots2)
                     .StartAsync(
                         "Thinking...",
                         async ctx =>
                         {
                             textDocumentAPI.Text = SelectedNumber + Environment.NewLine;
                             return await parserAndInterpreter.WaitAsync();
                         });


            if (results is not null && results.Count > 0 && results[0].SummarizedResultData is not null)
            {
                var result = results[Math.Max(0, results.Count - 2)];
                bool isError = result.SummarizedResultData!.IsOfType(PredefinedTokenAndDataTypeNames.Error);
                var output = result.SummarizedResultData.GetDisplayText(Culture.English);
                if (!string.IsNullOrWhiteSpace(output))
                {
                    if (isError)
                        continue;

                    SelectedNumber = output;
                }
            }
            lastString = SelectedNumber;

            AnsiConsole.WriteLine();
        }

    }
    public async Task NumberTextboxChanging()
    {
        char[] buffer = new char[SelectedNumber.Length];
        var span = buffer.AsSpan();
        SelectedNumber.AsSpan().CopyTo(span);

        var op = _calculatorService.GetArithmeticOperator(span);
        textDocumentAPI.Text = SelectedNumber;

        if (op is ArithmeticOperator.None)
            return;

        SelectedArithmeticOP = op;

        X = _calculatorService.GetNumberX(span, _calculatorService.ArithmeticOperatorToString(SelectedArithmeticOP).ToString().AsSpan());

        var results = await parserAndInterpreter.WaitAsync();

        if (FoundSmartResults(results))
        {
            var result = results[Math.Max(0, results.Count - 2)];
            bool isError = result.SummarizedResultData!.IsOfType(PredefinedTokenAndDataTypeNames.Error);
            var output = result.SummarizedResultData.GetDisplayText(Culture.English);
            if (!string.IsNullOrWhiteSpace(output))
            {
                var cmp = SelectedNumber[..^1];
                if (isError || cmp == output)
                    goto SkipUpdate;

                SelectedNumber = output;
            }
            textDocumentAPI.Text = "";
            return;
        }
    SkipUpdate:
        Total = Total == 0 ? X : SelectedArithmeticOP is ArithmeticOperator.Equal ? _calculatorService.CalculateTotal(X, Total, lastArithmeticOP) : X;

        lastArithmeticOP = op;
        SelectedNumber = Total > 0 && lastArithmeticOP is ArithmeticOperator.Equal ? Total.ToString() : SelectedNumber = "";
    }

    public async Task NumberTextboxFocused()
    {
        if (!AutoCopyPaste || !SelectedNumber.Equals(string.Empty))
            return;

        var copy = await Clipboard.GetContent().GetTextAsync();

        if (copy is null || !double.TryParse(copy, out double parsed))
            return;

        SelectedNumber = copy;
    }

    public bool FoundSmartResults(IReadOnlyList<ParserAndInterpreterResultLine>? results) => (results is not null && results.Count > 0 && results[0].SummarizedResultData is not null);


    [RelayCommand]
    private void AppendNumberWithOP(char op) =>
        SelectedNumber += op;

    public void Receive(RequestNumberChanged message)
    {
        switch (message.VmMessage)
        {
            case InterVmMessage.SelectedNumberChanged:
                _ = NumberTextboxChanging();
                break;
            case InterVmMessage.SelectedNumberFocused:
                _ = NumberTextboxFocused();
                break;
            case InterVmMessage.ExecutedRequested:
                if (!SelectedNumber.Contains('='))
                    AppendNumberWithOPCommand.Execute('=');
                break;
        }
    }
}