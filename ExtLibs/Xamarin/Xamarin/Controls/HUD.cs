using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using log4net;
using Xamarin.Controls;
using MissionPlanner.Utilities.Drawing;
using SkiaSharp;
using Color = System.Drawing.Color ;
using Rectangle = System.Drawing.Rectangle;
using PointF = System.Drawing.PointF;
using RectangleF = System.Drawing.RectangleF;
using Point = System.Drawing.Point;
using Graphics = MissionPlanner.Utilities.Drawing.Graphics;

namespace MissionPlanner.Controls
{
    public class HUD : MySKGLView
    {
        public Hashtable CustomItems = new Hashtable();

        public bool HoldInvalidation = false;

        public int huddrawtime = 0;

     
        public bool SixteenXNine = false;

        /// <summary>
        /// this is to reduce cpu usage
        /// </summary>
        public bool streamjpgenable = false;

        const double deg2rad = (1.0 / rad2deg);

        const double rad2deg = (180 / Math.PI);

        private static readonly SolidBrush AltGroundBrush = new SolidBrush(Color.FromArgb(100, Color.BurlyWood));

        private static readonly ILog log =
                                                                                    LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly SolidBrush SlightlyTransparentWhiteBrush =
            new SolidBrush(Color.FromArgb(220, 255, 255, 255));

        private static readonly SolidBrush SolidBrush = new SolidBrush(Color.FromArgb(0x55, 0xff, 0xff, 0xff));
        private readonly object _bgimagelock = new object();
        private readonly Pen _blackPen = new Pen(Color.Black, 2);
        private readonly Pen _greenPen = new Pen(Color.Green, 2);
        /// <summary>
        /// pen for drawstring
        /// </summary>
        private readonly Pen _p = new Pen(Color.FromArgb(0x26, 0x27, 0x28), 2f);

        private readonly Pen _redPen = new Pen(Color.Red, 2);
        private readonly SolidBrush _whiteBrush = new SolidBrush(Color.White);
        /// <summary>
        /// pth for drawstring
        /// </summary>
        private readonly GraphicsPath pth = new GraphicsPath();

        private float _airspeed = 0;
        private float _alt = 0;
        float _AOA = 0;
        private float _batterylevel = 0;
        private float _batteryremaining = 0;
        private Image _bgimage;
        float _critAOA = 25;
        float _critSSA = 30;
        private float _current = 0;
        private DateTime _datetime;
        private float _disttowp = 0;
        private float _gpsfix = 0;
        private float _gpsfix2 = 0;
        private float _gpshdop = 0;
        private float _gpshdop2 = 0;
        float _greenSSAp = 10;
        private Color _groundColor1 = Color.FromArgb(0x9b, 0xb8, 0x24);
        private Color _groundColor2 = Color.FromArgb(0x41, 0x4f, 0x07);
        private float _groundcourse = 0;
        private float _groundspeed = 0;
        private float _heading = 0;
        private Color _hudcolor = Color.White;
        private float _linkqualitygcs = 0;
        private bool _lowairspeed = false;
        private bool _lowgroundspeed = false;
        private string _mode = "Manual";
        private float _navpitch = 0;
        private float _navroll = 0;
        private float _pitch = 0;
        float _redSSAp = 90;
        private float _roll = 0;
        private Color _skyColor1 = Color.Blue;
        private Color _skyColor2 = Color.LightBlue;
        float _SSA = 0;
        private MemoryStream _streamjpg = new MemoryStream();
        private float _targetalt = 0;
        private float _targetheading = 0;
        private float _targetspeed = 0;
        private character[] _texture = new character[2];
        private float _turnrate = 0;
        private float _verticalspeed = 0;
        private Pen _whitePen = new Pen(Color.White, 2);
        private int _wpno = 0;
        private float _xtrack_error = 0;
        float _yellowSSAp = 60;
        private DateTime armedtimer = DateTime.MinValue;
        private Dictionary<int, character> charDict = new Dictionary<int, character>();
        private int count = 0;
        private DateTime countdate = DateTime.Now;
        Rectangle ekfhitzone = new Rectangle();
        // move these global as they rarely change - reduce GC
        private Font font = new Font(HUDT.Font, 10);

        private Utilities.Drawing.IGraphics graphicsObject;
        bool inOnPaint = false;
        DateTime lastinvalidate = DateTime.MinValue;
        string otherthread = "";
        private object paintlock = new object();
        private System.ComponentModel.ComponentResourceManager resources =
            new System.ComponentModel.ComponentResourceManager(typeof(HUD));

        private bool started = false;
        private DateTime starttime = DateTime.MinValue;
        private bool statuslast = false;
        private object streamlock = new object();
        private DateTime textureResetDateTime = DateTime.Now;

        Rectangle vibehitzone = new Rectangle();

        //      private static ImageCodecInfo ici = GetImageCodec("image/jpeg");
        //      private static EncoderParameters eps = new EncoderParameters(1);
        public HUD()
        {
            opengl =
                displayvibe =
                    displayekf =
                        displayheading =
                            displayspeed =
                                displayalt =
                                    displayconninfo =
                                        displayxtrack = displayrollpitch = displaygps = bgon = hudon = batteryon = true;

            displayAOASSA = false;

            this.Name = "Hud";
        }

        public event EventHandler ekfclick;

        public event EventHandler vibeclick;

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float airspeed
        {
            get { return _airspeed; }
            set
            {
                if (_airspeed != value)
                {
                    _airspeed = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float alt
        {
            get { return _alt; }
            set
            {
                if (_alt != value)
                {
                    _alt = value;
                    this.Invalidate();
                }
            }
        }

        public string altunit { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float AOA
        {
            get { return _AOA; }
            set
            {
                if (_AOA != value)
                {
                    _AOA = value;
                    displayAOASSA = true;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float batterylevel
        {
            get { return _batterylevel; }
            set
            {
                if (_batterylevel != value)
                {
                    _batterylevel = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), DefaultValue(true)]
        public bool batteryon { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float batteryremaining
        {
            get { return _batteryremaining; }
            set
            {
                if (_batteryremaining != value)
                {
                    _batteryremaining = value;
                    this.Invalidate();
                }
            }
        }

        public Image bgimage
        {
            set
            {
                lock (this._bgimagelock)
                {
                    try
                    {
                        _bgimage = (Image)value;
                    }
                    catch
                    {
                        _bgimage = null;
                    }

                    this.Invalidate();
                }
            }
            get { return _bgimage; }
        }

        [System.ComponentModel.Browsable(true), DefaultValue(true)]
        public bool bgon { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public bool connected { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float critAOA
        {
            get { return _critAOA; }
            set
            {
                if (_critAOA != value)
                {
                    _critAOA = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float critSSA
        {
            get { return _critSSA; }
            set
            {
                if (_critSSA != value)
                {
                    _critSSA = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float current
        {
            get { return _current; }
            set
            {
                if (_current != value)
                {
                    _current = value;
                    this.Invalidate();
                    if (_current > 0) batteryon = true;
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public DateTime datetime
        {
            get { return _datetime; }
            set
            {
                if (_datetime.Hour == value.Hour && _datetime.Minute == value.Minute &&
                    _datetime.Second == value.Second)
                    return;
                if (_datetime != value)
                {
                    _datetime = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), DefaultValue(true)]
        public bool displayalt { get; set; }

        [System.ComponentModel.Browsable(true), DefaultValue(true)]
        public bool displayAOASSA { get; set; }

        [System.ComponentModel.Browsable(true), DefaultValue(true)]
        public bool displayconninfo { get; set; }

        [System.ComponentModel.Browsable(true), DefaultValue(true)]
        public bool displayekf { get; set; }

        [System.ComponentModel.Browsable(true), DefaultValue(true)]
        public bool displaygps { get; set; }

        [System.ComponentModel.Browsable(true), DefaultValue(true)]
        public bool displayheading { get; set; }

        [System.ComponentModel.Browsable(true), DefaultValue(true)]
        public bool displayrollpitch { get; set; }

        [System.ComponentModel.Browsable(true), DefaultValue(true)]
        public bool displayspeed { get; set; }

        [System.ComponentModel.Browsable(true), DefaultValue(true)]
        public bool displayvibe { get; set; }

        [System.ComponentModel.Browsable(true), DefaultValue(true)]
        public bool displayxtrack { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float disttowp
        {
            get { return _disttowp; }
            set
            {
                if (_disttowp != value)
                {
                    _disttowp = value;
                    this.Invalidate();
                }
            }
        }

        public string distunit { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float ekfstatus { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public bool failsafe { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float gpsfix
        {
            get { return _gpsfix; }
            set
            {
                if (_gpsfix != value)
                {
                    _gpsfix = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float gpsfix2
        {
            get { return _gpsfix2; }
            set
            {
                if (_gpsfix2 != value)
                {
                    _gpsfix2 = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float gpshdop
        {
            get { return _gpshdop; }
            set
            {
                if (_gpshdop != value)
                {
                    _gpshdop = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float gpshdop2
        {
            get { return _gpshdop2; }
            set
            {
                if (_gpshdop2 != value)
                {
                    _gpshdop2 = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float groundalt { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values"), DefaultValue(typeof(Color), "0x9bb824")]
        public Color groundColor1
        {
            get { return _groundColor1; }
            set { _groundColor1 = value; }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values"), DefaultValue(typeof(Color), "0x414f07")]
        public Color groundColor2
        {
            get { return _groundColor2; }
            set { _groundColor2 = value; }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float groundcourse
        {
            get { return _groundcourse; }
            set
            {
                if (_groundcourse != value)
                {
                    _groundcourse = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float groundspeed
        {
            get { return _groundspeed; }
            set
            {
                if (_groundspeed != value)
                {
                    _groundspeed = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float heading
        {
            get { return _heading; }
            set
            {
                if (_heading != value)
                {
                    _heading = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public Color hudcolor
        {
            get { return this._whitePen.Color; }
            set
            {
                _hudcolor = value;
                this._whitePen = new Pen(value, 2);
            }
        }

        [System.ComponentModel.Browsable(true), DefaultValue(true)]
        public bool hudon { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float linkqualitygcs
        {
            get { return _linkqualitygcs; }
            set
            {
                if (_linkqualitygcs != value)
                {
                    _linkqualitygcs = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public bool lowairspeed
        {
            get { return _lowairspeed; }
            set
            {
                if (_lowairspeed != value)
                {
                    _lowairspeed = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public bool lowgroundspeed
        {
            get { return _lowgroundspeed; }
            set
            {
                if (_lowgroundspeed != value)
                {
                    _lowgroundspeed = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public bool lowvoltagealert { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public string message { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public DateTime messagetime { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public string mode
        {
            get { return _mode; }
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float navpitch
        {
            get { return _navpitch; }
            set
            {
                if (_navpitch != value)
                {
                    _navpitch = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float navroll
        {
            get { return _navroll; }
            set
            {
                if (_navroll != value)
                {
                    _navroll = value;
                    this.Invalidate();
                }
            }
        }

        [Browsable(false)] public bool npotSupported { get; private set; }

        [DefaultValue(true)] public bool opengl { get; set; } = false;

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float pitch
        {
            get { return _pitch; }
            set
            {
                if (_pitch != value)
                {
                    _pitch = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float roll
        {
            get { return _roll; }
            set
            {
                if (_roll != value)
                {
                    _roll = value;
                    this.Invalidate();
                }
            }
        }

        public bool Russian { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public Color skyColor1
        {
            get { return _skyColor1; }
            set { _skyColor1 = value; }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public Color skyColor2
        {
            get { return _skyColor2; }
            set { _skyColor2 = value; }
        }

        public string speedunit { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float SSA
        {
            get { return _SSA; }
            set
            {
                if (_SSA != value)
                {
                    _SSA = value;
                    displayAOASSA = true;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public bool status { get; set; }

        //[System.ComponentModel.Browsable(false)]
        public MemoryStream streamjpg
        {
            get
            {
                lock (streamlock)
                {
                    return _streamjpg;
                }
            }
            set
            {
                lock (streamlock)
                {
                    _streamjpg = value;
                }
            }
        }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float targetalt
        {
            get { return _targetalt; }
            set
            {
                if (_targetalt != value)
                {
                    _targetalt = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float targetheading
        {
            get { return _targetheading; }
            set
            {
                if (_targetheading != value)
                {
                    _targetheading = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float targetspeed
        {
            get { return _targetspeed; }
            set
            {
                if (_targetspeed != value)
                {
                    _targetspeed = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float turnrate
        {
            get { return _turnrate; }
            set
            {
                if (_turnrate != value)
                {
                    _turnrate = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float verticalspeed
        {
            get { return _verticalspeed; }
            set
            {
                if (_verticalspeed != Math.Round(value, 1))
                {
                    _verticalspeed = (float)Math.Round(value, 1);
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float vibex { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float vibey { get; set; }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float vibez { get; set; }

        [Browsable(false)]
        public new bool VSync
        {
            get
            {
                try
                {
                    return base.VSync;
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                try
                {
                    base.VSync = value;
                }
                catch
                {
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public int wpno
        {
            get { return _wpno; }
            set
            {
                if (_wpno != value)
                {
                    _wpno = value;
                    this.Invalidate();
                }
            }
        }

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Values")]
        public float xtrack_error
        {
            get { return _xtrack_error; }
            set
            {
                if (_xtrack_error != value)
                {
                    _xtrack_error = value;
                    this.Invalidate();
                }
            }
        }

        public void doResize()
        {
            OnResize(EventArgs.Empty);
        }

  

        /// <summary>
        /// Override to prevent offscreen drawing the control - mono mac
        /// </summary>
        public new void Invalidate()
        {
            if (HoldInvalidation)
                return;

            if (!ThisReallyVisible())
            {
                //  return;
            }

            lastinvalidate = DateTime.Now;

            base.Invalidate();
        }

        public override void Refresh()
        {
            if (!ThisReallyVisible())
            {
                //  return;
            }

            base.Refresh();
            
        }
        

        /// <summary>
        /// this is to fix a mono off screen drawing issue
        /// </summary>
        /// <returns></returns>
        public bool ThisReallyVisible()
        {
            //Control ctl = Control.FromHandle(this.Handle);
            return this.Visible;
        }
        

        protected override void OnHandleCreated(EventArgs e)
        {
            try
            {
                if (opengl && !DesignMode)
                {
                    base.OnHandleCreated(e);
                }
            }
            catch (Exception ex)
            {
                log.Error("Expected failure on max/linux due to opengl support");
                log.Error(ex);
                opengl = false;
            } // macs/linux fail here
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            try
            {
                if (opengl && !DesignMode)
                {
                    base.OnHandleDestroyed(e);
                }
            }
            catch (Exception ex)
            {
                log.Info(ex.ToString());
                opengl = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            log.Info("OnLoad Start");
            

            log.Info("OnLoad Done");

            started = true;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (ekfhitzone.IntersectsWith(new Rectangle(e.X, e.Y, 5, 5)))
            {
                if (ekfclick != null)
                    ekfclick(this, null);
            }

            if (vibehitzone.IntersectsWith(new Rectangle(e.X, e.Y, 5, 5)))
            {
                if (vibeclick != null)
                    vibeclick(this, null);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (ekfhitzone.IntersectsWith(new Rectangle(e.X, e.Y, 5, 5)))
            {
                Cursor.Current = Cursors.Hand;
            }
            else if (vibehitzone.IntersectsWith(new Rectangle(e.X, e.Y, 5, 5)))
            {
                Cursor.Current = Cursors.Hand;
            }
            else
            {
                Cursor.Current = DefaultCursor;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //GL.Enable(EnableCap.AlphaTest)

            // Console.WriteLine("hud paint");

            // Console.WriteLine("hud ms " + (DateTime.Now.Millisecond));

          //  if (!started)
            //    return;


            if ((DateTime.Now - starttime).TotalMilliseconds < 30 && (_bgimage == null))
            {
                //Console.WriteLine("ms "+(DateTime.Now - starttime).TotalMilliseconds);
                //e.Graphics.DrawImageUnscaled(objBitmap, 0, 0);          
                return;
            }

            // force texture reset
            if (textureResetDateTime.Hour != DateTime.Now.Hour)
            {
                textureResetDateTime = DateTime.Now;
                doResize();
            }

            lock (this)
            {

                if (inOnPaint)
                {
                    log.Info("Was in onpaint Hud th:" + System.Threading.Thread.CurrentThread.Name + " in " +
                             otherthread);
                    return;
                }

                otherthread = System.Threading.Thread.CurrentThread.Name;

                inOnPaint = true;

            }

            starttime = DateTime.Now;

            try
            {



                graphicsObject = e.Graphics;

                doPaint();

            

             

            }
            catch (Exception ex)
            {
                log.Info(ex.ToString());
            }

            count++;

            huddrawtime += (int)(DateTime.Now - starttime).TotalMilliseconds;

            if (DateTime.Now.Second != countdate.Second)
            {
                countdate = DateTime.Now;
                Console.WriteLine("HUD " + count + " hz drawtime " + (huddrawtime / count) + " gl " + opengl);
                if ((huddrawtime / count) > 1000)
                    opengl = false;

                count = 0;
                huddrawtime = 0;
            }

            lock (this)
            {
                inOnPaint = false;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnResize(EventArgs e)
        {
            if (DesignMode || !IsHandleCreated || !started)
                return;

            base.OnResize(e);

            if (SixteenXNine)
            {
                int ht = (int)(this.Width / 1.777f);
                if (ht >= this.Height + 5 || ht <= this.Height - 5)
                {
                    this.Height = ht;
                    return;
                }
            }
            else
            {
                // 4x3
                int ht = (int)(this.Width / 1.333f);
                if (ht >= this.Height + 5 || ht <= this.Height - 5)
                {
                    this.Height = ht;
                    return;
                }
            }

          
            try
            {
                foreach (character texid in charDict.Values)
                {
                    try
                    {
                        texid.bitmap.Dispose();
                    }
                    catch
                    {
                    }
                }

     

                charDict.Clear();
            }
            catch
            {
            }

            try
            {
         
            }
            catch
            {
            }

            Refresh();
        }
        

        void doPaint()
        {
            //Console.WriteLine("hud paint "+DateTime.Now.Millisecond);
            bool isNaN = false;
            try
            {
    

                try
                {
                    graphicsObject.Clear(Color.Transparent);
                }
                catch
                {
                    // this is the first posible opengl call
                    // in vmware fusion on mac, this fails, so switch back to legacy
                    opengl = false;
                }

                if (_bgimage != null)
                {
                    bgon = false;
                    lock (this._bgimagelock)
                        lock (_bgimage)
                        {
                            try
                            {
                                graphicsObject.DrawImage(_bgimage, 0, 0, this.Width, this.Height);
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                                _bgimage = null;
                            }
                        }

                    if (hudon == false)
                    {
                        return;
                    }
                }
                else
                {
                    bgon = true;
                }

                // localize it
                float _roll = this._roll;

                if (float.IsNaN(_roll) || float.IsNaN(_pitch) || float.IsNaN(_heading))
                {
                    isNaN = true;

                    _roll = 0;
                    _pitch = 0;
                    _heading = 0;
                }

                graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);

                if (!Russian)
                {
                    // horizon
                    graphicsObject.RotateTransform(-_roll);
                }
                else
                {
                    _roll *= -1;
                }


                int fontsize = this.Height / 30; // = 10
                int fontoffset = fontsize - 10;

                float every5deg = -this.Height / 65;

                float pitchoffset = -_pitch * every5deg;

                int halfwidth = this.Width / 2;
                int halfheight = this.Height / 2;

                _whiteBrush.Color = this._whitePen.Color;

                // Reset pens
                this._blackPen.Width = 2;
                this._greenPen.Width = 2;
                this._redPen.Width = 2;

                if (!connected)
                {
                    this._whiteBrush.Color = Color.LightGray;
                    this._whitePen.Color = Color.LightGray;
                }
                else
                {
                    this._whitePen.Color = _hudcolor;
                }

                // draw sky
                if (bgon == true)
                {
                    RectangleF bg = new RectangleF(-halfwidth * 2, -halfheight * 2, this.Width * 2,
                        halfheight * 2 + pitchoffset);

                    if (bg.Height != 0)
                    {
                        using (LinearGradientBrush linearBrush = new LinearGradientBrush(
                            bg, _skyColor1, _skyColor2, LinearGradientMode.Vertical))
                        {
                            graphicsObject.FillRectangle(linearBrush, bg);
                        }
                    }
                    // draw ground

                    bg = new RectangleF(-halfwidth * 2, pitchoffset, this.Width * 2, halfheight * 2 - pitchoffset);

                    if (bg.Height != 0)
                    {
                        using (
                            LinearGradientBrush linearBrush = new LinearGradientBrush(
                                bg, _groundColor1, _groundColor2,
                                LinearGradientMode.Vertical))
                        {
                            graphicsObject.FillRectangle(linearBrush, bg);
                        }
                    }

                    //draw centerline
                    graphicsObject.DrawLine(this._whitePen, -halfwidth * 2, pitchoffset + 0, halfwidth * 2,
                        pitchoffset + 0);
                }

                
                graphicsObject.ResetTransform();

                if (displayrollpitch)
                {
                    graphicsObject.SetClip(new Rectangle(0, this.Height / 14, this.Width,
                        this.Height - this.Height / 14));

                    graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);

                    graphicsObject.RotateTransform(-_roll);

                    //draw pitch           

                    int lengthshort = this.Width / 14;
                    int lengthlong = this.Width / 10;

                    for (int a = -90; a <= 90; a += 5)
                    {
                        // limit to 40 degrees
                        if (a >= _pitch - 29 && a <= _pitch + 20)
                        {
                            if (a % 10 == 0)
                            {
                                if (a == 0)
                                {
                                    graphicsObject.DrawLine(this._greenPen, this.Width / 2 - lengthlong - halfwidth,
                                        pitchoffset + a * every5deg, this.Width / 2 + lengthlong - halfwidth,
                                        pitchoffset + a * every5deg);
                                }
                                else
                                {
                                    graphicsObject.DrawLine(this._whitePen, this.Width / 2 - lengthlong - halfwidth,
                                        pitchoffset + a * every5deg, this.Width / 2 + lengthlong - halfwidth,
                                        pitchoffset + a * every5deg);
                                }

                                drawstring(a.ToString(), font, fontsize + 2, _whiteBrush,
                                    this.Width / 2 - lengthlong - 30 - halfwidth - (int)(fontoffset * 1.7),
                                    pitchoffset + a * every5deg - 8 - fontoffset);
                            }
                            else
                            {
                                graphicsObject.DrawLine(this._whitePen, this.Width / 2 - lengthshort - halfwidth,
                                    pitchoffset + a * every5deg, this.Width / 2 + lengthshort - halfwidth,
                                    pitchoffset + a * every5deg);
                                //drawstring(e,a.ToString(), new Font("Arial", 10), whiteBrush, this.Width / 2 - lengthshort - 20 - halfwidth, this.Height / 2 + pitchoffset + a * every5deg - 8);
                            }
                        }
                    }

                    graphicsObject.ResetTransform();

                    // draw roll ind needle

                    graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);

                    lengthlong = this.Height / 66;

                    int extra = (int)(this.Height / 15.0 * 4.9f);

                    int lengthlongex = lengthlong + 2;

                    Point[] pointlist = new Point[3];
                    pointlist[0] = new Point(0, -lengthlongex * 2 - extra);
                    pointlist[1] = new Point(-lengthlongex, -lengthlongex - extra);
                    pointlist[2] = new Point(lengthlongex, -lengthlongex - extra);

                    this._redPen.Width = 2;

                    if (Math.Abs(_roll) > 45)
                    {
                        this._redPen.Width = 4;
                    }

                    graphicsObject.DrawPolygon(this._redPen, pointlist);

                    this._redPen.Width = 2;

                    int[] array = new int[] { -60, -45, -30, -20, -10, 0, 10, 20, 30, 45, 60 };

                    foreach (int a in array)
                    {
                        graphicsObject.ResetTransform();
                        graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);
                        graphicsObject.RotateTransform(a - _roll);
                        drawstring(String.Format("{0,2}", Math.Abs(a)), font, fontsize, _whiteBrush,
                            0 - 6 - fontoffset, -lengthlong * 8 - extra);
                        graphicsObject.DrawLine(this._whitePen, 0, -lengthlong * 3 - extra, 0,
                            -lengthlong * 3 - extra - lengthlong);
                    }

                    graphicsObject.ResetTransform();
                    graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);

                    // draw roll ind
                    RectangleF arcrect = new RectangleF(-lengthlong * 3 - extra, -lengthlong * 3 - extra,
                        (extra + lengthlong * 3) * 2f, (extra + lengthlong * 3) * 2f);

                    //DrawRectangle(Pens.Beige, arcrect);

                    graphicsObject.DrawArc(this._whitePen, arcrect, 180 + 30 + -_roll, 120); // 120

                    graphicsObject.ResetTransform();

                    //draw centre / current att

                    graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2); //  +this.Height / 14);

                    // plane wings
                    if (Russian)
                        graphicsObject.RotateTransform(-_roll);

                    Rectangle centercircle = new Rectangle(-halfwidth / 2, -halfwidth / 2, halfwidth, halfwidth);

                    //  graphicsObject.DrawEllipse(redPen, centercircle);
                    using (Pen redtemp =
                        new Pen(Color.FromArgb(200, this._redPen.Color.R, this._redPen.Color.G, this._redPen.Color.B),
                            4.0f))
                    {
                        // left
                        graphicsObject.DrawLine(redtemp, centercircle.Left - halfwidth / 5, 0, centercircle.Left, 0);
                        // right
                        graphicsObject.DrawLine(redtemp, centercircle.Right, 0, centercircle.Right + halfwidth / 5, 0);
                        // center point
                        graphicsObject.DrawLine(redtemp, 0 - 1, 0, centercircle.Right - halfwidth / 3,
                            0 + halfheight / 10);
                        graphicsObject.DrawLine(redtemp, 0 + 1, 0, centercircle.Left + halfwidth / 3,
                            0 + halfheight / 10);
                    }
                }

                // Flight Path vector
                if (displayAOASSA)
                {
                    graphicsObject.DrawEllipse(this._redPen,
                        new Rectangle((int)(-halfwidth / 40 - _SSA * every5deg),
                            (int)(-halfwidth / 40 - _AOA * every5deg), halfwidth / 20, halfwidth / 20));
                    graphicsObject.DrawLine(this._redPen, -halfwidth / 20 - _SSA * every5deg, 0 - _AOA * every5deg,
                        -halfwidth / 40 - _SSA * every5deg, 0 - _AOA * every5deg);
                    graphicsObject.DrawLine(this._redPen, halfwidth / 20 - _SSA * every5deg, 0 - _AOA * every5deg,
                        halfwidth / 40 - _SSA * every5deg, 0 - _AOA * every5deg);
                    graphicsObject.DrawLine(this._redPen, 0 - _SSA * every5deg, -halfwidth / 20 - _AOA * every5deg,
                        0 - _SSA * every5deg, -halfwidth / 40 - _AOA * every5deg);

                }

                //draw heading ind
                Rectangle headbg = new Rectangle(0, 0, this.Width - 0, this.Height / 14);

                graphicsObject.ResetTransform();
                graphicsObject.ResetClip();

                if (displayheading)
                {
                    graphicsObject.DrawRectangle(this._blackPen, headbg);


                    graphicsObject.FillRectangle(SolidBrush, headbg);

                    // center
                    //   graphicsObject.DrawLine(redPen, headbg.Width / 2, headbg.Bottom, headbg.Width / 2, headbg.Top);

                    //bottom line
                    graphicsObject.DrawLine(this._whitePen, headbg.Left + 5, headbg.Bottom - 5, headbg.Width - 5,
                        headbg.Bottom - 5);

                    float space = (headbg.Width - 10) / 120.0f;
                    int start = (int)Math.Round((_heading - 60), 1);

                    // draw for outside the 60 deg
                    if (_targetheading < start)
                    {
                        this._greenPen.Width = 6;
                        graphicsObject.DrawLine(this._greenPen, headbg.Left + 5 + space * 0, headbg.Bottom,
                            headbg.Left + 5 + space * (0), headbg.Top);
                    }

                    if (_targetheading > _heading + 60)
                    {
                        this._greenPen.Width = 6;
                        graphicsObject.DrawLine(this._greenPen, headbg.Left + 5 + space * 60, headbg.Bottom,
                            headbg.Left + 5 + space * (60), headbg.Top);
                    }

                    for (int a = start; a <= _heading + 60; a += 1)
                    {
                        // target heading
                        if (((int)(a + 360) % 360) == (int)_targetheading)
                        {
                            this._greenPen.Width = 6;
                            graphicsObject.DrawLine(this._greenPen, headbg.Left + 5 + space * (a - start),
                                headbg.Bottom, headbg.Left + 5 + space * (a - start), headbg.Top);
                        }

                        if (((int)(a + 360) % 360) == (int)_groundcourse)
                        {
                            this._blackPen.Width = 6;
                            graphicsObject.DrawLine(this._blackPen, headbg.Left + 5 + space * (a - start),
                                headbg.Bottom, headbg.Left + 5 + space * (a - start), headbg.Top);
                            this._blackPen.Width = 2;
                        }

                        if ((int)a % 15 == 0)
                        {
                            //Console.WriteLine(a + " " + Math.Round(a, 1, MidpointRounding.AwayFromZero));
                            //Console.WriteLine(space +" " + a +" "+ (headbg.Left + 5 + space * (a - start)));
                            graphicsObject.DrawLine(this._whitePen, headbg.Left + 5 + space * (a - start),
                                headbg.Bottom - 5, headbg.Left + 5 + space * (a - start), headbg.Bottom - 10);
                            int disp = (int)a;
                            if (disp < 0)
                                disp += 360;
                            disp = disp % 360;
                            if (disp == 0)
                            {
                                drawstring(HUDT.N.PadLeft(2), font, fontsize + 4, _whiteBrush,
                                    headbg.Left - 5 + space * (a - start) - fontoffset,
                                    headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                            }
                            else if (disp == 45)
                            {
                                drawstring(HUDT.NE.PadLeft(2), font, fontsize + 4, _whiteBrush,
                                    headbg.Left - 5 + space * (a - start) - fontoffset,
                                    headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                            }
                            else if (disp == 90)
                            {
                                drawstring(HUDT.E.PadLeft(2), font, fontsize + 4, _whiteBrush,
                                    headbg.Left - 5 + space * (a - start) - fontoffset,
                                    headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                            }
                            else if (disp == 135)
                            {
                                drawstring(HUDT.SE.PadLeft(2), font, fontsize + 4, _whiteBrush,
                                    headbg.Left - 5 + space * (a - start) - fontoffset,
                                    headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                            }
                            else if (disp == 180)
                            {
                                drawstring(HUDT.S.PadLeft(2), font, fontsize + 4, _whiteBrush,
                                    headbg.Left - 5 + space * (a - start) - fontoffset,
                                    headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                            }
                            else if (disp == 225)
                            {
                                drawstring(HUDT.SW.PadLeft(2), font, fontsize + 4, _whiteBrush,
                                    headbg.Left - 5 + space * (a - start) - fontoffset,
                                    headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                            }
                            else if (disp == 270)
                            {
                                drawstring(HUDT.W.PadLeft(2), font, fontsize + 4, _whiteBrush,
                                    headbg.Left - 5 + space * (a - start) - fontoffset,
                                    headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                            }
                            else if (disp == 315)
                            {
                                drawstring(HUDT.NW.PadLeft(2), font, fontsize + 4, _whiteBrush,
                                    headbg.Left - 5 + space * (a - start) - fontoffset,
                                    headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                            }
                            else
                            {
                                drawstring(String.Format("{0,3}", (int)(disp % 360)), font, fontsize,
                                    _whiteBrush, headbg.Left - 5 + space * (a - start) - fontoffset,
                                    headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                            }
                        }
                        else if ((int)a % 5 == 0)
                        {
                            graphicsObject.DrawLine(this._whitePen, headbg.Left + 5 + space * (a - start),
                                headbg.Bottom - 5, headbg.Left + 5 + space * (a - start), headbg.Bottom - 10);
                        }
                    }

                    RectangleF rect = new RectangleF(headbg.Width / 2 - (fontsize * 2.4f) / 2, 0, (fontsize * 2.4f),
                        headbg.Height);

                    //DrawRectangle(whitePen, rect);
                    graphicsObject.FillRectangle(SlightlyTransparentWhiteBrush, rect);

                    if (Math.Abs(_heading - _targetheading) < 4)
                    {
                        drawstring(String.Format("{0,3}", (int)(heading % 360)), font, fontsize,
                            _whiteBrush, headbg.Width / 2 - (fontsize * 1f),
                            headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                    }
                    else
                    {
                        drawstring(String.Format("{0,3}", (int)(heading % 360)), font, fontsize,
                            _whiteBrush, headbg.Width / 2 - (fontsize * 1f),
                            headbg.Bottom - 24 - (int)(fontoffset * 1.7));
                    }

                }
                //                Console.WriteLine("HUD 0 " + (DateTime.Now - starttime).TotalMilliseconds + " " + DateTime.Now.Millisecond);

                // xtrack error
                // center

                if (displayxtrack)
                {
                    float xtspace = this.Width / 10.0f / 3.0f;
                    int pad = 10;

                    float myxtrack_error = _xtrack_error;

                    myxtrack_error = Math.Min(myxtrack_error, 40);
                    myxtrack_error = Math.Max(myxtrack_error, -40);

                    //  xtrack - distance scale - space
                    float loc = myxtrack_error / 20.0f * xtspace;

                    // current xtrack
                    if (Math.Abs(myxtrack_error) == 40)
                    {
                        this._greenPen.Color = Color.FromArgb(128, this._greenPen.Color);
                    }

                    graphicsObject.DrawLine(this._greenPen, this.Width / 10 + loc, headbg.Bottom + 5,
                        this.Width / 10 + loc, headbg.Bottom + this.Height / 10);

                    this._greenPen.Color = Color.FromArgb(255, this._greenPen.Color);

                    graphicsObject.DrawLine(this._whitePen, this.Width / 10, headbg.Bottom + 5, this.Width / 10,
                        headbg.Bottom + this.Height / 10);

                    graphicsObject.DrawLine(this._whitePen, this.Width / 10 - xtspace, headbg.Bottom + 5 + pad,
                        this.Width / 10 - xtspace, headbg.Bottom + this.Height / 10 - pad);

                    graphicsObject.DrawLine(this._whitePen, this.Width / 10 - xtspace * 2, headbg.Bottom + 5 + pad,
                        this.Width / 10 - xtspace * 2, headbg.Bottom + this.Height / 10 - pad);

                    graphicsObject.DrawLine(this._whitePen, this.Width / 10 + xtspace, headbg.Bottom + 5 + pad,
                        this.Width / 10 + xtspace, headbg.Bottom + this.Height / 10 - pad);

                    graphicsObject.DrawLine(this._whitePen, this.Width / 10 + xtspace * 2, headbg.Bottom + 5 + pad,
                        this.Width / 10 + xtspace * 2, headbg.Bottom + this.Height / 10 - pad);

                    // rate of turn

                    this._whitePen.Width = 4;
                    graphicsObject.DrawLine(this._whitePen, this.Width / 10 - xtspace * 2 - xtspace / 2,
                        headbg.Bottom + this.Height / 10 + 10, this.Width / 10 - xtspace * 2 - xtspace / 2 + xtspace,
                        headbg.Bottom + this.Height / 10 + 10);

                    graphicsObject.DrawLine(this._whitePen, this.Width / 10 - xtspace * 0 - xtspace / 2,
                        headbg.Bottom + this.Height / 10 + 10, this.Width / 10 - xtspace * 0 - xtspace / 2 + xtspace,
                        headbg.Bottom + this.Height / 10 + 10);

                    graphicsObject.DrawLine(this._whitePen, this.Width / 10 + xtspace * 2 - xtspace / 2,
                        headbg.Bottom + this.Height / 10 + 10, this.Width / 10 + xtspace * 2 - xtspace / 2 + xtspace,
                        headbg.Bottom + this.Height / 10 + 10);

                    float myturnrate = _turnrate;
                    float trwidth = (this.Width / 10 + xtspace * 2 - xtspace / 2) -
                                    (this.Width / 10 - xtspace * 2 - xtspace / 2);

                    float range = 12;

                    myturnrate = Math.Min(myturnrate, range / 2);
                    myturnrate = Math.Max(myturnrate, (range / 2) * -1.0f);

                    loc = myturnrate / range * trwidth;

                    this._greenPen.Width = 4;

                    if (Math.Abs(myturnrate) == (range / 2))
                    {
                        this._greenPen.Color = Color.FromArgb(128, this._greenPen.Color);
                    }

                    graphicsObject.DrawLine(this._greenPen, this.Width / 10 + loc - xtspace / 2,
                        headbg.Bottom + this.Height / 10 + 10 + 3, this.Width / 10 + loc + xtspace / 2,
                        headbg.Bottom + this.Height / 10 + 10 + 3);
                    graphicsObject.DrawLine(this._greenPen, this.Width / 10 + loc,
                        headbg.Bottom + this.Height / 10 + 10 + 3, this.Width / 10 + loc,
                        headbg.Bottom + this.Height / 10 + 10 + 10);

                    this._greenPen.Color = Color.FromArgb(255, this._greenPen.Color);

                    this._whitePen.Width = 2;
                }

                // left scroller
                Rectangle scrollbg = new Rectangle(0, halfheight - halfheight / 2, this.Width / 10, this.Height / 2);

                if (displayspeed)
                {
                    graphicsObject.DrawRectangle(this._whitePen, scrollbg);

                    graphicsObject.FillRectangle(SolidBrush, scrollbg);

                    Point[] arrow = new Point[5];

                    arrow[0] = new Point(0, -10);
                    arrow[1] = new Point(scrollbg.Width - 10, -10);
                    arrow[2] = new Point(scrollbg.Width - 5, 0);
                    arrow[3] = new Point(scrollbg.Width - 10, 10);
                    arrow[4] = new Point(0, 10);

                    graphicsObject.TranslateTransform(0, this.Height / 2);

                    float viewrange = 26;

                    float speed = _airspeed;
                    if (speed == 0)
                        speed = _groundspeed;

                    float space = (scrollbg.Height) / viewrange;
                    float start = (long)(speed - viewrange / 2);

                    if (start > _targetspeed)
                    {
                        this._greenPen.Color = Color.FromArgb(128, this._greenPen.Color);
                        this._greenPen.Width = 6;
                        graphicsObject.DrawLine(this._greenPen, scrollbg.Left, scrollbg.Top,
                            scrollbg.Left + scrollbg.Width, scrollbg.Top);
                        this._greenPen.Color = Color.FromArgb(255, this._greenPen.Color);
                    }

                    if ((speed + viewrange / 2) < _targetspeed)
                    {
                        this._greenPen.Color = Color.FromArgb(128, this._greenPen.Color);
                        this._greenPen.Width = 6;
                        graphicsObject.DrawLine(this._greenPen, scrollbg.Left, scrollbg.Top - space * viewrange,
                            scrollbg.Left + scrollbg.Width, scrollbg.Top - space * viewrange);
                        this._greenPen.Color = Color.FromArgb(255, this._greenPen.Color);
                    }

                    long end = (long)(speed + viewrange / 2);
                    for (long a = (long)start; a <= end; a += 1)
                    {
                        if (a == (long)_targetspeed && _targetspeed != 0)
                        {
                            this._greenPen.Width = 6;
                            graphicsObject.DrawLine(this._greenPen, scrollbg.Left, scrollbg.Top - space * (a - start),
                                scrollbg.Left + scrollbg.Width, scrollbg.Top - space * (a - start));
                        }

                        if (a % 5 == 0)
                        {
                            //Console.WriteLine(a + " " + scrollbg.Right + " " + (scrollbg.Top - space * (a - start)) + " " + (scrollbg.Right - 20) + " " + (scrollbg.Top - space * (a - start)));
                            graphicsObject.DrawLine(this._whitePen, scrollbg.Right, scrollbg.Top - space * (a - start),
                                scrollbg.Right - 10, scrollbg.Top - space * (a - start));
                            drawstring(String.Format("{0,5}", a), font, fontsize, _whiteBrush, 0,
                                (float)(scrollbg.Top - space * (a - start) - 6 - fontoffset));
                        }
                    }

                    graphicsObject.DrawPolygon(this._blackPen, arrow);
                    graphicsObject.FillPolygon(Brushes.Black, arrow);
                    drawstring((speed).ToString("0") + speedunit, font, 10, (SolidBrush)Brushes.AliceBlue, 0, -9);

                    graphicsObject.ResetTransform();

                    // extra text data

                    if (_lowairspeed)
                    {
                        drawstring(HUDT.AS + _airspeed.ToString("0.0") + speedunit, font, fontsize,
                            (SolidBrush)Brushes.Red, 1, scrollbg.Bottom + 5);
                    }
                    else
                    {
                        drawstring(HUDT.AS + _airspeed.ToString("0.0") + speedunit, font, fontsize, _whiteBrush, 1,
                            scrollbg.Bottom + 5);
                    }

                    if (_lowgroundspeed)
                    {
                        drawstring(HUDT.GS + _groundspeed.ToString("0.0") + speedunit, font, fontsize,
                            (SolidBrush)Brushes.Red, 1, scrollbg.Bottom + fontsize + 2 + 10);
                    }
                    else
                    {
                        drawstring(HUDT.GS + _groundspeed.ToString("0.0") + speedunit, font, fontsize, _whiteBrush,
                            1, scrollbg.Bottom + fontsize + 2 + 10);
                    }
                }

                //drawstring(e,, new Font("Arial", fontsize + 2), whiteBrush, 1, scrollbg.Bottom + fontsize + 2 + 10);

                // right scroller
                scrollbg = new Rectangle(this.Width - this.Width / 10, halfheight - halfheight / 2, this.Width / 10,
                    this.Height / 2);

                if (displayalt)
                {
                    graphicsObject.DrawRectangle(this._whitePen, scrollbg);

                    graphicsObject.FillRectangle(SolidBrush, scrollbg);

                    Point[] arrow = new Point[5];

                    arrow[0] = new Point(0, -10);
                    arrow[1] = new Point(scrollbg.Width - 10, -10);
                    arrow[2] = new Point(scrollbg.Width - 5, 0);
                    arrow[3] = new Point(scrollbg.Width - 10, 10);
                    arrow[4] = new Point(0, 10);

                    graphicsObject.TranslateTransform(0, this.Height / 2);

                    int viewrange = 26;

                    float space = (scrollbg.Height) / (float)viewrange;
                    long start = ((int)_alt - viewrange / 2);

                    if (start > _targetalt)
                    {
                        this._greenPen.Color = Color.FromArgb(128, this._greenPen.Color);
                        this._greenPen.Width = 6;
                        graphicsObject.DrawLine(this._greenPen, scrollbg.Left, scrollbg.Top,
                            scrollbg.Left + scrollbg.Width, scrollbg.Top);
                        this._greenPen.Color = Color.FromArgb(255, this._greenPen.Color);
                    }

                    if ((_alt + viewrange / 2) < _targetalt)
                    {
                        this._greenPen.Color = Color.FromArgb(128, this._greenPen.Color);
                        this._greenPen.Width = 6;
                        graphicsObject.DrawLine(this._greenPen, scrollbg.Left, scrollbg.Top - space * viewrange,
                            scrollbg.Left + scrollbg.Width, scrollbg.Top - space * viewrange);
                        this._greenPen.Color = Color.FromArgb(255, this._greenPen.Color);
                    }

                    bool ground = false;

                    for (long a = start; a <= (_alt + viewrange / 2); a += 1)
                    {
                        if (a == Math.Round(_targetalt) && _targetalt != 0)
                        {
                            this._greenPen.Width = 6;
                            graphicsObject.DrawLine(this._greenPen, scrollbg.Left, scrollbg.Top - space * (a - start),
                                scrollbg.Left + scrollbg.Width, scrollbg.Top - space * (a - start));
                        }


                        // ground doesnt appear if we are not in view or below ground level
                        if (a == Math.Round(groundalt) && groundalt != 0 && ground == false)
                        {
                            graphicsObject.FillRectangle(AltGroundBrush,
                                new RectangleF(scrollbg.Left, scrollbg.Top - space * (a - start), scrollbg.Width,
                                    (space * (a - start))));
                        }

                        if (a % 5 == 0)
                        {
                            //Console.WriteLine(a + " " + scrollbg.Left + " " + (scrollbg.Top - space * (a - start)) + " " + (scrollbg.Left + 20) + " " + (scrollbg.Top - space * (a - start)));
                            graphicsObject.DrawLine(this._whitePen, scrollbg.Left, scrollbg.Top - space * (a - start),
                                scrollbg.Left + 10, scrollbg.Top - space * (a - start));
                            drawstring(String.Format("{0,5}", a), font, fontsize, _whiteBrush,
                                scrollbg.Left + 0 + (int)(0 * fontoffset),
                                scrollbg.Top - space * (a - start) - 6 - fontoffset);
                        }

                    }

                    this._greenPen.Width = 4;

                    // vsi

                    graphicsObject.ResetTransform();

                    PointF[] poly = new PointF[4];

                    poly[0] = new PointF(scrollbg.Left, scrollbg.Top);
                    poly[1] = new PointF(scrollbg.Left - scrollbg.Width / 4, scrollbg.Top + scrollbg.Width / 4);
                    poly[2] = new PointF(scrollbg.Left - scrollbg.Width / 4, scrollbg.Bottom - scrollbg.Width / 4);
                    poly[3] = new PointF(scrollbg.Left, scrollbg.Bottom);

                    //verticalspeed

                    viewrange = 12;

                    _verticalspeed = Math.Min(viewrange / 2, _verticalspeed);
                    _verticalspeed = Math.Max(viewrange / -2, _verticalspeed);

                    float scaledvalue = _verticalspeed / -viewrange * (scrollbg.Bottom - scrollbg.Top);

                    float linespace = (float)1 / -viewrange * (scrollbg.Bottom - scrollbg.Top);

                    PointF[] polyn = new PointF[4];

                    polyn[0] = new PointF(scrollbg.Left, scrollbg.Top + (scrollbg.Bottom - scrollbg.Top) / 2);
                    polyn[1] = new PointF(scrollbg.Left - scrollbg.Width / 4,
                        scrollbg.Top + (scrollbg.Bottom - scrollbg.Top) / 2);
                    polyn[2] = polyn[1];
                    float peak = 0;
                    if (scaledvalue > 0)
                    {
                        peak = -scrollbg.Width / 4;
                        if (scrollbg.Top + (scrollbg.Bottom - scrollbg.Top) / 2 + scaledvalue + peak <
                            scrollbg.Top + (scrollbg.Bottom - scrollbg.Top) / 2)
                            peak = -scaledvalue;
                    }
                    else if (scaledvalue < 0)
                    {
                        peak = +scrollbg.Width / 4;
                        if (scrollbg.Top + (scrollbg.Bottom - scrollbg.Top) / 2 + scaledvalue + peak >
                            scrollbg.Top + (scrollbg.Bottom - scrollbg.Top) / 2)
                            peak = -scaledvalue;
                    }

                    polyn[2] = new PointF(scrollbg.Left - scrollbg.Width / 4,
                        scrollbg.Top + (scrollbg.Bottom - scrollbg.Top) / 2 + scaledvalue + peak);
                    polyn[3] = new PointF(scrollbg.Left,
                        scrollbg.Top + (scrollbg.Bottom - scrollbg.Top) / 2 + scaledvalue);

                    //graphicsObject.DrawPolygon(redPen, poly);
                    graphicsObject.FillPolygon(Brushes.Blue, polyn);

                    // draw outsidebox
                    graphicsObject.DrawPolygon(this._whitePen, poly);

                    for (int a = 1; a < viewrange; a++)
                    {
                        graphicsObject.DrawLine(this._whitePen, scrollbg.Left - scrollbg.Width / 4,
                            scrollbg.Top - linespace * a, scrollbg.Left - scrollbg.Width / 8,
                            scrollbg.Top - linespace * a);
                    }

                    // draw arrow and text

                    graphicsObject.ResetTransform();
                    graphicsObject.TranslateTransform(this.Width, this.Height / 2);
                    graphicsObject.RotateTransform(180);

                    graphicsObject.DrawPolygon(this._blackPen, arrow);
                    graphicsObject.FillPolygon(Brushes.Black, arrow);
                    graphicsObject.ResetTransform();
                    graphicsObject.TranslateTransform(0, this.Height / 2);

                    drawstring(((int)_alt).ToString("0 ") + altunit, font, 10, (SolidBrush)Brushes.AliceBlue,
                        scrollbg.Left + 10, -9);
                    graphicsObject.ResetTransform();

                    // mode and wp dist and wp
                    drawstring(_mode, font, fontsize, _whiteBrush, scrollbg.Left - 30,
                        scrollbg.Bottom + 5);
                    drawstring((int)_disttowp + distunit + ">" + _wpno, font, fontsize, _whiteBrush,
                        scrollbg.Left - 30, scrollbg.Bottom + fontsize + 2 + 10);
                }

                if (displayconninfo)
                {
                    graphicsObject.DrawLine(this._greenPen, scrollbg.Left - 5,
                        scrollbg.Top - (int)(fontsize * 2.2) - 2 - 20, scrollbg.Left - 5,
                        scrollbg.Top - (int)(fontsize) - 2 - 20);
                    graphicsObject.DrawLine(this._greenPen, scrollbg.Left - 10,
                        scrollbg.Top - (int)(fontsize * 2.2) - 2 - 15, scrollbg.Left - 10,
                        scrollbg.Top - (int)(fontsize) - 2 - 20);
                    graphicsObject.DrawLine(this._greenPen, scrollbg.Left - 15,
                        scrollbg.Top - (int)(fontsize * 2.2) - 2 - 10, scrollbg.Left - 15,
                        scrollbg.Top - (int)(fontsize) - 2 - 20);

                    drawstring(_linkqualitygcs.ToString("0") + "%", font, fontsize, _whiteBrush,
                        scrollbg.Left, scrollbg.Top - (int)(fontsize * 2.2) - 2 - 20);
                    if (_linkqualitygcs == 0)
                    {
                        graphicsObject.DrawLine(this._redPen, scrollbg.Left,
                            scrollbg.Top - (int)(fontsize * 2.2) - 2 - 20, scrollbg.Left + 50,
                            scrollbg.Top - (int)(fontsize * 2.2) - 2);

                        graphicsObject.DrawLine(this._redPen, scrollbg.Left, scrollbg.Top - (int)(fontsize * 2.2) - 2,
                            scrollbg.Left + 50, scrollbg.Top - (int)(fontsize * 2.2) - 2 - 20);
                    }

                    drawstring(_datetime.ToString("HH:mm:ss"), font, fontsize, _whiteBrush,
                        scrollbg.Left - 30, scrollbg.Top - fontsize - 2 - 20);
                }

                // AOA
                if (displayAOASSA)
                {
                    scrollbg = new Rectangle((int)(this.Width - (double)this.Width / 6), halfheight + halfheight / 10,
                        this.Width / 25, this.Height / 5);

                    graphicsObject.ResetTransform();

                    graphicsObject.FillRectangle(Brushes.Red,
                        new RectangleF(scrollbg.Left, scrollbg.Top, scrollbg.Width,
                            scrollbg.Height * (100 - _redSSAp) / 100));
                    graphicsObject.FillRectangle(Brushes.Yellow,
                        new RectangleF(scrollbg.Left, scrollbg.Top + scrollbg.Height * (100 - _redSSAp) / 100,
                            scrollbg.Width, scrollbg.Height * (_redSSAp - _yellowSSAp) / 100));
                    graphicsObject.FillRectangle(Brushes.Green,
                        new RectangleF(scrollbg.Left, scrollbg.Top + scrollbg.Height * (100 - _yellowSSAp) / 100,
                            scrollbg.Width, scrollbg.Height * (_yellowSSAp - _greenSSAp) / 100));
                    graphicsObject.FillRectangle(Brushes.Blue,
                        new RectangleF(scrollbg.Left, scrollbg.Top + scrollbg.Height * (100 - _greenSSAp) / 100,
                            scrollbg.Width, scrollbg.Height * _greenSSAp / 100));

                    graphicsObject.DrawRectangle(this._whitePen, scrollbg);

                    float AOA_ind = scrollbg.Height * (100 - _greenSSAp) / 100 -
                                    (_AOA / _critAOA) * (scrollbg.Height * (_redSSAp - _greenSSAp) / 100);
                    if (AOA_ind < 0)
                        AOA_ind = 0;
                    if (AOA_ind > scrollbg.Height)
                        AOA_ind = scrollbg.Height;

                    PointF[] AOA_arrow = new PointF[3];
                    AOA_arrow[0] = new PointF(scrollbg.Left + scrollbg.Width / 5, scrollbg.Top + AOA_ind);
                    AOA_arrow[1] = new PointF(scrollbg.Left - scrollbg.Width / 2 + scrollbg.Width / 5,
                        scrollbg.Top + scrollbg.Width / 2 + AOA_ind);
                    AOA_arrow[2] = new PointF(scrollbg.Left - scrollbg.Width / 2 + scrollbg.Width / 5,
                        scrollbg.Top - scrollbg.Width / 2 + AOA_ind);

                    graphicsObject.FillPolygon(Brushes.Black, AOA_arrow);
                    graphicsObject.DrawPolygon(this._whitePen, AOA_arrow);
                }

                // battery
                if (batteryon)
                {
                    graphicsObject.ResetTransform();

                    string text = HUDT.Bat + _batterylevel.ToString("0.00v") + " " + _current.ToString("0.0 A");

                    text = HUDT.Bat + _batterylevel.ToString("0.00v") + " " + _current.ToString("0.0 A") + " " +
                           (_batteryremaining) + "%";

                    if (lowvoltagealert)
                    {
                        drawstring(text, font, fontsize + 2, (SolidBrush)Brushes.Red, fontsize,
                            this.Height - 30 - fontoffset);
                    }
                    else
                    {
                        drawstring(text, font, fontsize + 2, _whiteBrush, fontsize,
                            this.Height - 30 - fontoffset);
                    }
                }

                // gps
                if (displaygps)
                {
                    string gps = "";
                    SolidBrush col = _whiteBrush;
                    var _fix = Math.Max(_gpsfix, _gpsfix2);

                    if (_fix == 0)
                    {
                        gps = (HUDT.GPS0);
                        col = (SolidBrush)Brushes.Red;
                    }
                    else if (_fix == 1)
                    {
                        gps = (HUDT.GPS1);
                        col = (SolidBrush)Brushes.Red;
                    }
                    else if (_fix == 2)
                    {
                        gps = (HUDT.GPS2);
                    }
                    else if (_fix == 3)
                    {
                        gps = (HUDT.GPS3);
                    }
                    else if (_fix == 4)
                    {
                        gps = (HUDT.GPS4);
                    }
                    else if (_fix == 5)
                    {
                        gps = (HUDT.GPS5);
                    }
                    else if (_fix == 6)
                    {
                        gps = (HUDT.GPS6);
                    }
                    else
                    {
                        gps = _fix.ToString();
                    }

                    drawstring(gps, font, fontsize + 2, col, this.Width - 13 * fontsize,
                        this.Height - 30 - fontoffset);
                }

                if (isNaN)
                    drawstring("NaN Error " + DateTime.Now, font, this.Height / 30 + 10,
                        (SolidBrush)Brushes.Red, 50, 50);

                // custom user items
                graphicsObject.ResetTransform();
                int height = this.Height - 30 - fontoffset - fontsize - 8;
                foreach (string key in CustomItems.Keys)
                {
                    try
                    {
                        Custom item = (Custom)CustomItems[key];
                        if (item.Item == null)
                            continue;
                        if (item.Item.Name.Contains("lat") || item.Item.Name.Contains("lng"))
                        {
                            drawstring(item.Header + item.GetValue.ToString("0.#######"), font,
                                fontsize + 2, _whiteBrush, this.Width / 8, height);
                        }
                        else if (item.Item.Name == "battery_usedmah")
                        {
                            drawstring(item.Header + item.GetValue.ToString("0"), font, fontsize + 2,
                                _whiteBrush, this.Width / 8, height);
                        }
                        else if (item.Item.Name == "timeInAir")
                        {
                            double stime = item.GetValue;
                            int hrs = (int)(stime / (60 * 60));
                            //stime -= hrs * 60 * 60;
                            int mins = (int)(stime / (60)) % 60;
                            //stime = mins * 60;
                            int secs = (int)(stime % 60);
                            drawstring(
                                item.Header + hrs.ToString("00") + ":" + mins.ToString("00") + ":" +
                                secs.ToString("00"), font, fontsize + 2, _whiteBrush, this.Width / 8, height);
                        }
                        else
                        {
                            drawstring(item.Header + item.GetValue.ToString("0.##"), font, fontsize + 2,
                                _whiteBrush, this.Width / 8, height);
                        }

                        height -= fontsize + 5;
                    }
                    catch
                    {
                    }

                }




                graphicsObject.TranslateTransform(this.Width / 2, this.Height / 2);

                // draw armed

                if (status != statuslast)
                {
                    armedtimer = DateTime.Now;
                }

                if (status == false) // not armed
                {
                    //if ((armedtimer.AddSeconds(8) > DateTime.Now))
                    {
                        drawstring(HUDT.DISARMED, font, fontsize + 10, (SolidBrush)Brushes.Red, -85,
                            halfheight / -3);
                        statuslast = status;
                    }
                }
                else if (status == true) // armed
                {
                    if ((armedtimer.AddSeconds(8) > DateTime.Now))
                    {
                        drawstring(HUDT.ARMED, font, fontsize + 20, (SolidBrush)Brushes.Red, -70,
                            halfheight / -3);
                        statuslast = status;
                    }
                }

                if (failsafe == true)
                {
                    drawstring(HUDT.FAILSAFE, font, fontsize + 20, (SolidBrush)Brushes.Red, -85,
                        halfheight / -HUDT.FailsafeH);
                    statuslast = status;
                }

                if (message != "" && messagetime.AddSeconds(10) > DateTime.Now)
                {
                    drawstring(message, font, fontsize + 10, (SolidBrush)Brushes.Red, -halfwidth + 50,
                        halfheight / 3);
                }

                graphicsObject.ResetTransform();

                if (displayvibe)
                {
                    vibehitzone = new Rectangle(this.Width - 18 * fontsize, this.Height - 30 - fontoffset, 40,
                        fontsize * 2);

                    if (vibex > 30 || vibey > 30 || vibez > 30)
                    {
                        drawstring("Vibe", font, fontsize + 2, (SolidBrush)Brushes.Red, vibehitzone.X,
                            vibehitzone.Y);
                    }
                    else
                    {
                        drawstring("Vibe", font, fontsize + 2, _whiteBrush, vibehitzone.X,
                            vibehitzone.Y);
                    }
                }

                if (displayekf)
                {
                    ekfhitzone = new Rectangle(this.Width - 23 * fontsize, this.Height - 30 - fontoffset, 40,
                        fontsize * 2);

                    if (ekfstatus > 0.5)
                    {
                        if (ekfstatus > 0.8)
                        {
                            drawstring("EKF", font, fontsize + 2, (SolidBrush)Brushes.Red,
                                ekfhitzone.X,
                                ekfhitzone.Y);
                        }
                        else
                        {
                            drawstring("EKF", font, fontsize + 2, (SolidBrush)Brushes.Orange,
                                ekfhitzone.X,
                                ekfhitzone.Y);
                        }
                    }
                    else
                    {
                        drawstring("EKF", font, fontsize + 2, _whiteBrush, ekfhitzone.X, ekfhitzone.Y);
                    }
                }

                if (DesignMode)
                {
                    return;
                }

                //                Console.WriteLine("HUD 1 " + (DateTime.Now - starttime).TotalMilliseconds + " " + DateTime.Now.Millisecond);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                log.Info("hud error " + ex.ToString());
            }
        }


        void drawstring(string text, Font font, float fontsize, SolidBrush brush, float x, float y)
        {
            if (text == null || text == "")
                return;

            graphicsObject.DrawString(text, font, brush, x, y);
            return;

            float maxy = 0;

            foreach (char cha in text)
            {
                int charno = (int)cha;

                int charid = charno ^ (int)(fontsize * 1000) ^ brush.Color.ToArgb();

                if (!charDict.ContainsKey(charid))
                {
                    charDict[charid] = new character()
                    {
                        bitmap = new Bitmap(128, 128, SKColorType.Bgra8888),
                        size = (int)fontsize
                    };

                    charDict[charid].bitmap.MakeTransparent(Color.Transparent);

                    //charbitmaptexid

                    float maxx = this.Width / 150; // for space


                    // create bitmap
                    using (var gfx = Graphics.FromImage(charDict[charid].bitmap))
                    {
                        var pth = new GraphicsPath();

                        if (text != null)
                            pth.AddString(cha + "", font.FontFamily, 0, fontsize + 5, new Point((int)0, (int)0),
                                StringFormat.GenericTypographic);

                        charDict[charid].pth = pth;

                        gfx.SmoothingMode = SmoothingMode.AntiAlias;

                        gfx.DrawPath(this._p, pth);

                        //Draw the face

                        gfx.FillPath(brush, pth);


                        if (pth.PointCount > 0)
                        {
                            foreach (PointF pnt in pth.PathPoints)
                            {
                                if (pnt.X > maxx)
                                    maxx = pnt.X;

                                if (pnt.Y > maxy)
                                    maxy = pnt.Y;
                            }
                        }
                    }

                    charDict[charid].width = (int)(maxx + 2);
                }

                // draw it

                float scale = 1.0f;
                // dont draw spaces
                if (cha != ' ')
                {
                    graphicsObject.DrawImage(charDict[charid].bitmap, (int)x, (int)y, charDict[charid].bitmap.Width, charDict[charid].bitmap.Height);
                }
                else
                {

                }

                x += charDict[charid].width * scale;
            }

        }

        float wrap360(float noin)
        {
            if (noin < 0)
                return noin + 360;
            return noin;
        }

        public struct Custom
        {
            //public Point Position;
            //public float FontSize;
            public string Header;

            public System.Reflection.PropertyInfo Item;

            public static object src { get; set; }

            public double GetValue
            {
                get
                {
                    if (Item.PropertyType == typeof(Single))
                    {
                        return (double)(float)Item.GetValue(src, null);
                    }

                    if (Item.PropertyType == typeof(Int32))
                    {
                        return (double)(int)Item.GetValue(src, null);
                    }

                    if (Item.PropertyType == typeof(double))
                    {
                        return (double)Item.GetValue(src, null);
                    }

                    throw new Exception("Bad data type");
                }
            }
        }

        private class character
        {
            public Bitmap bitmap;
            public int gltextureid;
            public GraphicsPath pth;
            public int size;
            public int width;
        }
    }


}