using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// MenuStrip for the main menu bar, whose items are buttons rather than drop down menus.
    /// </summary>
    public class MainMenuStrip : MenuStrip
    {
        protected override void OnMouseDown(MouseEventArgs mea)
        {
            // Mono's ToolStrip.OnMouseDown releases the mouse capture of a MenuStrip on every press.
            // Under XWayland a touchscreen tap is an emulated pointer, and ungrabbing in the middle of
            // it ends the emulation, so the release never arrives and the button never clicks.
            // A plain button needs no menu tracking, so keep the capture and let OnMouseUp click it.
            if (Program.MONO && RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && mea.Button == MouseButtons.Left)
            {
                var item = GetItemAt(mea.Location);
                if (item != null && !(item is ToolStripDropDownItem) && !(item is ToolStripControlHost))
                    return;
            }

            base.OnMouseDown(mea);
        }
    }
}
