using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Represents a series of points.</summary>
    /// <remarks>OGC KML 2.2 Section 16.9</remarks>
    [KmlElement("coordinates")]
    public sealed class CoordinateCollection : Element, ICollection<Vector>, ICustomElement
    {
        private static readonly Regex Expression = CreateRegex();

        private List<Vector> _points = new List<Vector>();

        /// <summary>
        /// Gets the number of points contained in this instance.
        /// </summary>
        public int Count
        {
            get { return _points.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read-only.
        /// </summary>
        bool ICollection<Vector>.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether to process the children of the Element.
        /// </summary>
        bool ICustomElement.ProcessChildren
        {
            get { return true; }
        }

        /// <summary>Gets the value at the specified index.</summary>
        /// <param name="index">The zero-based index of the value to get.</param>
        /// <returns>The value at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than 0 or index is equal to or greater than
        /// <see cref="Count"/>.
        /// </exception>
        internal Vector this[int index]
        {
            get { return _points[index]; }
        }

        /// <summary>Adds a point to this instance.</summary>
        /// <param name="item">The point to be added.</param>
        /// <exception cref="ArgumentNullException">item is null.</exception>
        public void Add(Vector item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            _points.Add(item);
        }

        /// <summary>Removes all points from this instance.</summary>
        public void Clear()
        {
            _points.Clear();
        }

        /// <summary>
        /// Determines whether a point is contained in this instance.
        /// </summary>
        /// <param name="item">The point to locate.</param>
        /// <returns>
        /// true if the point is found in this instance; otherwise, false. This
        /// method also returns false if the specified value parameter is null.
        /// </returns>
        public bool Contains(Vector item)
        {
            return _points.Contains(item);
        }

        /// <summary>
        /// Copies this instance to a compatible one-dimensional array, starting
        /// at the specified index of the target array.
        /// </summary>
        /// <param name="array">The destination one-dimensional array.</param>
        /// <param name="arrayIndex">
        /// The zero-based index in array at which copying begins.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The number of points contained in this instance is greater than the
        /// available space from arrayIndex to the end of the destination array.
        /// </exception>
        /// <exception cref="ArgumentNullException">array is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// arrayIndex is less than 0.
        /// </exception>
        public void CopyTo(Vector[] array, int arrayIndex)
        {
            _points.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through this instance.
        /// </summary>
        /// <returns>An enumerator for this instance.</returns>
        public IEnumerator<Vector> GetEnumerator()
        {
            return _points.GetEnumerator();
        }

        /// <summary>
        /// Removes the first occurrence of a specific point from this instance.
        /// </summary>
        /// <param name="item">The point to remove.</param>
        /// <returns>
        /// true if the specified value parameter is successfully removed;
        /// otherwise, false. This method also returns false if the specified
        /// value parameter was not found or is null.
        /// </returns>
        public bool Remove(Vector item)
        {
            return _points.Remove(item);
        }

        /// <summary>Writes the start of an XML element.</summary>
        /// <param name="writer">An <see cref="XmlWriter"/> to write to.</param>
        void ICustomElement.CreateStartElement(XmlWriter writer)
        {
            // This element is being serialized so we need to update the InnerText
            StringBuilder sb = new StringBuilder();
            foreach (var point in _points)
            {
                if (point.Altitude != null)
                {
                    sb.AppendFormat(
                        KmlFormatter.Instance,
                        "{0},{1},{2}\n",
                        point.Longitude,
                        point.Latitude,
                        point.Altitude.Value);
                }
                else
                {
                    sb.AppendFormat(
                        KmlFormatter.Instance,
                        "{0},{1}\n",
                        point.Longitude,
                        point.Latitude);
                }
            }
            this.ClearInnerText();
            base.AddInnerText(sb.ToString()); // Don't use our overloaded version

            writer.WriteStartElement("coordinates", KmlNamespaces.Kml22Namespace);
        }

        /// <summary>
        /// Returns an enumerator that iterates through this instance.
        /// </summary>
        /// <returns>An enumerator for this instance.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>Parses the inner text of the XML element.</summary>
        /// <param name="text">The text content of the XML element.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Adding to the buffer would exceed <see cref="StringBuilder.MaxCapacity"/>.
        /// </exception>
        protected internal override void AddInnerText(string text)
        {
            base.AddInnerText(text);
            this.Parse(this.InnerText);
        }

        private static Regex CreateRegex()
        {
            // 16.9.1 coordinatesType description
            // String representing one or more coordinate tuples, with each tuple
            // consisting of decimal values for geodetic longitude, geodetic
            // latitude, and altitude. The altitude component is optional. The
            // coordinate separator is a comma and the tuple separator is a
            // whitespace. Longitude and latitude coordinates are expressed in
            // decimal degrees only.
            const string Seperator = @"\s*,\s*";
            const string Token = @"[^\s,]+";
            const string Longitude = "(?<lon>" + Token + ")" + Seperator;
            const string Latitude = "(?<lat>" + Token + ")";
            const string Altitude = "(?:" + Seperator + "(?<alt>" + Token + "))?"; // Optional
            const string Expression = Longitude + Latitude + Altitude + "\\b";

            return new Regex(Expression, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        }

        private void Parse(string input)
        {
            _points.Clear();
            foreach (Match match in Expression.Matches(input))
            {
                // Minimum required fields for a valid coordinate are latitude and longitude.
                double latitude, longitude;
                if (double.TryParse(match.Groups["lat"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out latitude) &&
                    double.TryParse(match.Groups["lon"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out longitude))
                {
                    Group altitudeGroup = match.Groups["alt"];
                    if (altitudeGroup.Success)
                    {
                        double altitude;
                        if (double.TryParse(altitudeGroup.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out altitude))
                        {
                            _points.Add(new Vector(latitude, longitude, altitude));
                            continue; // Success!
                        }
                    }
                    _points.Add(new Vector(latitude, longitude));
                }
            }
        }
    }
}
