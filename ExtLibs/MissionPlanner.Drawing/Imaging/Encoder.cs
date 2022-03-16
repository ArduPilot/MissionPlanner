namespace System.Drawing.Imaging
{
    public sealed class Encoder
    {
        public static object Quality = new Encoder(new Guid(492561589, -1462, 17709, new byte[8]
        {
            156,
            221,
            93,
            179,
            81,
            5,
            231,
            235
        }));

        private Guid _guid;

        public Guid Guid => _guid;

        public Encoder(Guid guid)
        {
            _guid = guid;
        }
    }
}