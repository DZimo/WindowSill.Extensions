using Windows.UI;
using WindowSill.SimpleCalculator.Enums;

namespace WindowSill.SimpleCalculator.Services
{
    public interface ICalculatorService
    {
        public bool ContainstArithmeticOperator(ReadOnlySpan<char> inputSpan);
        public ArithmeticOperator GetArithmeticOperator(ReadOnlySpan<char> inputSpan);
        public string ArithmeticOperatorToString(ArithmeticOperator inputSpan);
        public float GetNumberX(Span<char> inputSpan, ReadOnlySpan<char> op);
        public float CalculateTotal(float x, float total, ArithmeticOperator inputOperator);
    }
}
