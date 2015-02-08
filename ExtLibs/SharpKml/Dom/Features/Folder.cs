using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Used to organize <see cref="Feature"/> elements hierarchically.</summary>
    /// <remarks>OGC KML 2.2 Section 9.10</remarks>
    [KmlElement("Folder")]
    public sealed class Folder : Container
    {
        /// <summary>Initializes a new instance of the Folder class.</summary>
        public Folder()
        {
            this.RegisterValidChild<Feature>();
        }

        public Folder(string name)
        {
            this.RegisterValidChild<Feature>();
            this.Name = name;
        }
    }
}
