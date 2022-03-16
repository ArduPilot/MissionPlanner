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
	/// Represents a <c>rect</c> element
	/// </summary>
	public class SvgRectElement : SvgStyledTransformedElement
	{
		public SvgRectElement()
		{
		}

		public SvgRectElement(SvgLength x, SvgLength y, SvgLength w, SvgLength h)
		{
			X=x;
			Y=y;
			Width=w;
			Height=h;
		}

		public override string Name{get{return "rect";}}

		public SvgLength Width
		{
			get{return (SvgLength)_atts["width"];}
			set{_atts["width"] = value;}
		}
		public SvgLength Height
		{
			get{return (SvgLength)_atts["height"];}
			set{_atts["height"] = value;}
		}

		public SvgLength X
		{
			get{return (SvgLength)_atts["x"];}
			set{_atts["x"] = value;}
		}
		public SvgLength Y
		{
			get{return (SvgLength)_atts["y"];}
			set{_atts["y"] = value;}
		}
		
	}
}
