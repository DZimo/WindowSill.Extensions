using WindowSill.API;
using WindowSill.OutlookCalendar.Models;

namespace WindowSill.OutlookCalendar.Settings
{

    internal static class Settings
    {
        internal static readonly SettingDefinition<OfficeVersion> SelectedOfficeVersion = new(OfficeVersion.Office2016, typeof(Settings).Assembly);
    }
}