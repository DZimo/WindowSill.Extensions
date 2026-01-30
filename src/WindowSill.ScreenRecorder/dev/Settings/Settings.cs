using System.ComponentModel.Composition;
using WindowSill.API;

namespace WindowSill.ScreenRecorder.Settings
{
    [Export(typeof(Settings))]
    internal class Settings
    {
        internal static readonly SettingDefinition<string> ScreenshotSavePath = new(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), typeof(Settings).Assembly);
    }
}
