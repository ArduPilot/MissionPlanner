/*
    Copyright © 2003 RiskCare Ltd. All rights reserved.
    Copyright © 2010 SvgNet & SvgGdi Bridge Project. All rights reserved.
    Copyright © 2015, 2017 Rafael Teixeira, Mojmír Němeček, Benjamin Peterson and Other Contributors

    Original source code licensed with BSD-2-Clause spirit, treat it thus, see accompanied LICENSE for more
*/

using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SvgNet.SvgTypes
{
    /// <summary>
    /// The units in which an SvgAngle can be specified
    /// </summary>
    public enum SvgAngleType
    {
        SVG_ANGLETYPE_UNKNOWN = 0,
        SVG_ANGLETYPE_UNSPECIFIED = 1,
        SVG_ANGLETYPE_DEG = 2,
        SVG_ANGLETYPE_RAD = 3,
        SVG_ANGLETYPE_GRAD = 4,
    }

    /// <summary>
    /// The various units in which an SvgLength can be specified.
    /// </summary>
    public enum SvgLengthType
    {
        SVG_LENGTHTYPE_UNKNOWN = 0,
        SVG_LENGTHTYPE_NUMBER = 1,
        SVG_LENGTHTYPE_PERCENTAGE = 2,
        SVG_LENGTHTYPE_EMS = 3,
        SVG_LENGTHTYPE_EXS = 4,
        SVG_LENGTHTYPE_PX = 5,
        SVG_LENGTHTYPE_CM = 6,
        SVG_LENGTHTYPE_MM = 7,
        SVG_LENGTHTYPE_IN = 8,
        SVG_LENGTHTYPE_PT = 9,
        SVG_LENGTHTYPE_PC = 10,
    }

    /// <summary>
    /// The various different types of segment that make up an SVG path, as listed in the SVG Path grammar.
    /// </summary>
    public enum SvgPathSegType
    {
        SVG_SEGTYPE_UNKNOWN = 0,
        SVG_SEGTYPE_MOVETO,
        SVG_SEGTYPE_CLOSEPATH,
        SVG_SEGTYPE_LINETO,
        SVG_SEGTYPE_HLINETO,
        SVG_SEGTYPE_VLINETO,
        SVG_SEGTYPE_CURVETO,
        SVG_SEGTYPE_SMOOTHCURVETO,
        SVG_SEGTYPE_BEZIERTO,
        SVG_SEGTYPE_SMOOTHBEZIERTO,
        SVG_SEGTYPE_ARCTO
    }

    /// <summary>
    /// A segment in an Svg path.  This is not a real SVG type; it is not in the SVG spec.  It is provided for making paths
    /// easier to specify and parse.
    /// </summary>
    public class PathSeg : ICloneable
    {
        public bool _abs;
        public float[] _data;
        public SvgPathSegType _type;

        public PathSeg(SvgPathSegType t, bool a, float[] arr)
        {
            _type = t;
            _abs = a;
            _data = arr;
        }

        public bool Abs { get { return _abs; } }

        public string Char
        {
            get
            {
                switch (_type)
                {
                    case SvgPathSegType.SVG_SEGTYPE_MOVETO: return (_abs ? "M" : "m");
                    case SvgPathSegType.SVG_SEGTYPE_CLOSEPATH: return "z";
                    case SvgPathSegType.SVG_SEGTYPE_LINETO: return (_abs ? "L" : "l");
                    case SvgPathSegType.SVG_SEGTYPE_HLINETO: return (_abs ? "H" : "h");
                    case SvgPathSegType.SVG_SEGTYPE_VLINETO: return (_abs ? "V" : "v");
                    case SvgPathSegType.SVG_SEGTYPE_CURVETO: return (_abs ? "C" : "c");
                    case SvgPathSegType.SVG_SEGTYPE_SMOOTHCURVETO: return (_abs ? "S" : "s");
                    case SvgPathSegType.SVG_SEGTYPE_BEZIERTO: return (_abs ? "Q" : "q");
                    case SvgPathSegType.SVG_SEGTYPE_SMOOTHBEZIERTO: return (_abs ? "T" : "t");
                    case SvgPathSegType.SVG_SEGTYPE_ARCTO: return (_abs ? "A" : "a");
                }

                throw new SvgException("Invalid PathSeg type", _type.ToString());
            }
        }

        public float[] Data { get { return _data; } }

        public SvgPathSegType Type { get { return _type; } }

        public object Clone()
        {
            return new PathSeg(_type, _abs, (float[])_data.Clone());
        }
    };

    /// <summary>
    /// An angle, as found here and there throughout the SVG spec
    /// </summary>
    public class SvgAngle : ICloneable
    {
        public SvgAngle(string s)
        {
            FromString(s);
        }

        public SvgAngle(float num, SvgAngleType type)
        {
            Value = num;
            Type = type;
        }

        public SvgAngleType Type { get; set; }

        public float Value { get; set; }

        public static implicit operator SvgAngle(string s)
        {
            return new SvgAngle(s);
        }

        public object Clone()
        {
            return new SvgAngle(Value, Type);
        }

        public void FromString(string s)
        {
            var i = s.LastIndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
            if (i == -1)
                return;

            Value = int.Parse(s.Substring(0, i + 1), CultureInfo.InvariantCulture);

            switch (s.Substring(i + 1))
            {
                case "grad":
                    Type = SvgAngleType.SVG_ANGLETYPE_GRAD;
                    break;

                case "rad":
                    Type = SvgAngleType.SVG_ANGLETYPE_RAD;
                    break;

                case "deg":
                    Type = SvgAngleType.SVG_ANGLETYPE_DEG;
                    break;

                case "":
                    Type = SvgAngleType.SVG_ANGLETYPE_UNSPECIFIED;
                    break;

                default:
                    throw new SvgException("Invalid SvgAngle", s);
            }
        }

        public override string ToString()
        {
            var s = Value.ToString("F", CultureInfo.InvariantCulture);
            switch (Type)
            {
                case SvgAngleType.SVG_ANGLETYPE_DEG:
                case SvgAngleType.SVG_ANGLETYPE_UNSPECIFIED:
                    s += "deg";
                    break;

                case SvgAngleType.SVG_ANGLETYPE_GRAD:
                    s += "grad";
                    break;

                case SvgAngleType.SVG_ANGLETYPE_RAD:
                    s += "rad";
                    break;
            }
            return s;
        }
    }

    /// <summary>
    /// A color, as found in CSS2 and used in SVG.  As well as a GDI Color object, SvgColor stores
    /// the string it was initialized from, so that when a color specified as 'black' is written out,
    /// it will be written 'black' rather than '#000000'
    /// </summary>
    public class SvgColor : ICloneable
    {
        public SvgColor(string s)
        {
            FromString(s);
        }

        public SvgColor(Color c)
        {
            Color = c;
        }

        public SvgColor(Color c, string s)
        {
            Color = c;
            _original_string = s;
        }

        public Color Color { get; set; }

        public static implicit operator SvgColor(Color c)
        {
            return new SvgColor(c);
        }

        public static implicit operator SvgColor(string s)
        {
            return new SvgColor(s);
        }

        public object Clone()
        {
            return new SvgColor(Color, _original_string);
        }

        /// <summary>
        /// As well as parsing the four types of CSS color descriptor (rgb, #xxxxxx, color name, and system color name),
        /// the FromString of this type stores the original string
        /// </summary>
        /// <param name="s"></param>
        public void FromString(string s)
        {
            _original_string = s;

            if (s.StartsWith("#"))
            {
                FromHexString(s);
                return;
            }

            var rg = new Regex(@"[rgbRGB]{3}");
            if (rg.Match(s).Success)
            {
                FromRGBString(s);
                return;
            }

            Color = Color.FromName(s);

            if (Color.A == 0)
                throw new SvgException("Invalid SvgColor", s);
        }

        /// <summary>
        /// If the SvgColor was constructed from a string, use that string; otherwise use rgb() form
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_original_string != null)
                return _original_string;

            var s = "rgb(";
            s += Color.R.ToString();
            s += ",";
            s += Color.G.ToString();
            s += ",";
            s += Color.B.ToString();
            s += ")";

            return s;
        }

        private static readonly Hashtable _stdcols = new Hashtable();
        private string _original_string;

        private void FromHexString(string s)
        {
            int r, g, b;
            s = s.Substring(1);

            if (s.Length == 3)
            {
                r = int.Parse(s.Substring(0, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                g = int.Parse(s.Substring(1, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                b = int.Parse(s.Substring(2, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                r += r * 16;
                g += g * 16;
                b += b * 16;
                Color = Color.FromArgb(r, g, b);
            }
            else if (s.Length == 6)
            {
                r = int.Parse(s.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                g = int.Parse(s.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                b = int.Parse(s.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                Color = Color.FromArgb(r, g, b);
            }
            else
            {
                throw new SvgException("Invalid SvgColor", s);
            }
        }

        private void FromRGBString(string s)
        {
#pragma warning disable CC0021 // Use nameof
            int r, g, b;
            var rg = new Regex(@"[rgbRGB ]+\( *(?<r>\d+)[, ]+(?<g>\d+)[, ]+(?<b>\d+) *\)");
            var m = rg.Match(s);
            if (m.Success)
            {
                r = int.Parse(m.Groups["r"].Captures[0].Value, CultureInfo.InvariantCulture);
                g = int.Parse(m.Groups["g"].Captures[0].Value, CultureInfo.InvariantCulture);
                b = int.Parse(m.Groups["b"].Captures[0].Value, CultureInfo.InvariantCulture);

                Color = Color.FromArgb(r, g, b);
                return;
            }

            rg = new Regex(@"[rgbRGB ]+\( *(?<r>\d+)%[, ]+(?<g>\d+)%[, ]+(?<b>\d+)% *\)");
            m = rg.Match(s);
            if (m.Success)
            {
                r = int.Parse(m.Groups["r"].Captures[0].Value, CultureInfo.InvariantCulture) * 255 / 100;
                g = int.Parse(m.Groups["g"].Captures[0].Value, CultureInfo.InvariantCulture) * 255 / 100;
                b = int.Parse(m.Groups["b"].Captures[0].Value, CultureInfo.InvariantCulture) * 255 / 100;

                Color = Color.FromArgb(r, g, b);
                return;
            }
#pragma warning restore CC0021 // Use nameof

            throw new SvgException("Invalid SvgColor", s);
        }
    }

    /// <summary>
    /// A length or coordinate component (in SVG 1.1 the specification says they are the same)
    /// </summary>
    public class SvgLength : ICloneable
    {
        public SvgLength(string s)
        {
            FromString(s);
        }

        public SvgLength(float f)
        {
            Value = f;
            Type = SvgLengthType.SVG_LENGTHTYPE_UNKNOWN;
        }

        public SvgLength(float f, SvgLengthType type)
        {
            Value = f;
            Type = type;
        }

        public SvgLengthType Type { get; set; }

        public float Value { get; set; }

        public static implicit operator SvgLength(string s)
        {
            return new SvgLength(s);
        }

        public static implicit operator SvgLength(float s)
        {
            return new SvgLength(s);
        }

        public object Clone()
        {
            return new SvgLength(Value, Type);
        }

        public void FromString(string s)
        {
            var i = s.LastIndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
            if (i == -1)
                return;

            Value = float.Parse(s.Substring(0, i + 1), CultureInfo.InvariantCulture);

            switch (s.Substring(i + 1))
            {
                case "%":
                    Type = SvgLengthType.SVG_LENGTHTYPE_PERCENTAGE;
                    break;

                case "em":
                    Type = SvgLengthType.SVG_LENGTHTYPE_EMS;
                    break;

                case "ex":
                    Type = SvgLengthType.SVG_LENGTHTYPE_EXS;
                    break;

                case "px":
                    Type = SvgLengthType.SVG_LENGTHTYPE_PX;
                    break;

                case "cm":
                    Type = SvgLengthType.SVG_LENGTHTYPE_CM;
                    break;

                case "mm":
                    Type = SvgLengthType.SVG_LENGTHTYPE_MM;
                    break;

                case "in":
                    Type = SvgLengthType.SVG_LENGTHTYPE_IN;
                    break;

                case "pt":
                    Type = SvgLengthType.SVG_LENGTHTYPE_PT;
                    break;

                case "pc":
                    Type = SvgLengthType.SVG_LENGTHTYPE_PC;
                    break;

                case "":
                    Type = SvgLengthType.SVG_LENGTHTYPE_UNKNOWN;
                    break;

                default:
                    throw new SvgException("Invalid SvgLength", s);
            }
        }

        public override string ToString()
        {
            var s = Value.ToString("F", CultureInfo.InvariantCulture);
            switch (Type)
            {
                case SvgLengthType.SVG_LENGTHTYPE_PERCENTAGE:
                    s += "%";
                    break;

                case SvgLengthType.SVG_LENGTHTYPE_EMS:
                    s += "em";
                    break;

                case SvgLengthType.SVG_LENGTHTYPE_EXS:
                    s += "ex";
                    break;

                case SvgLengthType.SVG_LENGTHTYPE_PX:
                    s += "px";
                    break;

                case SvgLengthType.SVG_LENGTHTYPE_CM:
                    s += "cm";
                    break;

                case SvgLengthType.SVG_LENGTHTYPE_MM:
                    s += "mm";
                    break;

                case SvgLengthType.SVG_LENGTHTYPE_IN:
                    s += "in";
                    break;

                case SvgLengthType.SVG_LENGTHTYPE_PT:
                    s += "pt";
                    break;

                case SvgLengthType.SVG_LENGTHTYPE_PC:
                    s += "pc";
                    break;
            }
            return s;
        }
    }

    /// <summary>
    /// A number, as specified in the SVG standard.  It is stored as a float.
    /// </summary>
    public class SvgNumber : ICloneable
    {
        public SvgNumber(string s)
        {
            FromString(s);
        }

        public SvgNumber(int n)
        {
            _num = n;
        }

        public SvgNumber(float n)
        {
            _num = n;
        }

        public static implicit operator SvgNumber(string s)
        {
            return new SvgNumber(s);
        }

        public static implicit operator SvgNumber(int n)
        {
            return new SvgNumber(n);
        }

        public static implicit operator SvgNumber(float n)
        {
            return new SvgNumber(n);
        }

        public object Clone()
        {
            return new SvgNumber(_num);
        }

        /// <summary>
        /// float.Parse is used to parse the string.  float.Parse does not follow the exact rules of the SVG spec.
        /// </summary>
        /// <param name="s"></param>
        public void FromString(string s)
        {
            try
            {
                _num = float.Parse(s, CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new SvgException("Invalid SvgNumber", s);
            }
        }

        /// <summary>
        /// float.ToString is used to output a string.  This is true for all numbers in SvgNet.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _num.ToString("F", CultureInfo.InvariantCulture);
        }

        private float _num;
    }

    /*
        /// <summary>
        /// A rectangle.  The Svg spec does not define a rectangle type and this should probably be replaced with a
        /// number list
        /// </summary>
        public class SvgRect
        {
            RectangleF _rc;

            public SvgRect(string s)
            {
                FromString(s);
            }

            public SvgRect(RectangleF rc)
            {
                _rc = rc;
            }

            public void FromString(string s)
            {
                string[] toks = s.Split(new char[]{',', ' '});
                if (toks.Length != 4)
                    throw new SvgException("Invalid SvgRect", s);
                try
                {
                    _rc.X = float.Parse(toks[0].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                    _rc.Y = float.Parse(toks[1].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                    _rc.Width = float.Parse(toks[2].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                    _rc.Height = float.Parse(toks[3].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                }
                catch(Exception )
                {
                    throw new SvgException("Invalid SvgRect", s);
                }
            }

            public override string ToString()
            {
                string s = "";

                s += _rc.X.ToString();  s += ",";
                s += _rc.Y.ToString();  s += ",";
                s += _rc.Width.ToString();  s += ",";
                s += _rc.Height.ToString();

                return s;
            }

            public static implicit operator SvgRect(string s)
            {
                return new SvgRect(s);
            }
        }
    */

    /// <summary>
    /// A number list, as used in the SVG spec for e.g. the value of a viewBox attribute.  Basically a list of numbers in
    /// any format separated by whitespace and commas.
    /// </summary>
    public class SvgNumList : ICloneable
    {
        public SvgNumList(string s)
        {
            FromString(s);
        }

        public SvgNumList(float[] pts)
        {
            foreach (float p in pts)
            {
                _pts.Add(p);
            }
        }

        public int Count
        {
            get { return _pts.Count; }
        }

        public float this[int idx]
        {
            get { return (float)_pts[idx]; }
            set { _pts[idx] = value; }
        }

        public static implicit operator SvgNumList(string s)
        {
            return new SvgNumList(s);
        }

        public static implicit operator SvgNumList(float[] f)
        {
            return new SvgNumList(f);
        }

        public static float[] String2Floats(string s)
        {
            try
            {
                var sa = s.Split(new char[] { ',', ' ', '\t', '\r', '\n' });

                var arr = new ArrayList();

                foreach (string str in sa)
                {
                    if (str != "")
                    {
                        str.Trim();
                        arr.Add(float.Parse(str, CultureInfo.InvariantCulture));
                    }
                }

                return (float[])arr.ToArray(typeof(float));
            }
            catch (Exception)
            {
                throw new SvgException("Invalid number list", s);
            }
        }

        public object Clone()
        {
            return new SvgNumList((float[])_pts.ToArray(typeof(float)));
        }

        public void FromString(string s)
        {
            try
            {
                var fa = String2Floats(s);

                foreach (float f in fa)
                {
                    _pts.Add(f);
                }
            }
            catch (Exception)
            {
                throw new SvgException("Invalid SvgNumList", s);
            }
        }

        public override string ToString()
        {
            var builder = new System.Text.StringBuilder();
            foreach (float f in _pts)
            {
                builder.Append(f.ToString("F", CultureInfo.InvariantCulture)).Append(" ");
            }

            return builder.ToString();
        }

        private readonly ArrayList _pts = new ArrayList();
    }

    /// <summary>
    /// A path, composed of segments, as described in the SVG 1.1 spec section 8.3
    /// </summary>
    public class SvgPath : ICloneable
    {
        public SvgPath(string s)
        {
            FromString(s);
        }

        public int Count
        {
            get { return _path.Count; }
        }

        public PathSeg this[int idx]
        {
            get { return (PathSeg)_path[idx]; }
            set { _path[idx] = value; }
        }

        public static implicit operator SvgPath(string s)
        {
            return new SvgPath(s);
        }

        public object Clone()
        {
            //we resort to using to/from string rather than writing an efficient clone, for the moment.
            return new SvgPath(this.ToString());
        }

        /// <summary>
        /// The parsing of the path is not completely perfect yet.  You can only have one space between path elements.
        /// </summary>
        /// <param name="s"></param>
        public void FromString(string s)
        {
            var sa = s.Split(new char[] { ' ', ',', '\t', '\r', '\n' });

            PathSeg ps;
            var datasize = 0;
            var pt = SvgPathSegType.SVG_SEGTYPE_UNKNOWN;
            var abs = false;
            var i = 0;
            char segTypeChar;
            _path = new ArrayList();

            while (i < sa.Length)
            {
                if (sa[i] == "")
                {
                    i += 1;
                    continue;
                }

                //check for a segment-type character

                if (char.IsLetter(sa[i][0]))
                {
                    segTypeChar = sa[i][0];

                    if (segTypeChar == 'M' || segTypeChar == 'm')
                    {
                        pt = SvgPathSegType.SVG_SEGTYPE_MOVETO;
                        abs = (segTypeChar == 'M');
                        datasize = 2;
                    }
                    else if (segTypeChar == 'Z' || segTypeChar == 'z')
                    {
                        pt = SvgPathSegType.SVG_SEGTYPE_CLOSEPATH;
                        datasize = 0;
                    }
                    else if (segTypeChar == 'L' || segTypeChar == 'l')
                    {
                        pt = SvgPathSegType.SVG_SEGTYPE_LINETO;
                        abs = (segTypeChar == 'L');
                        datasize = 2;
                    }
                    else if (segTypeChar == 'H' || segTypeChar == 'h')
                    {
                        pt = SvgPathSegType.SVG_SEGTYPE_HLINETO;
                        abs = (segTypeChar == 'H');
                        datasize = 1;
                    }
                    else if (segTypeChar == 'V' || segTypeChar == 'v')
                    {
                        pt = SvgPathSegType.SVG_SEGTYPE_VLINETO;
                        abs = (segTypeChar == 'V');
                        datasize = 1;
                    }
                    else if (segTypeChar == 'C' || segTypeChar == 'c')
                    {
                        pt = SvgPathSegType.SVG_SEGTYPE_CURVETO;
                        abs = (segTypeChar == 'C');
                        datasize = 6;
                    }
                    else if (segTypeChar == 'S' || segTypeChar == 's')
                    {
                        pt = SvgPathSegType.SVG_SEGTYPE_SMOOTHCURVETO;
                        abs = (segTypeChar == 'S');
                        datasize = 4;
                    }
                    else if (segTypeChar == 'Q' || segTypeChar == 'q')
                    {
                        pt = SvgPathSegType.SVG_SEGTYPE_BEZIERTO;
                        abs = (segTypeChar == 'Q');
                        datasize = 4;
                    }
                    else if (segTypeChar == 'T' || segTypeChar == 't')
                    {
                        pt = SvgPathSegType.SVG_SEGTYPE_SMOOTHBEZIERTO;
                        abs = (segTypeChar == 'T');
                        datasize = 2;
                    }
                    else if (segTypeChar == 'A' || segTypeChar == 'a')
                    {
                        pt = SvgPathSegType.SVG_SEGTYPE_ARCTO;
                        abs = (segTypeChar == 'A');
                        datasize = 7;
                    }
                    else
                    {
                        throw new SvgException("Invalid SvgPath", s);
                    }

                    //strip off type character
                    sa[i] = sa[i].Substring(1);

                    if (sa[i] == "")
                        i += 1;
                }

                if (pt == SvgPathSegType.SVG_SEGTYPE_UNKNOWN)
                    throw new SvgException("Invalid SvgPath", s);

                var arr = new float[datasize];

                for (int j = 0; j < datasize; ++j)
                {
                    arr[j] = float.Parse(sa[i + j], CultureInfo.InvariantCulture);
                }

                ps = new PathSeg(pt, abs, arr);

                _path.Add(ps);

                i += datasize;
            }
        }

        public override string ToString()
        {
            PathSeg prev = null;
            var builder = new System.Text.StringBuilder();
            foreach (PathSeg seg in _path)
            {
                if (prev == null || (prev.Type != seg.Type || prev.Abs != seg.Abs))
                {
                    builder.Append(seg.Char).Append(" ");
                }
                foreach (float d in seg.Data)
                {
                    builder.Append(d.ToString(CultureInfo.InvariantCulture)).Append(" ");
                }
                prev = seg;
            }
            return builder.ToString();
        }

        private ArrayList _path;
    }

    /// <summary>
    /// A list of points, as specified in the SVG 1.1 spec section 9.8.  Only used in polygon and polyline elements.
    /// </summary>
    public class SvgPoints : ICloneable
    {
        public SvgPoints(string s)
        {
            FromString(s);
        }

        public SvgPoints(PointF[] pts)
        {
            foreach (PointF p in pts)
            {
                _pts.Add(p.X);
                _pts.Add(p.Y);
            }
        }

        /// <summary>
        /// The array must have an even length
        /// </summary>
        /// <param name="pts"></param>
        public SvgPoints(float[] pts)
        {
            if (pts.Length % 2 != 0)
                throw new SvgException("Invalid SvgPoints", pts.ToString());

            foreach (float p in pts)
            {
                _pts.Add(p);
            }
        }

        public static implicit operator SvgPoints(string s)
        {
            return new SvgPoints(s);
        }

        public static implicit operator SvgPoints(PointF[] pts)
        {
            return new SvgPoints(pts);
        }

        public object Clone()
        {
            return new SvgPoints((PointF[])_pts.ToArray(typeof(PointF)));
        }

        /// <summary>
        /// The standard boils down to a list of numbers in any format separated by any amount of wsp and commas;
        /// in other words it looks the same as a SvgNumList
        /// </summary>
        /// <param name="s"></param>
        public void FromString(string s)
        {
            try
            {
                var fa = SvgNumList.String2Floats(s);
                foreach (float f in fa)
                {
                    _pts.Add(f);
                }
            }
            catch (Exception)
            {
                throw new SvgException("Invalid SvgPoints", s);
            }

            if (_pts.Count % 2 != 0)
                throw new SvgException("Invalid SvgPoints", s);
        }

        public override string ToString()
        {
            var builder = new System.Text.StringBuilder();
            foreach (float f in _pts)
            {
                builder.Append(f.ToString("F", CultureInfo.InvariantCulture)).Append(" ");
            }
            return builder.ToString();
        }

        private readonly ArrayList _pts = new ArrayList();
    }

    /// <summary>
    /// Represents a URI reference within a style.  Local uri references are generally strings of the form
    /// <c>url(#elementID)</c>.   This class should not be confused with <see cref="SvgXRef"/> which represents
    /// the xlink:* properties of, for example, an <c>a</c> element.
    /// </summary>
    public class SvgUriReference : ICloneable
    {
        public SvgUriReference()
        {
        }

        public SvgUriReference(string href)
        {
            Href = href;
        }

        public SvgUriReference(SvgElement target)
        {
            Href = "#" + target.Id;
            if (target.Id == "")
            {
                throw new SvgException("Uri Reference cannot refer to an element with no id.", target.ToString());
            }
        }

        public string Href { get; set; }

        public object Clone()
        {
            return new SvgUriReference(Href);
        }

        public override string ToString()
        {
            return "url(" + Href + ")";
        }
    }

    /// <summary>
    /// Represents a URI reference.  Unlike most svg types, uri references are represented by more than one attribute
    /// of an element.  This means special measures are required to get and set uri references.
    /// </summary>
    public class SvgXRef : ICloneable
    {
        public SvgXRef()
        {
        }

        public SvgXRef(string href)
        {
            Href = href;
        }

        public SvgXRef(SvgStyledTransformedElement el)
        {
            ReadFromElement(el);
        }

        public string Actuate { get; set; } = "onLoad";

        public string Arcrole { get; set; }

        public string Href { get; set; }

        public string Role { get; set; }

        public string Show { get; set; }

        public string Title { get; set; }

        public string Type { get; set; } = "simple";

        public object Clone()
        {
            var r = new SvgXRef
            {
                Href = Href,
                Type = Type,
                Role = Role,
                Arcrole = Arcrole,
                Title = Title,
                Show = Show,
                Actuate = Actuate
            };
            return r;
        }

        public void ReadFromElement(SvgStyledTransformedElement el)
        {
            Href = (string)el["xlink:href"];
            Role = (string)el["xlink:role"];
            Arcrole = (string)el["xlink:arcrole"];
            Title = (string)el["xlink:title"];
            Show = (string)el["xlink:show"];

            //ignore the possibility of setting type and actuate for now
        }

        public override string ToString()
        {
            return Href;
        }

        public void WriteToElement(SvgStyledTransformedElement el)
        {
            el["xlink:href"] = Href;
            //if (_type != "simple") el["xlink:type"] = _type;
            el["xlink:role"] = Role;
            el["xlink:arcrole"] = Arcrole;
            el["xlink:title"] = Title;
            el["xlink:show"] = Show;
            //if (_type != "onLoad") el["xlink:actuate"] = _actuate;
        }
    }
}
