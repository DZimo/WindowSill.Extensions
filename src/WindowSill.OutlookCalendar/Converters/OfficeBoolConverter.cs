using WindowSill.OutlookCalendar.Enums;

namespace WindowSill.OutlookCalendar.Converters
{
    public class OfficeBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is OfficeVersion version)
            {
                return (int)version == 1;
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isOn)
            {
                return isOn ? OfficeVersion.OfficeGraphql : OfficeVersion.Office2016;
            }
            return OfficeVersion.Office2016;
        }
    }
}
