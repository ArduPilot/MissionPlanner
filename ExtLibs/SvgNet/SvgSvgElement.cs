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
	/// Represents a <c>svg</c> element
	/// </summary>
	public class SvgSvgElement : SvgElement
	{
		public SvgSvgElement()
		{
		}

		public SvgSvgElement(SvgLength width, SvgLength height)
		{
			Width=width;
			Height=height;
		}

		public SvgSvgElement(SvgLength width, SvgLength height, SvgNumList vport)
		{
			Width=width;
			Height=height;
			ViewBox=vport;
		}

		public SvgSvgElement(SvgLength x, SvgLength y, SvgLength width, SvgLength height, SvgNumList vport)
		{
			X=x;
			Y=y;
			Width=width;
			Height=height;
			ViewBox=vport;
		}

		public override string Name{get{return "svg";}}

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
		
		public string Version
		{
			get{return (string)_atts["version"];}
			set{_atts["version"] = value;}
		}

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
}
