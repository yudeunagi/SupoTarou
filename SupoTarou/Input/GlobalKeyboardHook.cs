using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SupoTarou.Input;

public sealed class GlobalKeyboardHook : IDisposable
{
    private const int WhKeyboardLl = 13;
    private const int WmKeyDown = 0x0100;
    private const int WmSysKeyDown = 0x0104;
    private const int WmKeyUp = 0x0101;
    private const int WmSysKeyUp = 0x0105;
    private const int VkSpace = 0x20;

    private readonly LowLevelKeyboardProc _hookCallback;
    private nint _hookHandle;
    private bool _spaceIsDown;

    public GlobalKeyboardHook()
    {
        _hookCallback = HookProcedure;
    }

    public event EventHandler? SpacePressed;

    public void Start()
    {
        if (_hookHandle != nint.Zero)
        {
            return;
        }

        _hookHandle = SetWindowsHookEx(WhKeyboardLl, _hookCallback, nint.Zero, 0);
        if (_hookHandle == nint.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to install the global keyboard hook.");
        }
    }

    public void Dispose()
    {
        if (_hookHandle == nint.Zero)
        {
            return;
        }

        _ = UnhookWindowsHookEx(_hookHandle);
        _hookHandle = nint.Zero;
        _spaceIsDown = false;
    }

    private nint HookProcedure(int nCode, nint wParam, nint lParam)
    {
        if (nCode >= 0)
        {
            var messageType = wParam.ToInt32();
            var keyboardData = Marshal.PtrToStructure<KbdLlHookStruct>(lParam);

            if (keyboardData.VirtualKeyCode == VkSpace)
            {
                if (messageType is WmKeyDown or WmSysKeyDown)
                {
                    if (!_spaceIsDown)
                    {
                        _spaceIsDown = true;
                        SpacePressed?.Invoke(this, EventArgs.Empty);
                    }
                }
                else if (messageType is WmKeyUp or WmSysKeyUp)
                {
                    _spaceIsDown = false;
                }
            }
        }

        return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
    }

    private delegate nint LowLevelKeyboardProc(int nCode, nint wParam, nint lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, nint hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(nint hhk);

    [DllImport("user32.dll")]
    private static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct KbdLlHookStruct
    {
        public int VirtualKeyCode;
        public int ScanCode;
        public int Flags;
        public int Time;
        public nint ExtraInfo;
    }
}
