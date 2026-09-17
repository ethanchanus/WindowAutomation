using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Windows.Automation;
using JetBrains.Annotations;
using Win32Interop.WinHandles.Internal;
using static System.Net.Mime.MediaTypeNames;
using static Win32Interop.WinHandles.Internal.NativeMethods;
using System.Drawing;
using System.Windows;
namespace Win32Interop.WinHandles
{
    /// <summary> Extension methods for <see cref="WindowHandle"/> </summary>
    public static class WindowHandleExtensions
    {
        /// <summary> Check if the given window handle is currently visible. </summary>
        /// <param name="windowHandle"> The window to act on. </param>
        /// <returns> true if the window is visible, false if not. </returns>
        public static bool IsVisible(this WindowHandle windowHandle)
        {
            return NativeMethods.IsWindowVisible(windowHandle.RawPtr);
        }

        /// <summary> Gets the Win32 class name of the given window. </summary>
        /// <param name="windowHandle"> The window handle to act on. </param>
        /// <returns> The class name of the passed in window. </returns>
        public static string GetClassName(this WindowHandle windowHandle)
        {
            int size = 255;
            int actualSize = 0;
            StringBuilder builder;
            do
            {
                builder = new StringBuilder(size);
                actualSize = NativeMethods.GetClassName(windowHandle.RawPtr, builder, builder.Capacity);
                size *= 2;
            } while (actualSize == size - 1);

            return builder.ToString();
        }

        /// <summary> Gets the text associated with the given window handle. </summary>
        /// <param name="windowHandle"> The window handle to act on. </param>
        /// <returns> The window text. </returns>
        [NotNull]
        public static string GetWindowText(this WindowHandle windowHandle)
        {
            int size = NativeMethods.GetWindowTextLength(windowHandle.RawPtr);
            if (size > 0)
            {
                var builder = new StringBuilder(size + 1);
                NativeMethods.GetWindowText(windowHandle.RawPtr, builder, builder.Capacity);
                return builder.ToString();
            }

            return String.Empty;
        }
        public static string GetHwndCaption(this WindowHandle wh)
        {
            string cap = wh.GetWindowText();
            if (cap == "")
                cap = wh.ListBoxGetCurSelText();

            if (cap == "")
                cap = wh.ComboBoxGetCurSelText();

            return cap;
        }
        /// <summary> Gets the text associated with the given window handle. </summary>
        /// <param name="windowHandle"> The window handle to act on. </param>
        /// <returns> The window text. </returns>
        public static IntPtr GetWindowStyle(this WindowHandle windowHandle)
        {
            return NativeMethods.GetWindowLong(windowHandle.RawPtr, (int)NativeMethods.GetWindowLongIdx.GWL_STYLE);
        }
        /// <summary> Gets the text associated with the given window handle. </summary>
        /// <param name="windowHandle"> The window handle to act on. </param>
        /// <returns> The window text. </returns>
        public static IntPtr GetWindowStyleEx(this WindowHandle windowHandle)
        {
            return NativeMethods.GetWindowLong(windowHandle.RawPtr, (int)NativeMethods.GetWindowLongIdx.GWL_EXSTYLE);
        }
        /// <summary> Gets the text associated with the given window handle. </summary>
        /// <param name="windowHandle"> The window handle to act on. </param>
        /// <returns> The window text. </returns>
        public static IntPtr GetWindowProcessId(this WindowHandle windowHandle)
        {
            IntPtr procId;
            NativeMethods.GetWindowThreadProcessId(windowHandle.RawPtr, out procId);
            return procId;
        }
        public static uint GetWindowThreadId(this WindowHandle windowHandle)
        {
            return 0;// NativeMethods.GetWindowThreadProcessId(windowHandle.RawPtr, 0);
        }

        public static WindowHandle GetWindowRealParent(this WindowHandle winHdl)
        {
            IntPtr hWndOwner;

            // To obtain a window's owner window, instead of using GetParent,
            // use GetWindow with the GW_OWNER flag.
            hWndOwner = NativeMethods.GetWindow(winHdl.RawPtr, NativeMethods.GetWindowFlags.GW_OWNER);

            if (IntPtr.Zero == hWndOwner)
                hWndOwner = NativeMethods.GetAncestor(winHdl.RawPtr, NativeMethods.GetAncestorFlags.GA_PARENT);
            
            return new WindowHandle(hWndOwner);
        }

        public static WindowHandle GetNextWindow(this WindowHandle winHdl)
        {
            IntPtr hwnd;

            // To obtain a window's owner window, instead of using GetParent,
            // use GetWindow with the GW_OWNER flag.
            hwnd = NativeMethods.GetWindow(winHdl.RawPtr, NativeMethods.GetWindowFlags.GW_HWNDNEXT);

            return new WindowHandle(hwnd);
        }

        public static WindowHandle GetPrevWindow(this WindowHandle winHdl)
        {
            IntPtr hwnd;

            // To obtain a window's owner window, instead of using GetParent,
            // use GetWindow with the GW_OWNER flag.
            hwnd = NativeMethods.GetWindow(winHdl.RawPtr, NativeMethods.GetWindowFlags.GW_HWNDPREV);

            return new WindowHandle(hwnd);
        }


        public static bool SetFocus(this WindowHandle hdl)
        {
            //IntPtr activeWindowThread = NativeMethods.GetWindowThreadProcessId(hdl, IntPtr.Zero);
            //IntPtr thisWindowThread = GetWindowThreadProcessId(this.Handle, IntPtr.Zero);

            //AttachThreadInput(activeWindowThread, thisWindowThread, true);
            //IntPtr focusedControlHandle = GetFocus();
            //AttachThreadInput(activeWindowThread, thisWindowThread, false);

            bool bRet = false;
            var currThreadId = NativeMethods.GetCurrentThreadId();
            IntPtr processId;
            var focusedThreadID = NativeMethods.GetWindowThreadProcessId(hdl.RawPtr, out processId);
            if (NativeMethods.AttachThreadInput(currThreadId, focusedThreadID, true))
                //            try
                bRet = (IntPtr.Zero == NativeMethods.SetFocus(hdl.RawPtr));
            //finally
            NativeMethods.AttachThreadInput(currThreadId, focusedThreadID, false);
            return bRet;
            //var i = NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.WM_SETFOCUS, (IntPtr)0, null);
            //return i == 0;
            //return (0 == NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.WM_SETFOCUS, (IntPtr)0, null));
        }
        private static int MakeDWord32Bit(int loWord, int hiWord)
        {
            return (loWord & 0xFFFF) + ((hiWord & 0xFFFF) << 16);
        }

        public static bool ListBoxSelectItemByIdx(this WindowHandle hdl, int itemIdx)
        {
            bool bRet = (itemIdx == NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.LB_SETCURSEL, (IntPtr)itemIdx, null));
            if (bRet)
            {
                WindowHandle wp = hdl.GetWindowRealParent();
                var ctrlId = NativeMethods.GetWindowLong(hdl.RawPtr, (int)NativeMethods.GetWindowLongIdx.GWL_ID);
                int wparam = MakeDWord32Bit((int)ctrlId, (int)NativeMethods.SendMessageMsgWmCmdCode.LBN_SELCHANGE);

                NativeMethods.SendMessage(wp.RawPtr, NativeMethods.SendMessageMsgId.WM_COMMAND, (IntPtr)wparam, hdl.RawPtr);
            }
            return bRet;
        }

        public static int ListBoxGetCurSelItemIdx(this WindowHandle hdl)
        {
            return NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.LB_GETCURSEL, IntPtr.Zero, null);
        }
        public static int ListBoxGetTextIdxLen(this WindowHandle hdl, int itemIdx)
        {
            return NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.LB_GETTEXTLEN, (IntPtr) itemIdx, null);
        }
        public static string ListBoxGetTextByIdx(this WindowHandle hdl, int itemIdx)
        {
            string txt = "";
            int txtLen = ListBoxGetTextIdxLen(hdl, itemIdx);
            if (txtLen != -1)
            {
                //IntPtr pRetTxt = Marshal.AllocHGlobal(txtLen);
                //Marshal.WriteByte(pRetTxt, 0, 0);

                //var idx = NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.LB_GETTEXT, (IntPtr)0, pRetTxt);
                //txt = Marshal.PtrToStringUni(pRetTxt);
                //Marshal.FreeHGlobal(pRetTxt);
                var builder = new StringBuilder(txtLen + 1);
                NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.LB_GETTEXT, (IntPtr)itemIdx, builder);
                txt = builder.ToString();
            }
            return txt;
        }

        public static string ListBoxGetCurSelText(this WindowHandle hdl)
        {
            string txt = "";
            int idx = ListBoxGetCurSelItemIdx(hdl);
            if (idx != -1)
            {
                txt = ListBoxGetTextByIdx(hdl, idx);
            }
            return txt;

            //return NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.LB_GETCURSEL, IntPtr.Zero, IntPtr.Zero);
        }

        public static int ListBoxFindString(this WindowHandle hdl, string itemText)
        {
            var builder = new StringBuilder(itemText + char.MinValue);
            var idx = NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.LB_FINDSTRING, (IntPtr)0, builder);
            return idx;
        }
        public static int ListBoxSelectItemByStr(this WindowHandle hdl, string itemText)
        {
            var builder = new StringBuilder(itemText + char.MinValue);
            var itemIdx = NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.LB_SELECTSTRING, (IntPtr) (-1), builder);

            if (itemIdx != -1)
            {
                WindowHandle wp = hdl.GetWindowRealParent();
                var ctrlId = NativeMethods.GetWindowLong(hdl.RawPtr, (int)NativeMethods.GetWindowLongIdx.GWL_ID);
                int wparam = MakeDWord32Bit((int)ctrlId, (int)NativeMethods.SendMessageMsgWmCmdCode.LBN_SELCHANGE);

                NativeMethods.SendMessage(wp.RawPtr, NativeMethods.SendMessageMsgId.WM_COMMAND, (IntPtr)wparam, hdl.RawPtr);
            }
            return itemIdx;
        }


        public static bool ComboBoxSelectItemByIdx(this WindowHandle hdl, int itemIdx)
        {
            bool bRet = (itemIdx == NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.CB_SETCURSEL, (IntPtr)itemIdx, null));
            if (bRet)
            {
                WindowHandle wp = hdl.GetWindowRealParent();
                var ctrlId = NativeMethods.GetWindowLong(hdl.RawPtr, (int)NativeMethods.GetWindowLongIdx.GWL_ID);
                int wparam = MakeDWord32Bit((int)ctrlId, (int)NativeMethods.SendMessageMsgWmCmdCode.CBN_SELCHANGE);

                NativeMethods.SendMessage(wp.RawPtr, NativeMethods.SendMessageMsgId.WM_COMMAND, (IntPtr)wparam, hdl.RawPtr);
            }
            return bRet;

        }
        public static int ComboBoxSelectItemByStr(this WindowHandle hdl, string itemText)
        {
            var builder = new StringBuilder(itemText + char.MinValue);
            var itemIdx = NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.CB_SELECTSTRING, (IntPtr)(-1), builder);

            if (itemIdx != -1)
            {
                WindowHandle wp = hdl.GetWindowRealParent();
                var ctrlId = NativeMethods.GetWindowLong(hdl.RawPtr, (int)NativeMethods.GetWindowLongIdx.GWL_ID);
                int wparam = MakeDWord32Bit((int)ctrlId, (int)NativeMethods.SendMessageMsgWmCmdCode.CBN_SELCHANGE);

                NativeMethods.SendMessage(wp.RawPtr, NativeMethods.SendMessageMsgId.WM_COMMAND, (IntPtr)wparam, hdl.RawPtr);
            }
            return itemIdx;


        }

        public static int ComboBoxGetCurSelItemIdx(this WindowHandle hdl)
        {
            return NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.CB_GETCURSEL, IntPtr.Zero, null);
        }
        public static int ComboBoxGetTextIdxLen(this WindowHandle hdl, int itemIdx)
        {
            return NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.CB_GETLBTEXTLEN, (IntPtr)itemIdx, null);
        }
        public static string ComboBoxGetTextByIdx(this WindowHandle hdl, int itemIdx)
        {
            string txt = "";
            int txtLen = ComboBoxGetTextIdxLen(hdl, itemIdx);
            if (txtLen != -1)
            {
                var builder = new StringBuilder(txtLen + 1);
                NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.CB_GETLBTEXT, (IntPtr)itemIdx, builder);
                txt = builder.ToString();
            }
            return txt;
        }

        public static string ComboBoxGetCurSelText(this WindowHandle hdl)
        {
            string txt = "";
            int idx = ComboBoxGetCurSelItemIdx(hdl);
            if (idx != -1)
            {
                txt = ComboBoxGetTextByIdx(hdl, idx);
            }
            return txt;
        }

        public static int ComboBoxFindString(this WindowHandle hdl, string itemText)
        {
            var builder = new StringBuilder(itemText + char.MinValue);
            var idx = NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.CB_FINDSTRING, (IntPtr)0, builder);
            if (idx != -1)
            {
                string itemStr = ComboBoxGetTextByIdx(hdl, idx);
                if (!itemStr.StartsWith(itemText))
                    idx = -1;
            }
            return idx;
        }
        public static bool ButtonClick(this WindowHandle hdl)
        {
            NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.BM_CLICK, (IntPtr)0, null);
            return true;                       
        }
        public static bool ButtonSetCheck(this WindowHandle hdl)
        {
            AutomationElement e = AutomationElement.FromHandle(hdl.RawPtr);
            Object objPattern;
            TogglePattern togPattern;
            if (true == e.TryGetCurrentPattern(TogglePattern.Pattern, out objPattern))
            {
                togPattern = objPattern as TogglePattern;
                if (togPattern.Current.ToggleState != ToggleState.On)
                {
                    togPattern.Toggle();
                    return true;
                }
            }
            //NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.BM_SETCHECK, (IntPtr)1, null);
            return true;
        }
        public static bool ButtonSetUnCheck(this WindowHandle hdl)
        {
            AutomationElement e = AutomationElement.FromHandle(hdl.RawPtr);
            Object objPattern;
            TogglePattern togPattern;
            if (true == e.TryGetCurrentPattern(TogglePattern.Pattern, out objPattern))
            {
                togPattern = objPattern as TogglePattern;
                if (togPattern.Current.ToggleState != ToggleState.Off)
                {
                    togPattern.Toggle();
                    return true;
                }
            }
            //NativeMethods.SendMessage(hdl.RawPtr, NativeMethods.SendMessageMsgId.BM_SETCHECK, (IntPtr)0, null);
            return false;
        }
        public static bool ToolbarBtnClickStartWithToolTip(this WindowHandle hdl, string startWithToolTip)
        {
            bool bRet = false;
            AutomationElement ae = AutomationElement.FromHandle(hdl.RawPtr);
            AutomationElementCollection list = ae.FindAll(TreeScope.Descendants,
                new OrCondition(
                   new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button),
                   new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.CheckBox))
                   );
            foreach (AutomationElement element in list)
            {
                if (!string.IsNullOrEmpty(element.Current.HelpText) && element.Current.HelpText.StartsWith(startWithToolTip))
                {
                    Debug.WriteLine($"Found toolbar btn: '{element.Current.Name}', tooltip: '{element.Current.HelpText}'");

                    Object objPattern;
                    if (true == element.TryGetCurrentPattern(InvokePattern.Pattern, out objPattern))
                    {
                        InvokePattern pattern = objPattern as InvokePattern;
                        pattern.Invoke();
                        bRet = true;
                    }
                    else if (true == element.TryGetCurrentPattern(TogglePattern.Pattern, out objPattern))
                    {
                        TogglePattern pattern = objPattern as TogglePattern;
                        pattern.Toggle();
                        bRet = true;
                    }
                }
            }
            return bRet;
        }

        public static bool ToolbarBtnClickStartWithName(this WindowHandle hdl, string startWithName)
        {
            bool bRet = false;
            AutomationElement ae = AutomationElement.FromHandle(hdl.RawPtr);
            AutomationElementCollection list = ae.FindAll(TreeScope.Descendants,
                   new OrCondition(
                   new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button),
                   new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.CheckBox))
                   );
            foreach (AutomationElement element in list)
            {
                if (!string.IsNullOrEmpty(element.Current.Name) && element.Current.Name.StartsWith(startWithName))
                {
                    Debug.WriteLine($"Found toolbar btn: '{element.Current.Name}', tooltip: '{element.Current.HelpText}'");

                    Object objPattern;
                    if (true == element.TryGetCurrentPattern(InvokePattern.Pattern, out objPattern))
                    {
                        InvokePattern pattern = objPattern as InvokePattern;
                        pattern.Invoke();
                        bRet = true;
                    }
                    else if (true == element.TryGetCurrentPattern(TogglePattern.Pattern, out objPattern))
                    {
                        TogglePattern pattern = objPattern as TogglePattern;
                        pattern.Toggle();
                        bRet = true;
                    }
                }
            }
            return bRet;
        }


        public static bool TabControlSelectByName(this WindowHandle hdl, string tabName)
        {
            bool bRet = false;
            AutomationElement h = AutomationElement.FromHandle(hdl.RawPtr);
            AutomationElement tabItem = h.FindFirst(TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem),
                    new PropertyCondition(AutomationElement.NameProperty, tabName))
                );
            if (tabItem != null)
            {
                Object objPattern; 
                if (true == tabItem.TryGetCurrentPattern(SelectionItemPattern.Pattern, out objPattern))
                {
                    SelectionItemPattern pattern = objPattern as SelectionItemPattern;
                    pattern.Select();
                    bRet = true;
                }
            }
            return bRet; 
        }
        public static string GetUiAutomationId(this WindowHandle hdl)
        {
            AutomationElement h = AutomationElement.FromHandle(hdl.RawPtr);
            return h.Current.AutomationId;
        }

        public static bool ResizeWindowAtCurrMousePos(this WindowHandle hdl, int diffWidth, int diffHeight)
        {

            Point currMousePos;
            IntPtr hwnd, pHwnd;
            NativeMethods.Rect r = new Rect();
            int width, height;

            NativeMethods.GetCursorPos(out currMousePos);
            hwnd = NativeMethods.WindowFromPoint(currMousePos);
            pHwnd = NativeMethods.GetParent(hwnd);

            NativeMethods.GetWindowRect(hwnd, ref r);
            width = r.Right - r.Left;
            height = r.Bottom - r.Top;
            if (pHwnd != IntPtr.Zero)
            {
                Point refP = new Point(r.Left, r.Top);
                ScreenToClient(pHwnd, ref refP);
                r.Left = (int)refP.X;
                r.Top = (int)refP.Y;
            }

            return NativeMethods.MoveWindow(hwnd, r.Left, r.Top, width + diffWidth, height + diffHeight, false);
        }

    }
}