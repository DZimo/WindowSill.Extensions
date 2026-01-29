using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    private IRecorderService _recorderService;
    private ISillListView _view;

    public ScreenRecorderVm(IRecorderService recorderService, ISillListView view)
    {
        Instance = this;
        _recorderService = recorderService;
        _view = view;
    }

    public Task TestVm()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    public Task Capture()
    {
        _recorderService.CaptureScreenshot("C:\\KHRA\\screenshot.png", _view);
        return Task.CompletedTask;
    }

    //public SillView CreateView()
    //{
    //    return new SillView { Content = new ScreenRecorderView(this) };
    //}
}