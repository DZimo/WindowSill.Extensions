using CommunityToolkit.WinUI.Controls;
using WindowSill.API;

namespace WindowSill.PomodoroTimer.Settings
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
                                .Text("/WindowSill.SimpleCalculator/Misc/General".GetLocalizedString()),
                            new SettingsCard()
                                .Header("/WindowSill.SimpleCalculator/Misc/PopupSettings".GetLocalizedString())
                                .HeaderIcon(
                                    new FontIcon()
                                        .Glyph("\uE8C0"))
                                .Content(
                                    new ToggleSwitch()
                                        .IsOn(
                                            x => x.Binding(() => viewModel.TimeDisplayMode)
                                                  .TwoWay()
                                                  .UpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged)
                                        )
                                )
                        )
                )
            );
        }
    }
}
