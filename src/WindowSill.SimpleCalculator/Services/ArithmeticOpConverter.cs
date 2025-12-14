using WindowSill.SimpleCalculator.Enums;

namespace WindowSill.SimpleCalculator.Services
{
    public class ArithmeticOpConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not ArithmeticOperator op)
                return "";

            return op switch
            {
                ArithmeticOperator.Plus => "+",
                ArithmeticOperator.Minus => "-",
                ArithmeticOperator.Multiply => "*",
                ArithmeticOperator.Divide => "/",
                ArithmeticOperator.Equal => "=",
                ArithmeticOperator.None => " ",
                _ => ""
            };
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
