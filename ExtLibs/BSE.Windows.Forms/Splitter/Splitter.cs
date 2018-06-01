using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace BSE.Windows.Forms
{
    /// <summary>
    /// Represents a splitter control that enables the user to resize docked controls.
    /// </summary>
    /// <remarks>
    /// The splitter control supports in difference to the <see cref="System.Windows.Forms.Splitter"/> the using
    /// of a transparent backcolor.
    /// </remarks>
    /// <copyright>Copyright © 2006-2008 Uwe Eichkorn
    /// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    /// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    /// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    /// PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
    /// REMAINS UNCHANGED.
    /// </copyright>
    [DesignTimeVisibleAttribute(true)]
	[ToolboxBitmap(typeof(System.Windows.Forms.Splitter))]
	public partial class Splitter : System.Windows.Forms.Splitter
	{
		#region MethodsPublic
		/// <summary>
        /// Initializes a new instance of the Splitter class.
		/// </summary>
		public Splitter()
		{
            //The System.Windows.Forms.Splitter doesn't suports a transparent backcolor
            //With this, the using of a transparent backcolor is possible
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			InitializeComponent();
            this.BackColor = Color.Transparent;
		}

		#endregion
	}
}
