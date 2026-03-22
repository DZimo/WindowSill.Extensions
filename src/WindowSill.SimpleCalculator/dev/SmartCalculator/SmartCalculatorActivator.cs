using Microsoft.Recognizers.Text;
using NotepadBasedCalculator.Api;
using NotepadBasedCalculator.Core;
using System.ComponentModel.Composition;
using WindowSill.API;
using WindowSill.SimpleCalculator.UI;
using WindowSill.SimpleCalculator.ViewModels;

namespace WindowSill.SimpleCalculator.SmartCalculator
{
    [Export(typeof(ISillTextSelectionActivator))]
    [ActivationType(ActivatorName, baseName: null)]
    internal sealed class SmartCalculatorActivator : ISillTextSelectionActivator
    {
        internal const string ActivatorName = "SmartCalculator";

        public static SmartCalculatorVm SmartCalculatorVm = new SmartCalculatorVm();

        public async ValueTask<bool> GetShouldBeActivatedAsync(string selectedText, bool isReadOnly, CancellationToken cancellationToken)
        {
            if (SimpleCalculatorVm.Instance is null)
                return false;

            SimpleCalculatorVm.Instance.textDocumentAPI.Text = selectedText;
            var output = string.Empty;
            IReadOnlyList<ParserAndInterpreterResultLine> results;

            try
            {
                results = await SimpleCalculatorVm.Instance.parserAndInterpreter.WaitAsync();

                if (!SimpleCalculatorVm.Instance.FoundSmartResults(results))
                    return false;

                var result = results![Math.Max(0, results.Count - 2)];

                if (result.SummarizedResultData is null)
                    return false;

                bool isError = result.SummarizedResultData.IsOfType(PredefinedTokenAndDataTypeNames.Error);
                output = result.SummarizedResultData.GetDisplayText(Culture.English);
            }
            catch (Exception ex)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(output))
                return false;

            await ThreadHelper.RunOnUIThreadAsync(() =>
            {
                SmartCalculatorVm.CalculatedNumber = output;
            });
            return SimpleCalculatorVm.Instance.FoundSmartResults(results);
        }
    }
}
