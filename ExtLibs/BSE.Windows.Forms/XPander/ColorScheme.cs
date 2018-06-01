using System;
using System.Collections.Generic;
using System.Text;

namespace BSE.Windows.Forms
{
    /// <summary>
	/// Contains information for the drawing of panels or xpanderpanels in a xpanderpanellist. 
    /// </summary>
	/// <copyright>Copyright © 2006-2008 Uwe Eichkorn
    /// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    /// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    /// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    /// PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
    /// REMAINS UNCHANGED.
    /// </copyright>
	public enum ColorScheme
    {
        /// <summary>
        /// Draws the panels caption with <see cref="System.Windows.Forms.ProfessionalColors">ProfessionalColors</see>
        /// </summary>
		Professional,
        /// <summary>
        /// Draws the panels caption with custom colors.
        /// </summary>
		Custom
    }
}
