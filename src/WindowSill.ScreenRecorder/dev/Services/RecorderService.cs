using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.Storage.FileSystem;
using Windows.Win32.System.Memory;
using Windows.Win32.UI.WindowsAndMessaging;
using WindowSill.API;
using WindowSill.ScreenRecorder.Enums;

namespace WindowSill.ScreenRecorder.Services
{
    [Export(typeof(IRecorderService))]

    public class RecorderService : IRecorderService
    {
        public void CaptureScreenshot(string filePath, ISillListView view)
        {
            unsafe
            {
                const uint GENERIC_WRITE = 0x40000000;
                HDC hdcScreen = default;
                HDC hdcWindow = default;
                HDC hdcMemDC = default;
                HBITMAP hbmScreen = default;
                SafeHandle hFile = default;
                BITMAP bmpScreen;
                HGLOBAL hDIB = default;
                char* lpbitmap = null;
                HWND hWnd = default;
                uint dwBytesWritten = 0;

                try
                {
                    object window = null;
                    ThreadHelper.RunOnUIThreadAsync(() =>
                    {
                        //DependencyInjection.WindowService.
                        var tst = view.ViewList[0];
                        DependencyObject? current = tst;
                        while (current != null && !(current is Window))
                        {
                            current = VisualTreeHelper.GetParent(current);
                        }

                        var window = current;
                        if (window == null)
                            return;
                    });

                    hWnd = PInvoke.GetActiveWindow();

                    hdcScreen = PInvoke.GetDC(HWND.Null);
                    hdcWindow = PInvoke.GetDC(hWnd);

                    hdcMemDC = PInvoke.CreateCompatibleDC(hdcWindow);

                    if (hdcMemDC.IsNull)
                        throw new InvalidOperationException("CreateCompatibleDC failed");

                    RECT rcClient;
                    PInvoke.GetClientRect(hWnd, out rcClient);

                    int width = rcClient.right - rcClient.left;
                    int height = rcClient.bottom - rcClient.top;

                    PInvoke.SetStretchBltMode(hdcWindow, STRETCH_BLT_MODE.HALFTONE);

                    PInvoke.StretchBlt(
                        hdcWindow,
                        0, 0,
                        width, height,
                        hdcScreen,
                        0, 0,
                        PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXSCREEN),
                        PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYSCREEN),
                        ROP_CODE.SRCCOPY);

                    hbmScreen = PInvoke.CreateCompatibleBitmap(hdcWindow, width, height);
                    if (hbmScreen.IsNull)
                        throw new InvalidOperationException("CreateCompatibleBitmap failed");

                    PInvoke.SelectObject(hdcMemDC, hbmScreen);

                    PInvoke.BitBlt(
                        hdcMemDC,
                        0, 0,
                        width, height,
                        hdcWindow,
                        0, 0,
                        ROP_CODE.SRCCOPY);

                    PInvoke.GetObject(hbmScreen, sizeof(BITMAP), &bmpScreen);

                    BITMAPINFOHEADER bi = new BITMAPINFOHEADER
                    {
                        biSize = (uint)Marshal.SizeOf<BITMAPINFOHEADER>(),
                        biWidth = bmpScreen.bmWidth,
                        biHeight = bmpScreen.bmHeight,
                        biPlanes = 1,
                        biBitCount = 32,
                        biCompression = (uint)BI_COMPRESSION.BI_RGB
                    };

                    int dwBmpSize = ((bmpScreen.bmWidth * bi.biBitCount + 31) / 32) * 4 * bmpScreen.bmHeight;
                    //byte[] pixels = new byte[dwBmpSize];

                    hDIB = PInvoke.GlobalAlloc(GLOBAL_ALLOC_FLAGS.GHND, (nuint)dwBmpSize);
                    lpbitmap = (char*)PInvoke.GlobalLock(hDIB);

                    PInvoke.GetDIBits(
                        hdcWindow,
                        hbmScreen,
                        0,
                        (uint)bmpScreen.bmHeight,
                        lpbitmap,
                        (BITMAPINFO*)&bi,
                        DIB_USAGE.DIB_RGB_COLORS);

                    hFile = PInvoke.CreateFile(
                        filePath,
                        GENERIC_WRITE,
                        FILE_SHARE_MODE.FILE_SHARE_NONE,
                        null,
                        FILE_CREATION_DISPOSITION.CREATE_ALWAYS,
                        FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL,
                        default);

                    var dwSizeofDIB = dwBmpSize + Marshal.SizeOf<BITMAPFILEHEADER>() + Marshal.SizeOf<BITMAPINFOHEADER>();

                    BITMAPFILEHEADER bmfHeader = new BITMAPFILEHEADER
                    {
                        bfType = 0x4D42, // BM
                        bfOffBits = (uint)(Marshal.SizeOf<BITMAPFILEHEADER>() + Marshal.SizeOf<BITMAPINFOHEADER>()),
                        bfSize = (uint)(dwBmpSize + Marshal.SizeOf<BITMAPFILEHEADER>() + Marshal.SizeOf<BITMAPINFOHEADER>())
                    };

                    var unsafeHandle = (HANDLE)hFile.DangerousGetHandle();

                    PInvoke.WriteFile(unsafeHandle, (byte*)&bmfHeader, (uint)sizeof(BITMAPFILEHEADER), &dwBytesWritten, null);
                    PInvoke.WriteFile(unsafeHandle, (byte*)&bi, (uint)sizeof(BITMAPINFOHEADER), &dwBytesWritten, null);
                    PInvoke.WriteFile(unsafeHandle, (byte*)lpbitmap, (uint)dwBmpSize, &dwBytesWritten, null);

                    PInvoke.GlobalUnlock(hDIB);
                    PInvoke.GlobalFree(hDIB);
                }
                finally
                {
                    if (!hbmScreen.IsNull) PInvoke.DeleteObject(hbmScreen);
                    if (!hdcMemDC.IsNull) PInvoke.DeleteDC(hdcMemDC);
                    if (!hdcScreen.IsNull) PInvoke.ReleaseDC(HWND.Null, hdcScreen);
                    if (!hdcWindow.IsNull) PInvoke.ReleaseDC(hWnd, hdcWindow);
                }
            }
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
