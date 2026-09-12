using System.Runtime.InteropServices;

namespace SupoTarou.Services;

public sealed class ScreenColorSampler
{
    private const uint InvalidPixel = 0xFFFFFFFF;

    public bool TrySampleRgb(out byte red, out byte green, out byte blue)
    {
        red = 0;
        green = 0;
        blue = 0;

        if (!GetCursorPos(out var cursorPosition))
        {
            return false;
        }

        var desktopDc = GetDC(nint.Zero);
        if (desktopDc == nint.Zero)
        {
            return false;
        }

        var pixel = GetPixel(desktopDc, cursorPosition.X, cursorPosition.Y);
        _ = ReleaseDC(nint.Zero, desktopDc);

        if (pixel == InvalidPixel)
        {
            return false;
        }

        red = (byte)(pixel & 0x000000FF);
        green = (byte)((pixel & 0x0000FF00) >> 8);
        blue = (byte)((pixel & 0x00FF0000) >> 16);
        return true;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetCursorPos(out Point point);

    [DllImport("user32.dll")]
    private static extern nint GetDC(nint windowHandle);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(nint windowHandle, nint deviceContextHandle);

    [DllImport("gdi32.dll")]
    private static extern uint GetPixel(nint deviceContextHandle, int x, int y);

    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        public int X;
        public int Y;
    }
}
