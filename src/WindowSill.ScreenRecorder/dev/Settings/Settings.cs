using System.ComponentModel.Composition;
using WindowSill.API;

namespace WindowSill.SimpleCalculator.Settings
{
    [Export(typeof(Settings))]
    internal class Settings
    {
        internal static readonly SettingDefinition<bool> AutoPopup = new(true, typeof(Settings).Assembly);
        internal static readonly SettingDefinition<bool> AutoCopyPaste = new(true, typeof(Settings).Assembly);
    }
}
