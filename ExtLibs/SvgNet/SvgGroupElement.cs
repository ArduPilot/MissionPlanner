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
	/// Represents a <c>g</c> element.  It has no particular properties of its own.
	/// </summary>
	public class SvgGroupElement : SvgNet.SvgStyledTransformedElement
	{
		public SvgGroupElement()
		{
		}

		public SvgGroupElement(string id) : base(id)
		{
		}

		public override string Name{get{return "g";}}
	}

	/// <summary>
	/// Represents a <c>switch</c> element.  It has no particular properties of its own.
	/// </summary>
	public class SvgSwitchElement : SvgNet.SvgStyledTransformedElement
	{
		public SvgSwitchElement()
		{
		}

		public SvgSwitchElement(string id) : base(id)
		{
		}

		public override string Name{get{return "g";}}
	}


	/// <summary>
	/// Represents a <c>clippath</c> element.  It has no particular properties of its own.
	/// </summary>
	public class SvgClipPathElement : SvgNet.SvgElement
	{
		public SvgClipPathElement()
		{
		}

		public SvgClipPathElement(string id) : base(id)
	{
	}

		public override string Name{get{return "clipPath";}}
	}

	/// <summary>
	/// Represents an <c>a</c> element.  It has an xref and a target.
	/// </summary>
	public class SvgAElement : SvgNet.SvgStyledTransformedElement, IElementWithXRef
	{
		public SvgAElement()
		{
		}

		public SvgAElement(string href)
		{
			Href = href;
		}

		public override string Name{get{return "a";}}

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

		public string Target
		{
			get{return (string)_atts["target"];}
			set{_atts["target"] = value;}
		}
	}

	/// <summary>
	/// Represents a <c>defs</c> element.  It has no particular properties of its own.
	/// </summary>
	public class SvgDefsElement : SvgNet.SvgElement
	{
		public SvgDefsElement()
		{
		}

		public SvgDefsElement(string id) : base(id)
		{
		}

		public override string Name{get{return "defs";}}
	}

	/// <summary>
	/// Represents an element that is not yet represented by a class of its own.  
	/// </summary>
	public class SvgGenericElement : SvgNet.SvgElement
	{
		string _name;

		public SvgGenericElement()
		{
			_name = "generic svg node";
		}

		public SvgGenericElement(string name)
		{
			_name = name;
		}

		public override string Name{get{return _name;}}
	}

}
