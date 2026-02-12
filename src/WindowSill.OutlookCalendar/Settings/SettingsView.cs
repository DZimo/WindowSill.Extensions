using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml.Controls;
using WindowSill.API;

namespace WindowSill.ScreenRecorder.Settings
{
    internal class SettingsView : UserControl
    {
        public SettingsView(ISettingsProvider settingsProvider)
        {
            this.DataContext(
                new SettingsVm(settingsProvider),
                (view, viewModel) => view
                .Content(
                    new StackPanel()
                        .Spacing(2)
                        .Children(
                            new TextBlock()
                                .Style(x => x.ThemeResource("BodyStrongTextBlockStyle"))
                                .Margin(0, 0, 0, 8)
                                .Text("/WindowSill.ScreenRecorder/Misc/General".GetLocalizedString()),
                            new SettingsCard()
                                .Header("/WindowSill.ScreenRecorder/Misc/ScreenShotPath".GetLocalizedString())
                                .Description("/WindowSill.ScreenRecorder/Misc/ScreenShotPathDesc".GetLocalizedString())
                                .Tag("test")
                                .HeaderIcon(
                                    new FontIcon()
                                        .Glyph("\uECC5"))
                                .Content(
                                    new TextBox()
                                        .Text(x => x.Binding(() => viewModel.ScreenshotSavePath).TwoWay().UpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged))
                                ),
                           new SettingsCard()
                                .Header("/WindowSill.ScreenRecorder/Misc/VideoPath".GetLocalizedString())
                                .Description("/WindowSill.ScreenRecorder/Misc/VideoPathDesc".GetLocalizedString())
                                .Tag("test")
                                .HeaderIcon(
                                    new FontIcon()
                                        .Glyph("\uECC5"))
                                .Content(
                                    new TextBox()
                                        .Text(x => x.Binding(() => viewModel.VideoSavePath).TwoWay().UpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged))
                                )
                )));
        }
    }
}
