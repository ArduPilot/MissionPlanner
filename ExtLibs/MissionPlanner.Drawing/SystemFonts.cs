namespace System.Drawing
{
    public class SystemFonts
    {
        // each access used to build a new Font (typeface lookup + native paint); build each once
        private static Font _defaultFont, _messageBoxFont, _captionFont;

        public static Font DefaultFont
        {
            get { return _defaultFont ?? (_defaultFont = new Font("Sans Serif", 8.25f) {isSystemFont = true}); }
        }
        public static Font MessageBoxFont   {
            get { return _messageBoxFont ?? (_messageBoxFont = new Font("Sans Serif", 8.25f) {isSystemFont = true}); }
        }

        public static Font CaptionFont    {
            get { return _captionFont ?? (_captionFont = new Font("Sans Serif", 8.25f) {isSystemFont = true}); }
        }
    }
}