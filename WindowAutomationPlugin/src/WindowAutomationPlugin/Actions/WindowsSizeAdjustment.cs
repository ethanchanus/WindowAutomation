namespace Loupedeck.WindowAutomationPlugin
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Drawing;
    using System.Diagnostics;
    using Win32Interop.WinHandles;

    // This class implements an example adjustment that counts the rotation ticks of a dial.

    public class WindowsSizeAdjustment : PluginDynamicAdjustment
    {
        // This variable holds the current value of the counter.
        private enum ResizeDirection
        {
            ResizeWidth,
            ResizeHeight
        }
        private ResizeDirection m_resizeDir = ResizeDirection.ResizeWidth;

        // Initializes the adjustment class.
        // When `hasReset` is set to true, a reset command is automatically created for this adjustment.
        public WindowsSizeAdjustment()
            : base(displayName: "Windows Resize", description: "Windows Resize", groupName: "Adjustments", hasReset: true)
        {
        }

        // This method is called when the adjustment is executed.
        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            PluginLog.Info($"Windows Resize ApplyAdjustment {m_resizeDir}, diff {diff}");
            WindowHandle wh = TopLevelWindowUtils.GetForegroundWindow();

            if ( this.m_resizeDir == ResizeDirection.ResizeWidth)
            {
                wh.ResizeWindowAtCurrMousePos(diffWidth : diff*2, diffHeight: 0);
            }
            else
            {
                wh.ResizeWindowAtCurrMousePos(diffWidth: 0, diffHeight: diff*2);
            }

            this.AdjustmentValueChanged(); // Notify the Loupedeck service that the adjustment value has changed.
        }

        // This method is called when the reset command related to the adjustment is executed.
        protected override void RunCommand(String actionParameter)
        {
            m_resizeDir = (m_resizeDir == ResizeDirection.ResizeWidth) ? ResizeDirection.ResizeHeight : ResizeDirection.ResizeWidth;

            this.AdjustmentValueChanged(); // Notify the Loupedeck service that the adjustment value has changed.
        }

        // Returns the adjustment value that is shown next to the dial.
        //protected override String GetAdjustmentValue(String actionParameter) => this._counter.ToString();
    }
}
