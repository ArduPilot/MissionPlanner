namespace ZedGraph
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The reversible frame draws a dashed rectangle
    /// </summary>
    internal class ReversibleFrame
    {
        #region Enums

        /// <summary>
        /// The possible pen styles for creating pens.
        /// </summary>
        private enum PenStyle : int
        {
            Solid = 0,

            Dash = 1, /* -------  */

            Dot = 2, /* .......  */

            DashDot = 3, /* _._._._  */

            DashDotDot = 4, /* _.._.._  */

            Invisible = 5,

            InsideFrame = 6,
        }

        /// <summary>
        /// The available raster operation types.
        /// </summary>
        private enum RasterOperation
        {
            Black = 1, /*  0       */

            NOTMERGEPEN = 2, /* DPon     */

            MASKNOTPEN = 3, /* DPna     */

            NOTCOPYPEN = 4, /* PN       */

            MASKPENNOT = 5, /* PDna     */

            NOT = 6, /* Dn       */

            XORPEN = 7, /* DPx      */

            NOTMASKPEN = 8, /* DPan     */

            MASKPEN = 9, /* DPa      */

            NOTXORPEN = 10, /* DPxn     */

            NOP = 11, /* D        */

            MERGENOTPEN = 12, /* DPno     */

            COPYPEN = 13, /* P        */

            MERGEPENNOT = 14, /* PDno     */

            MERGEPEN = 15, /* DPo      */

            WHITE = 16, /*  1       */

            LAST = 16,
        }

        private enum StockObject : int
        {
            WhiteBrush = 0,

            LightGrayBrush = 1,

            GrayBrush = 2,

            DarkGrayBrush = 3,

            BlackBrush = 4,

            NullBrush = 5,

            WhitePen = 6,

            BlackPen = 7,

            NullPen = 8,

            OemFixedFont = 10,

            AnsiFixedFont = 11,

            AnsiVaribleFont = 12,

            SystemFont = 13,

            DeviceDefaultFont = 14,

            DefaultPalette = 15,

            SystemFixedFont = 16,
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Draws the reversible frame with the given background color for the specified control rectangle.
        /// </summary>
        /// <param name="g">The graphics object.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="rectangle">The rectangle.</param>
        public static void Draw(Graphics g, Color backgroundColor, Rectangle rectangle)
        {
            RasterOperation mode;
            Color alternateColor;
            if (backgroundColor.GetBrightness() < 0.5)
            {
                mode = RasterOperation.NOTXORPEN;
                alternateColor = Color.White;
            }
            else
            {
                mode = RasterOperation.XORPEN;
                alternateColor = Color.Black;
            }

            var hdc = g.GetHdc();

            try
            {
                IntPtr pen = CreatePen((int)PenStyle.Dot, 1, ColorTranslator.ToWin32(backgroundColor));

                int previousMode = SetROP2(new HandleRef(null, hdc), (int)mode);
                IntPtr previousBrush = SelectObject(new HandleRef(null, hdc), new HandleRef(null, GetStockObject((int)StockObject.NullBrush)));
                IntPtr previousPen = SelectObject(new HandleRef(null, hdc), new HandleRef(null, pen));
                SetBkColor(new HandleRef(null, hdc), ColorTranslator.ToWin32(alternateColor));

                Rectangle(new HandleRef(null, hdc), rectangle.X, rectangle.Y, rectangle.Right, rectangle.Bottom);

                SetROP2(new HandleRef(null, hdc), previousMode);
                SelectObject(new HandleRef(null, hdc), new HandleRef(null, previousBrush));
                SelectObject(new HandleRef(null, hdc), new HandleRef(null, previousPen));

                if (pen != IntPtr.Zero)
                {
                    DeleteObject(new HandleRef(null, pen));
                }
            }
            finally
            {
                g.ReleaseHdc();
            }
        }

        #endregion

        #region Methods

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreatePen(int style, int width, int color);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DeleteObject(HandleRef obj);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetStockObject(int index);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool Rectangle(HandleRef dc, int left, int top, int right, int bottom);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SelectObject(HandleRef dc, HandleRef obj);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SetBkColor(HandleRef dc, int color);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SetROP2(HandleRef dc, int drawMode);

        #endregion
    }
}
