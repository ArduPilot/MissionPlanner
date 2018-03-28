/*
	Copyright © 2003 RiskCare Ltd. All rights reserved.
	Copyright © 2010 SvgNet & SvgGdi Bridge Project. All rights reserved.
	Copyright © 2015 Rafael Teixeira, Mojmír Němeček, Benjamin Peterson and Other Contributors

	Original source code licensed with BSD-2-Clause spirit, treat it thus, see accompanied LICENSE for more
*/


using System;
using SvgNet.SvgTypes;

namespace SvgNet
{
	/// <summary>
	/// Interface for SvgElements that have a text node.
	/// </summary>
	public interface IElementWithText
	{
		string Text
		{get;set;}
	}

	/// <summary>
	/// Interface for SvgElements that xlink to another element, e.g. <c>use</c>
	/// </summary>
	public interface IElementWithXRef
	{
		SvgXRef XRef
		{get;set;}

		string Href
		{get;set;}
	}

}
