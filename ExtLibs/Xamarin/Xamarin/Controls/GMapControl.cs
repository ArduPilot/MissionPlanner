
using ExifLibrary;
using MissionPlanner.Utilities;
using MissionPlanner.Utilities.Drawing;
using OpenTK.Graphics;
using OpenTK.Platform;
using SkiaSharp;
using Xamarin.Controls;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;
using PointF = System.Drawing.PointF;
using RectangleF = System.Drawing.RectangleF;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using SizeF = System.Drawing.SizeF;

namespace GMap.NET.WindowsForms
{


    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using GMap.NET;
    using GMap.NET.Internals;
    using GMap.NET.ObjectModel;
    using System.Diagnostics;
    using GMap.NET.MapProviders;

#if !PocketPC
    using System.Runtime.Serialization.Formatters.Binary;
    using GMap.NET.Projections;
#else
   
#endif

    /// <summary>
    /// GMap.NET control for Windows Forms
    /// </summary>   
    public partial class GMapControl : MySKGLView, Interface
   {
#if !PocketPC
      /// <summary>
      /// occurs when clicked on marker
      /// </summary>
      public event MarkerClick OnMarkerClick;

      /// <summary>
      /// occurs when clicked on polygon
      /// </summary>
      public event PolygonClick OnPolygonClick;

      /// <summary>
      /// occurs when clicked on route
      /// </summary>
      public event RouteClick OnRouteClick;

      /// <summary>
      /// occurs on mouse enters route area
      /// </summary>
      public event RouteEnter OnRouteEnter;

      /// <summary>
      /// occurs on mouse leaves route area
      /// </summary>
      public event RouteLeave OnRouteLeave;

      /// <summary>
      /// occurs when mouse selection is changed
      /// </summary>        
      public event SelectionChange OnSelectionChange;
#endif

      /// <summary>
      /// occurs on mouse enters marker area
      /// </summary>
      public event MarkerEnter OnMarkerEnter;

      /// <summary>
      /// occurs on mouse leaves marker area
      /// </summary>
      public event MarkerLeave OnMarkerLeave;

      /// <summary>
      /// occurs on mouse enters Polygon area
      /// </summary>
      public event PolygonEnter OnPolygonEnter;

      /// <summary>
      /// occurs on mouse leaves Polygon area
      /// </summary>
      public event PolygonLeave OnPolygonLeave;

      /// <summary>
      /// list of overlays, should be thread safe
      /// </summary>
      public readonly ObservableCollectionThreadSafe<GMapOverlay> _Overlays = new ObservableCollectionThreadSafe<GMapOverlay>();
      public ObservableCollectionThreadSafe<GMapOverlay> Overlays
      {
          get { return _Overlays; }
      }

      /// <summary>
      /// max zoom
      /// </summary>         
      [Category("GMap.NET")]
      [Description("maximum zoom level of map")]
      public int MaxZoom
      {
         get
         {
            return Core.maxZoom;
         }
         set
         {
            Core.maxZoom = value;
         }
      }

      /// <summary>
      /// min zoom
      /// </summary>      
      [Category("GMap.NET")]
      [Description("minimum zoom level of map")]
      public int MinZoom
      {
         get
         {
            return Core.minZoom;
         }
         set
         {
            Core.minZoom = value;
         }
      }

      /// <summary>
      /// map zooming type for mouse wheel
      /// </summary>
      [Category("GMap.NET")]
      [Description("map zooming type for mouse wheel")]
      public MouseWheelZoomType MouseWheelZoomType
      {
         get
         {
            return Core.MouseWheelZoomType;
         }
         set
         {
            Core.MouseWheelZoomType = value;
         }
      }

      /// <summary>
      /// text on empty tiles
      /// </summary>
      public string EmptyTileText = "We are sorry, but we don't\nhave imagery at this zoom\nlevel for this region.";

      /// <summary>
      /// pen for empty tile borders
      /// </summary>
#if !PocketPC
      public Pen EmptyTileBorders = new Pen(Brushes.White, 1);
#else
      public Pen EmptyTileBorders = new Pen(Color.White, 1);
#endif

      public bool ShowCenter = true;

      /// <summary>
      /// pen for scale info
      /// </summary>
#if !PocketPC
      public Pen ScalePen = new Pen(Brushes.Blue, 1);
      public Pen CenterPen = new Pen(Brushes.Red, 1);
#else
      public Pen ScalePen = new Pen(Color.Blue, 1);
      public Pen CenterPen = new Pen(Color.Red, 1);
#endif

#if !PocketPC
      /// <summary>
      /// area selection pen
      /// </summary>
      public Pen SelectionPen = new Pen(Brushes.Blue, 2);

      Brush SelectedAreaFill = new SolidBrush(Color.FromArgb(33, Color.RoyalBlue));
      Color selectedAreaFillColor = Color.FromArgb(33, Color.RoyalBlue);

      /// <summary>
      /// background of selected area
      /// </summary>
      [Category("GMap.NET")]
      [Description("background color od the selected area")]
      public Color SelectedAreaFillColor
      {
         get
         {
            return selectedAreaFillColor;
         }
         set
         {
            if(selectedAreaFillColor != value)
            {
               selectedAreaFillColor = value;

               if(SelectedAreaFill != null)
               {
                  SelectedAreaFill.Dispose();
                  SelectedAreaFill = null;
               }
               SelectedAreaFill = new SolidBrush(selectedAreaFillColor);
            }
         }
      }

      HelperLineOptions helperLineOption = HelperLineOptions.DontShow;

      /// <summary>
      /// draw lines at the mouse pointer position
      /// </summary>
      [Browsable(false)]
      public HelperLineOptions HelperLineOption
      {
         get
         {
            return helperLineOption;
         }
         set
         {
            helperLineOption = value;
            renderHelperLine = (helperLineOption == HelperLineOptions.ShowAlways);
            if(Core.IsStarted)
            {
               Invalidate();
            }
         }
      }

      public Pen HelperLinePen = new Pen(Color.Blue, 1);
      bool renderHelperLine = false;

      protected override void OnKeyDown(KeyEventArgs e)
      {
         if(HelperLineOption == HelperLineOptions.ShowOnModifierKey)
         {
            renderHelperLine = (e.Modifiers == Keys.Shift || e.Modifiers == Keys.Alt);
            if(renderHelperLine)
            {
               Invalidate();
            }
         }
         base.OnKeyDown(e);
      }

      protected override void OnKeyUp(KeyEventArgs e)
      {
         if(HelperLineOption == HelperLineOptions.ShowOnModifierKey)
         {
            renderHelperLine = (e.Modifiers == Keys.Shift || e.Modifiers == Keys.Alt);
            if(!renderHelperLine)
            {
               Invalidate();
            }
         }
         base.OnKeyUp(e);
      }
#endif

      Brush EmptytileBrush = new SolidBrush(Color.Navy);
      Color emptyTileColor = Color.Navy;

      /// <summary>
      /// color of empty tile background
      /// </summary>
      [Category("GMap.NET")]
      [Description("background color of the empty tile")]
      public Color EmptyTileColor
      {
         get
         {
            return emptyTileColor;
         }
         set
         {
            if(emptyTileColor != value)
            {
               emptyTileColor = value;

               if(EmptytileBrush != null)
               {
                  EmptytileBrush.Dispose();
                  EmptytileBrush = null;
               }
               EmptytileBrush = new SolidBrush(emptyTileColor);
            }
         }
      }

#if PocketPC
      readonly Brush TileGridLinesTextBrush = new SolidBrush(Color.Red);
      readonly Brush TileGridMissingTextBrush = new SolidBrush(Color.White);
      readonly Brush CopyrightBrush = new SolidBrush(Color.Navy);
#endif

      /// <summary>
      /// show map scale info
      /// </summary>
      public bool MapScaleInfoEnabled = false;

      /// <summary>
      /// enables filling empty tiles using lower level images
      /// </summary>
      public bool FillEmptyTiles = true;

      /// <summary>
      /// if true, selects area just by holding mouse and moving
      /// </summary>
      public bool DisableAltForSelection = false;

      /// <summary>
      /// retry count to get tile 
      /// </summary>
      [Browsable(false)]
      public int RetryLoadTile
      {
         get
         {
            return Core.RetryLoadTile;
         }
         set
         {
            Core.RetryLoadTile = value;
         }
      }

      /// <summary>
      /// how many levels of tiles are staying decompresed in memory
      /// </summary>
      [Browsable(false)]
      public int LevelsKeepInMemmory
      {
         get
         {
            return Core.LevelsKeepInMemmory;
         }

         set
         {
            Core.LevelsKeepInMemmory = value;
         }
      }

      /// <summary>
      /// map dragg button
      /// </summary>
      [Category("GMap.NET")]
      public MouseButtons DragButton = MouseButtons.Right;

      private bool showTileGridLines = false;

      /// <summary>
      /// shows tile gridlines
      /// </summary>
      [Category("GMap.NET")]
      [Description("shows tile gridlines")]
      public bool ShowTileGridLines
      {
         get
         {
            return showTileGridLines;
         }
         set
         {
            showTileGridLines = value;
            Invalidate();
         }
      }

      /// <summary>
      /// current selected area in map
      /// </summary>
      private RectLatLng selectedArea;

      [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public RectLatLng SelectedArea
      {
         get
         {
            return selectedArea;
         }
         set
         {
            selectedArea = value;

            if(Core.IsStarted)
            {
               Invalidate();
            }
         }
      }

      /// <summary>
      /// map boundaries
      /// </summary>
      public RectLatLng? BoundsOfMap = null;

      /// <summary>
      /// enables integrated DoubleBuffer for running on windows mobile
      /// </summary>
#if !PocketPC
      public bool ForceDoubleBuffer = false;
      readonly bool MobileMode = false;
#else
      readonly bool ForceDoubleBuffer = true;
#endif

       /// <summary>
       /// stops immediate marker/route/polygon invalidations;
       /// call Refresh to perform single refresh and reset invalidation state
       /// </summary>
       public bool HoldInvalidation { get; set; } = false;

      /// <summary>
      /// call this to stop HoldInvalidation and perform single forced instant refresh 
      /// </summary>
      public override void Refresh()
      {
         HoldInvalidation = false;

         lock(Core.invalidationLock)
         {
            Core.lastInvalidation = DateTime.Now;
         }

         base.Refresh();
      }

#if !DESIGN
      /// <summary>
      /// enque built-in thread safe invalidation
      /// </summary>
      public new void Invalidate()
      {
         if(Core.Refresh != null)
         {
            Core.Refresh.Set();
         }
      }
#endif

#if !PocketPC
      private bool _GrayScale = false;

      [Category("GMap.NET")]
      public bool GrayScaleMode
      {
         get
         {
            return _GrayScale;
         }
         set
         {
            _GrayScale = value;
            ColorMatrix = (value == true ? ColorMatrixs.GrayScale : null);
         }
      }

      private bool _Negative = false;

      [Category("GMap.NET")]
      public bool NegativeMode
      {
         get
         {
            return _Negative;
         }
         set
         {
            _Negative = value;
            ColorMatrix = (value == true ? ColorMatrixs.Negative : null);
         }
      }

      ColorMatrix colorMatrix;

      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [Browsable(false)]
      public ColorMatrix ColorMatrix
      {
         get
         {
            return colorMatrix;
         }
         set
         {
            colorMatrix = value;
            if(GMapProvider.TileImageProxy != null && GMapProvider.TileImageProxy is GMapImageProxy)
            {
               (GMapProvider.TileImageProxy as GMapImageProxy).ColorMatrix = value;
               if(Core.IsStarted)
               {
                  ReloadMap();
               }
            }
         }
      }
#endif

      // internal stuff
       public readonly Core _Core = new Core();
       public Core Core
       {
           get { return _Core; }
       }

      internal readonly Font CopyrightFont = new Font(FontFamily.GenericSansSerif, 7, FontStyle.Regular);
#if !PocketPC
      internal readonly Font MissingDataFont = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Bold);
#else
      internal readonly Font MissingDataFont = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular);
#endif
      Font ScaleFont = new Font(FontFamily.GenericSansSerif, 5, FontStyle.Italic);
      internal readonly StringFormat CenterFormat = new StringFormat();
      internal readonly StringFormat BottomFormat = new StringFormat();
#if !PocketPC
      readonly ImageAttributes TileFlipXYAttributes = new ImageAttributes();
#endif
      double zoomReal;
      Bitmap backBuffer;
      Graphics gxOff;

#if !DESIGN
      /// <summary>
      /// construct
      /// </summary>
      public GMapControl()
      {
#if !PocketPC
          if (!IsDesignerHosted)
#endif
          {
#if !PocketPC
              this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
              this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
              this.SetStyle(ControlStyles.UserPaint, true);
              this.SetStyle(ControlStyles.Opaque, true);
              ResizeRedraw = true;

              TileFlipXYAttributes.SetWrapMode(WrapMode.TileFlipXY);

              // only one mode will be active, to get mixed mode create new ColorMatrix
              GrayScaleMode = GrayScaleMode;
              NegativeMode = NegativeMode;
#endif
              Core.SystemType = "WindowsForms";

              RenderMode = RenderMode.GDI_PLUS;

              CenterFormat.Alignment = StringAlignment.Center;
              CenterFormat.LineAlignment = StringAlignment.Center;

              BottomFormat.Alignment = StringAlignment.Center;

#if !PocketPC
              BottomFormat.LineAlignment = StringAlignment.Far;
#endif

              if (GMaps.Instance.IsRunningOnMono)
              {
                  // no imports to move pointer
                  MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
              }

              Overlays.CollectionChanged += new NotifyCollectionChangedEventHandler(Overlays_CollectionChanged);
            }
      }

#endif

      static GMapControl()
      {       
#if !PocketPC
          if (!IsDesignerHosted)
#endif
          {
              GMapImageProxy.Enable();
#if !PocketPC
              GMaps.Instance.SQLitePing();
#endif
          }
      }

      void Overlays_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         if(e.NewItems != null)
         {
            foreach(GMapOverlay obj in e.NewItems)
            {
               if(obj != null)
               {
                  obj.Control = this;
               }
            }

            if(Core.IsStarted && !HoldInvalidation)
            {
               Invalidate();
            }
         }
      }

      void invalidatorEngage(object sender, ProgressChangedEventArgs e)
      {
         base.Invalidate();
      }

      /// <summary>
      /// update objects when map is draged/zoomed
      /// </summary>
      internal void ForceUpdateOverlays()
      {
         try
         {
            HoldInvalidation = true;

            foreach(GMapOverlay o in Overlays)
            {
               if(o.IsVisibile)
               {
                  o.ForceUpdate();
               }
            }
         }
         finally
         {
            Refresh();
         }
      }

      /// <summary>
      /// render map in GDI+
      /// </summary>
      /// <param name="g"></param>
      void DrawMap(IGraphics g)
      {
         if(Core.updatingBounds || MapProvider == EmptyProvider.Instance || MapProvider == null)
         {
            Debug.WriteLine("Core.updatingBounds");
            return;
         }

         g.CompositingMode = CompositingMode.SourceOver;
         g.InterpolationMode = InterpolationMode.NearestNeighbor;

         Core.tileDrawingListLock.AcquireReaderLock();
         Core.Matrix.EnterReadLock();
          try
          {
              foreach (var tilePoint in Core.tileDrawingList)
              {
                  {
                      Core.tileRect.Location = tilePoint.PosPixel;
                      if (ForceDoubleBuffer)
                      {
#if !PocketPC
                          if (MobileMode)
                          {
                              Core.tileRect.Offset(Core.renderOffset);
                          }
#else
                     Core.tileRect.Offset(Core.renderOffset);
#endif
                      }
                      Core.tileRect.OffsetNegative(Core.compensationOffset);

                      //if(Core.currentRegion.IntersectsWith(Core.tileRect) || IsRotated)
                      {
                          bool found = false;

                          Tile t = Core.Matrix.GetTileWithNoLock(Core.Zoom, tilePoint.PosXY);
                          if (t.NotEmpty)
                          {
                              // render tile
                              {
                                  foreach (GMapImage img in t.Overlays)
                                  {
                                      if (img != null && img.Img != null)
                                      {
                                          if (!found)
                                              found = true;

                                          if (!img.IsParent)
                                          {
#if !PocketPC
                                              if (!MapRenderTransform.HasValue)
                                              {
                                                  g.DrawImage(img.Img, Core.tileRect.X, Core.tileRect.Y,
                                                      Core.tileRect.Width, Core.tileRect.Height);
                                              }
                                              else
                                              {
                                                  g.DrawImage(img.Img,
                                                      new Rectangle((int) Core.tileRect.X, (int) Core.tileRect.Y,
                                                          (int) Core.tileRect.Width, (int) Core.tileRect.Height), 0, 0,
                                                      Core.tileRect.Width, Core.tileRect.Height, GraphicsUnit.Pixel,
                                                      TileFlipXYAttributes);
                                              }
#else
                                    g.DrawImage(img.Img, (int) Core.tileRect.X, (int) Core.tileRect.Y);
#endif
                                          }
#if !PocketPC
                                          else
                                          {
                                              // TODO: move calculations to loader thread
                                              System.Drawing.RectangleF srcRect =
                                                  new System.Drawing.RectangleF(
                                                      (float) (img.Xoff*(img.Img.Width/img.Ix)),
                                                      (float) (img.Yoff*(img.Img.Height/img.Ix)), (img.Img.Width/img.Ix),
                                                      (img.Img.Height/img.Ix));
                                              System.Drawing.Rectangle dst =
                                                  new System.Drawing.Rectangle((int) Core.tileRect.X,
                                                      (int) Core.tileRect.Y, (int) Core.tileRect.Width,
                                                      (int) Core.tileRect.Height);

                                              g.DrawImage(img.Img, dst, srcRect.X, srcRect.Y, srcRect.Width,
                                                  srcRect.Height, GraphicsUnit.Pixel, TileFlipXYAttributes);
                                          }
#endif
                                      }
                                  }
                              }
                          }
#if !PocketPC
                          else if (FillEmptyTiles && MapProvider.Projection is MercatorProjection)
                          {
                              #region -- fill empty lines --

                              int zoomOffset = 1;
                              Tile parentTile = Tile.Empty;
                              long Ix = 0;

                              while (!parentTile.NotEmpty && zoomOffset < Core.Zoom && zoomOffset <= LevelsKeepInMemmory)
                              {
                                  Ix = (long) Math.Pow(2, zoomOffset);
                                  parentTile = Core.Matrix.GetTileWithNoLock(Core.Zoom - zoomOffset++,
                                      new GPoint((int) (tilePoint.PosXY.X/Ix), (int) (tilePoint.PosXY.Y/Ix)));
                              }

                              if (parentTile.NotEmpty)
                              {
                                  long Xoff = Math.Abs(tilePoint.PosXY.X - (parentTile.Pos.X*Ix));
                                  long Yoff = Math.Abs(tilePoint.PosXY.Y - (parentTile.Pos.Y*Ix));

                                  // render tile 
                                  {
                                      foreach (GMapImage img in parentTile.Overlays)
                                      {
                                          if (img != null && img.Img != null && !img.IsParent)
                                          {
                                              if (!found)
                                                  found = true;

                                              System.Drawing.RectangleF srcRect =
                                                  new System.Drawing.RectangleF((float) (Xoff*(img.Img.Width/Ix)),
                                                      (float) (Yoff*(img.Img.Height/Ix)), (img.Img.Width/Ix),
                                                      (img.Img.Height/Ix));
                                              System.Drawing.Rectangle dst =
                                                  new System.Drawing.Rectangle((int) Core.tileRect.X,
                                                      (int) Core.tileRect.Y, (int) Core.tileRect.Width,
                                                      (int) Core.tileRect.Height);

                                              g.DrawImage(img.Img, dst, srcRect.X, srcRect.Y, srcRect.Width,
                                                  srcRect.Height, GraphicsUnit.Pixel, TileFlipXYAttributes);
                                              g.FillRectangle(SelectedAreaFill, dst);
                                          }
                                      }
                                  }
                              }

                              #endregion
                          }
#endif
                          // add text if tile is missing
                          if (!found)
                          {
                              lock (Core.FailedLoads)
                              {
                                  var lt = new LoadTask(tilePoint.PosXY, Core.Zoom);
                                  if (Core.FailedLoads.ContainsKey(lt))
                                  {
                                      var ex = Core.FailedLoads[lt];
#if !PocketPC
                                      g.FillRectangle(EmptytileBrush,
                                          new RectangleF(Core.tileRect.X, Core.tileRect.Y, Core.tileRect.Width,
                                              Core.tileRect.Height));

                                      g.DrawString("Exception: " + ex.Message, MissingDataFont, Brushes.Red,
                                          new RectangleF(Core.tileRect.X + 11, Core.tileRect.Y + 11,
                                              Core.tileRect.Width - 11, Core.tileRect.Height - 11));

                                      g.DrawString(EmptyTileText, MissingDataFont, Brushes.Blue,
                                          new RectangleF(Core.tileRect.X, Core.tileRect.Y, Core.tileRect.Width,
                                              Core.tileRect.Height), CenterFormat);

#else
                              g.FillRectangle(EmptytileBrush, new System.Drawing.Rectangle((int) Core.tileRect.X, (int) Core.tileRect.Y, (int) Core.tileRect.Width, (int) Core.tileRect.Height));

                              g.DrawString("Exception: " + ex.Message, MissingDataFont, TileGridMissingTextBrush, new RectangleF(Core.tileRect.X + 11, Core.tileRect.Y + 11, Core.tileRect.Width - 11, Core.tileRect.Height - 11));

                              g.DrawString(EmptyTileText, MissingDataFont, TileGridMissingTextBrush, new RectangleF(Core.tileRect.X, Core.tileRect.Y + Core.tileRect.Width / 2 + (ShowTileGridLines ? 11 : -22), Core.tileRect.Width, Core.tileRect.Height), BottomFormat);
#endif

                                      g.DrawRectangle(EmptyTileBorders, (int) Core.tileRect.X, (int) Core.tileRect.Y,
                                          (int) Core.tileRect.Width, (int) Core.tileRect.Height);
                                  }
                              }
                          }

                          if (ShowTileGridLines)
                          {
                              g.DrawRectangle(EmptyTileBorders, (int) Core.tileRect.X, (int) Core.tileRect.Y,
                                  (int) Core.tileRect.Width, (int) Core.tileRect.Height);
                              {
#if !PocketPC
                                  g.DrawString(
                                      (tilePoint.PosXY == Core.centerTileXYLocation ? "CENTER: " : "TILE: ") + tilePoint,
                                      MissingDataFont, Brushes.Red,
                                      new RectangleF(Core.tileRect.X, Core.tileRect.Y, Core.tileRect.Width,
                                          Core.tileRect.Height), CenterFormat);
#else
                           g.DrawString((tilePoint.PosXY == Core.centerTileXYLocation ? "" : "TILE: ") + tilePoint, MissingDataFont, TileGridLinesTextBrush, new RectangleF(Core.tileRect.X, Core.tileRect.Y, Core.tileRect.Width, Core.tileRect.Height), CenterFormat);
#endif
                              }
                          }
                      }
                  }
              }
          }
          finally
          {
              g.CompositingMode = CompositingMode.SourceOver;
              g.InterpolationMode = InterpolationMode.Default;

              Core.Matrix.LeaveReadLock();
              Core.tileDrawingListLock.ReleaseReaderLock();
          }
      }

      /// <summary>
      /// updates markers local position
      /// </summary>
      /// <param name="marker"></param>
      public void UpdateMarkerLocalPosition(GMapMarker marker)
      {
         GPoint p = FromLatLngToLocal(marker.Position);
         {
#if !PocketPC
            if(!MobileMode)
            {
               p.OffsetNegative(Core.renderOffset);
            }
#endif

            var f = new System.Drawing.Point((int)(p.X + marker.Offset.X), (int)(p.Y + marker.Offset.Y));
            marker.LocalPosition = f;
         }
      }

      /// <summary>
      /// updates routes local position
      /// </summary>
      /// <param name="route"></param>
      public void UpdateRouteLocalPosition(GMapRoute route)
      {
         route.LocalPoints.Clear();

         for (int i = 0; i < route.Points.Count; i++)
         {
            PointLatLng pg = route.Points[i]; 
            GPoint p = FromLatLngToLocal(pg);

#if !PocketPC
            if(!MobileMode)
            {
               p.OffsetNegative(Core.renderOffset);
            }
#endif

            //            if(IsRotated)
            //            {
            //#if !PocketPC
            //               System.Drawing.Point[] tt = new System.Drawing.Point[] { new System.Drawing.Point(p.X, p.Y) };
            //               rotationMatrix.TransformPoints(tt);
            //               var f = tt[0];

            //               p.X = f.X;
            //               p.Y = f.Y;
            //#endif
            //            }

            route.LocalPoints.Add(p);
         }
#if !PocketPC
         route.UpdateGraphicsPath();
#endif
      }

       /// <summary>
       /// updates polygons local position
       /// </summary>
       /// <param name="polygon"></param>
       public void UpdatePolygonLocalPosition(GMapPolygon polygon)
       {
           polygon.LocalPoints.Clear();

           for (int i = 0; i < polygon.Points.Count; i++) 
           {
               PointLatLng pg = polygon.Points[i]; 
               GPoint p = FromLatLngToLocal(pg);

#if !PocketPC
               if (!MobileMode)
               {
                   p.OffsetNegative(Core.renderOffset);
               }
#endif

               //            if(IsRotated)
               //            {
               //#if !PocketPC
               //               System.Drawing.Point[] tt = new System.Drawing.Point[] { new System.Drawing.Point(p.X, p.Y) };
               //               rotationMatrix.TransformPoints(tt);
               //               var f = tt[0];

               //               p.X = f.X;
               //               p.Y = f.Y;
               //#endif
               //            }

               polygon.LocalPoints.Add(p);
           }
#if !PocketPC
           unchecked
           {
               try
               {
                   polygon.UpdateGraphicsPath();
               }
               catch
               {
                   
               }
           }
#endif
       }

       /// <summary>
      /// sets zoom to max to fit rect
      /// </summary>
      /// <param name="rect"></param>
      /// <returns></returns>
      public bool SetZoomToFitRect(RectLatLng rect)
      {
         if(lazyEvents)
         {
            lazySetZoomToFitRect = rect;
         }
         else
         {
            int maxZoom = Core.GetMaxZoomToFitRect(rect);
            if(maxZoom > 0)
            {
               PointLatLng center = new PointLatLng(rect.Lat - (rect.HeightLat / 2), rect.Lng + (rect.WidthLng / 2));
               Position = center;

               if(maxZoom > MaxZoom)
               {
                  maxZoom = MaxZoom;
               }

               if((int)Zoom != maxZoom)
               {
                  Zoom = maxZoom;
               }

               return true;
            }
         }

         return false;
      }

      RectLatLng? lazySetZoomToFitRect = null;
      bool lazyEvents = true;

      /// <summary>
      /// sets to max zoom to fit all markers and centers them in map
      /// </summary>
      /// <param name="overlayId">overlay id or null to check all</param>
      /// <returns></returns>
      public bool ZoomAndCenterMarkers(string overlayId)
      {
         RectLatLng? rect = GetRectOfAllMarkers(overlayId);
         if(rect.HasValue)
         {
            return SetZoomToFitRect(rect.Value);
         }

         return false;
      }

      /// <summary>
      /// zooms and centers all route
      /// </summary>
      /// <param name="overlayId">overlay id or null to check all</param>
      /// <returns></returns>
      public bool ZoomAndCenterRoutes(string overlayId)
      {
         RectLatLng? rect = GetRectOfAllRoutes(overlayId);
         if(rect.HasValue)
         {
            return SetZoomToFitRect(rect.Value);
         }

         return false;
      }

      /// <summary>
      /// zooms and centers route 
      /// </summary>
      /// <param name="route"></param>
      /// <returns></returns>
      public bool ZoomAndCenterRoute(MapRoute route)
      {
         RectLatLng? rect = GetRectOfRoute(route);
         if(rect.HasValue)
         {
            return SetZoomToFitRect(rect.Value);
         }

         return false;
      }

      /// <summary>
      /// gets rectangle with all objects inside
      /// </summary>
      /// <param name="overlayId">overlay id or null to check all</param>
      /// <returns></returns>
      public RectLatLng? GetRectOfAllMarkers(string overlayId)
      {
         RectLatLng? ret = null;

         double left = double.MaxValue;
         double top = double.MinValue;
         double right = double.MinValue;
         double bottom = double.MaxValue;

         foreach(GMapOverlay o in Overlays)
         {
            if(overlayId == null || o.Id == overlayId)
            {
               if(o.IsVisibile && o.Markers.Count > 0)
               {
                  foreach(GMapMarker m in o.Markers)
                  {
                     if(m.IsVisible)
                     {
                        // left
                        if(m.Position.Lng < left)
                        {
                           left = m.Position.Lng;
                        }

                        // top
                        if(m.Position.Lat > top)
                        {
                           top = m.Position.Lat;
                        }

                        // right
                        if(m.Position.Lng > right)
                        {
                           right = m.Position.Lng;
                        }

                        // bottom
                        if(m.Position.Lat < bottom)
                        {
                           bottom = m.Position.Lat;
                        }
                     }
                  }
               }
            }
         }

         if(left != double.MaxValue && right != double.MinValue && top != double.MinValue && bottom != double.MaxValue)
         {
            ret = RectLatLng.FromLTRB(left, top, right, bottom);
         }

         return ret;
      }

      /// <summary>
      /// gets rectangle with all objects inside
      /// </summary>
      /// <param name="overlayId">overlay id or null to check all</param>
      /// <returns></returns>
      public RectLatLng? GetRectOfAllRoutes(string overlayId)
      {
         RectLatLng? ret = null;

         double left = double.MaxValue;
         double top = double.MinValue;
         double right = double.MinValue;
         double bottom = double.MaxValue;

         foreach(GMapOverlay o in Overlays)
         {
            if(overlayId == null || o.Id == overlayId)
            {
               if(o.IsVisibile && o.Routes.Count > 0)
               {
                  foreach(GMapRoute route in o.Routes)
                  {
                     if(route.IsVisible && route.From.HasValue && route.To.HasValue)
                     {
                        foreach(PointLatLng p in route.Points)
                        {
                           // left
                           if(p.Lng < left)
                           {
                              left = p.Lng;
                           }

                           // top
                           if(p.Lat > top)
                           {
                              top = p.Lat;
                           }

                           // right
                           if(p.Lng > right)
                           {
                              right = p.Lng;
                           }

                           // bottom
                           if(p.Lat < bottom)
                           {
                              bottom = p.Lat;
                           }
                        }
                     }
                  }
               }
            }
         }

         if(left != double.MaxValue && right != double.MinValue && top != double.MinValue && bottom != double.MaxValue)
         {
            ret = RectLatLng.FromLTRB(left, top, right, bottom);
         }

         return ret;
      }

      /// <summary>
      /// gets rect of route
      /// </summary>
      /// <param name="route"></param>
      /// <returns></returns>
      public RectLatLng? GetRectOfRoute(MapRoute route)
      {
         RectLatLng? ret = null;

         double left = double.MaxValue;
         double top = double.MinValue;
         double right = double.MinValue;
         double bottom = double.MaxValue;

         if(route.From.HasValue && route.To.HasValue)
         {
            foreach(PointLatLng p in route.Points)
            {
               // left
               if(p.Lng < left)
               {
                  left = p.Lng;
               }

               // top
               if(p.Lat > top)
               {
                  top = p.Lat;
               }

               // right
               if(p.Lng > right)
               {
                  right = p.Lng;
               }

               // bottom
               if(p.Lat < bottom)
               {
                  bottom = p.Lat;
               }
            }
            ret = RectLatLng.FromLTRB(left, top, right, bottom);
         }
         return ret;
      }

#if !PocketPC
      /// <summary>
      /// gets image of the current view
      /// </summary>
      /// <returns></returns>
      public Image ToImage()
      {
         Image ret = null;
         try
         {
            using(Bitmap bitmap = new Bitmap(Width, Height))
            {
               using(Graphics g = Graphics.FromImage(bitmap))
               {
                  using(Graphics gg = this.CreateGraphics())
                  {
#if !PocketPC
                 //    g.CopyFromScreen(PointToScreen(new System.Drawing.Point()).X, PointToScreen(new System.Drawing.Point()).Y, 0, 0, new System.Drawing.Size(Width, Height));
#else
                     throw new NotImplementedException("Not implemeted for PocketPC");
#endif
                  }
               }

               // Convert the Image to a png
               using(MemoryStream ms = new MemoryStream())
               {
                  bitmap.Save(ms, SKEncodedImageFormat.Png);
#if !PocketPC
                  ret = Image.FromStream(ms);
#else
                  throw new NotImplementedException("Not implemeted for PocketPC");
#endif
               }
            }
         }
         catch
         {
            ret = null;
         }
         return ret;
      }
#endif

      /// <summary>
      /// offset position in pixels
      /// </summary>
      /// <param name="x"></param>
      /// <param name="y"></param>
      public void Offset(int x, int y)
      {
         if(IsHandleCreated)
         {
            // need to fix in rotated mode usinf rotationMatrix
            // ...
            Core.DragOffset(new GPoint(x, y));

            ForceUpdateOverlays();
         }
      }

      #region UserControl Events

#if !PocketPC
      public readonly static bool IsDesignerHosted = LicenseManager.UsageMode == LicenseUsageMode.Designtime;

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);

         if (!IsDesignerHosted)
         {
             //MethodInvoker m = delegate
             //{
             //   Thread.Sleep(444);

             //OnSizeChanged(null);

             if (lazyEvents)
             {
                 lazyEvents = false;

                 if (lazySetZoomToFitRect.HasValue)
                 {
                     SetZoomToFitRect(lazySetZoomToFitRect.Value);
                     lazySetZoomToFitRect = null;
                 }
             }
             Core.OnMapOpen().ProgressChanged += new ProgressChangedEventHandler(invalidatorEngage);
             ForceUpdateOverlays();
             //};
             //this.BeginInvoke(m);
         }
      }
#else
      //delegate void MethodInvoker();
      bool IsHandleCreated = false;

      protected override void OnPaintBackground(PaintEventArgs e)
      {
         if(!IsHandleCreated)
         {
            IsHandleCreated = true;

            if(lazyEvents)
            {
               lazyEvents = false;

               if(lazySetZoomToFitRect.HasValue)
               {
                  SetZoomToFitRect(lazySetZoomToFitRect.Value);
                  lazySetZoomToFitRect = null;
               }
            }

            Core.OnMapOpen().ProgressChanged += new ProgressChangedEventHandler(invalidatorEngage);
            ForceUpdateOverlays();
         }
      }
#endif

#if !PocketPC
      protected override void OnCreateControl()
      {
         base.OnCreateControl();

      }

#endif

      protected override void Dispose(bool disposing)
      {
         if(disposing)
         {
            Core.OnMapClose();

            Overlays.CollectionChanged -= new NotifyCollectionChangedEventHandler(Overlays_CollectionChanged);

            foreach(var o in Overlays)
            {
               o.Dispose();
            }
            Overlays.Clear();

            ScaleFont.Dispose();
            ScalePen.Dispose();
            CenterFormat.Dispose();
            CenterPen.Dispose();
            BottomFormat.Dispose();
            CopyrightFont.Dispose();
            EmptyTileBorders.Dispose();
            EmptytileBrush.Dispose();

#if !PocketPC
            SelectedAreaFill.Dispose();
            SelectionPen.Dispose();
#endif
            if(backBuffer != null)
            {
               backBuffer.Dispose();
               backBuffer = null;
            }

            if(gxOff != null)
            {
               gxOff.Dispose();
               gxOff = null;
            }
            }
         base.Dispose(disposing);
      }

      PointLatLng selectionStart;
      PointLatLng selectionEnd;

#if !PocketPC
      float? MapRenderTransform = null;
#endif

      public Color EmptyMapBackground = Color.WhiteSmoke;

#if !DESIGN
       protected override void OnPaint(PaintEventArgs e)
       {

           var start = DateTime.Now;

           doPaint(e.Graphics);

           var ts = (DateTime.Now - start);

           Console.WriteLine("map render {0}", ts.TotalSeconds);
           base.OnPaint(e);



       }

       public void doPaint(IGraphics e)
       {
         {
            e.Clear(EmptyMapBackground);

#if !PocketPC
            if(MapRenderTransform.HasValue)
            {
               if(!MobileMode)
               {
                  var center = new GPoint(Width / 2, Height / 2);
                  var delta = center;
                  delta.OffsetNegative(Core.renderOffset);
                  var pos = center;
                  pos.OffsetNegative(delta);

                  e.RotateTransform(-Bearing);

                  e.ScaleTransform(MapRenderTransform.Value, MapRenderTransform.Value, MatrixOrder.Append);
                  e.TranslateTransform(pos.X, pos.Y, MatrixOrder.Append);

                  DrawMap(e);
                  e.ResetTransform();

                  e.TranslateTransform(pos.X, pos.Y, MatrixOrder.Append);
               }
               else
               {
                  DrawMap(e);
                  e.ResetTransform();
               }
               OnPaintOverlays(e);
            }
            else
#endif
            {
#if !PocketPC
               if(IsRotated)
               {
                  #region -- rotation --

                  e.TextRenderingHint = TextRenderingHint.AntiAlias;
                  e.SmoothingMode = SmoothingMode.AntiAlias;

                  var center = new GPoint(Width / 2, Height / 2);
                  var delta = center;
                  delta.OffsetNegative(Core.renderOffset);
                  var pos = center;
                  pos.OffsetNegative(delta);

                  e.RotateTransform(-Bearing);

                  e.TranslateTransform(pos.X, pos.Y, MatrixOrder.Append);

                  DrawMap(e);
                  e.ResetTransform();

                  e.TranslateTransform(pos.X, pos.Y, MatrixOrder.Append);

                  #endregion
               }
               else
#endif
               {
#if !PocketPC
                  if(!MobileMode)
                  {
                     e.TranslateTransform(Core.renderOffset.X, Core.renderOffset.Y);
                  }
#endif
                  DrawMap(e);
                  OnPaintOverlays(e);
               }
            }
         }

         
      }
#endif

#if !PocketPC
      readonly Matrix rotationMatrix = new Matrix();
      readonly Matrix rotationMatrixInvert = new Matrix();

      /// <summary>
      /// updates rotation matrix
      /// </summary>
      void UpdateRotationMatrix()
      {
         PointF center = new PointF(Core.Width / 2, Core.Height / 2);

        // rotationMatrix.Reset();
        // rotationMatrix.RotateAt(-Bearing, center);

        // rotationMatrixInvert.Reset();
        // rotationMatrixInvert.RotateAt(-Bearing, center);
        // rotationMatrixInvert.Invert();
      }

      /// <summary>
      /// returs true if map bearing is not zero
      /// </summary>    
      [Browsable(false)]
      public bool IsRotated
      {
         get
         {
            return Core.IsRotated;
         }
      }

      /// <summary>
      /// bearing for rotation of the map
      /// </summary>
      [Category("GMap.NET")]
      public float Bearing
      {
         get
         {
            return Core.bearing;
         }
         set
         {
            if(Core.bearing != value)
            {
               bool resize = Core.bearing == 0;
               Core.bearing = value;

               //if(VirtualSizeEnabled)
               //{
               //   c.X += (Width - Core.vWidth) / 2;
               //   c.Y += (Height - Core.vHeight) / 2;
               //}

               UpdateRotationMatrix();

               if(value != 0 && value % 360 != 0)
               {
                  Core.IsRotated = true;

                  if(Core.tileRectBearing.Size == Core.tileRect.Size)
                  {
                     Core.tileRectBearing = Core.tileRect;
                     Core.tileRectBearing.Inflate(1, 1);
                  }
               }
               else
               {
                  Core.IsRotated = false;
                  Core.tileRectBearing = Core.tileRect;
               }

               if(resize)
               {
                  Core.OnMapSizeChanged(Width, Height);
               }

               if(!HoldInvalidation && Core.IsStarted)
               {
                  ForceUpdateOverlays();
               }
            }
         }
      }
#endif

      /// <summary>
      /// override, to render something more
      /// </summary>
      /// <param name="g"></param>
      protected virtual void OnPaintOverlays(IGraphics g)
      {
#if !PocketPC
         g.SmoothingMode = SmoothingMode.Default;
#endif
         foreach(GMapOverlay o in Overlays)
         {
            if(o.IsVisibile)
            {
               o.OnRender(g);
            }
         }

         // center in virtual spcace...
#if DEBUG
         g.DrawLine(ScalePen, -20, 0, 20, 0);
         g.DrawLine(ScalePen, 0, -20, 0, 20);

#if PocketPC
         g.DrawString("debug build", CopyrightFont, CopyrightBrush, 2, CopyrightFont.Size);
#else
         g.DrawString("debug build", CopyrightFont, Brushes.Blue, 2, CopyrightFont.Height);
#endif

#endif

#if !PocketPC

         if(!MobileMode)
         {
            g.ResetTransform();
         }

         if(!SelectedArea.IsEmpty)
         {
            GPoint p1 = FromLatLngToLocal(SelectedArea.LocationTopLeft);
            GPoint p2 = FromLatLngToLocal(SelectedArea.LocationRightBottom);

            long x1 = p1.X;
            long y1 = p1.Y;
            long x2 = p2.X;
            long y2 = p2.Y;

            g.DrawRectangle(SelectionPen, x1, y1, x2 - x1, y2 - y1);
            g.FillRectangle(SelectedAreaFill, x1, y1, x2 - x1, y2 - y1);
         }

         if(renderHelperLine)
         {
            var p = PointToClient(Form.MousePosition);

            g.DrawLine(HelperLinePen, p.X, 0, p.X, Height);
            g.DrawLine(HelperLinePen, 0, p.Y, Width, p.Y);
         }
#endif
         if(ShowCenter)
         {
            g.DrawLine(CenterPen, Width / 2 - 5, Height / 2, Width / 2 + 5, Height / 2);
            g.DrawLine(CenterPen, Width / 2, Height / 2 - 5, Width / 2, Height / 2 + 5);
         }

         #region -- copyright --

         if(!string.IsNullOrEmpty(Core.provider.Copyright))
         {
#if !PocketPC
            g.DrawString(Core.provider.Copyright, CopyrightFont, Brushes.Navy, 3, Height - CopyrightFont.Height - 5);
#else
            g.DrawString(Core.provider.Copyright, CopyrightFont, CopyrightBrush, 3, Height - CopyrightFont.Size - 15);
#endif
         }

         #endregion

         #region -- draw scale --
#if !PocketPC
         if(MapScaleInfoEnabled)
         {
             var brush = new SolidBrush(ScalePen.Color);
             double transform = 1;
             if (MapRenderTransform.HasValue)
                 transform = MapRenderTransform.Value;

            if(Width > Core.pxRes5000km)
            {
                g.DrawRectangle(ScalePen, 10, 10, (int)(Core.pxRes5000km * transform), 10);
                g.DrawString("5000Km", ScaleFont, brush, (int)(Core.pxRes5000km * transform) + 10, 11);
            }
            if(Width > Core.pxRes1000km)
            {
               g.DrawRectangle(ScalePen, 10, 10, (int)(Core.pxRes1000km* transform), 10);
               g.DrawString("1000Km", ScaleFont, brush, (int)(Core.pxRes1000km * transform) + 10, 11);
            }
            if(Width > Core.pxRes100km && Zoom > 2)
            {
               g.DrawRectangle(ScalePen, 10, 10, (int)(Core.pxRes100km* transform), 10);
               g.DrawString("100Km", ScaleFont, brush, (int)(Core.pxRes100km * transform) + 10, 11);
            }
            if(Width > Core.pxRes10km && Zoom > 5)
            {
               g.DrawRectangle(ScalePen, 10, 10,(int)( Core.pxRes10km* transform), 10);
               g.DrawString("10Km", ScaleFont, brush, (int)(Core.pxRes10km * transform) + 10, 11);
            }
            if(Width > Core.pxRes1000m && Zoom >= 10)
            {
                g.DrawRectangle(ScalePen, 10, 10, (int)(Core.pxRes1000m * transform), 10);
                g.DrawString("1000m", ScaleFont, brush, (int)(Core.pxRes1000m * transform) + 10, 11);
            }
            if(Width > Core.pxRes100m && Zoom > 11)
            {
                g.DrawRectangle(ScalePen, 10, 10, (int)(Core.pxRes100m * transform), 10);
                g.DrawString("100m", ScaleFont, brush, (int)(Core.pxRes100m * transform) + 9, 11);
            }
         }
#endif
         #endregion
      }

#if !PocketPC

      /// <summary>
      /// shrinks map area, useful just for testing
      /// </summary>
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [Browsable(false)]
      public bool VirtualSizeEnabled
      {
         get
         {
            return Core.VirtualSizeEnabled;
         }
         set
         {
            Core.VirtualSizeEnabled = value;
         }
      }

      protected override void OnSizeChanged(EventArgs e)
      {
         base.OnSizeChanged(e);
#else
      protected override void OnResize(EventArgs e)
      {
         base.OnResize(e);
#endif
         if(Width == 0 || Height == 0)
         {
            Debug.WriteLine("minimized");
            return;
         }

         if(Width == Core.Width && Height == Core.Height)
         {
            Debug.WriteLine("maximized");
            return;
         }

#if !PocketPC
         if(!IsDesignerHosted)
#endif
         {
            if(ForceDoubleBuffer)
            {
               if(backBuffer != null)
               {
                  backBuffer.Dispose();
                  backBuffer = null;
               }
               if(gxOff != null)
               {
                  gxOff.Dispose();
                  gxOff = null;
               }

               backBuffer = new Bitmap(Width, Height);
               gxOff = Graphics.FromImage(backBuffer);
            }

#if !PocketPC
            if(VirtualSizeEnabled)
            {
               Core.OnMapSizeChanged(Core.vWidth, Core.vHeight);
            }
            else
#endif
            {
               Core.OnMapSizeChanged(Width, Height);
            }
            //Core.currentRegion = new GRect(-50, -50, Core.Width + 50, Core.Height + 50);

            if(Visible && IsHandleCreated && Core.IsStarted)
            {
#if !PocketPC
               if(IsRotated)
               {
                  UpdateRotationMatrix();
               }
#endif
               try
               {
                   ForceUpdateOverlays();
               }
               catch (System.OverflowException ex) { System.Diagnostics.Debug.WriteLine(ex); }
            }
         }
      }

      bool isSelected = false;
      protected override void OnMouseDown(MouseEventArgs e)
      {
         if(!IsMouseOverMarker)
         {
#if !PocketPC
            if(e.Button == DragButton && CanDragMap)
#else
            if(CanDragMap)
#endif
            {
#if !PocketPC
               Core.mouseDown = ApplyRotationInversion(e.X, e.Y);
#else
               Core.mouseDown = new GPoint(e.X, e.Y);
#endif
               this.Invalidate();
            }
            else if(!isSelected)
            {
               isSelected = true;
               SelectedArea = RectLatLng.Empty;
               selectionEnd = PointLatLng.Empty;
               selectionStart = FromLocalToLatLng(e.X, e.Y);
            }
         }

         base.OnMouseDown(e);
      }

      protected override void OnMouseUp(MouseEventArgs e)
      {
         base.OnMouseUp(e);

         if(isSelected)
         {
            isSelected = false;
         }

         if(Core.IsDragging)
         {
            if(isDragging)
            {
               isDragging = false;
               Debug.WriteLine("IsDragging = " + isDragging);
#if !PocketPC
               this.Cursor = cursorBefore;
               cursorBefore = null;
#endif
            }
            Core.EndDrag();

            if(BoundsOfMap.HasValue && !BoundsOfMap.Value.Contains(Position))
            {
               if(Core.LastLocationInBounds.HasValue)
               {
                  Position = Core.LastLocationInBounds.Value;
               }
            }
         }
         else
         {
#if !PocketPC
            if(e.Button == DragButton)
            {
               Core.mouseDown = GPoint.Empty;
            }

            if(!selectionEnd.IsEmpty && !selectionStart.IsEmpty)
            {
               bool zoomtofit = false;

               if(!SelectedArea.IsEmpty && Form.ModifierKeys == Keys.Shift)
               {
                  zoomtofit = SetZoomToFitRect(SelectedArea);
               }

               if(OnSelectionChange != null)
               {
                  OnSelectionChange(SelectedArea, zoomtofit);
               }
            }
            else
            {
               Invalidate();
            }
#endif
         }
      }

#if !PocketPC
      protected override void OnMouseClick(MouseEventArgs e)
      {
         if(!Core.IsDragging)
         {
            for(int i = Overlays.Count - 1; i >= 0; i--)
            {
               GMapOverlay o = Overlays[i];
               if(o != null && o.IsVisibile)
               {
                  foreach(GMapMarker m in o.Markers)
                  {
                     if(m.IsVisible && m.IsHitTestVisible)
                     {
                        #region -- check --

                        if((MobileMode && m.LocalArea.Contains(e.X, e.Y)) || (!MobileMode && m.LocalAreaInControlSpace.Contains(e.X, e.Y)))
                        {
                           if(OnMarkerClick != null)
                           {
                              OnMarkerClick(m, e);
                           }
                           break;
                        }

                        #endregion
                     }
                  }

                  foreach(GMapRoute m in o.Routes)
                  {
                     if(m.IsVisible && m.IsHitTestVisible)
                     {
                        #region -- check --

                        GPoint rp = new GPoint(e.X, e.Y);
#if !PocketPC
                        if(!MobileMode)
                        {
                           rp.OffsetNegative(Core.renderOffset);
                        }
#endif
                        if(m.IsInside((int)rp.X, (int)rp.Y))
                        {
                           if(OnRouteClick != null)
                           {
                              OnRouteClick(m, e);
                           }
                           break;
                        }
                        #endregion
                     }
                  }

                  foreach(GMapPolygon m in o.Polygons)
                  {
                     if(m.IsVisible && m.IsHitTestVisible)
                     {
                        #region -- check --
                        if(m.IsInside(FromLocalToLatLng(e.X, e.Y)))
                        {
                           if(OnPolygonClick != null)
                           {
                              OnPolygonClick(m, e);
                           }
                           break;
                        }
                        #endregion
                     }
                  }
               }
            }
         }

         //m_mousepos = e.Location;
         //if(HelperLineOption == HelperLineOptions.ShowAlways)
         //{
         //   base.Invalidate();
         //}

         base.OnMouseClick(e);
      }
#endif
#if !PocketPC
      /// <summary>
      /// apply transformation if in rotation mode
      /// </summary>
      GPoint ApplyRotationInversion(int x, int y)
      {
         GPoint ret = new GPoint(x, y);

         if(IsRotated)
         {

            System.Drawing.Point[] tt = new System.Drawing.Point[] { new System.Drawing.Point(x, y) };
         //   rotationMatrixInvert.TransformPoints(tt);
            var f = tt[0];

            ret.X = f.X;
            ret.Y = f.Y;
         }

         return ret;
      }

      /// <summary>
      /// apply transformation if in rotation mode
      /// </summary>
      GPoint ApplyRotation(int x, int y)
      {
         GPoint ret = new GPoint(x, y);

         if(IsRotated)
         {
            System.Drawing.Point[] tt = new System.Drawing.Point[] { new System.Drawing.Point(x, y) };
          //  rotationMatrix.TransformPoints(tt);
            var f = tt[0];

            ret.X = f.X;
            ret.Y = f.Y;
         }

         return ret;
      }

      Cursor cursorBefore = Cursors.Default;
#endif

      /// <summary>
      /// Gets the width and height of a rectangle centered on the point the mouse
      /// button was pressed, within which a drag operation will not begin.
      /// </summary>
#if !PocketPC
      public Size DragSize = Size.Empty;// SystemInformation.DragSize;
#else
      public Size DragSize = new Size(4, 4);
#endif

      protected override void OnMouseMove(MouseEventArgs e)
      {
         if(!Core.IsDragging && !Core.mouseDown.IsEmpty)
         {
#if PocketPC
            GPoint p = new GPoint(e.X, e.Y);
#else
            GPoint p = ApplyRotationInversion(e.X, e.Y);
#endif
            if(Math.Abs(p.X - Core.mouseDown.X) * 2 >= DragSize.Width || Math.Abs(p.Y - Core.mouseDown.Y) * 2 >= DragSize.Height)
            {
               Core.BeginDrag(Core.mouseDown);
            }
         }

         if(Core.IsDragging)
         {
            if(!isDragging)
            {
               isDragging = true;
               Debug.WriteLine("IsDragging = " + isDragging);

#if !PocketPC
               cursorBefore = this.Cursor;
               this.Cursor = Cursors.SizeAll;
#endif
            }

            if(BoundsOfMap.HasValue && !BoundsOfMap.Value.Contains(Position))
            {
               // ...
            }
            else
            {
#if !PocketPC
               Core.mouseCurrent = ApplyRotationInversion(e.X, e.Y);
#else
               Core.mouseCurrent = new GPoint(e.X, e.Y);
#endif
               Core.Drag(Core.mouseCurrent);

#if !PocketPC
               if(MobileMode)
               {
                  ForceUpdateOverlays();
               }
#else
               ForceUpdateOverlays();
#endif

               base.Invalidate();
            }
         }
         else
         {
#if !PocketPC
            if(isSelected && !selectionStart.IsEmpty && (Form.ModifierKeys == Keys.Alt || Form.ModifierKeys == Keys.Shift || DisableAltForSelection))
            {
               selectionEnd = FromLocalToLatLng(e.X, e.Y);
               {
                  GMap.NET.PointLatLng p1 = selectionStart;
                  GMap.NET.PointLatLng p2 = selectionEnd;

                  double x1 = Math.Min(p1.Lng, p2.Lng);
                  double y1 = Math.Max(p1.Lat, p2.Lat);
                  double x2 = Math.Max(p1.Lng, p2.Lng);
                  double y2 = Math.Min(p1.Lat, p2.Lat);

                  SelectedArea = new RectLatLng(y1, x1, x2 - x1, y1 - y2);
               }
            }
            else
#endif
               if(Core.mouseDown.IsEmpty)
               {
                  for(int i = Overlays.Count - 1; i >= 0; i--)
                  {
                     GMapOverlay o = Overlays[i];
                     if(o != null && o.IsVisibile)
                     {
                        foreach(GMapMarker m in o.Markers)
                        {
                           if(m.IsVisible && m.IsHitTestVisible)
                           {
                              #region -- check --
#if !PocketPC
                              if((MobileMode && m.LocalArea.Contains(e.X, e.Y)) || (!MobileMode && m.LocalAreaInControlSpace.Contains(e.X, e.Y)))
#else
                           if(m.LocalArea.Contains(e.X, e.Y))
#endif
                              {
                                 if(!m.IsMouseOver)
                                 {
#if !PocketPC
                                    SetCursorHandOnEnter();
#endif
                                    m.IsMouseOver = true;
                                    IsMouseOverMarker = true;

                                    if(OnMarkerEnter != null)
                                    {
                                       OnMarkerEnter(m);
                                    }

                                    Invalidate();
                                 }
                              }
                              else if(m.IsMouseOver)
                              {
                                 m.IsMouseOver = false;
                                 IsMouseOverMarker = false;
#if !PocketPC
                                 RestoreCursorOnLeave();
#endif
                                 if(OnMarkerLeave != null)
                                 {
                                    OnMarkerLeave(m);
                                 }

                                 Invalidate();
                              }
                              #endregion
                           }
                        }

#if !PocketPC
                        foreach(GMapRoute m in o.Routes)
                        {
                           if(m.IsVisible && m.IsHitTestVisible)
                           {
                              #region -- check --

                              GPoint rp = new GPoint(e.X, e.Y);
#if !PocketPC
                              if(!MobileMode)
                              {
                                 rp.OffsetNegative(Core.renderOffset);
                              }
#endif
                              if(m.IsInside((int)rp.X, (int)rp.Y))
                              {
                                 if(!m.IsMouseOver)
                                 {
#if !PocketPC
                                    SetCursorHandOnEnter();
#endif
                                    m.IsMouseOver = true;
                                    IsMouseOverRoute = true;

                                    if(OnRouteEnter != null)
                                    {
                                       OnRouteEnter(m);
                                    }

                                    Invalidate();
                                 }
                              }
                              else
                              {
                                 if(m.IsMouseOver)
                                 {
                                    m.IsMouseOver = false;
                                    IsMouseOverRoute = false;
#if !PocketPC
                                    RestoreCursorOnLeave();
#endif
                                    if(OnRouteLeave != null)
                                    {
                                       OnRouteLeave(m);
                                    }

                                    Invalidate();
                                 }
                              }
                              #endregion
                           }
                        }
#endif

                        foreach(GMapPolygon m in o.Polygons)
                        {
                           if(m.IsVisible && m.IsHitTestVisible)
                           {
                              #region -- check --                               
#if !PocketPC
                               GPoint rp = new GPoint(e.X, e.Y);

                               if (!MobileMode)
                               {
                                   rp.OffsetNegative(Core.renderOffset);
                               }

                              if(m.IsInsideLocal((int)rp.X, (int)rp.Y))
#else
                              if (m.IsInside(FromLocalToLatLng(e.X, e.Y)))
#endif
                              {
                                 if(!m.IsMouseOver)
                                 {
#if !PocketPC
                                    SetCursorHandOnEnter();
#endif
                                    m.IsMouseOver = true;
                                    IsMouseOverPolygon = true;

                                    if(OnPolygonEnter != null)
                                    {
                                       OnPolygonEnter(m);
                                    }

                                    Invalidate();
                                 }
                              }
                              else
                              {
                                 if(m.IsMouseOver)
                                 {
                                    m.IsMouseOver = false;
                                    IsMouseOverPolygon = false;
#if !PocketPC
                                    RestoreCursorOnLeave();
#endif
                                    if(OnPolygonLeave != null)
                                    {
                                       OnPolygonLeave(m);
                                    }

                                    Invalidate();
                                 }
                              }
                              #endregion
                           }
                        }
                     }
                  }
               }

#if !PocketPC
            if(renderHelperLine)
            {
               base.Invalidate();
            }
#endif
         }

         base.OnMouseMove(e);
      }

#if !PocketPC

      public void RestoreCursorOnLeave()
      {
         if(overObjectCount <= 0 && cursorBefore != null)
         {
            overObjectCount = 0;
            this.Cursor = this.cursorBefore;
            cursorBefore = null;
         }
      }

      internal void SetCursorHandOnEnter()
      {
         if(overObjectCount <= 0 && Cursor != Cursors.Hand)
         {
            overObjectCount = 0;
            cursorBefore = this.Cursor;
            this.Cursor = Cursors.Hand;
         }
      }

      /// <summary>
      /// prevents focusing map if mouse enters it's area
      /// </summary>
      public bool DisableFocusOnMouseEnter = false;

      protected override void OnMouseEnter(EventArgs e)
      {
         base.OnMouseEnter(e);

         if(!DisableFocusOnMouseEnter)
         {
            Focus();
         }
         mouseIn = true;
      }

      bool mouseIn = false;

      protected override void OnMouseLeave(EventArgs e)
      {
          base.OnMouseLeave(e);
          mouseIn = false;
      }

      /// <summary>
      /// reverses MouseWheel zooming direction
      /// </summary>
      public bool InvertedMouseWheelZooming = false;

      /// <summary>
      /// lets you zoom by MouseWheel even when pointer is in area of marker
      /// </summary>
      public bool IgnoreMarkerOnMouseWheel = false;

      protected override void OnMouseWheel(MouseEventArgs e)
      {
         base.OnMouseWheel(e);         

         if(mouseIn && (!IsMouseOverMarker || IgnoreMarkerOnMouseWheel) && !Core.IsDragging)
         {
            if(Core.mouseLastZoom.X != e.X && Core.mouseLastZoom.Y != e.Y)
            {
               if(MouseWheelZoomType == MouseWheelZoomType.MousePositionAndCenter)
               {
                  Core.position = FromLocalToLatLng(e.X, e.Y);
               }
               else if(MouseWheelZoomType == MouseWheelZoomType.ViewCenter)
               {
                  Core.position = FromLocalToLatLng((int)Width / 2, (int)Height / 2);
               }
               else if(MouseWheelZoomType == MouseWheelZoomType.MousePositionWithoutCenter)
               {
                  Core.position = FromLocalToLatLng(e.X, e.Y);
               }

               Core.mouseLastZoom.X = e.X;
               Core.mouseLastZoom.Y = e.Y;
            }

            // set mouse position to map center
            if(MouseWheelZoomType != MouseWheelZoomType.MousePositionWithoutCenter)
            {
               if(!GMaps.Instance.IsRunningOnMono)
               {
               //   System.Drawing.Point p = PointToScreen(new System.Drawing.Point(Width / 2, Height / 2));
                //  Stuff.SetCursorPos((int)p.X, (int)p.Y);
               }
            }

            Core.MouseWheelZooming = true;

            if(e.Delta > 0)
            {
               if(!InvertedMouseWheelZooming)
               {
                  Zoom = ((int)Zoom) + 1;
               }
               else
               {
                  Zoom = ((int)(Zoom + 0.99)) - 1;
               }
            }
            else if(e.Delta < 0)
            {
               if(!InvertedMouseWheelZooming)
               {
                  Zoom = ((int)(Zoom + 0.99)) - 1;
               }
               else
               {
                  Zoom = ((int)Zoom) + 1;
               }
            }

            Core.MouseWheelZooming = false;
         }
      }
#endif
      #endregion

      #region IGControl Members

      /// <summary>
      /// Call it to empty tile cache & reload tiles
      /// </summary>
      public void ReloadMap()
      {
         Core.ReloadMap();
      }

      /// <summary>
      /// set current position using keywords
      /// </summary>
      /// <param name="keys"></param>
      /// <returns>true if successfull</returns>
      public GeoCoderStatusCode SetPositionByKeywords(string keys)
      {
         GeoCoderStatusCode status = GeoCoderStatusCode.Unknow;

         GeocodingProvider gp = MapProvider as GeocodingProvider;
         if(gp == null)
         {
            gp = GMapProviders.OpenStreetMap as GeocodingProvider;
         }

         if(gp != null)
         {
            var pt = gp.GetPoint(keys, out status);
            if(status == GeoCoderStatusCode.G_GEO_SUCCESS && pt.HasValue)
            {
               Position = pt.Value;
            }
         }

         return status;
      }

      /// <summary>
      /// gets world coordinate from local control coordinate 
      /// </summary>
      /// <param name="x"></param>
      /// <param name="y"></param>
      /// <returns></returns>
      public PointLatLng FromLocalToLatLng(int x, int y)
      {
#if !PocketPC
         if(MapRenderTransform.HasValue)
         {
            //var xx = (int)(Core.renderOffset.X + ((x - Core.renderOffset.X) / MapRenderTransform.Value));
            //var yy = (int)(Core.renderOffset.Y + ((y - Core.renderOffset.Y) / MapRenderTransform.Value));

            //PointF center = new PointF(Core.Width / 2, Core.Height / 2);

            //Matrix m = new Matrix();
            //m.Translate(-Core.renderOffset.X, -Core.renderOffset.Y);
            //m.Scale(MapRenderTransform.Value, MapRenderTransform.Value);

            //System.Drawing.Point[] tt = new System.Drawing.Point[] { new System.Drawing.Point(x, y) };
            //m.TransformPoints(tt);
            //var z = tt[0];

            //

            x = (int)(Core.renderOffset.X + ((x - Core.renderOffset.X) / MapRenderTransform.Value));
            y = (int)(Core.renderOffset.Y + ((y - Core.renderOffset.Y) / MapRenderTransform.Value));
         }

         if(IsRotated)
         {
            System.Drawing.Point[] tt = new System.Drawing.Point[] { new System.Drawing.Point(x, y) };
         //   rotationMatrixInvert.TransformPoints(tt);
            var f = tt[0];

            if(VirtualSizeEnabled)
            {
               f.X += (Width - Core.vWidth) / 2;
               f.Y += (Height - Core.vHeight) / 2;
            }

            x = f.X;
            y = f.Y;
         }
#endif
         return Core.FromLocalToLatLng(x, y);
      }

      /// <summary>
      /// gets local coordinate from world coordinate
      /// </summary>
      /// <param name="point"></param>
      /// <returns></returns>
      public GPoint FromLatLngToLocal(PointLatLng point)
      {
         GPoint ret = Core.FromLatLngToLocal(point);

#if !PocketPC
         if(MapRenderTransform.HasValue)
         {
            ret.X = (int)(Core.renderOffset.X + ((Core.renderOffset.X - ret.X) * -MapRenderTransform.Value));
            ret.Y = (int)(Core.renderOffset.Y + ((Core.renderOffset.Y - ret.Y) * -MapRenderTransform.Value));
         }

         if(IsRotated)
         {
            System.Drawing.Point[] tt = new System.Drawing.Point[] { new System.Drawing.Point((int)ret.X, (int)ret.Y) };
        //    rotationMatrix.TransformPoints(tt);
            var f = tt[0];

            if(VirtualSizeEnabled)
            {
               f.X += (Width - Core.vWidth) / 2;
               f.Y += (Height - Core.vHeight) / 2;
            }

            ret.X = f.X;
            ret.Y = f.Y;
         }

#endif
         return ret;
      }


      private ScaleModes scaleMode = ScaleModes.Integer;

      [Category("GMap.NET")]
      [Description("map scale type")]
      public ScaleModes ScaleMode
      {
         get
         {
            return scaleMode;
         }
         set
         {
            scaleMode = value;
         }
      }

      [Category("GMap.NET"), DefaultValue(0)]
      public double Zoom
      {
         get
         {
            return zoomReal;
         }
         set
         {
            if(zoomReal != value)
            {
               Debug.WriteLine("ZoomPropertyChanged: " + zoomReal + " -> " + value);

               if(value > MaxZoom)
               {
                  zoomReal = MaxZoom;
               }
               else if(value < MinZoom)
               {
                  zoomReal = MinZoom;
               }
               else
               {
                  zoomReal = value;
               }

#if !PocketPC
               double remainder = value % 1;
               if(ScaleMode == ScaleModes.Fractional && remainder != 0)
               {
                  float scaleValue = (float)Math.Pow(2d, remainder);
                  {
                     MapRenderTransform = scaleValue;
                  }

                  ZoomStep = Convert.ToInt32(value - remainder);
               }
               else
#endif
               {
#if !PocketPC
                  MapRenderTransform = null;
#endif
                  ZoomStep = (int)Math.Floor(value);
                  //zoomReal = ZoomStep;
               }

               if(Core.IsStarted && !IsDragging)
               {
                  ForceUpdateOverlays();
               }
            }
         }
      }

      /// <summary>
      /// map zoom level
      /// </summary>
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [Browsable(false)]
      internal int ZoomStep
      {
         get
         {
            return Core.Zoom;
         }
         set
         {
            if(value > MaxZoom)
            {
               Core.Zoom = MaxZoom;
            }
            else if(value < MinZoom)
            {
               Core.Zoom = MinZoom;
            }
            else
            {
               Core.Zoom = value;
            }
         }
      }

      /// <summary>
      /// current map center position
      /// </summary>
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [Browsable(false)]
      public PointLatLng Position
      {
         get
         {
            return Core.Position;
         }
         set
         {
            Core.Position = value;

            if(Core.IsStarted)
            {
               ForceUpdateOverlays();
            }
         }
      }

      /// <summary>
      /// current position in pixel coordinates
      /// </summary>
      [Browsable(false)]
      public GPoint PositionPixel
      {
         get
         {
            return Core.PositionPixel;
         }
      }

      /// <summary>
      /// location of cache
      /// </summary>
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [Browsable(false)]
      public string CacheLocation
      {
         get
         {
#if !DESIGN
            return CacheLocator.Location;
#else
            return string.Empty;
#endif
         }
         set
         {
#if !DESIGN
            CacheLocator.Location = value;
#endif
         }
      }

      bool isDragging = false;

      /// <summary>
      /// is user dragging map
      /// </summary>
      [Browsable(false)]
      public bool IsDragging
      {
         get
         {
            return isDragging;
         }
      }

      bool isMouseOverMarker;
      internal int overObjectCount = 0;

      /// <summary>
      /// is mouse over marker
      /// </summary>
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [Browsable(false)]
      public bool IsMouseOverMarker
      {
         get
         {
            return isMouseOverMarker;
         }
          set
         {
            isMouseOverMarker = value;
            overObjectCount += value ? 1 : -1;
         }
      }

      bool isMouseOverRoute;

      /// <summary>
      /// is mouse over route
      /// </summary>
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [Browsable(false)]
      public bool IsMouseOverRoute
      {
         get
         {
            return isMouseOverRoute;
         }
          set
         {
            isMouseOverRoute = value;
            overObjectCount += value ? 1 : -1;            
         }
      }

      bool isMouseOverPolygon;

      /// <summary>
      /// is mouse over polygon
      /// </summary>
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [Browsable(false)]
      public bool IsMouseOverPolygon
      {
         get
         {
            return isMouseOverPolygon;
         }
          set
         {
            isMouseOverPolygon = value;
            overObjectCount += value ? 1 : -1;
         }
      }

      /// <summary>
      /// gets current map view top/left coordinate, width in Lng, height in Lat
      /// </summary>
      [Browsable(false)]
      public RectLatLng ViewArea
      {
         get
         {
            return Core.ViewArea;
         }
      }

      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [Browsable(false)]
      public GMapProvider MapProvider
      {
         get
         {
            return Core.Provider;
         }
         set
         {
            if(Core.Provider == null || !Core.Provider.Equals(value))
            {
               Debug.WriteLine("MapType: " + Core.Provider.Name + " -> " + value.Name);

               RectLatLng viewarea = SelectedArea;
               if(viewarea != RectLatLng.Empty)
               {
                  Position = new PointLatLng(viewarea.Lat - viewarea.HeightLat / 2, viewarea.Lng + viewarea.WidthLng / 2);
               }
               else
               {
                  viewarea = ViewArea;
               }

               Core.Provider = value;

               if(Core.IsStarted)
               {
                  if(Core.zoomToArea)
                  {
                     // restore zoomrect as close as possible
                     if(viewarea != RectLatLng.Empty && viewarea != ViewArea)
                     {
                        int bestZoom = Core.GetMaxZoomToFitRect(viewarea);
                        if(bestZoom > 0 && Zoom != bestZoom)
                        {
                           Zoom = bestZoom;
                        }
                     }
                  }
                  else
                  {
                     ForceUpdateOverlays();
                  }
               }
            }
         }
      }

      /// <summary>
      /// is routes enabled
      /// </summary>
      [Category("GMap.NET")]
      public bool RoutesEnabled
      {
         get
         {
            return Core.RoutesEnabled;
         }
         set
         {
            Core.RoutesEnabled = value;
         }
      }

      /// <summary>
      /// is polygons enabled
      /// </summary>
      [Category("GMap.NET")]
      public bool PolygonsEnabled
      {
         get
         {
            return Core.PolygonsEnabled;
         }
         set
         {
            Core.PolygonsEnabled = value;
         }
      }

      /// <summary>
      /// is markers enabled
      /// </summary>
      [Category("GMap.NET")]
      public bool MarkersEnabled
      {
         get
         {
            return Core.MarkersEnabled;
         }
         set
         {
            Core.MarkersEnabled = value;
         }
      }

      /// <summary>
      /// can user drag map
      /// </summary>
      [Category("GMap.NET")]
      public bool CanDragMap
      {
         get
         {
            return Core.CanDragMap;
         }
         set
         {
            Core.CanDragMap = value;
         }
      }

      /// <summary>
      /// map render mode
      /// </summary>
      [Browsable(false)]
      public RenderMode RenderMode
      {
         get
         {
            return Core.RenderMode;
         }
         internal set
         {
            Core.RenderMode = value;
         }
      }

      /// <summary>
      /// gets map manager
      /// </summary>
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      [Browsable(false)]
      public GMaps Manager
      {
         get
         {
            return GMaps.Instance;
         }
      }

      #endregion

      #region IGControl event Members

      /// <summary>
      /// occurs when current position is changed
      /// </summary>
      public event PositionChanged OnPositionChanged
      {
         add
         {
            Core.OnCurrentPositionChanged += value;
         }
         remove
         {
            Core.OnCurrentPositionChanged -= value;
         }
      }

      /// <summary>
      /// occurs when tile set load is complete
      /// </summary>
      public event TileLoadComplete OnTileLoadComplete
      {
         add
         {
            Core.OnTileLoadComplete += value;
         }
         remove
         {
            Core.OnTileLoadComplete -= value;
         }
      }

      /// <summary>
      /// occurs when tile set is starting to load
      /// </summary>
      public event TileLoadStart OnTileLoadStart
      {
         add
         {
            Core.OnTileLoadStart += value;
         }
         remove
         {
            Core.OnTileLoadStart -= value;
         }
      }

      /// <summary>
      /// occurs on map drag
      /// </summary>
      public event MapDrag OnMapDrag
      {
         add
         {
            Core.OnMapDrag += value;
         }
         remove
         {
            Core.OnMapDrag -= value;
         }
      }

      /// <summary>
      /// occurs on map zoom changed
      /// </summary>
      public event MapZoomChanged OnMapZoomChanged
      {
         add
         {
            Core.OnMapZoomChanged += value;
         }
         remove
         {
            Core.OnMapZoomChanged -= value;
         }
      }

      /// <summary>
      /// occures on map type changed
      /// </summary>
      public event MapTypeChanged OnMapTypeChanged
      {
         add
         {
            Core.OnMapTypeChanged += value;
         }
         remove
         {
            Core.OnMapTypeChanged -= value;
         }
      }

      /// <summary>
      /// occurs on empty tile displayed
      /// </summary>
      public event EmptyTileError OnEmptyTileError
      {
         add
         {
            Core.OnEmptyTileError += value;
         }
         remove
         {
            Core.OnEmptyTileError -= value;
         }
      }

      #endregion

#if !PocketPC
      #region Serialization

      static readonly BinaryFormatter BinaryFormatter = new BinaryFormatter();
      private IWindowInfo _windowInfoinfo;
      private GraphicsContext _graphicsContextGraphicsContext;
      private SKSurface _skSurface;
      private SKSize _screenCanvasSize;
      private GRContext _grContext;

      /// <summary>
      /// Serializes the overlays.
      /// </summary>
      /// <param name="stream">The stream.</param>
      public void SerializeOverlays(Stream stream)
      {
         if(stream == null)
         {
            throw new ArgumentNullException("stream");
         }

         // Create an array from the overlays
         GMapOverlay[] overlayArray = new GMapOverlay[this.Overlays.Count];
         this.Overlays.CopyTo(overlayArray, 0);

         // Serialize the overlays
         BinaryFormatter.Serialize(stream, overlayArray);
      }

      /// <summary>
      /// De-serializes the overlays.
      /// </summary>
      /// <param name="stream">The stream.</param>
      public void DeserializeOverlays(Stream stream)
      {
         if(stream == null)
         {
            throw new ArgumentNullException("stream");
         }

         // De-serialize the overlays
         GMapOverlay[] overlayArray = BinaryFormatter.Deserialize(stream) as GMapOverlay[];

         // Populate the collection of overlays.
         foreach(GMapOverlay overlay in overlayArray)
         {
            overlay.Control = this;
            this.Overlays.Add(overlay);
         }

         this.ForceUpdateOverlays();
      }

      #endregion
#endif
   }

   public enum ScaleModes
   {
      /// <summary>
      /// no scaling
      /// </summary>
      Integer,

#if !PocketPC
      /// <summary>
      /// scales to fractional level, CURRENT VERSION DOESN'T HANDLE OBJECT POSITIONS CORRECLTY, 
      /// http://greatmaps.codeplex.com/workitem/16046
      /// </summary>
      Fractional,
#endif
   }

#if !PocketPC
   public enum HelperLineOptions
   {
      DontShow = 0,
      ShowAlways = 1,
      ShowOnModifierKey = 2
   }

   public delegate void SelectionChange(RectLatLng Selection, bool ZoomToFit);
#endif
}
