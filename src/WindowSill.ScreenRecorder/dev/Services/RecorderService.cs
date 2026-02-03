using ScreenRecorderLib;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.Storage.FileSystem;
using Windows.Win32.System.Memory;
using WindowSill.API;
using WindowSill.ScreenRecorder.Enums;

namespace WindowSill.ScreenRecorder.Services
{
    [Export(typeof(IRecorderService))]

    public class RecorderService : IRecorderService
    {
        private Recorder _rec = Recorder.CreateRecorder();

        public bool IsRecording { get; set; } = false;

        public void CaptureScreenshot(string filePath, ISillListView view)
        {
            unsafe
            {
                const uint GENERIC_WRITE = 0x40000000;
                HDC hdcScreen = default;
                HDC hdcWindow = default;
                HDC hdcMemDC = default;
                HBITMAP hbmScreen = default;
                BITMAP bmpScreen;
                uint dwBytesWritten = 0;
                uint dwSizeofDIB = 0;
                SafeHandle hFile = default;
                HGLOBAL hDIB = default;
                uint dwBmpSize = 0;
                char* lpbitmap = null;
                HWND hWnd = default;

                try
                {
                    hWnd = PInvoke.GetActiveWindow();

                    hdcScreen = PInvoke.GetDC(HWND.Null);
                    hdcWindow = PInvoke.GetDC(hWnd);

                    hdcMemDC = PInvoke.CreateCompatibleDC(hdcWindow);

                    if (hdcMemDC.IsNull)
                        throw new InvalidOperationException("CreateCompatibleDC failed");

                    RECT rcClient;
                    PInvoke.GetClientRect(hWnd, out rcClient);

                    //int width = rcClient.right - rcClient.left;
                    //int height = rcClient.bottom - rcClient.top;

                    int width = 1920;
                    int height = 1080;

                    //PInvoke.SetStretchBltMode(hdcWindow, STRETCH_BLT_MODE.HALFTONE);

                    //var stretch = PInvoke.StretchBlt(hdcWindow,
                    //    0, 0,
                    //    rcClient.right, rcClient.bottom,
                    //    hdcScreen,
                    //    0, 0,
                    //    PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXSCREEN),
                    //    PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYSCREEN),
                    //    ROP_CODE.SRCCOPY);

                    //if (!stretch)
                    //    throw new InvalidOperationException("StretchBlt failed");

                    hbmScreen = PInvoke.CreateCompatibleBitmap(hdcWindow, width, height);
                    if (hbmScreen.IsNull)
                        throw new InvalidOperationException("CreateCompatibleBitmap failed");

                    PInvoke.SelectObject(hdcMemDC, hbmScreen);

                    PInvoke.BitBlt(hdcMemDC,
                        0, 0,
                        width, height,
                        hdcScreen,
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
                        biCompression = (uint)BI_COMPRESSION.BI_RGB,
                        biSizeImage = 0,
                        biXPelsPerMeter = 0,
                        biYPelsPerMeter = 0,
                        biClrUsed = 0,
                        biClrImportant = 0
                    };

                    dwBmpSize = (uint)(((bmpScreen.bmWidth * bi.biBitCount + 31) / 32) * 4 * bmpScreen.bmHeight);
                    //byte[] pixels = new byte[dwBmpSize];

                    hDIB = PInvoke.GlobalAlloc(GLOBAL_ALLOC_FLAGS.GHND, dwBmpSize);
                    lpbitmap = (char*)PInvoke.GlobalLock(hDIB);

                    PInvoke.GetDIBits(hdcWindow, hbmScreen, 0,
                        (uint)bmpScreen.bmHeight,
                        lpbitmap,
                        (BITMAPINFO*)&bi, DIB_USAGE.DIB_RGB_COLORS);

                    hFile = PInvoke.CreateFile(
                        filePath,
                        GENERIC_WRITE,
                        0,
                        null,
                        FILE_CREATION_DISPOSITION.CREATE_ALWAYS,
                        FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL, null);

                    dwSizeofDIB = (uint)(dwBmpSize + Marshal.SizeOf<BITMAPFILEHEADER>() + Marshal.SizeOf<BITMAPINFOHEADER>());

                    BITMAPFILEHEADER bmfHeader = new BITMAPFILEHEADER
                    {
                        bfOffBits = (uint)(Marshal.SizeOf<BITMAPFILEHEADER>() + Marshal.SizeOf<BITMAPINFOHEADER>()),
                        bfSize = dwSizeofDIB,
                        bfType = 0x4D42
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
            if (IsRecording)
            {
                IsRecording = false;
                StopRecording();
                return;
            }

            _rec.OnRecordingComplete += Rec_OnRecordingComplete;
            _rec.OnRecordingFailed += Rec_OnRecordingFailed;
            _rec.OnStatusChanged += Rec_OnStatusChanged;

            _rec.Record(filePath);
            IsRecording = true;
        }

        public void StopRecording()
        {
            _rec.Stop();
        }

        private void Rec_OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
        {
            var path = e.FilePath;
        }
        private void Rec_OnRecordingFailed(object sender, RecordingFailedEventArgs e)
        {
            var error = e.Error;
        }
        private void Rec_OnStatusChanged(object sender, RecordingStatusEventArgs e)
        {
            RecorderStatus status = e.Status;
        }
    }
}
