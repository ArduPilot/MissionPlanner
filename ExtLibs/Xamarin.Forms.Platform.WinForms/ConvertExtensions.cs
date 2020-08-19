using WDrawing = System.Drawing;
using WForms = System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	internal static class ConvertExtensions
	{
		/*
		public static Brush ToBrush(this Color color)
		{
			return new SolidColorBrush(color.ToWindowsColor());
		}
		*/

		public static WForms.HorizontalAlignment ToWindowsHorizontalAlignment(this TextAlignment horizonatal)
		{
			switch (horizonatal)
			{
				case TextAlignment.Start: return WForms.HorizontalAlignment.Left;
				case TextAlignment.Center: return WForms.HorizontalAlignment.Center;
				case TextAlignment.End: return WForms.HorizontalAlignment.Right;
			}
			return WForms.HorizontalAlignment.Left;
		}

		public static WDrawing.Color ToWindowsColor(this Color color)
		{
			return color.ToWindowsColor(WDrawing.SystemColors.Control);
		}

		public static WDrawing.Color ToWindowsColor(this Color color, WDrawing.Color defaultColor)
		{
			return 
				color == Color.Default ?
					defaultColor :
					WDrawing.Color.FromArgb((byte)(color.A * 255), (byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255));
		}

		public static WDrawing.FontStyle ToWindowsFontStyle(this FontAttributes self)
		{
			switch (self)
			{
				case FontAttributes.Bold:
					{
						return WDrawing.FontStyle.Bold;
					}

				case FontAttributes.Italic:
					{
						return WDrawing.FontStyle.Italic;
					}
			}
			return WDrawing.FontStyle.Regular;
		}

		public static Rectangle ToXamarinRectangle(this WDrawing.Rectangle self)
		{
			return new Rectangle(self.Left, self.Top, self.Width, self.Height);
		}
	}
}