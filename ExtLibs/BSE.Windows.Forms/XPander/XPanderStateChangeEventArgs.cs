using System;

namespace BSE.Windows.Forms
{
    /// <summary>
    /// Provides data for the XPanderStateChange event.
    /// </summary>
    /// <copyright>Copyright © 2006-2008 Uwe Eichkorn
    /// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    /// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    /// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    /// PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
    /// REMAINS UNCHANGED.
    /// </copyright>
	public class XPanderStateChangeEventArgs : EventArgs
	{
		#region FieldsPrivate
		
		private bool m_bExpand;
		
		#endregion
		
		#region Properties
		
		/// <summary>
        /// Gets a value indicating whether the panel expands.
		/// </summary>
        public bool Expand
		{
			get {return m_bExpand;}
		}
		
		#endregion

		#region MethodsPublic
        /// <summary>
        /// arguments used when a XPanderStateChange event occurs.
        /// </summary>
        /// <param name="bExpand">Gets a value indicating whether the panel expands.</param>
		public XPanderStateChangeEventArgs(bool bExpand)
		{
			this.m_bExpand = bExpand;
		}

		#endregion
	}
}
