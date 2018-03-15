/*
	Copyright © 2003 RiskCare Ltd. All rights reserved.
	Copyright © 2010 SvgNet & SvgGdi Bridge Project. All rights reserved.
	Copyright © 2015 Rafael Teixeira, Mojmír Němeček, Benjamin Peterson and Other Contributors

	Original source code licensed with BSD-2-Clause spirit, treat it thus, see accompanied LICENSE for more
*/


using System;
using SvgNet.SvgTypes;

namespace SvgNet.SvgElements
{
	/// <summary>
	/// Represents an <c>ellipse</c> element.
	/// </summary>
	public class SvgEllipseElement : SvgStyledTransformedElement
	{
		public SvgEllipseElement()
		{
		}

		public SvgEllipseElement(SvgLength cx, SvgLength cy, SvgLength rx, SvgLength ry)
		{
			CX=cx;
			CY=cy;
			RX=rx;
			RY=ry;
		}

		public override string Name{get{return "ellipse";}}

		public SvgLength CX
		{
			get{return (SvgLength)_atts["cx"];}
			set{_atts["cx"] = value;}
		}
		public SvgLength CY
		{
			get{return (SvgLength)_atts["cy"];}
			set{_atts["cy"] = value;}
		}

		public SvgLength RX
		{
			get{return (SvgLength)_atts["rx"];}
			set{_atts["rx"] = value;}
		}
		public SvgLength RY
		{
			get{return (SvgLength)_atts["ry"];}
			set{_atts["ry"] = value;}
		}
	}
}
