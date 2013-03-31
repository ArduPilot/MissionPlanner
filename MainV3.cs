using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.IO;

using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

using AssortedWidgets;
using AssortedWidgets.GLFont;
using AssortedWidgets.Graphics;
using AssortedWidgets.Widgets;
using AssortedWidgets.Events;

using ArdupilotMega.Utilities;
using IronPython.Hosting;
using log4net;
using ArdupilotMega.Controls;
using System.Security.Cryptography;
using ArdupilotMega.Comms;
using ArdupilotMega.Arduino;
using ArdupilotMega.Widgets;
using AssortedWidgets.Managers;

namespace ArdupilotMega
{
   public class MainV3 : GameWindow
    {
       float angle = 0;

       /// <summary>
       /// Active Comport interface
       /// </summary>
       public static MAVLink comPort = new MAVLink();
       /// <summary>
       /// passive comports
       /// </summary>
       public static List<MAVLink> Comports = new List<MAVLink>();

       /// <summary>
       /// Comport name
       /// </summary>
       public static string comPortName = "";
       /// <summary>
       /// use to store all internal config
       /// </summary>
       public static Hashtable config = new Hashtable();
       /// <summary>
       /// mono detection
       /// </summary>
       public static bool MONO = false;
       /// <summary>
       /// speech engine enable
       /// </summary>
       public static bool speechEnable = false;
       /// <summary>
       /// spech engine static class
       /// </summary>
       public static Speech speechEngine = null;
       /// <summary>
       /// joystick static class
       /// </summary>
      // public static Joystick joystick = null;
       /// <summary>
       /// track last joystick packet sent. used to control rate
       /// </summary>
       DateTime lastjoystick = DateTime.Now;
       /// <summary>
       /// hud background image grabber from a video stream - not realy that efficent. ie no hardware overlays etc.
       /// </summary>
       public static WebCamService.Capture cam = null;
       /// <summary>
       /// controls the main serial reader thread
       /// </summary>
       bool serialThread = false;
       /// <summary>
       /// used for mini http server for websockets/mjpeg video stream, and network link kmls
       /// </summary>
       private TcpListener listener;
       /// <summary>
       /// track the last heartbeat sent
       /// </summary>
       private DateTime heatbeatSend = DateTime.Now;
       /// <summary>
       /// used to call anything as needed.
       /// </summary>
       public static MainV3 instance = null;

       public static string LogDir
       {
           get
           {
               if (config["logdirectory"] == null)
                   return _logdir;
               return config["logdirectory"].ToString();
           }
           set
           {
               _logdir = value;
               config["logdirectory"] = value;
           }
       }
       static string _logdir ="";// Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"logs";

       public static MainSwitcher View;

       /// <summary>
       /// used to feed in a network link kml to the http server
       /// </summary>
       public string georefkml = "";
       public string mavelous_web ="";// Application.StartupPath + Path.DirectorySeparatorChar + @"mavelous_web\";
       public string georefimagepath = "";

       /// <summary>
       /// store the time we first connect
       /// </summary>
       DateTime connecttime = DateTime.Now;

       /// <summary>
       /// enum of firmwares
       /// </summary>
       public enum Firmwares
       {
           ArduPlane,
           ArduCopter2,
           ArduRover,
           Ateryx
       }

       DateTime connectButtonUpdate = DateTime.Now;
       /// <summary>
       /// declared here if i want a "single" instance of the form
       /// ie configuration gets reloaded on every click
       /// </summary>
       GCSViews.FlightData FlightData;
       GCSViews.FlightPlanner FlightPlanner;
       //GCSViews.ConfigurationView.Setup Configuration;
       GCSViews.Simulation Simulation;
       //GCSViews.Firmware Firmware;
       //GCSViews.Terminal Terminal;

       //private Form connectionStatsForm;
       private ConnectionStats _connectionStats;

       /// <summary>
       /// This 'Control' is the toolstrip control that holds the comport combo, baudrate combo etc
       /// Otiginally seperate controls, each hosted in a toolstip sqaure, combined into this custom
       /// control for layout reasons.
       /// </summary>
       static internal ConnectionControl _connectionControl;

       public MainV3(uint width, uint height)
            : base((int)width, (int)height)
        {
            MainV3.instance = this;

            UI.Instance.Init(width, height);
            Keyboard.KeyDown += Keyboard_KeyDown;
            Mouse.ButtonDown += new EventHandler<MouseButtonEventArgs>(Mouse_ButtonDown);
            Mouse.ButtonUp += new EventHandler<MouseButtonEventArgs>(Mouse_ButtonUp);

            Menu menuConnect = new Menu("Connect");
            Menu menuFlightData = new Menu("FlightData");
            Menu menuFlightPlanner = new Menu("FlightPlanner");
            Menu menuConfig = new Menu("Config");
            Menu menuFirmware = new Menu("Firmware");
            Menu menuTerminal = new Menu("Terminal");
            Menu menuHelp = new Menu("Help");
            Menu menuExit = new Menu("Exit");

            MenuItemButton menuItemPort = new MenuItemButton("Port");
            MenuItemButton menuItemBaud = new MenuItemButton("Baud");

            menuConnect.AddItem(menuItemPort);
            menuConnect.AddItem(menuItemBaud);


            MenuBar.Instance.AddMenu(menuConnect);
            MenuBar.Instance.AddMenu(menuFlightData);
            MenuBar.Instance.AddMenu(menuFlightPlanner);
            MenuBar.Instance.AddMenu(menuConfig);
            MenuBar.Instance.AddMenu(menuFirmware);
            MenuBar.Instance.AddMenu(menuTerminal);
            MenuBar.Instance.AddMenu(menuHelp);
            MenuBar.Instance.AddMenu(menuExit);

            menuConnect.MousePressedEvent += new MousePressedHandler(MenuConnect_Click);
            menuExit.MousePressedEvent += new AssortedWidgets.Widgets.MousePressedHandler(menuItemFileExit_MousePressedEvent);

            Dialog dl = new Dialog("test", 23, 32, 150, 150);
            PictureBox picb = new PictureBox();
            picb.Image = ArdupilotMega.Properties.Resources.x8;

            dl.Add(picb);

            DialogManager.Instance.SetModelessDialog(dl);

            CustomMessageBox.Show("test", "test2",MessageBox.MessageBoxButtons.YesNo);
        }

       private void MenuConnect_Click(MouseEvent me)
       {
           Comms.CommsSerialScan.Scan(false);

           DateTime deadline = DateTime.Now.AddSeconds(50);

           while (Comms.CommsSerialScan.foundport == false)
           {
               System.Threading.Thread.Sleep(100);

               if (DateTime.Now > deadline)
               {
                   MessageBox.Show("Timeout waiting for autoscan/no mavlink device connected");
                   return;
               }
           }

           MainV2.comPort.BaseStream.PortName = Comms.CommsSerialScan.portinterface.PortName;
           MainV2.comPort.BaseStream.BaudRate = Comms.CommsSerialScan.portinterface.BaudRate;

           MainV2.comPort.Open(true);
       }

       #region Keyboard_KeyDown

       /// <summary>
       /// Occurs when a key is pressed.
       /// </summary>
       /// <param name="sender">The KeyboardDevice which generated this event.</param>
       /// <param name="me">The key that was pressed.</param>
       void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
       {
           if (e.Key == Key.Escape)
               this.Exit();

           if (e.Key == Key.F12)
               if (this.WindowState == WindowState.Fullscreen)
                   this.WindowState = WindowState.Normal;
               else
                   this.WindowState = WindowState.Fullscreen;
       }

       #endregion

       void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
       {
           UI.Instance.ImportMousePress(e.Button, e.X, e.Y);
       }
       void Mouse_ButtonUp(object sender, MouseButtonEventArgs e)
       {
           UI.Instance.ImportMouseRelease(e.Button, e.X, e.Y);
       }

       #region OnLoad

       /// <summary>
       /// Setup OpenGL and load resources here.
       /// </summary>
       /// <param name="me">Not used.</param>
       protected override void OnLoad(EventArgs e)
       {
           GL.ClearColor(Color.FromArgb(87, 104, 100));
           GL.ShadeModel(ShadingModel.Smooth);
           //GL.ClearDepth(1.0f);
           //GL.DepthFunc(DepthFunction.Lequal);
           //GL.Enable(EnableCap.DepthTest);
           GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
           //GL.Enable(EnableCap.Blend);
           //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
       }

       void menuItemFileExit_MousePressedEvent(AssortedWidgets.Events.MouseEvent me)
       {
           Close();
       }

       #endregion

       #region OnReSize

       /// <summary>
       /// Respond to reSize events here.
       /// </summary>
       /// <param name="me">Contains information on the new GameWindow Size.</param>
       /// <remarks>There is no need to call the base implementation.</remarks>
       protected override void OnResize(EventArgs e)
       {
           base.OnResize(e);

           GL.Viewport(0, 0, Width, Height);

           double aspect_ratio = Width / (double)Height;

           OpenTK.Matrix4 perspective = OpenTK.Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect_ratio, 1, 64);
           GL.MatrixMode(MatrixMode.Projection);
           GL.LoadMatrix(ref perspective);

           UI.Instance.ReSize((uint)Width, (uint)Height);
       }

       #endregion

       #region OnUpdateFrame

       /// <summary>
       /// Add your game logic here.
       /// </summary>
       /// <param name="me">Contains timing information.</param>
       /// <remarks>There is no need to call the base implementation.</remarks>
       protected override void OnUpdateFrame(FrameEventArgs e)
       {
           base.OnUpdateFrame(e);
       }
       #endregion

       #region OnRenderFrame

       /// <summary>
       /// Add your game rendering code here.
       /// </summary>
       /// <param name="me">Contains timing information.</param>
       /// <remarks>There is no need to call the base implementation.</remarks>
       protected override void OnRenderFrame(FrameEventArgs e)
       {
           base.OnRenderFrame(e);

           GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

           Matrix4 lookat = Matrix4.LookAt(0, 5, 5, 0, 0, 0, 0, 1, 0);
           GL.MatrixMode(MatrixMode.Modelview);
           GL.LoadMatrix(ref lookat);

           angle += 10 * (float)e.Time;
           GL.Rotate(angle, 0.0f, 1.0f, 0.0f);

           GL.Disable(EnableCap.Lighting);

           DrawCube();

           UI.Instance.ImportMouseMotion(Mouse.X, Mouse.Y);
           UI.Instance.BeginPaint();
           UI.Instance.EndPaint();

           SwapBuffers();
       }

       #region private void DrawCube()

       private void DrawCube()
       {
           GL.Begin(BeginMode.Quads);

           GL.Color3(Color.Silver);
           GL.Vertex3(-1.0f, -1.0f, -1.0f);
           GL.Vertex3(-1.0f, 1.0f, -1.0f);
           GL.Vertex3(1.0f, 1.0f, -1.0f);
           GL.Vertex3(1.0f, -1.0f, -1.0f);

           GL.Color3(Color.Honeydew);
           GL.Vertex3(-1.0f, -1.0f, -1.0f);
           GL.Vertex3(1.0f, -1.0f, -1.0f);
           GL.Vertex3(1.0f, -1.0f, 1.0f);
           GL.Vertex3(-1.0f, -1.0f, 1.0f);

           GL.Color3(Color.Moccasin);

           GL.Vertex3(-1.0f, -1.0f, -1.0f);
           GL.Vertex3(-1.0f, -1.0f, 1.0f);
           GL.Vertex3(-1.0f, 1.0f, 1.0f);
           GL.Vertex3(-1.0f, 1.0f, -1.0f);

           GL.Color3(Color.IndianRed);
           GL.Vertex3(-1.0f, -1.0f, 1.0f);
           GL.Vertex3(1.0f, -1.0f, 1.0f);
           GL.Vertex3(1.0f, 1.0f, 1.0f);
           GL.Vertex3(-1.0f, 1.0f, 1.0f);

           GL.Color3(Color.PaleVioletRed);
           GL.Vertex3(-1.0f, 1.0f, -1.0f);
           GL.Vertex3(-1.0f, 1.0f, 1.0f);
           GL.Vertex3(1.0f, 1.0f, 1.0f);
           GL.Vertex3(1.0f, 1.0f, -1.0f);

           GL.Color3(Color.ForestGreen);
           GL.Vertex3(1.0f, -1.0f, -1.0f);
           GL.Vertex3(1.0f, 1.0f, -1.0f);
           GL.Vertex3(1.0f, 1.0f, 1.0f);
           GL.Vertex3(1.0f, -1.0f, 1.0f);

           GL.End();
       }

       #endregion

       void DrawQuad(int xlu, int ylu, int xrd, int yrd, int texSize)
       {
           float auxU = 0, auxV = 0;

           int xld = xlu;
           int yld = yrd;
           int xru = xrd;
           int yru = ylu;

           GL.Begin(BeginMode.Quads);

           CalculeOpenGLTexCoord(xlu, ylu, ref auxU, ref auxV, texSize);
           GL.TexCoord2(auxU, auxV);
           GL.Vertex2(220f, 220f);
           CalculeOpenGLTexCoord(xld, yld, ref auxU, ref auxV, texSize);
           GL.TexCoord2(auxU, auxV);
           GL.Vertex2(220f, 400f);
           CalculeOpenGLTexCoord(xrd, yrd, ref auxU, ref auxV, texSize);
           GL.TexCoord2(auxU, auxV);
           GL.Vertex2(480f, 400f);
           CalculeOpenGLTexCoord(xru, yru, ref auxU, ref auxV, texSize);
           GL.TexCoord2(auxU, auxV);
           GL.Vertex2(480f, 220f);

           GL.End();
       }
       public static void CalculeOpenGLTexCoord(int x, int y, ref float u, ref float v, int SizeTex)
       {
           if (x < 0)
               x = 0;
           if (y < 0)
               y = 0;

           u = (x + 0.35f) / (float)SizeTex;
           v = (SizeTex - (y + 0.35f)) / (float)SizeTex;
       }

       #endregion

       #region OnUnload

       protected override void OnUnload(EventArgs e)
       {
           base.OnUnload(e);

           TextureManager.Singleton.UnloadAll();
       }
       #endregion OnUnload
    }
}
