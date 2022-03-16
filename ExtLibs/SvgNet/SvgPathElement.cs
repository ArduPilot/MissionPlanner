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
	/// Represents a <c>path</c> element
	/// </summary>
	public class SvgPathElement : SvgNet.SvgStyledTransformedElement
	{
		public SvgPathElement()
		{
		}

		public override string Name{get{return "path";}}

		public SvgPath D
		{
			get{return (SvgPath)_atts["d"];}
			set{_atts["d"] = value.ToString();}
		}

		public SvgNumber PathLength
		{
			get{return (SvgNumber)_atts["pathlength"];}
			set{_atts["pathlength"] = value;}
		}

	}
}
