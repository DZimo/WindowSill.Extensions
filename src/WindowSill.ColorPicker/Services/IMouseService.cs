using Windows.UI;

namespace WindowSill.ColorPicker.Services
{
    public interface IMouseService
    {
        public event EventHandler MouseExited;

        public string GetColorAtCursorNative();

        public void BeginHook();

        public void EndHook();

        public string ColorToHEX(Color rgb);

        public void Dispoese();
    }
}
