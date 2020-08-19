namespace System.Drawing.Imaging
{
    public sealed class FrameDimension
    {
        private static FrameDimension time = new FrameDimension(new Guid("{6aedbd6d-3fb5-418a-83a6-7f45229dc872}"));

        private static FrameDimension resolution =
            new FrameDimension(new Guid("{84236f7b-3bd3-428f-8dab-4ea1439ca315}"));

        private static FrameDimension page = new FrameDimension(new Guid("{7462dc86-6180-4c7e-8e3f-ee7333a7a483}"));

        private Guid guid;

        public Guid Guid => guid;

        public static FrameDimension Time => time;

        public static FrameDimension Resolution => resolution;

        public static FrameDimension Page => page;

        public FrameDimension(Guid guid)
        {
            this.guid = guid;
        }

        public override bool Equals(object o)
        {
            FrameDimension frameDimension = o as FrameDimension;
            if (frameDimension == null)
            {
                return false;
            }

            return guid == frameDimension.guid;
        }

        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }

        public override string ToString()
        {
            if (this == time)
            {
                return "Time";
            }

            if (this == resolution)
            {
                return "Resolution";
            }

            if (this == page)
            {
                return "Page";
            }

            return "[FrameDimension: " + guid + "]";
        }
    }
}