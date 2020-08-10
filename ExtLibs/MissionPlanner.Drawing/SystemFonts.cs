namespace System.Drawing
{
    public class SystemFonts
    {
        public static Font DefaultFont { get; set; } = new Font("", 12); // points
        public static Font MessageBoxFont { get; set; } = new Font("", 12);

        public static Font CaptionFont { get; set; } = new Font("", 12);
    }
}