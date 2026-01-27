using System.ComponentModel.Composition;
using WindowSill.ScreenRecorder.Enums;

namespace WindowSill.ScreenRecorder.Services
{
    [Export(typeof(IRecorderService))]

    public class RecorderService : IRecorderService
    {
        public void CaptureScreenshot(string filePath)
        {
            throw new NotImplementedException();
        }

        public void StartRecording(string filePath, RecordQuality quality)
        {
            throw new NotImplementedException();
        }

        public void StopRecording()
        {
            throw new NotImplementedException();
        }
    }
}
