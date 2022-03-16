/*
	Copyright © 2003 RiskCare Ltd. All rights reserved.
	Copyright © 2010 SvgNet & SvgGdi Bridge Project. All rights reserved.
	Copyright © 2015 Rafael Teixeira, Mojmír Němeček, Benjamin Peterson and Other Contributors

	Original source code licensed with BSD-2-Clause spirit, treat it thus, see accompanied LICENSE for more
*/


using System;
using System.Xml;
using SvgNet.SvgTypes;

namespace SvgNet
{
	/// <summary>
	/// Represents the text contained in a title, desc, text, or tspan element.  Maps to an XmlText object in an XML document.  It is inherited from
	/// </summary>
	public class TextNode : SvgNet.SvgElement
	{
		string _s;

		public TextNode()
		{
		}

		public TextNode(string s)
		{
			Text = s;
		}


		public override string Name{get{return "a text node, not an svg element";}}

		/// <summary>
		/// Adds a child, and sets the child's parent to this element.
		/// </summary>
		/// <param name="ch"></param>
		public override void AddChild(SvgElement ch)
		{
			throw new SvgException("A TextNode cannot have children");
		}


		/// <summary>
		/// Adds a variable number of children
		/// </summary>
		/// <param name="ch"></param>
		public override void AddChildren(params SvgElement[] ch)
		{
			throw new SvgException("A TextNode cannot have children");
		}


		public string Text
		{
			get{return _s;}
			set{_s = value;}
		}

		/// <summary>
		/// Given a document and a current node, read this element from the node.
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="el"></param>
		public override void ReadXmlElement(XmlDocument doc, XmlElement el)
		{
			throw new SvgException("TextNode::ReadXmlElement should not be called; " +
				"the value should be filled in with a string when the XML doc is being read.", "");
		}

		/// <summary>
		/// Overridden to simply create an XML text node below the parent.
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="parent"></param>
		public override void WriteXmlElements(XmlDocument doc, XmlElement parent)
		{
			XmlText xt = doc.CreateTextNode(_s);

			if(parent == null)
				doc.AppendChild(xt);
			else
				parent.AppendChild(xt);
		}
	}
}

namespace SvgNet.SvgElements
{

	/// <summary>
	/// Represents a <c>text</c> element.  The SVG text element is unusual in that it expects actual XML text nodes below
	/// it, rather than consisting only of attributes and child elements  (other elements like this are title, desc, and tspan).
	/// <c>SvgTextElement</c> therefore has to be serialized
	/// to XML slightly differently.
	/// </summary>
	public class SvgTextElement : SvgNet.SvgStyledTransformedElement, IElementWithText
	{
		public SvgTextElement()
		{
		}

		public SvgTextElement(string s)
		{
			TextNode tn = new TextNode(s);
			AddChild(tn);
		}

		public SvgTextElement(string s, float x, float y)
		{
			TextNode tn = new TextNode(s);
			AddChild(tn);
			X=x;
			Y=y;
		}

		public override string Name{get{return "text";}}

		public SvgLength DX
		{
			get{return (SvgLength)_atts["dx"];}
			set{_atts["dx"] = value;}
		}
		public SvgLength DY
		{
			get{return (SvgLength)_atts["dy"];}
			set{_atts["dy"] = value;}
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

		public SvgNumList Rotate
		{
			get{return (SvgNumList)_atts["rotate"];}
			set{_atts["rotate"] = value;}
		}
		public SvgLength TextLength
		{
			get{return (SvgLength)_atts["textLength"];}
			set{_atts["textLength"] = value;}
		}
		public string LengthAdjust
		{
			get{return (string)_atts["lengthAdjust"];}
			set{_atts["lengthAdjust"] = value;}
		}

		public string Text
		{
			get{return ((TextNode)_children[0]).Text;}
			set{((TextNode)_children[0]).Text = value;}
		}
	}

	/// <summary>
	/// Represents a <c>tspan</c> element.  The tspan element is unique in that it expects actual XML text nodes below
	/// it, rather than consisting only of attributes and child elements.  <c>SvgTextElement</c> therefore has to be serialized
	/// to XML slightly differently.
	/// </summary>
	public class SvgTspanElement : SvgNet.SvgStyledTransformedElement, IElementWithText
	{

		public SvgTspanElement()
		{
		}

		public SvgTspanElement(string s)
		{
			TextNode tn = new TextNode(s);
			AddChild(tn);
		}

		public SvgTspanElement(string s, float x, float y)
		{
			TextNode tn = new TextNode(s);
			AddChild(tn);
			X=x;
			Y=y;
		}

		public override string Name{get{return "tspan";}}

		public SvgLength DX
		{
			get{return (SvgLength)_atts["dx"];}
			set{_atts["dx"] = value;}
		}
		public SvgLength DY
		{
			get{return (SvgLength)_atts["dy"];}
			set{_atts["dy"] = value;}
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

		public SvgNumList Rotate
		{
			get{return (SvgNumList)_atts["rotate"];}
			set{_atts["rotate"] = value;}
		}
		public SvgLength TextLength
		{
			get{return (SvgLength)_atts["textLength"];}
			set{_atts["textLength"] = value;}
		}
		public string LengthAdjust
		{
			get{return (string)_atts["lengthAdjust"];}
			set{_atts["lengthAdjust"] = value;}
		}

		public string Text
		{
			get{return ((TextNode)_children[0]).Text;}
			set{((TextNode)_children[0]).Text = value;}
		}

	}

	/// <summary>
	/// Represents a <c>tref</c> element.
	/// </summary>
	public class SvgTrefElement : SvgNet.SvgStyledTransformedElement, IElementWithXRef
	{

		public SvgTrefElement()
		{
		}

		public SvgTrefElement(string s)
		{
			Href = s;
		}

		public SvgTrefElement(string s, float x, float y)
		{
			Href = s;
			X=x;
			Y=y;
		}

		public override string Name{get{return "tref";}}

		public SvgLength DX
		{
			get{return (SvgLength)_atts["dx"];}
			set{_atts["dx"] = value;}
		}
		public SvgLength DY
		{
			get{return (SvgLength)_atts["dy"];}
			set{_atts["dy"] = value;}
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

		public SvgNumList Rotate
		{
			get{return (SvgNumList)_atts["rotate"];}
			set{_atts["rotate"] = value;}
		}
		public SvgLength TextLength
		{
			get{return (SvgLength)_atts["textLength"];}
			set{_atts["textLength"] = value;}
		}
		public string LengthAdjust
		{
			get{return (string)_atts["lengthAdjust"];}
			set{_atts["lengthAdjust"] = value;}
		}

		public string Text
		{
			get{return ((TextNode)_children[0]).Text;}
			set{((TextNode)_children[0]).Text = value;}
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
