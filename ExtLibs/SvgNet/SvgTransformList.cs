/*
	Copyright © 2003 RiskCare Ltd. All rights reserved.
	Copyright © 2010 SvgNet & SvgGdi Bridge Project. All rights reserved.
	Copyright © 2015 Rafael Teixeira, Mojmír Němeček, Benjamin Peterson and Other Contributors

	Original source code licensed with BSD-2-Clause spirit, treat it thus, see accompanied LICENSE for more
*/


using System.Drawing;
using System.Collections;
using System;
using System.Drawing.Drawing2D;

namespace SvgNet.SvgTypes
{

	/// <summary>
	/// Represents a single element in an SVG transformation list.  The transformation is represented internally as a
	/// GDI+ Matrix object.
	/// </summary>
	public class SvgTransform : ICloneable
	{
		Matrix _m;

		public SvgTransform()
		{
			_m = new Matrix();
		}

		public SvgTransform(string s)
		{
			FromString(s);
		}

		public SvgTransform(Matrix m)
		{
			_m = m;
		}

		public object Clone()
		{
			return new SvgTransform(_m.Clone());
		}

		/// <summary>
		/// Parse a transformation according to the SVG standard.  This is complex enough that it makes
		/// me wish it was worth using a real parser, but antlr is so unwieldy.
		/// </summary>
		public void FromString(string s)
		{
			_m = new Matrix();

			string name, args;

			int idx = s.IndexOf("(");

			if (idx != -1) {
				name = s.Substring(0, idx).Trim();

				int idx2 = s.IndexOf(")");

				if (idx2 != -1) {
					args = s.Substring(idx + 1, (idx2 - idx) - 1);
					float[] points = SvgNumList.String2Floats(args);

					if (name.IndexOf("matrix") != -1) {
						if (points.Length == 6) {
							_m = new Matrix(points[0], points[1], points[2], points[3], points[4], points[5]);
							return;
						}
					} else if (name.IndexOf("translate") != -1) {
						if (points.Length == 1) {
							_m.Translate(points[0], 0);
							return;
						}
						if (points.Length == 2) {
							_m.Translate(points[0], points[1]);
							return;
						}
					} else if (name.IndexOf("scale") != -1) {
						if (points.Length == 1) {
							_m.Scale(points[0], 0);
							return;
						}
						if (points.Length == 2) {
							_m.Scale(points[0], points[1]);
							return;
						}
					} else if (name.IndexOf("rotate") != -1) {
						if (points.Length == 1) {
							_m.Rotate(points[0]);
							return;
						} else if (points.Length == 3) {
							_m.Translate(points[1], points[2]);
							_m.Rotate(points[0]);
							_m.Translate(points[1] * -1, points[2] * -1);
							return;
						}
					} else if (name.IndexOf("skewX") != -1) {
						if (points.Length == 1) {
							_m.Shear(points[0], 0);
							return;
						}
					} else if (name.IndexOf("skewY") != -1) {
						if (points.Length == 1) {
							_m.Shear(0, points[0]);
							return;
						}
					}
				}
			}

			throw new SvgException("Invalid SvgTransformation", s);
		}

		/// <summary>
		/// Currently, we always output as matrix() no matter how the transform was specified.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string result = "matrix(";

			foreach (float f in _m.Elements) {
				result += f.ToString("F", System.Globalization.CultureInfo.InvariantCulture);
				result += " ";
			}

			result += ")";

			return result;
		}


		public Matrix Matrix
		{
			get { return _m; }
			set { _m = value; }
		}
	}

	/// <summary>
	/// Represents an SVG transform-list, as specified in section 7.6 of the SVG 1.1 standard.
	/// </summary>
	public class SvgTransformList : ICloneable
	{
		ArrayList _t = new ArrayList();

		public SvgTransformList()
		{
		}

		public SvgTransformList(string s)
		{
			FromString(s);
		}

		public SvgTransformList(Matrix m)
		{
			SvgTransform tr = new SvgTransform(m);
			_t.Add(tr);
		}

		public object Clone()
		{
			//use to/from string as a shortcut
			return new SvgTransformList(this.ToString());
		}

		public void Add(string trans)
		{
			_t.Add(new SvgTransform(trans));
		}

		public void Add(Matrix m)
		{
			_t.Add(new SvgTransform(m));
		}

		/// <summary>
		/// Parse a string containing a whitespace-separated list of transformations as per the SVG
		/// standard
		/// </summary>
		public void FromString(string s)
		{
			int start = -1, end = 0;

			do {
				end = s.IndexOf(")", start + 1);

				if (end == -1) return;

				SvgTransform trans = new SvgTransform(s.Substring(start + 1, end - start));

				_t.Add(trans);

				start = end;

			}
			while (true);
		}

		public override string ToString()
		{
			string result = "";

			foreach (SvgTransform tr in _t) {
				result += tr.ToString();
				result += " ";
			}

			return result;
		}

		public SvgTransform this[int idx]
		{
			get { return (SvgTransform)_t[idx]; }
			set { _t[idx] = value; }
		}

		public int Count
		{
			get { return _t.Count; }
		}

		public static implicit operator SvgTransformList(string s)
		{
			return new SvgTransformList(s);
		}

		public static implicit operator SvgTransformList(Matrix m)
		{
			return new SvgTransformList(m);
		}

	}
}