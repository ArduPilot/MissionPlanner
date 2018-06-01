using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Text;

namespace BSE.Windows.Forms
{
    /// <summary>
    /// Set the TextRenderingHint.ClearTypeGridFit until instance disposed.
    /// </summary>
    public class UseClearTypeGridFit : IDisposable
    {
        #region FieldsPrivate
        private Graphics m_graphics;
        private TextRenderingHint m_textRenderingHint;
        #endregion

        #region MethodsPublic
        /// <summary>
        /// Initialize a new instance of the UseClearTypeGridFit class.
        /// </summary>
		/// <param name="graphics">Graphics instance.</param>
        public UseClearTypeGridFit(Graphics graphics)
        {
			if (graphics == null)
			{
				throw new ArgumentNullException("graphics",
					string.Format(System.Globalization.CultureInfo.InvariantCulture,
					BSE.Windows.Forms.Properties.Resources.IDS_ArgumentException,
					"graphics"));
			}

			this.m_graphics = graphics;
            this.m_textRenderingHint = this.m_graphics.TextRenderingHint;
            this.m_graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        }
		/// <summary>
		/// destructor of the UseClearTypeGridFit class.
		/// </summary>
		~UseClearTypeGridFit()
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
			if (disposing == true)
			{
				//Revert the TextRenderingHint back to original setting.
				this.m_graphics.TextRenderingHint = this.m_textRenderingHint;
			}
		}
		#endregion
    }
}
