using WindowSill.API;
using WindowSill.ScreenRecorder.Enums;

namespace WindowSill.ScreenRecorder.Services
{
    public interface IRecorderService
    {
        public bool IsRecording { get; set; }

        public void StartRecording(string filePath, RecordQuality quality);

        public void StopRecording();

        public void CaptureScreenshot(string filePath, ISillListView view);
    }
}
