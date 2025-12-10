using System.ComponentModel.Composition;
using System.Drawing;
using Windows.UI;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;

namespace WindowSill.ColorPicker.Services
{
    [Export(typeof(IMouseService))]

    public class MouseService : IMouseService
    {
        public Windows.UI.Color CurrentColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Windows.UI.Color IMouseService.CurrentColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler Exited;

        [ImportingConstructor]
        public MouseService()
        {

        }
        public string GetColorAtCursorNative()
        {
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

        public void ShowColorNative()
        {
            throw new NotImplementedException();
        }
        public void Dispoese()
        {
            throw new NotImplementedException();
        }
    }
}
