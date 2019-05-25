
using MissionPlanner.Utilities.Drawing;

namespace GMap.NET.WindowsForms
{
    using System;
    using System.Drawing;
    using System.Runtime.Serialization;

    /// <summary>
    /// GMap.NET marker
    /// </summary>
    [Serializable]
#if !PocketPC
   public class GMapToolTip : ISerializable, IDisposable
#else
   public class GMapToolTip: IDisposable
#endif
   {
      GMapMarker marker;
      public GMapMarker Marker
      {
         get
         {
            return marker;
         }
         internal set
         {
            marker = value;
         }
      }

      public Point Offset;

      public static readonly StringFormat DefaultFormat = new StringFormat();

      /// <summary>
      /// string format
      /// </summary>
      [NonSerialized]
      public readonly StringFormat Format = DefaultFormat;

#if !PocketPC
      public static readonly Font DefaultFont = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold, GraphicsUnit.Pixel);
#else
      public static readonly Font DefaultFont = new Font(FontFamily.GenericSansSerif, 6, FontStyle.Bold);
#endif

      /// <summary>
      /// font
      /// </summary>
      [NonSerialized]
      public Font Font = DefaultFont;

#if !PocketPC
      public static readonly Pen DefaultStroke = new Pen(Color.FromArgb(140, Color.MidnightBlue));
#else
      public static readonly Pen DefaultStroke = new Pen(Color.MidnightBlue);
#endif

      /// <summary>
      /// specifies how the outline is painted
      /// </summary>
      [NonSerialized]
      public Pen Stroke = DefaultStroke;

#if !PocketPC
      public static readonly Brush DefaultFill = new SolidBrush(Color.FromArgb(222, Color.AliceBlue));
#else
      public static readonly Brush DefaultFill = new System.Drawing.SolidBrush(Color.AliceBlue);
#endif

      /// <summary>
      /// background color
      /// </summary>
      [NonSerialized]
      public Brush Fill = DefaultFill;

      public static readonly Brush DefaultForeground = new SolidBrush(Color.Navy);

      /// <summary>
      /// text foreground
      /// </summary>
      [NonSerialized]
      public Brush Foreground = DefaultForeground;

      /// <summary>
      /// text padding
      /// </summary>
      public Size TextPadding = new Size(10, 10);

      static GMapToolTip()
      {
          DefaultStroke.Width = 2;

#if !PocketPC
          DefaultStroke.LineJoin = LineJoin.Round;
          DefaultStroke.StartCap = LineCap.RoundAnchor;
#endif

#if !PocketPC
          DefaultFormat.LineAlignment = StringAlignment.Center;
#endif
          DefaultFormat.Alignment = StringAlignment.Center;
      }   

      public GMapToolTip(GMapMarker marker)
      {
         this.Marker = marker;
         this.Offset = new Point(14, -44);
      }

      public virtual void OnRender(IGraphics g)
      {
         System.Drawing.Size st = g.MeasureString(Marker.ToolTipText, Font).ToSize();
         System.Drawing.Rectangle rect = new System.Drawing.Rectangle(Marker.ToolTipPosition.X, Marker.ToolTipPosition.Y - st.Height, st.Width + TextPadding.Width, st.Height + TextPadding.Height);
         rect.Offset(Offset.X, Offset.Y);

         g.DrawLine(Stroke, Marker.ToolTipPosition.X, Marker.ToolTipPosition.Y, rect.X, rect.Y + rect.Height / 2);

         g.FillRectangle(Fill, rect);
         g.DrawRectangle(Stroke, rect);

#if PocketPC
         rect.Offset(0, (rect.Height - st.Height) / 2);
#endif

         g.DrawString(Marker.ToolTipText, Font, Foreground, rect, Format);
      }

#if !PocketPC
      #region ISerializable Members

      /// <summary>
      /// Initializes a new instance of the <see cref="GMapToolTip"/> class.
      /// </summary>
      /// <param name="info">The info.</param>
      /// <param name="context">The context.</param>
      protected GMapToolTip(SerializationInfo info, StreamingContext context)
      {
         this.Offset = Extensions.GetStruct<Point>(info, "Offset", Point.Empty);
         this.TextPadding = Extensions.GetStruct<Size>(info, "TextPadding", new Size(10, 10));
      }

      /// <summary>
      /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
      /// </summary>
      /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
      /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
      /// <exception cref="T:System.Security.SecurityException">
      /// The caller does not have the required permission.
      /// </exception>
      public void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         info.AddValue("Offset", this.Offset);
         info.AddValue("TextPadding", this.TextPadding);
      }

      #endregion
#endif

      #region IDisposable Members

      bool disposed = false;

      public void Dispose()
      {
         if(!disposed)
         {
            disposed = true;
         }
      }

      #endregion
   }
}
