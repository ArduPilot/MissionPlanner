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
	/// Represents a <c>polygon</c> element
	/// </summary>
	public class SvgPolygonElement : SvgNet.SvgStyledTransformedElement
	{
		public SvgPolygonElement()
		{
		}

		public SvgPolygonElement(SvgPoints points)
		{
			Points = points;
		}

		public override string Name{get{return "polygon";}}

		public SvgPoints Points
		{
			get{return (SvgPoints)_atts["points"];}
			set{_atts["points"] = value;}
		}
	}
}
