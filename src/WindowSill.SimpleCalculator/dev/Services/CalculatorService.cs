using System.ComponentModel.Composition;
using WindowSill.SimpleCalculator.Enums;

namespace WindowSill.SimpleCalculator.Services
{
    [Export(typeof(ICalculatorService))]

    public class CalculatorService : ICalculatorService
    {
        public string ArithmeticOperatorToString(ArithmeticOperator inputSpan)
        {
            switch (inputSpan)
            {
                case ArithmeticOperator.Plus:
                    return "+";
                case ArithmeticOperator.Minus:
                    return "-";
                case ArithmeticOperator.Divide:
                    return "/";
                case ArithmeticOperator.Multiply:
                    return "*";
                case ArithmeticOperator.Equal:
                    return "=";
                case ArithmeticOperator.None:
                    return "";
                default:
                    return "";
            }
        }

        public float CalculateTotal(float x, float total, ArithmeticOperator inputOperator)
        {
            switch (inputOperator)
            {
                case ArithmeticOperator.Plus:
                    return total + x;
                case ArithmeticOperator.Minus:
                    return total - x;
                case ArithmeticOperator.Divide:
                    return total / x;
                case ArithmeticOperator.Multiply:
                    return total * x;
                case ArithmeticOperator.None:
                    return total;
                default:
                    return total;
            }
        }

        public bool ContainstArithmeticOperator(ReadOnlySpan<char> inputSpan)
        {
            return inputSpan.Contains('+') || inputSpan.Contains('-') || inputSpan.Contains('*') || inputSpan.Contains('/');
        }

        public ArithmeticOperator GetArithmeticOperator(ReadOnlySpan<char> inputSpan)
        {
            switch (inputSpan)
            {
                case var span when span.Contains('+'):
                    return ArithmeticOperator.Plus;
                case var span when span.Contains('-'):
                    return ArithmeticOperator.Minus;
                case var span when span.Contains('/'):
                    return ArithmeticOperator.Divide;
                case var span when span.Contains('*'):
                    return ArithmeticOperator.Multiply;
                case var span when span.Contains('='):
                    return ArithmeticOperator.Equal;
                default:
                    return ArithmeticOperator.None;
            }
        }

        public float GetNumberX(Span<char> inputSpan, ReadOnlySpan<char> op)
        {
            inputSpan.Replace<char>(',', '.');
            inputSpan.Replace(op.ToArray().First(), ' ');
            float.TryParse(inputSpan.ToString(), out float result);
            return result;
        }
    }
}
