using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace BSE.Windows.Forms
{
   /// <summary>
   /// Used to group collections of controls. 
   /// </summary>
   /// <copyright>Copyright © 2006-2008 Uwe Eichkorn
   /// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
   /// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
   /// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
   /// PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
   /// REMAINS UNCHANGED.
   /// </copyright>
   public interface IPanel
   {
      /// <summary>
      /// Gets or sets the style of the panel.
      /// </summary>
      BSE.Windows.Forms.PanelStyle PanelStyle
      {
         get;
         set;
      }
      /// <summary>
      /// Gets or sets the color schema which is used for the panel.
      /// </summary>
      BSE.Windows.Forms.ColorScheme ColorScheme
      {
         get;
         set;
      }
      /// <summary>
      /// Gets or sets a value indicating whether the control shows a border
      /// </summary>
      bool ShowBorder
      {
         get;
         set;
      }
      /// <summary>
      /// Gets or sets a value indicating whether the expand icon in the caption bar is visible.
      /// </summary>
      bool ShowExpandIcon
      {
         get;
         set;
      }
      /// <summary>
      /// Gets or sets a value indicating whether the close icon in the caption bar is visible.
      /// </summary>
      bool ShowCloseIcon
      {
         get;
         set;
      }
      /// <summary>
      /// Expands the panel or xpanderpanel.
      /// </summary>
      bool Expand
      {
         get;
         set;
      }
   }
}
