using WindowSill.OutlookCalendar.Enums;

namespace WindowSill.OutlookCalendar.Converters
{
    public class AccountBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is AccountType version)
            {
                return (int)version == 1;
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isOn)
            {
                return isOn ? AccountType.Company : AccountType.Personal;
            }
            return AccountType.Personal;
        }
    }
}
