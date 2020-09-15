namespace System.Drawing.Imaging
{
    public class EncoderParameters
    {
        private int v;

        public EncoderParameters(int v)
        {
            this.v = v;
        }

        private EncoderParameter[] _param = new EncoderParameter[2];

        public EncoderParameter[] Param
        {
            get { return _param; }
            set { _param = value; }
        }
    }
}