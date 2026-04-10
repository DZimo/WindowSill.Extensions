using System.ComponentModel.Composition;
using WindowSill.API;
using WindowSill.OutlookCalendar.Enums;

namespace WindowSill.OutlookCalendar.Settings
{
    [Export(typeof(Settings))]
    public static class Settings
    {
        public static readonly SettingDefinition<OfficeVersion> SelectedOfficeVersion = new(OfficeVersion.Office2016, typeof(Settings).Assembly);

        //public static readonly SettingDefinition<AccountType> SelectedAccountType = new(AccountType.Personal, typeof(Settings).Assembly);
    }
}