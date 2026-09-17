using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;

namespace Win32Interop.WinHandles.Internal
{
  internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

  /// <summary> Win32 methods. </summary>
  internal static class NativeMethods
  {
    public const bool EnumWindows_ContinueEnumerating = true;
    public const bool EnumWindows_StopEnumerating = false;

    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);


        [DllImport("user32.dll")]
    internal static extern IntPtr FindWindow(string sClassName, string sAppName);

    [DllImport("user32.dll")]
    internal static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    internal static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out IntPtr processId);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    internal static extern int GetClassName(IntPtr hWnd,
                                            StringBuilder lpClassName,
                                            int nMaxCount);


        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        private static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);
        
        public static IntPtr GetWindowLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
            {
                return GetWindowLong32(hWnd, nIndex);
            }
            return GetWindowLongPtr64(hWnd, nIndex);
        }

        [Flags]
        public enum GetWindowLongIdx : int
        {
            GWL_WNDPROC         =(-4),
            GWL_HINSTANCE       =(-6),
            GWL_HWNDPARENT      =(-8),
            GWL_STYLE           =(-16),
            GWL_EXSTYLE         =(-20),
            GWL_USERDATA        =(-21),
            GWL_ID              =(-12),
        }

        [Flags]
        public enum WindowsStyle : long
        {
            WS_OVERLAPPED = 0x00000000L,
            WS_POPUP = 0x80000000L,
            WS_CHILD = 0x40000000L,
            WS_MINIMIZE = 0x20000000L,
            WS_VISIBLE = 0x10000000L,
            WS_DISABLED = 0x08000000L,
            WS_CLIPSIBLINGS = 0x04000000L,
            WS_CLIPCHILDREN = 0x02000000L,
            WS_MAXIMIZE = 0x01000000L,
            WS_CAPTION = 0x00C00000L,     /* WS_BORDER | WS_DLGFRAME  */
            WS_BORDER = 0x00800000L,
            WS_DLGFRAME = 0x00400000L,
            WS_VSCROLL = 0x00200000L,
            WS_HSCROLL = 0x00100000L,
            WS_SYSMENU = 0x00080000L,
            WS_THICKFRAME = 0x00040000L,
            WS_GROUP = 0x00020000L,
            WS_TABSTOP = 0x00010000L,
            WS_MINIMIZEBOX = 0x00020000L,
            WS_MAXIMIZEBOX = 0x00010000L,
        }

        public enum WindowsExStyle : long
        {
            WS_EX_DLGMODALFRAME = 0x00000001L,
            WS_EX_NOPARENTNOTIFY = 0x00000004L,
            WS_EX_TOPMOST = 0x00000008L,
            WS_EX_ACCEPTFILES = 0x00000010L,
            WS_EX_TRANSPARENT = 0x00000020L,
            WS_EX_MDICHILD = 0x00000040L,
            WS_EX_TOOLWINDOW = 0x00000080L,
            WS_EX_WINDOWEDGE = 0x00000100L,
            WS_EX_CLIENTEDGE = 0x00000200L,
            WS_EX_CONTEXTHELP = 0x00000400L,
            WS_EX_RIGHT = 0x00001000L,
            WS_EX_LEFT = 0x00000000L,
            WS_EX_RTLREADING = 0x00002000L,
            WS_EX_LTRREADING = 0x00000000L,
            WS_EX_LEFTSCROLLBAR = 0x00004000L,
            WS_EX_RIGHTSCROLLBAR = 0x00000000L,
            WS_EX_CONTROLPARENT = 0x00010000L,
            WS_EX_STATICEDGE = 0x00020000L,
            WS_EX_APPWINDOW = 0x00040000L,
            WS_EX_LAYERED = 0x00080000L,
            WS_EX_NOINHERITLAYOUT = 0x00100000L, // Disable inheritence of mirroring by children
            WS_EX_LAYOUTRTL = 0x00400000L, // Right to left mirroring
            WS_EX_COMPOSITED = 0x02000000L,
            WS_EX_NOACTIVATE = 0x08000000L,
        }

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowFlags uCmd);

        public enum GetWindowFlags : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }
        public enum GetAncestorFlags : uint
        {
            GA_PARENT= 1,
            GA_ROOT = 2,
            GA_ROOTOWNER = 3
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetAncestor(IntPtr hwnd, GetAncestorFlags gaFlags);

        [DllImport("user32.dll", SetLastError = true)] 
        public static extern IntPtr SetFocus(IntPtr hWnd);

        public enum SendMessageMsgId : int
        {
            WM_SETFOCUS     = 0x0007,
            CB_SETCURSEL    = 0x014E,
            CB_FINDSTRING   = 0x014C,
            CB_SELECTSTRING = 0x014D,
            CB_GETCURSEL    = 0x0147,
            CB_GETLBTEXT    = 0x0148,
            CB_GETLBTEXTLEN = 0x0149,

            LB_SETCURSEL    = 0x0186,
            LB_GETCURSEL    = 0x0188,
            LB_GETSEL       = 0x0187,
            LB_GETTEXT      = 0x0189,
            LB_GETTEXTLEN   = 0x018A,
            LB_FINDSTRING   = 0x018F,
            LB_SELECTSTRING = 0x018C,

            TTM_GETTEXT     = 0x0400+56,
            BM_CLICK        = 0x00F5,
            BM_SETCHECK     = 0x00F1,
            WM_COMMAND      = 0x0111
        }

        public enum SendMessageMsgWmCmdCode : int
        {
            CBN_SELCHANGE = 0x0001,
            LBN_SELCHANGE = 0x0001

            //CB_SETCURSEL = 0x014E,
            //CB_FINDSTRING = 0x014C,
            //CB_SELECTSTRING = 0x014D,
            //CB_GETCURSEL = 0x0147,
            //CB_GETLBTEXT = 0x0148,
            //CB_GETLBTEXTLEN = 0x0149,

            //LB_SETCURSEL = 0x0186,
            //LB_GETCURSEL = 0x0188,
            //LB_GETSEL = 0x0187,
            //LB_GETTEXT = 0x0189,
            //LB_GETTEXTLEN = 0x018A,
            //LB_FINDSTRING = 0x018F,
            //LB_SELECTSTRING = 0x018C,

            //TTM_GETTEXT = 0x0400 + 56,
            //BM_CLICK = 0x00F5,
            //BM_SETCHECK = 0x00F1
        }
        //public struct RECT
        //{
        //    private int Left;
        //    private int Top;
        //    private int Right;
        //    private int Bottom;
        //}

        //public struct SendMessageMsgToolTipInfo
        //{
        //    public int cbSize;
        //    public int uFlags;
        //    public IntPtr hwnd;
        //    public IntPtr uId;
        //    public RECT rect;
        //    public IntPtr hinst;

        //    [MarshalAs(UnmanagedType.LPTStr)]
        //    public string lpszText;

        //    public IntPtr lParam;
        //}


        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, SendMessageMsgId uMsg, IntPtr wParam, StringBuilder lParam);

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hwnd, SendMessageMsgId msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool AttachThreadInput(IntPtr idAttach,
                         IntPtr idAttachTo, bool fAttach);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentThreadId();

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref Rect Rect);

        [DllImport("dwmapi.dll")]
        static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out Rect pvAttribute, int cbAttribute);


        [Flags]
        public enum DwmWindowAttribute : uint
        {
            DWMWA_NCRENDERING_ENABLED = 1,
            DWMWA_NCRENDERING_POLICY,
            DWMWA_TRANSITIONS_FORCEDISABLED,
            DWMWA_ALLOW_NCPAINT,
            DWMWA_CAPTION_BUTTON_BOUNDS,
            DWMWA_NONCLIENT_RTL_LAYOUT,
            DWMWA_FORCE_ICONIC_REPRESENTATION,
            DWMWA_FLIP3D_POLICY,
            DWMWA_EXTENDED_FRAME_BOUNDS,
            DWMWA_HAS_ICONIC_BITMAP,
            DWMWA_DISALLOW_PEEK,
            DWMWA_EXCLUDED_FROM_PEEK,
            DWMWA_CLOAK,
            DWMWA_CLOAKED,
            DWMWA_FREEZE_REPRESENTATION,
            DWMWA_LAST
        }

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point p);
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);
    }
}