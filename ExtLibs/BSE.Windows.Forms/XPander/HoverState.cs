using System;
using System.Collections.Generic;
using System.Text;

namespace BSE.Windows.Forms
{
	/// <summary>
	/// Specifies constants that define the hoverstate at the captionbar or a part of it on a Panel or XPanderPanel.
	/// </summary>
    /// <copyright>Copyright © 2008 Uwe Eichkorn
    /// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    /// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    /// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    /// PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
    /// REMAINS UNCHANGED.
    /// </copyright>
    public enum HoverState
	{
		/// <summary>
		/// The hoverstate in its normal state (none of the other states apply).
		/// </summary>
		None,
		/// <summary>
		/// The hoverstate over which a mouse pointer is resting.
		/// </summary>
		Hover
	}
}
