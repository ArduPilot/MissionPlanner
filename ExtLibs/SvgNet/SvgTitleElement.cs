/*
	Copyright © 2003 RiskCare Ltd. All rights reserved.
	Copyright © 2010 SvgNet & SvgGdi Bridge Project. All rights reserved.
	Copyright © 2015 Rafael Teixeira, Mojmír Němeček, Benjamin Peterson and Other Contributors

	Original source code licensed with BSD-2-Clause spirit, treat it thus, see accompanied LICENSE for more
*/


using System;
using System.Xml;

namespace SvgNet.SvgElements
{
	/// <summary>
	/// Represents an SVG <c>desc</c> element.  As with the SvgTextElement, the payload is in the enclosed text rather than in attributes and 
	/// subelements, so we need to specially add text when serializing.
	/// </summary>
	public class SvgTitleElement : SvgNet.SvgElement, IElementWithText
	{

		public SvgTitleElement()
		{
			TextNode tn = new TextNode("");
			AddChild(tn);
		}

		public SvgTitleElement(string s)
		{
			TextNode tn = new TextNode(s);
			AddChild(tn);
		}

		public override string Name{get{return "title";}}

		public string Text
		{
			get{return ((TextNode)_children[0]).Text;}
			set{((TextNode)_children[0]).Text = value;}
		}
	}

	/// <summary>
	/// Represents an SVG <c>desc</c> element.  As with the SvgTextElement, the payload is in the enclosed text rather than in attributes and 
	/// subelements, so we need to specially add text when serializing.
	/// </summary>
	public class SvgDescElement : SvgNet.SvgElement, IElementWithText
	{

		public SvgDescElement()
		{
			TextNode tn = new TextNode("");
			AddChild(tn);
		}

		public SvgDescElement(string s)
		{
			TextNode tn = new TextNode(s);
			AddChild(tn);
		}

		public override string Name{get{return "desc";}}

		public string Text
		{
			get{return ((TextNode)_children[0]).Text;}
			set{((TextNode)_children[0]).Text = value;}
		}

	}
}
