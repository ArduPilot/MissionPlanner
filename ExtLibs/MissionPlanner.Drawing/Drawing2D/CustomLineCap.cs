namespace System.Drawing.Drawing2D
{
    public class CustomLineCap : MarshalByRefObject, ICloneable, IDisposable
    {
        public object Clone()
        {
            return this;
        }

        public void Dispose()
        {
        }
    }
}