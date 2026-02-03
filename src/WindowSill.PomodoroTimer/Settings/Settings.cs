using System.ComponentModel.Composition;
using WindowSill.API;
using WindowSill.PomodoroTimer.Models;

namespace WindowSill.PomodoroTimer.Settings
{
    [Export(typeof(Settings))]
    internal class Settings
    {
        internal static readonly SettingDefinition<TimeDisplayMode> DisplayMode = new(TimeDisplayMode.TimeLeft, typeof(Settings).Assembly);
        internal static readonly SettingDefinition<int> ShortBreakDuration = new(5, typeof(Settings).Assembly);
        internal static readonly SettingDefinition<int> LongBreakDuration = new(15, typeof(Settings).Assembly);
    }
}
