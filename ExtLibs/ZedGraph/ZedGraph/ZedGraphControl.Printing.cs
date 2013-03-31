//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Threading;

namespace ZedGraph
{
	partial class ZedGraphControl
	{

	#region Printing

		/// <summary>
		/// Handler for the "Page Setup..." context menu item.   Displays a
		/// <see cref="PageSetupDialog" />.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void MenuClick_PageSetup( object sender, EventArgs e )
		{
			DoPageSetup();
		}

		/// <summary>
		/// Handler for the "Print..." context menu item.   Displays a
		/// <see cref="PrintDialog" />.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void MenuClick_Print( object sender, EventArgs e )
		{
			DoPrint();
		}

		/// <summary>
		/// Rendering method used by the print context menu items
		/// </summary>
		/// <param name="sender">The applicable <see cref="PrintDocument" />.</param>
		/// <param name="e">A <see cref="PrintPageEventArgs" /> instance providing
		/// page bounds, margins, and a Graphics instance for this printed output.
		/// </param>
		private void Graph_PrintPage( object sender, PrintPageEventArgs e )
		{
			PrintDocument pd = sender as PrintDocument;

			MasterPane mPane = this.MasterPane;
			bool[] isPenSave = new bool[mPane.PaneList.Count + 1];
			bool[] isFontSave = new bool[mPane.PaneList.Count + 1];
			isPenSave[0] = mPane.IsPenWidthScaled;
			isFontSave[0] = mPane.IsFontsScaled;
			for ( int i = 0; i < mPane.PaneList.Count; i++ )
			{
				isPenSave[i + 1] = mPane[i].IsPenWidthScaled;
				isFontSave[i + 1] = mPane[i].IsFontsScaled;
				if ( _isPrintScaleAll )
				{
					mPane[i].IsPenWidthScaled = true;
					mPane[i].IsFontsScaled = true;
				}
			}

			RectangleF saveRect = mPane.Rect;
			SizeF newSize = mPane.Rect.Size;
			if ( _isPrintFillPage && _isPrintKeepAspectRatio )
			{
				float xRatio = (float)e.MarginBounds.Width / (float)newSize.Width;
				float yRatio = (float)e.MarginBounds.Height / (float)newSize.Height;
				float ratio = Math.Min( xRatio, yRatio );

				newSize.Width *= ratio;
				newSize.Height *= ratio;
			}
			else if ( _isPrintFillPage )
				newSize = e.MarginBounds.Size;

			mPane.ReSize( e.Graphics, new RectangleF( e.MarginBounds.Left,
				e.MarginBounds.Top, newSize.Width, newSize.Height ) );
			mPane.Draw( e.Graphics );

			using ( Graphics g = this.CreateGraphics() )
			{
				mPane.ReSize( g, saveRect );
				//g.Dispose();
			}

			mPane.IsPenWidthScaled = isPenSave[0];
			mPane.IsFontsScaled = isFontSave[0];
			for ( int i = 0; i < mPane.PaneList.Count; i++ )
			{
				mPane[i].IsPenWidthScaled = isPenSave[i + 1];
				mPane[i].IsFontsScaled = isFontSave[i + 1];
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Drawing.Printing.PrintDocument" /> instance
		/// that is used for all of the context menu printing functions.
		/// </summary>
		public PrintDocument PrintDocument
		{
			get
			{
				// Add a try/catch pair since the users of the control can't catch this one
				try
				{
					if ( _pdSave == null )
					{
						_pdSave = new PrintDocument();
						_pdSave.PrintPage += new PrintPageEventHandler( Graph_PrintPage );
					}
				}
				catch ( Exception exception )
				{
					MessageBox.Show( exception.Message );
				}

				return _pdSave;
			}
			set { _pdSave = value; }
		}

		/// <summary>
		/// Display a <see cref="PageSetupDialog" /> to the user, allowing them to modify
		/// the print settings for this <see cref="ZedGraphControl" />.
		/// </summary>
		public void DoPageSetup()
		{
			PrintDocument pd = PrintDocument;

			// Add a try/catch pair since the users of the control can't catch this one
			try
			{
				if ( pd != null )
				{
					//pd.PrintPage += new PrintPageEventHandler( GraphPrintPage );
					PageSetupDialog setupDlg = new PageSetupDialog();
					setupDlg.Document = pd;

					if ( setupDlg.ShowDialog() == DialogResult.OK )
					{
						pd.PrinterSettings = setupDlg.PrinterSettings;
						pd.DefaultPageSettings = setupDlg.PageSettings;

						// BUG in PrintDocument!!!  Converts in/mm repeatedly
						// http://support.microsoft.com/?id=814355
						// from http://www.vbinfozine.com/tpagesetupdialog.shtml, by Palo Mraz
						//if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
						//{
						//	setupDlg.Document.DefaultPageSettings.Margins = PrinterUnitConvert.Convert(
						//	setupDlg.Document.DefaultPageSettings.Margins,
						//	PrinterUnit.Display, PrinterUnit.TenthsOfAMillimeter );
						//}
					}
				}
			}

			catch ( Exception exception )
			{
				MessageBox.Show( exception.Message );
			}
		}

		/// <summary>
		/// Display a <see cref="PrintDialog" /> to the user, allowing them to select a
		/// printer and print the <see cref="MasterPane" /> contained in this
		/// <see cref="ZedGraphControl" />.
		/// </summary>
		public void DoPrint()
		{
			// Add a try/catch pair since the users of the control can't catch this one
			try
			{
				PrintDocument pd = PrintDocument;

				if ( pd != null )
				{
					//pd.PrintPage += new PrintPageEventHandler( Graph_PrintPage );
					PrintDialog pDlg = new PrintDialog();
					pDlg.Document = pd;
					if ( pDlg.ShowDialog() == DialogResult.OK )
						pd.Print();
				}
			}
			catch ( Exception exception )
			{
				MessageBox.Show( exception.Message );
			}

		}

		/// <summary>
		/// Display a <see cref="PrintPreviewDialog" />, allowing the user to preview and
		/// subsequently print the <see cref="MasterPane" /> contained in this
		/// <see cref="ZedGraphControl" />.
		/// </summary>
		public void DoPrintPreview()
		{
			// Add a try/catch pair since the users of the control can't catch this one
			try
			{
				PrintDocument pd = PrintDocument;

				if ( pd != null )
				{
					PrintPreviewDialog ppd = new PrintPreviewDialog();
					//pd.PrintPage += new PrintPageEventHandler( Graph_PrintPage );
					ppd.Document = pd;
					ppd.Show( this );
				}
			}
			catch ( Exception exception )
			{
				MessageBox.Show( exception.Message );
			}
		}

	#endregion

	}
}