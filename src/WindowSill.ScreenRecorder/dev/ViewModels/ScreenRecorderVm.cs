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

    private string selectedScreenshotPath
    {
        get
        {
            if (_settingsProvider.GetSetting<string>(Settings.Settings.ScreenshotSavePath) == string.Empty)
                _settingsProvider.SetSetting<string>(Settings.Settings.ScreenshotSavePath, (Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)));

            return _settingsProvider.GetSetting<string>(Settings.Settings.ScreenshotSavePath);
        }
    }

    private string selectedScreenshotName = string.Empty;

    private IRecorderService _recorderService;
    private ISillListView _view;
    private ISettingsProvider _settingsProvider;

    public ScreenRecorderVm(IRecorderService recorderService, ISillListView view, ISettingsProvider settingsProvider)
    {
        Instance = this;
        _recorderService = recorderService;
        _view = view;

        _settingsProvider = settingsProvider;
    }

    public Task TestVm()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    public Task Capture()
    {
        selectedScreenshotName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        _recorderService.CaptureScreenshot(System.IO.Path.Combine(selectedScreenshotPath, selectedScreenshotName), _view);
        return Task.CompletedTask;
    }

    //public SillView CreateView()
    //{
    //    return new SillView { Content = new ScreenRecorderView(this) };
    //}
}