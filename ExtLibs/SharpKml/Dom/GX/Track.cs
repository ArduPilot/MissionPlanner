using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using SharpKml.Base;

namespace SharpKml.Dom.GX
{
    /// <summary>
    /// Describes how an object moves through the world over a given time period.
    /// </summary>
    /// <remarks>This is not part of the OGC KML 2.2 standard.</remarks>
    [KmlElement("Track", KmlNamespaces.GX22Namespace)]
    public sealed class Track : Geometry
    {
        private static readonly XmlComponent AnglesComponent = new XmlComponent(null, "angles", KmlNamespaces.GX22Namespace);
        private static readonly XmlComponent CoordComponent = new XmlComponent(null, "coord", KmlNamespaces.GX22Namespace);
        private static readonly XmlComponent WhenComponent = new XmlComponent(null, "when", KmlNamespaces.Kml22Namespace);

        private ExtendedData _data;
        private Model _model;

        /// <summary>Initializes a new instance of the Track class.</summary>
        public Track()
        {
            this.RegisterValidChild<WhenElement>();
            this.RegisterValidChild<CoordElement>();
            this.RegisterValidChild<AnglesElement>();
        }

        /// <summary>
        /// Gets or sets how the altitude value should be interpreted.
        /// </summary>
        [KmlElement("altitudeMode")]
        public Dom.AltitudeMode? AltitudeMode { get; set; }

        /// <summary>
        /// Gets a collection of <see cref="Vector"/> containing the heading,
        /// tilt and roll for the icons and models.
        /// </summary>
        public IEnumerable<Angle> Angles
        {
            get
            {
                return from e in this.Children.OfType<AnglesElement>()
                       select e.Value;
            }
        }

        /// <summary>Gets a collection of coordinates for the Track.</summary>
        public IEnumerable<Vector> Coordinates
        {
            get
            {
                return from e in this.Children.OfType<CoordElement>()
                       select e.Value;
            }
        }

        /// <summary>
        /// Gets or sets custom data elements defined in a <see cref="Schema"/>
        /// earlier in the KML file.
        /// </summary>
        [KmlElement(null)]
        public ExtendedData ExtendedData
        {
            get { return _data; }
            set { this.UpdatePropertyChild(value, ref _data); }
        }

        /// <summary>
        /// Gets or sets extended altitude mode information.
        /// </summary>
        [KmlElement("altitudeMode", KmlNamespaces.GX22Namespace)]
        public GX.AltitudeMode? GXAltitudeMode { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Model"/> used to indicate the current
        /// position on the track.
        /// </summary>
        [KmlElement(null)]
        public Model Model
        {
            get { return _model; }
            set { this.UpdatePropertyChild(value, ref _model); }
        }

        /// <summary>
        /// Gets a collection of time values that corresponds to a position.
        /// </summary>
        public IEnumerable<string> When
        {
            get
            {
                return from e in this.Children.OfType<WhenElement>()
                       select e.InnerText;
            }
        }

        /// <summary>
        /// Adds the specified value to <see cref="Angles"/>.</summary>
        /// <param name="value">The value to add.</param>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        public void AddAngle(Angle value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.AddChild(new AnglesElement(value));
        }

        /// <summary>
        /// Adds the specified value to <see cref="Coordinates"/>.</summary>
        /// <param name="value">The value to add.</param>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        public void AddCoordinate(Vector value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.AddChild(new CoordElement(value));
        }

        /// <summary>
        /// Adds the specified value to <see cref="When"/>.</summary>
        /// <param name="value">The value to add.</param>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        public void AddWhen(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.AddChild(new WhenElement(value));
        }

        /// <summary>
        /// Processes the &lt;gx:angles&gt;, &lt;gx:coord&gt; and &lt;when&gt;
        /// elements.
        /// </summary>
        /// <param name="orphan">The <see cref="Element"/> to add.</param>
        protected internal override void AddOrphan(Element orphan)
        {
            UnknownElement unknown = orphan as UnknownElement;
            if (unknown != null)
            {
                XmlComponent data = unknown.UnknownData;
                if (AnglesComponent.Equals(data))
                {
                    this.AddChild(new AnglesElement(unknown.InnerText));
                    return;
                }
                if (CoordComponent.Equals(data))
                {
                    this.AddChild(new CoordElement(unknown.InnerText));
                    return;
                }
                if (WhenComponent.Equals(data))
                {
                    this.AddChild(new WhenElement(unknown.InnerText));
                    return;
                }
            }
            base.AddOrphan(orphan);
        }

        /// <summary>Used to correctly serialize a Vector.</summary>
        private abstract class VectorElement : Element, ICustomElement
        {
            
            private readonly double _x;
            
            private readonly double _y;
            
            private readonly double _z;

            /// <summary>Initializes a new instance of the VectorElement class.</summary>
            /// <param name="value">The value to serialize.</param>
            public VectorElement(string value)
            {
                // The vector is stored with ' ' as the separator.
                string[] values = value.Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);

                _x = GetValue(values, 0);
                _y = GetValue(values, 1);
                _z = GetValue(values, 2);
            }

            /// <summary>Gets the Vector value of this instance.</summary>
            /// <param name="x">The first value.</param>
            /// <param name="y">The second value.</param>
            /// <param name="z">The third value.</param>
            protected VectorElement(double x, double y, double z)
            {
                _x = x;
                _y = y;
                _z = z;
            }

            /// <summary>
            /// Gets a value indicating whether to process the children of the Element.
            /// </summary>
            public bool ProcessChildren
            {
                get { return false; }
            }

            /// <summary>Gets the name of the XML element.</summary>
            protected abstract string Name { get; }

            protected double X
            {
                get { return _x; }
            }
            protected double Y
            {
                get { return _y; }
            }
            protected double Z
            {
                get { return _z; }
            }
            /// <summary>Writes the start of an XML element.</summary>
            /// <param name="writer">An <see cref="XmlWriter"/> to write to.</param>
            public void CreateStartElement(XmlWriter writer)
            {
                string value = string.Format(KmlFormatter.Instance, "{0} {1} {2}", _x, _y, _z);

                writer.WriteElementString(KmlNamespaces.GX22Prefix, this.Name, KmlNamespaces.GX22Namespace, value);
            }

            private static double GetValue(string[] array, int index)
            {
                if (index < array.Length)
                {
                    double value;
                    if (double.TryParse(array[index], NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                    {
                        return value;
                    }
                }
                return default(double);
            }
        }

        /// <summary>Used to correctly serialize a Vector in Angles.</summary>
        private class AnglesElement : VectorElement
        {
            /// <summary>Initializes a new instance of the AnglesElement class.</summary>
            /// <param name="value">The value to serialize.</param>
            public AnglesElement(Angle value)
                : base(value.Heading, value.Pitch, value.Roll) // Must be stored in this order.
            {
            }

            /// <summary>Initializes a new instance of the AnglesElement class.</summary>
            /// <param name="value">The value to serialize.</param>
            public AnglesElement(string value)
                : base(value)
            {
            }
            public Angle Value
            {
                get { return new Angle(this.Y, this.X, this.Z); }
            }

            /// <summary>Gets the name of the XML element.</summary>
            protected override string Name
            {
                get { return "angles"; }
            }
        }

        /// <summary>Used to correctly serialize a Vector in Coordinates.</summary>
        private class CoordElement : VectorElement
        {
            /// <summary>Initializes a new instance of the CoordElement class.</summary>
            /// <param name="value">The value to serialize.</param>
            public CoordElement(Vector value)
                : base(value.Longitude, value.Latitude, value.Altitude.GetValueOrDefault()) // Must be stored in this order.
            {
            }

            /// <summary>Initializes a new instance of the CoordElement class.</summary>
            /// <param name="value">The value to serialize.</param>
            public CoordElement(string value)
                : base(value)
            {
            }
            public Vector Value
            {
                get { return new Vector(this.Y, this.X, this.Z); }
            }

            /// <summary>Gets the name of the XML element.</summary>
            protected override string Name
            {
                get { return "coord"; }
            }
        }

        /// <summary>Used to correctly serialize the strings in When.</summary>
        private class WhenElement : Element, ICustomElement
        {
            private readonly string _value;

            /// <summary>Initializes a new instance of the WhenElement class.</summary>
            /// <param name="value">
            /// The value to set the <see cref="Element.InnerText"/> to.
            /// </param>
            public WhenElement(string value)
            {
                _value = value;
            }

            /// <summary>
            /// Gets a value indicating whether to process the children of the Element.
            /// </summary>
            public bool ProcessChildren
            {
                get { return false; }
            }

            /// <summary>Writes the start of an XML element.</summary>
            /// <param name="writer">An <see cref="XmlWriter"/> to write to.</param>
            public void CreateStartElement(XmlWriter writer)
            {
                writer.WriteElementString("when", KmlNamespaces.Kml22Namespace, _value);
            }
        }
    }
}
