using CommunityToolkit.Diagnostics;

using Microsoft.UI.Xaml.Media.Imaging;
using Windows.UI;
using WindowSill.API;

namespace WindowSill.PerfCounter.UI;

public sealed class PomodoroTimerView : Button
{
    private int _pomodoroType = 25;

    private readonly ImageIcon startIcon = new();
    private readonly ImageIcon stopIcon = new();
    private readonly ImageIcon quarterIcon = new();
    private readonly ImageIcon breakIcon = new();
    private readonly ImageIcon workIcon = new();

    public PomodoroTimerView(IPluginInfo pluginInfo, PomodoroTimerVm pomodoroVm)
    {
        //var sillFontSize = (Double)Application.Current.Resources["SillFontSize"];

        this.DataContext(
          pomodoroVm,
          (view, vm) => view
          .Style(x => x.StaticResource("SillButtonStyle"))
          .Height(double.NaN)
          .Background(new SolidColorBrush(Colors.Transparent))
          .Content(
              new Grid()
                  .Children(
                      new SillOrientedStackPanel()
                          .VerticalAlignment(VerticalAlignment.Center)
                          .Spacing(8)
                          .Children(
                              new StackPanel()
                                  .Orientation(Orientation.Vertical)
                                  .Spacing(4)
                                  .Children(
                                      new Button()
                                          .Style(x => x.StaticResource("SillButtonStyle"))
                                          .Content(
                                              new StackPanel()
                                              .Orientation(Orientation.Vertical)
                                              .Children(
                                                  new StackPanel()
                                                     .Height(5)
                                                     .Background(new SolidColorBrush(Colors.Aqua)),
                                                  new TextBlock()
                                                     .FontSize(x => x.ThemeResource("SillFontSize"))
                                                     .Text(_pomodoroType.ToString())
                                                  )
                                  )),
                              new StackPanel()
                                  .VerticalAlignment(VerticalAlignment.Center)
                                  .Orientation(Orientation.Horizontal)
                                  .Spacing(4)
                                  .Children(
                                      new Button()
                                          .Style(x => x.StaticResource("IconButton"))
                                          .Content("\xE768"),
                                      new Button()
                                          .Style(x => x.StaticResource("IconButton"))
                                          .Content("\xE71a")
                                  ),
                               new StackPanel()
                                  .VerticalAlignment(VerticalAlignment.Center)
                                  .Orientation(Orientation.Horizontal)
                                  .Spacing(1)
                                  .Children(
                                      new TextBlock()
                                          .Text(x => x.Binding(() => vm.TimeLeft).OneWay())
                                          
                                  )
                              )
                      )
        ));
    }
}
