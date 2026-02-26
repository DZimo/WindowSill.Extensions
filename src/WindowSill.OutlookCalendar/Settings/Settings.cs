using System.ComponentModel.Composition;
using WindowSill.API;
using WindowSill.OutlookCalendar.Enums;

namespace WindowSill.OutlookCalendar.Settings
{
    [Export(typeof(Settings))]
    internal static class Settings
    {
        internal static readonly SettingDefinition<OfficeVersion> SelectedOfficeVersion = new(OfficeVersion.Office2016, typeof(Settings).Assembly);

        internal static readonly SettingDefinition<AccountType> SelectedAccountType = new(AccountType.Personal, typeof(Settings).Assembly);
    }
}