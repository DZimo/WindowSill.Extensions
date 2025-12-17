using System.ComponentModel.Composition;
using System.Drawing;
using Windows.UI;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using WindowSill.API;
using Color = Windows.UI.Color;

namespace WindowSill.ColorPicker.Services
{
    [Export(typeof(IMouseService))]

    public class MouseService : IMouseService
    {
        private IPluginInfo _pluginInfo;
        public event EventHandler MouseExited;

        private UnhookWindowsHookExSafeHandle _mouseHookHandle;
        private HOOKPROC _mouseDelegate;

        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_MBUTTONDOWN = 0x0207;

        [ImportingConstructor]
        public MouseService(IPluginInfo pluginInfo)
        {
            _pluginInfo = pluginInfo;
            GetMouseEvent();
        }

        private void GetMouseEvent()
        {
            if (_mouseHookHandle != null && !_mouseHookHandle.IsInvalid)
                return;

            _mouseDelegate = MouseHookProc;

            _mouseHookHandle = PInvoke.SetWindowsHookEx(
                WINDOWS_HOOK_ID.WH_MOUSE_LL,
                _mouseDelegate,
                default,
                0);
        }

        private LRESULT MouseHookProc(int nCode, WPARAM wParam, LPARAM lParam)
        {
            if (nCode >= 0)
            {
                uint msg = (uint)wParam;

                if (msg == WM_LBUTTONDOWN || msg == WM_RBUTTONDOWN ||msg == WM_MBUTTONDOWN)
                    MouseExited?.Invoke(this, EventArgs.Empty);
            }

            return PInvoke.CallNextHookEx(default,nCode,new WPARAM((nuint)wParam),new LPARAM((nint)lParam));
        }

        public string GetColorAtCursorNative()
        {
            PInvoke.SetCursor(
                PInvoke.LoadCursor(null, "CROSS")
            );

            var path = _pluginInfo.GetPluginContentDirectory();
            using var hCursor = PInvoke.LoadCursorFromFile(path + "/Assets/colorpicker_cursor.cur");

            PInvoke.SetCursor(hCursor);

            var colorhex = "";

            if (!PInvoke.GetCursorPos(out Point p))
                return colorhex;

            HDC hdc = PInvoke.GetDC(HWND.Null);

            if (hdc == HDC.Null)
                return colorhex;

            uint colorRef = PInvoke.GetPixel(hdc, p.X, p.Y);

            PInvoke.ReleaseDC(HWND.Null, hdc);

            if (colorRef == 0xFFFFFFFF)
                return colorhex;

            var r = (byte)(colorRef & 0xFF);
            var g = (byte)((colorRef >> 8) & 0xFF);
            var b = (byte)((colorRef >> 16) & 0xFF);

            colorhex = $"#{r:X2}{g:X2}{b:X2}";
            return colorhex;
        }

        public string ColorToHEX(Color rgb) => $"#{rgb.R:X2}{rgb.G:X2}{rgb.B:X2}";

        public void Dispoese()
        {
            throw new NotImplementedException();
        }
    }
}
