using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BSE.Windows.Forms
{
   /// <summary>
   /// Set the SmoothingMode=AntiAlias until instance disposed.
   /// </summary>
   public class UseAntiAlias : IDisposable
   {
      #region FieldsPrivate

      private Graphics m_graphics;
      private SmoothingMode m_smoothingMode;

      #endregion

      #region MethodsPublic
      /// <summary>
      /// Initialize a new instance of the UseAntiAlias class.
      /// </summary>
      /// <param name="graphics">Graphics instance.</param>
      public UseAntiAlias(Graphics graphics)
      {
         if(graphics == null)
         {
            throw new ArgumentNullException("graphics",
               string.Format(System.Globalization.CultureInfo.InvariantCulture,
               BSE.Windows.Forms.Properties.Resources.IDS_ArgumentException,
               "graphics"));
         }

         this.m_graphics = graphics;
         this.m_smoothingMode = m_graphics.SmoothingMode;
         this.m_graphics.SmoothingMode = SmoothingMode.AntiAlias;
      }
      /// <summary>
      /// destructor of the UseAntiAlias class.
      /// </summary>
      ~UseAntiAlias()
      {
         Dispose(false);
      }
      /// <summary>
      /// Releases all resources used by the class. 
      /// </summary>
      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }
      #endregion

      #region MethodsProtected
      /// <summary> 
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected virtual void Dispose(bool disposing)
      {
         if(disposing == true)
         {
            //Revert the SmoothingMode back to original setting.
            this.m_graphics.SmoothingMode = this.m_smoothingMode;
         }
      }
      #endregion
   }
}
