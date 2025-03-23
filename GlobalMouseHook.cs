using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public sealed class GlobalMouseHook : IDisposable
{
    #region Win32 API 声明
    private const int WH_MOUSE_LL = 14;
    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_LBUTTONUP = 0x0202;
    private const int WM_RBUTTONDOWN = 0x0204;
    private const int WM_RBUTTONUP = 0x0205;
    private const int WM_MBUTTONDOWN = 0x0207;
    private const int WM_MBUTTONUP = 0x0208;
    private const int WM_MOUSEWHEEL = 0x020A;

    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT
    {
        public POINT Point;
        public int MouseData;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
    #endregion

    #region 事件和字段
    public event EventHandler<GlobalMouseEventArgs> MouseAction;
    private readonly LowLevelMouseProc _proc;
    private IntPtr _hookId = IntPtr.Zero;
    private bool _disposed;
    #endregion

    public GlobalMouseHook()
    {
        _proc = HookCallback;
        InstallHook();
    }

    private void InstallHook()
    {
        using (var curProcess = Process.GetCurrentProcess())
        using (var curModule = curProcess.MainModule)
        {
            _hookId = SetWindowsHookEx(WH_MOUSE_LL, _proc,
                GetModuleHandle(curModule.ModuleName), 0);

            if (_hookId == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            var hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
            var eventType = GetEventType(wParam);
            var button = GetMouseButton(wParam);

            var args = new GlobalMouseEventArgs(
                button: button,
                eventType: eventType,
                x: hookStruct.Point.X,
                y: hookStruct.Point.Y,
                delta: eventType == MouseEventType.Wheel ?
                    (hookStruct.MouseData >> 16) & 0xFFFF : 0);

            MouseAction?.Invoke(this, args);
        }
        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private MouseButtons GetMouseButton(IntPtr wParam)
    {
        return (int)wParam switch
        {
            WM_LBUTTONDOWN or WM_LBUTTONUP => MouseButtons.Left,
            WM_RBUTTONDOWN or WM_RBUTTONUP => MouseButtons.Right,
            WM_MBUTTONDOWN or WM_MBUTTONUP => MouseButtons.Middle,
            _ => MouseButtons.None
        };
    }

    private MouseEventType GetEventType(IntPtr wParam)
    {
        return (int)wParam switch
        {
            WM_LBUTTONDOWN or WM_RBUTTONDOWN or WM_MBUTTONDOWN => MouseEventType.Down,
            WM_LBUTTONUP or WM_RBUTTONUP or WM_MBUTTONUP => MouseEventType.Up,
            WM_MOUSEWHEEL => MouseEventType.Wheel,
            _ => MouseEventType.None
        };
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (_hookId != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }

        _disposed = true;
    }

    ~GlobalMouseHook()
    {
        Dispose(false);
    }
}

public enum MouseEventType
{
    None,
    Down,
    Up,
    Wheel
}

public class GlobalMouseEventArgs : EventArgs
{
    public MouseButtons Button { get; }
    public MouseEventType EventType { get; }
    public int X { get; }
    public int Y { get; }
    public int Delta { get; }

    public GlobalMouseEventArgs(MouseButtons button, MouseEventType eventType,
        int x, int y, int delta)
    {
        Button = button;
        EventType = eventType;
        X = x;
        Y = y;
        Delta = delta;
    }
}