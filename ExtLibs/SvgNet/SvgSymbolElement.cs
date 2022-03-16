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
	/// Represents an SVG <c>symbol</c> element.
	/// </summary>
	public class SvgSymbolElement : SvgNet.SvgElement
	{
		public SvgSymbolElement()
		{
		}

		public override string Name{get{return "symbol";}}

		public SvgNumList ViewBox
		{
			get{return (SvgNumList)_atts["viewbox"];}
			set{_atts["viewbox"] = value;}
		}

		public string PreserveAspectRatio
		{
			get{return (string)_atts["preserveAspectRatio"];}
			set{_atts["preserveAspectRatio"] = value;}
		}
	}

	/// <summary>
	/// Represents an SVG <c>use</c> element.
	/// </summary>
	public class SvgUseElement : SvgNet.SvgStyledTransformedElement, IElementWithXRef
	{
		public SvgUseElement()
		{
		}

		public SvgUseElement(SvgXRef xref)
		{
			XRef = xref;
		}

		public SvgUseElement(string href)
		{
			Href = href;
		}

		public SvgUseElement(SvgLength x, SvgLength y, SvgXRef xref)
		{
			XRef = xref;
			X = x;
			Y = y;
		}

		public SvgUseElement(SvgLength x, SvgLength y, string href)
		{
			Href = href;
			X = x;
			Y = y;
		}

		public override string Name{get{return "use";}}

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

		public SvgXRef XRef
		{
			get{return new SvgXRef(this);}
			set{value.WriteToElement(this);}
		}

		public string Href
		{
			get{return (string)_atts["xlink:href"];}
			set{_atts["xlink:href"] = value;}
		}

	}


	
	/// <summary>
	/// Represents an SVG <c>image</c> element.
	/// </summary>
	public class SvgImageElement : SvgNet.SvgStyledTransformedElement, IElementWithXRef
	{
		public SvgImageElement()
		{
		}

		public SvgImageElement(SvgXRef xref)
		{
			XRef = xref;
		}

		public SvgImageElement(SvgLength x, SvgLength y, SvgXRef xref)
		{
			XRef = xref;
			X = x;
			Y = y;
		}

		public SvgImageElement(string href)
		{
			Href = href;
		}

		public SvgImageElement(SvgLength x, SvgLength y, string href)
		{
			Href = href;
			X = x;
			Y = y;
		}

		public override string Name{get{return "image";}}

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

		public string PreserveAspectRatio
		{
			get{return (string)_atts["preserveAspectRatio"];}
			set{_atts["preserveAspectRatio"] = value;}
		}

		public SvgXRef XRef
		{
			get{return new SvgXRef(this);}
			set{value.WriteToElement(this);}
		}

		public string Href
		{
			get{return (string)_atts["xlink:href"];}
			set{_atts["xlink:href"] = value;}
		}

	}
}
