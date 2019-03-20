/*
	Copyright © 2003 RiskCare Ltd. All rights reserved.
	Copyright © 2010 SvgNet & SvgGdi Bridge Project. All rights reserved.
	Copyright © 2015 Rafael Teixeira, Mojmír Němeček, Benjamin Peterson and Other Contributors

	Original source code licensed with BSD-2-Clause spirit, treat it thus, see accompanied LICENSE for more
*/


using System;
using System.Drawing;
using SvgNet.SvgTypes; 

namespace SvgNet.SvgElements
{
	/// <summary>
	/// Represents a <c>polyline</c> element
	/// </summary>
	public class SvgPolylineElement : SvgNet.SvgStyledTransformedElement
	{
		public SvgPolylineElement()
		{
		}

		public SvgPolylineElement(SvgPoints points)
		{
			Points = points;
		}

		public override string Name{get{return "polyline";}}

		public SvgPoints Points
		{
			get{return (SvgPoints)_atts["points"];}
			set{_atts["points"] = value;}
		}
	}
}
