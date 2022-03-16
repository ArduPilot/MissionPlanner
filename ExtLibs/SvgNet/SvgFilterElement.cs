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
	/// 
	/// </summary>
	public class SvgFilterElement : SvgNet.SvgElement
	{
		public SvgFilterElement()
		{
		}

		public SvgFilterElement(SvgLength x, SvgLength y, SvgLength w, SvgLength h)
		{
			X=x;
			Y=y;
			Width=w;
			Height=h;
		}

		public override string Name{get{return "filter";}}

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

		public string FilterRes
		{
			get{return (string)_atts["filterRes"];}
			set{_atts["filterRes"] = value;}
		}

		public string FilterUnits
		{
			get{return (string)_atts["filterUnits"];}
			set{_atts["filterUnits"] = value;}
		}

		public string PrimitiveUnits
		{
			get{return (string)_atts["primitiveUnits"];}
			set{_atts["primitiveUnits"] = value;}
		}
	}
}

namespace SvgNet.SvgElements.FilterElements
{
}
