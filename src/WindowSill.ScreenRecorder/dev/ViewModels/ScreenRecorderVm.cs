using CommunityToolkit.Mvvm.ComponentModel;
using WindowSill.API;
using WindowSill.ScreenRecorder.Services;

namespace WindowSill.ScreenRecorder.ViewModels;

public partial class ScreenRecorderVm : ObservableObject
{
    public static ScreenRecorderVm Instance { get; private set; }

    [ObservableProperty]
    private int colorFontSize = 12;

    [ObservableProperty]
    private int colorboxHeight = 18;

    public ScreenRecorderVm(IRecorderService recorderService)
    {
        Instance = this;
    }

    public Task TestVm()
    {
        return Task.CompletedTask;
    }

    public SillView CreateView()
    {
        return new SillView { Content = new ScreenRecorderView(this) };
    }
}