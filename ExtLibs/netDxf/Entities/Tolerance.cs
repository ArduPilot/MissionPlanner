#region netDxf library, Copyright (C) 2009-2016 Daniel Carvajal (haplokuon@gmail.com)

//                        netDxf library
// Copyright (C) 2009-2016 Daniel Carvajal (haplokuon@gmail.com)
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Text;
using System.Text.RegularExpressions;
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a tolerance <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Tolerance :
        EntityObject
    {
        #region delegates and events

        public delegate void ToleranceStyleChangedEventHandler(Tolerance sender, TableObjectChangedEventArgs<DimensionStyle> e);

        public event ToleranceStyleChangedEventHandler ToleranceStyleChanged;

        protected virtual DimensionStyle OnDimensionStyleChangedEvent(DimensionStyle oldStyle, DimensionStyle newStyle)
        {
            ToleranceStyleChangedEventHandler ae = this.ToleranceStyleChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<DimensionStyle> eventArgs = new TableObjectChangedEventArgs<DimensionStyle>(oldStyle, newStyle);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newStyle;
        }

        #endregion

        #region private fields

        private ToleranceEntry entry1;
        private ToleranceEntry entry2;
        private string height;
        private bool showProjectedToleranceZoneSymbol;
        private string datumIdentifier;

        private DimensionStyle style;
        private Vector3 position;
        private double rotation;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Tolerance</c> class.
        /// </summary>
        public Tolerance()
            : this(null, Vector3.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Tolerance</c> class.
        /// </summary>
        /// <param name="tolerance"></param>
        public Tolerance(ToleranceEntry tolerance)
            : this(tolerance, Vector3.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Tolerance</c> class.
        /// </summary>
        /// <param name="tolerance"></param>
        /// <param name="position"></param>
        public Tolerance(ToleranceEntry tolerance, Vector2 position)
            : this(tolerance, new Vector3(position.X, position.Y, 0.0))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Tolerance</c> class.
        /// </summary>
        /// <param name="tolerance"></param>
        /// <param name="position"></param>
        public Tolerance(ToleranceEntry tolerance, Vector3 position)
            : base(EntityType.Tolerance, DxfObjectCode.Tolerance)
        {
            this.entry1 = tolerance;
            this.entry2 = null;
            this.height = string.Empty;
            this.showProjectedToleranceZoneSymbol = false;
            this.datumIdentifier = string.Empty;

            this.style = DimensionStyle.Default;
            this.position = position;
            this.rotation = 0.0;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the first tolerance entry.
        /// </summary>
        public ToleranceEntry Entry1
        {
            get { return this.entry1; }
            set { this.entry1 = value; }
        }

        /// <summary>
        /// Gets or sets the second tolerance entry.
        /// </summary>
        public ToleranceEntry Entry2
        {
            get { return this.entry2; }
            set { this.entry2 = value; }
        }

        /// <summary>
        /// Gets or sets the projected tolerance zone value.
        /// </summary>
        /// <remarks>
        /// A projected tolerance zone controls the variation in height of the extended portion of a fixed perpendicular part
        /// and refines the tolerance to that specified by positional tolerances.
        /// </remarks>
        public string Height
        {
            get { return this.height; }
            set { this.height = value; }
        }

        /// <summary>
        /// Gets or sets if the projected tolerance zone symbol will be shown after the projected tolerance zone value.
        /// </summary>
        public bool ShowProjectedToleranceZoneSymbol
        {
            get { return this.showProjectedToleranceZoneSymbol; }
            set { this.showProjectedToleranceZoneSymbol = value; }
        }

        /// <summary>
        /// Gets or sets the datum identifying symbol.
        /// </summary>
        /// <remarks>
        /// A datum is a theoretically exact geometric reference from which you can establish the location and tolerance zones of other features.
        /// A point, line, plane, cylinder, or other geometry can serve as a datum.
        /// </remarks>
        public string DatumIdentifier
        {
            get { return this.datumIdentifier; }
            set { this.datumIdentifier = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="DimensionStyle">leader style</see>.
        /// </summary>
        public DimensionStyle Style
        {
            get { return this.style; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.style = this.OnDimensionStyleChangedEvent(this.style, value);
            }
        }

        /// <summary>
        /// Gets or sets the leader <see cref="Vector3">position</see> in world coordinates.
        /// </summary>
        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        /// <summary>
        /// Gets or sets the leader rotation in degrees.
        /// </summary>
        public double Rotation
        {
            get { return this.rotation; }
            set { this.rotation = MathHelper.NormalizeAngle(value); }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Converts the actual tolerance to its string representation.
        /// </summary>
        /// <returns>The tolerance string representation.</returns>
        public string ToStringRepresentation()
        {
            StringBuilder value = new StringBuilder();
            bool newLine = false;

            if (this.entry1 != null)
            {
                value.Append(ToleranceEntryToString(this.entry1));
                newLine = true;
            }

            if (this.entry2 != null)
            {
                if (newLine)
                    value.Append("^J");

                value.Append(ToleranceEntryToString(this.entry2));
                newLine = true;
            }

            if (!(string.IsNullOrEmpty(this.height) && !this.showProjectedToleranceZoneSymbol))
            {
                if (newLine)
                    value.Append("^J");

                value.Append(this.height);
                if (this.showProjectedToleranceZoneSymbol)
                    value.Append("{\\Fgdt;p}");
                newLine = true;
            }

            if (!string.IsNullOrEmpty(this.datumIdentifier))
            {
                if (newLine)
                    value.Append("^J");

                value.Append(this.datumIdentifier);
            }

            return value.ToString();
        }

        /// <summary>
        /// Converts the string representation of a tolerance to its tolerance entity equivalent.
        /// </summary>
        /// <param name="s">A string that represents a tolerance to convert.</param>
        /// <returns>The Tolerance entity equivalent to the tolerance contained in s.</returns>
        public static Tolerance ParseRepresentation(string s)
        {
            string[] lines = Regex.Split(s, "\\^J");

            ToleranceEntry t1 = null;
            ToleranceEntry t2 = null;
            string height = string.Empty;
            bool showProjSymbol = false;
            string datumIdentifier = string.Empty;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.StartsWith("{") || line.StartsWith("%%v"))
                {
                    switch (i)
                    {
                        case 0:
                            t1 = ParseToleranceEntry(line);
                            break;
                        case 1:
                            t2 = ParseToleranceEntry(line);
                            break;
                    }
                }
                else
                {
                    if (i == lines.Length - 1)
                    {
                        datumIdentifier = line;
                    }
                    else
                    {
                        StringBuilder value = new StringBuilder();

                        CharEnumerator chars = line.GetEnumerator();
                        while (chars.MoveNext())
                        {
                            char token = chars.Current;
                            if (token == '{')
                            {
                                char symbol = Symbol(chars);
                                if (symbol == 'p')
                                    showProjSymbol = true;
                            }
                            else
                            {
                                value.Append(token);
                            }
                        }
                        height = value.ToString();
                    }
                }
            }

            Tolerance tolerance = new Tolerance
            {
                Entry1 = t1,
                Entry2 = t2,
                Height = height,
                ShowProjectedToleranceZoneSymbol = showProjSymbol,
                DatumIdentifier = datumIdentifier
            };

            return tolerance;
        }

        /// <summary>
        /// Tries to convert the specified string representation of a tolerance to its tolerance entity equivalent.
        /// A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="s">A string that represents the tolerance to convert.</param>
        /// <param name="result">If the conversion has been successful, it contains the tolerance entity equivalent to the string representation; otherwise, null.</param>
        /// <returns>True if the string was converted successfully; otherwise, false.</returns>
        public static bool TryParseRepresentation(string s, out Tolerance result)
        {
            try
            {
                result = ParseRepresentation(s);
            }
            catch
            {
                result = null;
                return false;
            }
            return true;
        }

        #endregion

        #region private ToString methods

        private static string ToleranceEntryToString(ToleranceEntry entry)
        {
            StringBuilder value = new StringBuilder();
            switch (entry.GeometricSymbol)
            {
                case ToleranceGeometricSymbol.None:
                    break;
                case ToleranceGeometricSymbol.Position:
                    value.Append("{\\Fgdt;j}");
                    break;
                case ToleranceGeometricSymbol.Concentricity:
                    value.Append("{\\Fgdt;r}");
                    break;
                case ToleranceGeometricSymbol.Symmetry:
                    value.Append("{\\Fgdt;i}");
                    break;
                case ToleranceGeometricSymbol.Parallelism:
                    value.Append("{\\Fgdt;f}");
                    break;
                case ToleranceGeometricSymbol.Perpendicularity:
                    value.Append("{\\Fgdt;b}");
                    break;
                case ToleranceGeometricSymbol.Angularity:
                    value.Append("{\\Fgdt;a}");
                    break;
                case ToleranceGeometricSymbol.Cylindricity:
                    value.Append("{\\Fgdt;g}");
                    break;
                case ToleranceGeometricSymbol.Flatness:
                    value.Append("{\\Fgdt;c}");
                    break;
                case ToleranceGeometricSymbol.Roundness:
                    value.Append("{\\Fgdt;e}");
                    break;
                case ToleranceGeometricSymbol.Straightness:
                    value.Append("{\\Fgdt;u}");
                    break;
                case ToleranceGeometricSymbol.ProfileSurface:
                    value.Append("{\\Fgdt;d}");
                    break;
                case ToleranceGeometricSymbol.ProfileLine:
                    value.Append("{\\Fgdt;k}");
                    break;
                case ToleranceGeometricSymbol.CircularRunout:
                    value.Append("{\\Fgdt;h}");
                    break;
                case ToleranceGeometricSymbol.TotalRunOut:
                    value.Append("{\\Fgdt;t}");
                    break;
            }

            value.Append(ToleranceValueToString(entry.Tolerance1));
            value.Append(ToleranceValueToString(entry.Tolerance2));
            value.Append(DatumValueToString(entry.Datum1));
            value.Append(DatumValueToString(entry.Datum2));
            value.Append(DatumValueToString(entry.Datum3));

            return value.ToString();
        }

        private static string ToleranceValueToString(ToleranceValue tolerance)
        {
            StringBuilder value = new StringBuilder();
            value.Append("%%v");

            if (tolerance != null)
            {
                if (tolerance.ShowDiameterSymbol)
                    value.Append("{\\Fgdt;n}");
                value.Append(tolerance.Value);
                switch (tolerance.MaterialCondition)
                {
                    case ToleranceMaterialCondition.None:
                        break;
                    case ToleranceMaterialCondition.Maximum:
                        value.Append("{\\Fgdt;m}");
                        break;
                    case ToleranceMaterialCondition.Least:
                        value.Append("{\\Fgdt;l}");
                        break;
                    case ToleranceMaterialCondition.Regardless:
                        value.Append("{\\Fgdt;s}");
                        break;
                }
            }

            return value.ToString();
        }

        private static string DatumValueToString(DatumReferenceValue datum)
        {
            StringBuilder value = new StringBuilder();
            value.Append("%%v");

            if (datum != null)
            {
                value.Append(datum.Value);
                switch (datum.MaterialCondition)
                {
                    case ToleranceMaterialCondition.None:
                        break;
                    case ToleranceMaterialCondition.Maximum:
                        value.Append("{\\Fgdt;m}");
                        break;
                    case ToleranceMaterialCondition.Least:
                        value.Append("{\\Fgdt;l}");
                        break;
                    case ToleranceMaterialCondition.Regardless:
                        value.Append("{\\Fgdt;s}");
                        break;
                }
            }

            return value.ToString();
        }

        #endregion

        #region private Parse methods

        private static ToleranceEntry ParseToleranceEntry(string line)
        {
            string[] values = Regex.Split(line, "%%v");

            ToleranceGeometricSymbol geom = ToleranceGeometricSymbol.None;
            ToleranceValue t1 = null;
            ToleranceValue t2 = null;
            DatumReferenceValue d1 = null;
            DatumReferenceValue d2 = null;
            DatumReferenceValue d3 = null;

            if (!string.IsNullOrEmpty(values[0]))
            {
                if (values[0].StartsWith("{"))
                {
                    // geometric symbol
                    CharEnumerator chars = values[0].GetEnumerator();
                    char symbol = Symbol(chars);
                    geom = ParseGeometricSymbol(symbol);
                }
            }

            for (int i = 1; i < values.Length; i++)
            {
                string value = values[i];

                switch (i)
                {
                    case 1:
                        t1 = ParseToleranceValue(value);
                        break;
                    case 2:
                        t2 = ParseToleranceValue(value);
                        break;
                    case 3:
                        d1 = ParseDatumReferenceValue(value);
                        break;
                    case 4:
                        d2 = ParseDatumReferenceValue(value);
                        break;
                    case 5:
                        d3 = ParseDatumReferenceValue(value);
                        break;
                    default:
                        throw new FormatException("The tolerance string representation is not well formatted");
                }
            }

            ToleranceEntry t = new ToleranceEntry
            {
                GeometricSymbol = geom,
                Tolerance1 = t1,
                Tolerance2 = t2,
                Datum1 = d1,
                Datum2 = d2,
                Datum3 = d3
            };

            return t;
        }

        private static char Symbol(CharEnumerator chars)
        {
            while (chars.MoveNext())
            {
                if (chars.Current == ';')
                {
                    if (chars.MoveNext())
                    {
                        char s = chars.Current;
                        if (chars.MoveNext())
                        {
                            if (chars.Current == '}')
                                return s;

                            throw new FormatException("The tolerance string representation is not well formatted");
                        }
                        throw new FormatException("The tolerance string representation is not well formatted");
                    }
                    throw new FormatException("The tolerance string representation is not well formatted");
                }
            }
            throw new FormatException("The tolerance string representation is not well formatted");
        }

        private static ToleranceGeometricSymbol ParseGeometricSymbol(char symbol)
        {
            ToleranceGeometricSymbol geom;
            switch (symbol)
            {
                case 'j':
                    geom = ToleranceGeometricSymbol.Position;
                    break;
                case 'r':
                    geom = ToleranceGeometricSymbol.Concentricity;
                    break;
                case 'i':
                    geom = ToleranceGeometricSymbol.Symmetry;
                    break;
                case 'f':
                    geom = ToleranceGeometricSymbol.Parallelism;
                    break;
                case 'b':
                    geom = ToleranceGeometricSymbol.Perpendicularity;
                    break;
                case 'a':
                    geom = ToleranceGeometricSymbol.Angularity;
                    break;
                case 'g':
                    geom = ToleranceGeometricSymbol.Cylindricity;
                    break;
                case 'c':
                    geom = ToleranceGeometricSymbol.Flatness;
                    break;
                case 'e':
                    geom = ToleranceGeometricSymbol.Roundness;
                    break;
                case 'u':
                    geom = ToleranceGeometricSymbol.Straightness;
                    break;
                case 'd':
                    geom = ToleranceGeometricSymbol.ProfileSurface;
                    break;
                case 'k':
                    geom = ToleranceGeometricSymbol.ProfileLine;
                    break;
                case 'h':
                    geom = ToleranceGeometricSymbol.CircularRunout;
                    break;
                case 't':
                    geom = ToleranceGeometricSymbol.TotalRunOut;
                    break;
                default:
                    geom = ToleranceGeometricSymbol.None;
                    break;
            }

            return geom;
        }

        private static ToleranceMaterialCondition ParseMaterialCondition(char symbol)
        {
            ToleranceMaterialCondition mat;
            switch (symbol)
            {
                case 'm':
                    mat = ToleranceMaterialCondition.Maximum;
                    break;
                case 'l':
                    mat = ToleranceMaterialCondition.Least;
                    break;
                case 's':
                    mat = ToleranceMaterialCondition.Regardless;
                    break;
                default:
                    mat = ToleranceMaterialCondition.None;
                    break;
            }

            return mat;
        }

        private static ToleranceValue ParseToleranceValue(string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;

            bool hasDiameterSymbol = false;
            StringBuilder value = new StringBuilder();
            ToleranceMaterialCondition mat = ToleranceMaterialCondition.None;

            CharEnumerator chars = s.GetEnumerator();
            while (chars.MoveNext())
            {
                char token = chars.Current;
                if (token == '{')
                {
                    char symbol = Symbol(chars);
                    if (symbol == 'n')
                        hasDiameterSymbol = true;
                    else
                        mat = ParseMaterialCondition(symbol);
                }
                else
                {
                    value.Append(token);
                }
            }

            ToleranceValue t = new ToleranceValue
            {
                ShowDiameterSymbol = hasDiameterSymbol,
                Value = value.ToString(),
                MaterialCondition = mat
            };

            return t;
        }

        private static DatumReferenceValue ParseDatumReferenceValue(string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;

            StringBuilder value = new StringBuilder();
            ToleranceMaterialCondition mat = ToleranceMaterialCondition.None;

            CharEnumerator chars = s.GetEnumerator();
            while (chars.MoveNext())
            {
                char token = chars.Current;
                if (token == '{')
                {
                    char symbol = Symbol(chars);
                    mat = ParseMaterialCondition(symbol);
                }
                else
                {
                    value.Append(token);
                }
            }

            DatumReferenceValue d = new DatumReferenceValue
            {
                Value = value.ToString(),
                MaterialCondition = mat
            };

            return d;
        }

        #endregion

        #region overrides

        public override object Clone()
        {
            Tolerance entity = new Tolerance
            {
                //EntityObject properties
                Layer = (Layer) this.Layer.Clone(),
                Linetype = (Linetype) this.Linetype.Clone(),
                Color = (AciColor) this.Color.Clone(),
                Lineweight = this.Lineweight,
                Transparency = (Transparency) this.Transparency.Clone(),
                LinetypeScale = this.LinetypeScale,
                Normal = this.Normal,
                IsVisible = this.IsVisible,
                //Tolerance properties
                Entry1 = (ToleranceEntry) this.entry1.Clone(),
                Entry2 = (ToleranceEntry) this.entry2.Clone(),
                Height = this.height,
                ShowProjectedToleranceZoneSymbol = this.showProjectedToleranceZoneSymbol,
                DatumIdentifier = this.datumIdentifier,
                Style = (DimensionStyle) this.style.Clone(),
                Position = this.position,
                Rotation = this.rotation
            };

            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData) data.Clone());

            return entity;
        }

        #endregion
    }
}