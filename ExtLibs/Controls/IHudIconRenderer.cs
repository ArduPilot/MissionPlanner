using System.Drawing;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// Defines a custom renderer for an EKF or VIBE icon slot on the HUD.
    /// </summary>
    public interface IHudIconRenderer
    {
        /// <summary>
        /// Renders the icon into the given hit zone rectangle.
        /// </summary>
        /// <remarks>
        /// Called every HUD paint cycle with the transform already reset to
        /// screen coordinates. Use the HUD's public drawing methods
        /// (DrawString, DrawImage, FillRectangle, DrawRectangle).
        /// </remarks>
        void Render(HUD hud, Rectangle hitZone, int fontsize, bool useIcons);

        /// <summary>
        /// Called when the user clicks this icon slot.
        /// </summary>
        void OnClick();

        /// <summary>
        /// Gets a value indicating whether to show a hand cursor when hovering over this icon slot.
        /// </summary>
        bool ShowHandCursor { get; }
    }
}
