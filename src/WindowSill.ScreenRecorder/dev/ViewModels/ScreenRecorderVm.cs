using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WindowSill.API;
using WindowSill.ScreenRecorder.Enums;
using WindowSill.ScreenRecorder.Services;

namespace WindowSill.ScreenRecorder.ViewModels;

public partial class ScreenRecorderVm : ObservableObject
{
    public static ScreenRecorderVm Instance { get; private set; }

    [ObservableProperty]
    private int colorFontSize = 12;

    [ObservableProperty]
    private int colorboxHeight = 18;


    [ObservableProperty]
    private Visibility recordButtonVisible = Visibility.Visible;


    [ObservableProperty]
    private Visibility recordButtonInvisible = Visibility.Collapsed;

    //[ObservableProperty]
    //private ImageIcon iconTest = new ImageIcon('\xE722');

    [ObservableProperty]
    public FontIconSource iconTest = new FontIconSource
    {
        Glyph = "\xE722",
    };

    private string selectedScreenshotPath
    {
        get
        {
            if (_settingsProvider.GetSetting<string>(Settings.Settings.ScreenshotSavePath) == string.Empty)
                _settingsProvider.SetSetting<string>(Settings.Settings.ScreenshotSavePath, (Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)));

            return _settingsProvider.GetSetting<string>(Settings.Settings.ScreenshotSavePath);
        }
    }

    private string selectedVideoPath
    {
        get
        {
            if (_settingsProvider.GetSetting<string>(Settings.Settings.VideoSavePath) == string.Empty)
                _settingsProvider.SetSetting<string>(Settings.Settings.VideoSavePath, (Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)));

            return _settingsProvider.GetSetting<string>(Settings.Settings.VideoSavePath);
        }
    }

    [ObservableProperty]
    private char recordGlyph = '\xE714';

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

    [RelayCommand]
    public async Task Record()
    {
        await ThreadHelper.RunOnUIThreadAsync(() =>
        {
            RecordGlyph = _recorderService.IsRecording ? '\xE714' : '\xE71A';

            RecordButtonVisible = _recorderService.IsRecording ? Visibility.Visible : Visibility.Collapsed;
            RecordButtonInvisible = _recorderService.IsRecording ? Visibility.Collapsed : Visibility.Visible;
        });

        selectedScreenshotName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        _recorderService.StartRecording(System.IO.Path.Combine(selectedVideoPath, selectedScreenshotName), RecordQuality.High);
    }
}