using System;
using System.Drawing;

namespace MPEGBuilder1UI
{
	/// <summary>
	/// Summary description for ImageFunctions.
	/// </summary>
	public class ImageFunctions
	{
		// Class of functions to manipulate any image into a
		// format that can be MPEG encoded
		private Bitmap img;

		public ImageFunctions()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void setImage(Bitmap imgIn)
		{
			img = imgIn;
		}

		public Size getBMPSize()
		{
			Size sz = img.Size;
			return sz;
		}
			

		public void padWidth(Bitmap img1)
		{
			int i, j;
			int fillW;

			for (i=0; i<img.Width; i++)
				for (j=0; j<img.Height; j++)
					img1.SetPixel(i,j,img.GetPixel(i,j));

			fillW = 16 - img.Width%16;
			for (i=0; i<img1.Height; i++)
				for (j=img1.Width-fillW; j<img1.Width; j++)
					img1.SetPixel(j, i, Color.LightGray);
		}

		public void padHeight(Bitmap img1)
		{
			int i, j;
			int fillH;

			for (i=0; i<img.Width; i++)
				for (j=0; j<img.Height; j++)
					img1.SetPixel(i,j,img.GetPixel(i,j));

			fillH = 16 - img.Height%16;
			for (i=0; i<img1.Width; i++)
				for (j=img1.Height-fillH; j<img1.Height; j++)
					img1.SetPixel(i, j, Color.LightGray);
		}
	}
}
