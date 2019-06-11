using System;
using System.ComponentModel;

namespace MissionPlanner.Utilities.Drawing
{
    [Flags]
    public enum ControlStyles
    {
        /// <summary>If <see langword="true" />, the control is a container-like control.</summary>
        ContainerControl = 0x1,
        /// <summary>If <see langword="true" />, the control paints itself rather than the operating system doing so. If <see langword="false" />, the <see cref="E:System.Windows.Forms.Control.Paint" /> event is not raised. This style only applies to classes derived from <see cref="T:System.Windows.Forms.Control" />.</summary>
        UserPaint = 0x2,
        /// <summary>If <see langword="true" />, the control is drawn opaque and the background is not painted.</summary>
        Opaque = 0x4,
        /// <summary>If <see langword="true" />, the control is redrawn when it is resized.</summary>
        ResizeRedraw = 0x10,
        /// <summary>If <see langword="true" />, the control has a fixed width when auto-scaled. For example, if a layout operation attempts to rescale the control to accommodate a new <see cref="T:System.Drawing.Font" />, the control's <see cref="P:System.Windows.Forms.Control.Width" /> remains unchanged.</summary>
        FixedWidth = 0x20,
        /// <summary>If <see langword="true" />, the control has a fixed height when auto-scaled. For example, if a layout operation attempts to rescale the control to accommodate a new <see cref="T:System.Drawing.Font" />, the control's <see cref="P:System.Windows.Forms.Control.Height" /> remains unchanged.</summary>
        FixedHeight = 0x40,
        /// <summary>If <see langword="true" />, the control implements the standard <see cref="E:System.Windows.Forms.Control.Click" /> behavior.</summary>
        StandardClick = 0x100,
        /// <summary>If <see langword="true" />, the control can receive focus.</summary>
        Selectable = 0x200,
        /// <summary>If <see langword="true" />, the control does its own mouse processing, and mouse events are not handled by the operating system.</summary>
        UserMouse = 0x400,
        /// <summary>If <see langword="true" />, the control accepts a <see cref="P:System.Windows.Forms.Control.BackColor" /> with an alpha component of less than 255 to simulate transparency. Transparency will be simulated only if the <see cref="F:System.Windows.Forms.ControlStyles.UserPaint" /> bit is set to <see langword="true" /> and the parent control is derived from <see cref="T:System.Windows.Forms.Control" />.</summary>
        SupportsTransparentBackColor = 0x800,
        /// <summary>If <see langword="true" />, the control implements the standard <see cref="E:System.Windows.Forms.Control.DoubleClick" /> behavior. This style is ignored if the <see cref="F:System.Windows.Forms.ControlStyles.StandardClick" /> bit is not set to <see langword="true" />.</summary>
        StandardDoubleClick = 0x1000,
        /// <summary>If <see langword="true" />, the control ignores the window message WM_ERASEBKGND to reduce flicker. This style should only be applied if the <see cref="F:System.Windows.Forms.ControlStyles.UserPaint" /> bit is set to <see langword="true" />.</summary>
        AllPaintingInWmPaint = 0x2000,
        /// <summary>If <see langword="true" />, the control keeps a copy of the text rather than getting it from the <see cref="P:System.Windows.Forms.Control.Handle" /> each time it is needed. This style defaults to <see langword="false" />. This behavior improves performance, but makes it difficult to keep the text synchronized.</summary>
        CacheText = 0x4000,
        /// <summary>If <see langword="true" />, the <see cref="M:System.Windows.Forms.Control.OnNotifyMessage(System.Windows.Forms.Message)" /> method is called for every message sent to the control's <see cref="M:System.Windows.Forms.Control.WndProc(System.Windows.Forms.Message@)" />. This style defaults to <see langword="false" />. <see cref="F:System.Windows.Forms.ControlStyles.EnableNotifyMessage" /> does not work in partial trust.</summary>
        EnableNotifyMessage = 0x8000,
        /// <summary>If <see langword="true" />, drawing is performed in a buffer, and after it completes, the result is output to the screen. Double-buffering prevents flicker caused by the redrawing of the control. If you set <see cref="F:System.Windows.Forms.ControlStyles.DoubleBuffer" /> to <see langword="true" />, you should also set <see cref="F:System.Windows.Forms.ControlStyles.UserPaint" /> and <see cref="F:System.Windows.Forms.ControlStyles.AllPaintingInWmPaint" /> to <see langword="true" />.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        DoubleBuffer = 0x10000,
        /// <summary>If <see langword="true" />, the control is first drawn to a buffer rather than directly to the screen, which can reduce flicker. If you set this property to <see langword="true" />, you should also set the <see cref="F:System.Windows.Forms.ControlStyles.AllPaintingInWmPaint" /> to <see langword="true" />.</summary>
        OptimizedDoubleBuffer = 0x20000,
        /// <summary>Specifies that the value of the control's Text property, if set, determines the control's default Active Accessibility name and shortcut key.</summary>
        UseTextForAccessibility = 0x40000
    }
}