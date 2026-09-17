namespace Loupedeck.WindowAutomationPlugin.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml.Linq;
    using System.Collections;
    using Win32Interop.WinHandles;

    internal class WndAction
    {
        public WndAction(/*string name,*/ string dspname, string desc, Func<WindowHandle, string, string, bool> act)
        {
            //Name = name;
            DisplayName = dspname;
            Description = desc;
            Action = act;
        }
        //string Name { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public Func<WindowHandle, string, string, bool> Action { get; }
    }

    internal class WindowsCommand : ActionEditorCommand
    {
        private static readonly IDictionary<string, WndAction> m_WndActionList = new Dictionary<string, WndAction>()
        {
            {
                "wndActionSetFocus",
                new WndAction(
                    dspname: "Set Window Focus",
                    desc: "",
                    act: (WindowHandle wh, string p1, string p2) => (wh.SetFocus()) )
            },
            //{ 
            //    "wndActionSetFocusTabIdx",    
            //    new WndAction(
            //        dspname: "Set Focus on given Tab Index",
            //        desc: "",
            //        act: (WindowHandle wh, string p1, string p2) => true )
            //},
            {
                "wndActionComboBoxSelectIdx",
                new WndAction(
                    dspname: "Select ComboBox with given Index",
                    desc: "",act: (WindowHandle wh, string p1, string p2) =>
                    {
                        if (Int32.TryParse(p1, out int idx))
                            return (wh.ComboBoxSelectItemByIdx(idx));
                        else
                            return false;
                    })
            },
            {
                "wndActionComboBoxSelectStr",
                new WndAction(
                    dspname: "Select ComboBox with given text",
                    desc: "",act: (WindowHandle wh, string p1, string p2) => (-1 != wh.ComboBoxSelectItemByStr(p1)))
            },
            {
                "wndActionListBoxSelectIdx",
                new WndAction(
                    dspname: "Select List Box with given Index",
                    desc: "",
                    act: (WindowHandle wh, string p1, string p2) =>
                    {
                        if (Int32.TryParse(p1, out int idx))
                            return (wh.ListBoxSelectItemByIdx(idx));
                        else
                            return false;
                    })
            },
            {
                "wndActionListBoxSelectStr",
                new WndAction(
                    dspname: "Select ListBox with given text",
                    desc: "",act: (WindowHandle wh, string p1, string p2) => (-1 != wh.ListBoxSelectItemByStr(p1)))
            },
            {
                "wndActionBtnClick",
                new WndAction(
                    dspname: "Button click",
                    desc: "",act: (WindowHandle wh, string p1, string p2) => wh.ButtonClick())
            },
            {
                "wndActionBtnCheck",
                new WndAction(
                    dspname: "Checkbox check",
                    desc: "",act: (WindowHandle wh, string p1, string p2) => wh.ButtonSetCheck())
            },
            {
                "wndActionBtnUncheck",
                new WndAction(
                    dspname: "Checkbox uncheck",
                    desc: "",act: (WindowHandle wh, string p1, string p2) => wh.ButtonSetUnCheck())
            },
            {
                "wndActionToolbarBtnClickName",
                new WndAction(
                    dspname: "Toolbar Button Click on name start with",
                    desc: "",act: (WindowHandle wh, string p1, string p2) => wh.ToolbarBtnClickStartWithName(p1))
            },
            {
                "wndActionToolbarBtnClickTooltip",
                new WndAction(
                    dspname: "Toolbar Button Click on tooltip start with",
                    desc: "",act: (WindowHandle wh, string p1, string p2) => wh.ToolbarBtnClickStartWithToolTip(p1))
            },
            {
                "wndActionTabControlSelectByName",
                new WndAction(
                    dspname: "Select Tab name",
                    desc: "",act: (WindowHandle wh, string p1, string p2) => wh.TabControlSelectByName(p1))
            },

            //{ 
            //    "wndActionFlashWnd",          
            //    new WndAction(
            //        dspname: "Flash Window",
            //        desc: "",
            //        act: (WindowHandle wh, string p1, string p2) => true ) 
            //}
        };

        public WindowsCommand()
        {

            this.DisplayName = "Windows Command";
            this.Description = "Control Windows. This is particularly useful when creating Multi-Actions (can be found in Custom category) and trying to target a specific Windows.";
            this.GroupName = "";

            //this.ActionEditor.AddControlEx(
            //    new ActionEditorTextbox(name: "procName", "Process Name ~", description: "Text the windows title containt"));

            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox(name: "wndText", "Title ~", description: "Text that windows/control title contains"));
            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox(name: "clsName", "Class Name ~", description: "Text that windows/control class contains"));
            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox(name: "uiAutoId", "UI Automation ID", description: "Windows/control's UI Automation ID "));

            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox(name: "cmbLstHasString", "ComboBox/ListBox has string ~", description: "Text that ComboBox/ListBox's text starts with"));

            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox(name: "pWndText", "Parent Title ~", description: "Text that parent windows/control title contains"));
            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox(name: "pClsName", "Parent Class Name ~", description: "Text that parent windows/control class contains"));

            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox(name: "nextWndText", "Next Title ~", description: "Text that next windows/control title contains"));
            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox(name: "nextClsName", "Next Class Name ~", description: "Text that next windows/control class contains"));

            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox(name: "prevWndText", "Previous Title ~", description: "Text that previous windows/control title contains"));
            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox(name: "prevClsName", "Previous Name ~", description: "Text that previous windows/control class contains"));

            this.ActionEditor.AddControlEx(
                new ActionEditorListbox(name: "wndAction", labelText: "Apply Command :", description: "Apply command on found windows//controls"));

            this.ActionEditor.AddControlEx(
                new ActionEditorTextbox(name: "wndActionData1", "Command Parameter #1: ", description: "Text the windows title containt"));

            //this.ActionEditor.AddControlEx(
            //    new ActionEditorTextbox(name: "wndActionData2", "Command Parameter #2: ", description: "Text the windows title containt"));

            this.ActionEditor.ListboxItemsRequested += this.OnActionEditorListboxItemsRequested;
            //this.ActionEditor.ControlValueChanged += this.OnActionEditorControlValueChanged;
        }

        //private void OnActionEditorControlValueChanged(Object sender, ActionEditorControlValueChangedEventArgs e)
        //{
        //    Debug.WriteLine(e.ControlName.ToString());
        //}

        private void OnActionEditorListboxItemsRequested(Object sender, ActionEditorListboxItemsRequestedEventArgs e)
        {
            if (e.ControlName.EqualsNoCase("wndAction"))
            {
                foreach (KeyValuePair<string, WndAction> kv in m_WndActionList)
                {
                    e.AddItem(
                        name: kv.Key.ToString(),
                        displayName: kv.Value.DisplayName,
                        description: kv.Value.Description);
                }
            }
            else
            {
                this.Plugin.Log.Error($"Unexpected control name '{e.ControlName}'");
            }
        }

        protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
        {
            PluginLog.Info($"Runcommand # of param: {actionParameters.Parameters.Count}");
            //Debug.WriteLine($"Runcommand # of param: {actionParameters.Parameters.Count}");
            if (actionParameters.Parameters.Count == 0)
            {
                return false;
            }
            else
            {
                string /*procName, */wndText, clsName, cmbLstHasString, uiAutoId, pWndText, pClsName, prevWndText, prevClsName, nextWndText, nextClsName;
                string wndAction, wndActionData1, wndActionData2;
                //string pChildWndText, pChildClsName;

                //actionParameters.TryGetString("procName", out procName);
                actionParameters.TryGetString("wndText", out wndText);
                actionParameters.TryGetString("clsName", out clsName);
                actionParameters.TryGetString("cmbLstHasString", out cmbLstHasString);
                actionParameters.TryGetString("uiAutoId", out uiAutoId);

                actionParameters.TryGetString("pWndText", out pWndText);
                actionParameters.TryGetString("pClsName", out pClsName);

                actionParameters.TryGetString("prevWndText", out prevWndText);
                actionParameters.TryGetString("prevClsName", out prevClsName);
                actionParameters.TryGetString("nextWndText", out nextWndText);
                actionParameters.TryGetString("nextClsName", out nextClsName);

                actionParameters.TryGetString("wndAction", out wndAction);
                actionParameters.TryGetString("wndActionData1", out wndActionData1);
                actionParameters.TryGetString("wndActionData2", out wndActionData2);

                wndText = (wndText != null) ? wndText.ToUpper() : null;
                clsName = (clsName != null) ? clsName.ToUpper() : null;
                //cmbLstHasString = (cmbLstHasString != null) ? cmbLstHasString.ToUpper() : null;

                pWndText = (pWndText != null) ? pWndText.ToUpper() : null;
                pClsName = (pClsName != null) ? pClsName.ToUpper() : null;

                prevWndText = (prevWndText != null) ? prevWndText.ToUpper() : null;
                prevClsName = (prevClsName != null) ? prevClsName.ToUpper() : null;

                nextWndText = (nextWndText != null) ? nextWndText.ToUpper() : null;
                nextClsName = (nextClsName != null) ? nextClsName.ToUpper() : null;


                PluginLog.Info($"RunCommand with data: " +
                    $"wndText:'{wndText}' " +
                    $"clsName:'{clsName}' " +
                    $"cmbLstHasString:'{cmbLstHasString}' " +
                    $"parent WndText:'{pWndText}' " +
                    $"parent ClsName:'{pClsName}' " +
                    $"next WndText:'{nextWndText}' " +
                    $"next ClsName:'{nextClsName}' " +
                    $"prev WndText:'{prevWndText}' " +
                    $"prev ClsName:'{prevClsName}' " +
                    $"wndAction:'{wndAction}' " +
                    $"wndActionData1:'{wndActionData1}' " +
                    $"wndActionData2:'{wndActionData2}' ");

                WindowHandle focusedWnd = TopLevelWindowUtils.GetForegroundWindow();
                IEnumerable<WindowHandle> allWnd;
                IEnumerable<WindowHandle> allChildWnd;

                allWnd = ChildWindowUtils.FindChildWindows(
                    new List<WindowHandle>() { focusedWnd },
                    w =>
                    {
                        WindowHandle pHwnd = w.GetWindowRealParent();
                        WindowHandle nextHwnd = w.GetNextWindow();
                        WindowHandle prevHwnd = w.GetPrevWindow();
                        bool bRet;

                        bRet = ((String.IsNullOrEmpty(wndText)) || w.GetHwndCaption().ToUpper().Contains(wndText))
                                && ((String.IsNullOrEmpty(clsName)) || w.GetClassName().ToUpper().Contains(clsName));

                        bRet = (bRet) && (String.IsNullOrEmpty(pWndText) || pHwnd.GetHwndCaption().ToUpper().Contains(pWndText))
                            && (String.IsNullOrEmpty(pClsName) || pHwnd.GetClassName().ToUpper().Contains(pClsName));

                        bRet = (bRet) && (String.IsNullOrEmpty(prevWndText) || prevHwnd.GetHwndCaption().ToUpper().Contains(prevWndText))
                                && (String.IsNullOrEmpty(prevClsName) || prevHwnd.GetClassName().ToUpper().Contains(prevClsName));

                        bRet = (bRet) && (String.IsNullOrEmpty(nextWndText) || nextHwnd.GetHwndCaption().ToUpper().Contains(nextWndText))
                            && (String.IsNullOrEmpty(nextClsName) || nextHwnd.GetClassName().ToUpper().Contains(nextClsName));

                        bRet = (bRet) && (String.IsNullOrEmpty(cmbLstHasString) || (-1 != w.ComboBoxFindString(cmbLstHasString)));
                        bRet = (bRet) && (String.IsNullOrEmpty(cmbLstHasString) || (-1 != w.ListBoxFindString(cmbLstHasString)));

                        bRet = (bRet) && (String.IsNullOrEmpty(uiAutoId) || (w.GetUiAutomationId().Equals(uiAutoId)));

                        if (bRet)
                        {
                            PluginLog.Info($"Found window: " +
                                $"0x{w.RawPtr.ToString("X8")}, " +
                                $"text:'{w.GetHwndCaption()}', class:'{w.GetClassName()}' " +
                                $"parent hwnd: 0x{pHwnd.RawPtr.ToString("X8")} parent text:'{pHwnd.GetHwndCaption()}', parent cls:'{pHwnd.GetClassName()}', " +
                                $"next hwnd: 0x{nextHwnd.RawPtr.ToString("X8")} next text:'{nextHwnd.GetHwndCaption()}', next cls:'{nextHwnd.GetClassName()}', " +
                                $"prev hwnd: 0x{prevHwnd.RawPtr.ToString("X8")}prev text:'{prevHwnd.GetHwndCaption()} ', prev cls:' {prevHwnd.GetClassName()}'");
                        }

                        return bRet;
                    }
                );

                PluginLog.Info($"Focus Window: 0x{focusedWnd.RawPtr.ToString("X8")} '{focusedWnd.GetWindowText()}'");                
                foreach (WindowHandle wh in allWnd)
                {
                    bool bRet = false;

                    PluginLog.Info($"Found window: " +
                        $"0x{wh.RawPtr.ToString("X8")}, " +
                        $"'{wh.GetWindowText()} with action: '{(String.IsNullOrEmpty(wndAction) ? "null" : wndAction)}'" +
                        $" and action defined: {m_WndActionList.ContainsKey(wndAction)}");

                    if (m_WndActionList.ContainsKey(wndAction))
                    {
                        bRet = m_WndActionList[wndAction].Action(wh, wndActionData1, wndActionData2);
                    }

                    PluginLog.Info($" 0x{wh.RawPtr.ToString("X8")} ==> {wndAction} action return {bRet}");
                }
            }

            return true;
        }
    }
}