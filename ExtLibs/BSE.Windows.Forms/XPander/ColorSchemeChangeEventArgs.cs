using System;
using System.Collections.Generic;
using System.Text;

namespace BSE.Windows.Forms
{
    /// <summary>
    /// Provides data for the ColorSchemeChange event.
    /// </summary>
    /// <copyright>Copyright © 2006-2008 Uwe Eichkorn
    /// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    /// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    /// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    /// PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
    /// REMAINS UNCHANGED.
    /// </copyright>
    public class ColorSchemeChangeEventArgs : EventArgs
    {
        #region FieldsPrivate

        private ColorScheme m_eColorSchema;

        #endregion

        #region Properties
        /// <summary>
        /// Gets the color schema which is used for the panel.
        /// </summary>
        public ColorScheme ColorSchema
        {
            get { return this.m_eColorSchema; }
        }
        #endregion

        #region MethodsPublic
        /// <summary>
        /// Arguments used when a ColorSchemeChange event occurs.
        /// </summary>
        /// <param name="eColorSchema">The color schema which is used for the panel.</param>
        public ColorSchemeChangeEventArgs(ColorScheme eColorSchema)
        {
            this.m_eColorSchema = eColorSchema;
        }

        #endregion
    }
}
