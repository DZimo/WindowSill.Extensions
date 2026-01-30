using CommunityToolkit.WinUI.Controls;
using WindowSill.API;
using WindowSill.PomodoroTimer.Models;

namespace WindowSill.PomodoroTimer.Settings
{
    internal class SettingsView : UserControl
    {
        private string elapsedText = "/WindowSill.PomodoroTimer/Misc/ElapsedText".GetLocalizedString();
        private string remainingText = "/WindowSill.PomodoroTimer/Misc/RemainingText".GetLocalizedString();

        public SettingsView(ISettingsProvider settingsProvider, SettingsVm settingsVm)
        {
            this.DataContext(
                settingsVm,
                (view, viewModel) => view
                .Content(
                    new StackPanel()
                        .Spacing(2)
                        .Children(
                            new TextBlock()
                                .Style(x => x.ThemeResource("BodyStrongTextBlockStyle"))
                                .Margin(0, 0, 0, 8)
                                .Text("/WindowSill.PomodoroTimer/Misc/General".GetLocalizedString()),
                            new SettingsCard()
                                .Header("/WindowSill.PomodoroTimer/Misc/DisplayMode".GetLocalizedString())
                                .Description("/WindowSill.PomodoroTimer/Misc/DisplayModeDesc".GetLocalizedString())
                                .Tag("test")
                                .HeaderIcon(
                                    new FontIcon()
                                        .Glyph("\uECC5"))
                                .Content(
                              new ComboBox()
                                  .ItemsSource(new[] { remainingText, elapsedText })
                                  .SelectedItem(
                                        x => x.Binding(() => viewModel.TimeDisplayMode)
                                              .TwoWay()
                                              .UpdateSourceTrigger(UpdateSourceTrigger.Default)
                                              .Convert(o => o switch
                                              {
                                                  TimeDisplayMode.TimeLeft => remainingText,
                                                  TimeDisplayMode.TimeElapsed => elapsedText,
                                                  _ => ""
                                              })
                                            .ConvertBack(o =>
                                            {
                                                if (o is not string res)
                                                    return TimeDisplayMode.TimeLeft;

                                                return res.Equals(remainingText) ? TimeDisplayMode.TimeLeft : TimeDisplayMode.TimeElapsed;
                                            })
                                              )
                                   )
                                )
                )
            );
        }
    }
}
