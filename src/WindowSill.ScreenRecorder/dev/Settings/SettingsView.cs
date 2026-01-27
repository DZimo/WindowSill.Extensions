using CommunityToolkit.WinUI.Controls;
using WindowSill.API;

namespace WindowSill.SimpleCalculator.Settings
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
                                            x => x.Binding(() => viewModel.IsAutoOpen)
                                                  .TwoWay()
                                                  .UpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged)
                                        )
                                ),
                             new SettingsCard()
                                .Header("/WindowSill.SimpleCalculator/Misc/CopySettings".GetLocalizedString())
                                .HeaderIcon(
                                    new FontIcon()
                                        .Glyph("\uE8C8"))
                                .Content(
                                    new ToggleSwitch()
                                        .IsOn(
                                            x => x.Binding(() => viewModel.IsAutoPaste)
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
