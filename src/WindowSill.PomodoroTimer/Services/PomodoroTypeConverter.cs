using WindowSill.PomodoroTimer.Models;
using WindowSill.PomodoroTimer.Services;

namespace WindowSill.PomodoroTimer.Services
{
    public class PomodoroTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not PomodoroType op)
                return "";

            return op switch
            {
                PomodoroType.Short => "25",
                PomodoroType.Long => "50",
            };
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
