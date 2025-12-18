using WindowSill.API;
using Converters = WindowSill.PomodoroTimer.Common.Converters;

namespace WindowSill.PomodoroTimer.UI;

public sealed class PomodoroTimerView : UserControl
{
    private readonly ImageIcon startIcon = new();
    private readonly ImageIcon stopIcon = new();
    private readonly ImageIcon quarterIcon = new();
    private readonly ImageIcon breakIcon = new();
    private readonly ImageIcon workIcon = new();

    public PomodoroTimerView(IPluginInfo pluginInfo, PomodoroTimerVm pomodoroVm)
    {
        var sillFontSize = (Double)Application.Current.Resources["SillFontSize"];

        this.DataContext(
          pomodoroVm,
          (view, vm) => view
          .Content(
              new Grid()
                  .Children(
                      new SillOrientedStackPanel()
                          .VerticalAlignment(VerticalAlignment.Center)
                          .HorizontalAlignment(HorizontalAlignment.Center)
                          .Spacing(1)
                          .Children(
                              new StackPanel()
                                  .Orientation(Orientation.Vertical)
                                  .VerticalAlignment(VerticalAlignment.Center)
                                  .HorizontalAlignment(HorizontalAlignment.Center)
                                  .Padding(1)
                                  .Margin(1)
                                  .Children(
                                      new Button()
                                          .Style(x => x.StaticResource("SillButtonStyle"))
                                          .HorizontalAlignment(HorizontalAlignment.Center)
                                          .Command(() => vm.ChangePomodoroTypeCommand)
                                          .Content(
                                              new StackPanel()
                                              .Orientation(Orientation.Vertical)
                                              .Children(
                                                  new StackPanel()
                                                     .Height(x => x.Binding(() => vm.ProgressHeight).OneWay())
                                                     .Width(x => x.Binding(() => vm.ProgressWidth).OneWay())
                                                     .HorizontalAlignment(HorizontalAlignment.Center)
                                                     .Background(x => x.Binding(() => vm.PomodoroColor).OneWay()),
                                                  new TextBlock()
                                                     .HorizontalAlignment(HorizontalAlignment.Center)
                                                     .Text(x => x.Binding(() => vm.PomodoroType).Converter(Converters.PomodoroTypeConverter))
                                                  )
                                  )),
                              new StackPanel()
                                  .Orientation(Orientation.Horizontal)
                                  .Children(
                                      new Button()
                                          .Style(x => x.StaticResource("IconButton"))
                                          .Command(x => x.Binding(() => vm.StartPomodoroCommand))
                                          .Content("\xE768"),
                                      new Button()
                                          .Style(x => x.StaticResource("IconButton"))
                                          .Command(x => x.Binding(() => vm.StopPomodoroCommand))
                                          .Content("\xE71a")
                                  ),
                               new StackPanel()
                                  .VerticalAlignment(VerticalAlignment.Center)
                                  .HorizontalAlignment(HorizontalAlignment.Center)
                                  .Orientation(Orientation.Horizontal)
                                  .Children(
                                      new Button()
                                     .Style(x => x.StaticResource("SillButtonStyle"))
                                     .Command(() => vm.ChangeTimeDisplayCommand)
                                      .Content(
                                        new TextBlock()
                                          .Text(x => x.Binding(() => vm.TimeLeft).OneWay()))
                                          
                                  )
                              )
                      )
        ));
    }
}
