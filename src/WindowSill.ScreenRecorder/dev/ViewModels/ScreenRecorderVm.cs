using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WindowSill.API;
using WindowSill.ScreenRecorder.Enums;
using WindowSill.ScreenRecorder.Services;
using Timer = System.Timers.Timer;

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

    [ObservableProperty]
    private string videoTimeElapsed = string.Empty;

    private Timer recordTimer = new();

    private int elapsedSeconds = 0;

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
        recordTimer.Interval = 1000;
        recordTimer.Start();
        recordTimer.Elapsed += RecordTimer_Elapsed;
    }

    private async void RecordTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        await ThreadHelper.RunOnUIThreadAsync(() =>
        {
            VideoTimeElapsed = $"{(elapsedSeconds / 60):D2}:{(elapsedSeconds % 60):D2}";
        });
        elapsedSeconds++;
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
        if (!_recorderService.IsRecording)
            elapsedSeconds = 0;

        await ThreadHelper.RunOnUIThreadAsync(() =>
        {
            RecordGlyph = _recorderService.IsRecording ? '\xE714' : '\xE71A';

            RecordButtonVisible = _recorderService.IsRecording ? Visibility.Visible : Visibility.Collapsed;
            RecordButtonInvisible = _recorderService.IsRecording ? Visibility.Collapsed : Visibility.Visible;

            VideoTimeElapsed = $"(remaining % 60)".ToString();

        });

        selectedScreenshotName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        _recorderService.StartRecording(System.IO.Path.Combine(selectedVideoPath, selectedScreenshotName), RecordQuality.High);
    }
}