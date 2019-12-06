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
	/// Represents an SVG linearGradient element
	/// </summary>
	public class SvgLinearGradient : SvgStyledTransformedElement
	{
		public SvgLinearGradient()
		{
		}

		public SvgLinearGradient(SvgLength x1, SvgLength y1, SvgLength x2, SvgLength y2)
		{
			X1=x1;
			Y1=y1;
			X2=x2;
			Y2=y2;
		}

		public override string Name{get{return "linearGradient";}}


		public SvgLength X1
		{
			get{return (SvgLength)_atts["x1"];}
			set{_atts["x1"] = value;}
		}
		public SvgLength Y1
		{
			get{return (SvgLength)_atts["y1"];}
			set{_atts["y1"] = value;}
		}

		public SvgLength X2
		{
			get{return (SvgLength)_atts["x2"];}
			set{_atts["x2"] = value;}
		}
		public SvgLength Y2
		{
			get{return (SvgLength)_atts["y2"];}
			set{_atts["y2"] = value;}
		}

		public string GradientUnits
		{
			get{return (string)_atts["gradientUnits"];}
			set{_atts["gradientUnits"] = value;}
		}

		public SvgTransformList GradientTransform
		{
			get{return (SvgTransformList)_atts["gradientTransform"];}
			set{_atts["gradientTransform"] = value;}
		}

		public string SpreadMethod 
		{
			get{return (string)_atts["spreadMethod"];}
			set{_atts["spreadMethod"] = value;}
		}

	}

	/// <summary>
	/// Represents an svg radialGradient element
	/// </summary>
	public class SvgRadialGradient : SvgStyledTransformedElement
	{
		public SvgRadialGradient()
		{
		}

		public override string Name{get{return "radialGradient";}}


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

		public SvgLength R
		{
			get{return (SvgLength)_atts["r"];}
			set{_atts["r"] = value;}
		}

		public SvgLength FX
		{
			get{return (SvgLength)_atts["fx"];}
			set{_atts["fx"] = value;}
		}
		public SvgLength FY
		{
			get{return (SvgLength)_atts["fy"];}
			set{_atts["fy"] = value;}
		}

		public string GradientUnits
		{
			get{return (string)_atts["gradientUnits"];}
			set{_atts["gradientUnits"] = value;}
		}

		public SvgTransformList GradientTransform
		{
			get{return (SvgTransformList)_atts["gradientTransform"];}
			set{_atts["gradientTransform"] = value;}
		}

		public string SpreadMethod 
		{
			get{return (string)_atts["spreadMethod"];}
			set{_atts["spreadMethod"] = value;}
		}
	}

	/// <summary>
	/// Represents an SVG stop element, which specifies one color in a gradient.
	/// </summary>
	public class SvgStopElement : SvgStyledTransformedElement
	{
		public SvgStopElement()
		{
		}

		public SvgStopElement(SvgLength num, SvgColor col)
		{
			Offset=num;

			Style.Set("stop-color", col);
		}

		public override string Name{get{return "stop";}}

		public SvgLength Offset
		{
			get{return (SvgLength)_atts["offset"];}
			set{_atts["offset"] = value;}
		}

	}

	/// <summary>
	/// Represents an SVG pattern element, which defines a fill pattern by defining a viewport onto a subscene.
	/// </summary>
	public class SvgPatternElement : SvgStyledTransformedElement
	{
		public SvgPatternElement()
		{
		}

		public SvgPatternElement(SvgLength width, SvgLength height, SvgNumList vport)
		{
			Width=width;
			Height=height;
			ViewBox=vport;
		}

		public SvgPatternElement(SvgLength x, SvgLength y, SvgLength width, SvgLength height, SvgNumList vport)
		{
			X=x;
			Y=y;
			Width=width;
			Height=height;
			ViewBox=vport;
		}

		public override string Name{get{return "pattern";}}

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

		public string PatternUnits
		{
			get{return (string)_atts["patternUnits"];}
			set{_atts["patternUnits"] = value;}
		}

		public string PatternContentUnits
		{
			get{return (string)_atts["patternContentUnits"];}
			set{_atts["patternContentUnits"] = value;}
		}

		public SvgTransformList PatternTransform
		{
			get{return (SvgTransformList)_atts["patternTransform"];}
			set{_atts["patternTransform"] = value;}
		}
	}


}
