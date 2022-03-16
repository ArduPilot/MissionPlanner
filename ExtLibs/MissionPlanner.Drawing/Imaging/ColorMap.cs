namespace System.Drawing.Imaging
{
    public sealed class ColorMap
    {
        private Color _oldColor;

        private Color _newColor;

        public Color OldColor
        {
            get { return _oldColor; }
            set { _oldColor = value; }
        }

        public Color NewColor
        {
            get { return _newColor; }
            set { _newColor = value; }
        }
    }
}