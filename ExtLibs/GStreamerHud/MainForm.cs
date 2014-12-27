using Gst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GStreamerHud
{
    public class MainForm : Form
    {
        Gst.GLib.MainLoop m_GLibMainLoop;
        System.Threading.Thread m_GLibThread;
        Pipeline m_Pipeline;

        Element m_Source, m_Sink;

        //udpsrc port=9000  buffer-size=60000 ! application/x-rtp,encoding-name=H264,payload=96 ! rtph264depay ! h264parse ! queue ! avdec_h264

        public MainForm()
        {
            // These environment variables are necessary to locate GStreamer libraries, and to stop it from loading

            // wrong libraries installed elsewhere on the system.

            string apppath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            System.Environment.SetEnvironmentVariable("GST_PLUGIN_PATH", "");

            System.Environment.SetEnvironmentVariable("GST_PLUGIN_SYSTEM_PATH", apppath + @"\gstreamer\bin\plugins");

            System.Environment.SetEnvironmentVariable("PATH", @"C:\Windows;"

                                                        + apppath + @"\gstreamer\lib;"

                                                        + apppath + @"\gstreamer\bin");

            System.Environment.SetEnvironmentVariable("GST_REGISTRY", apppath + @"\gstreamer\bin\registry.bin");



            // These are for saving debug information.

            System.Environment.SetEnvironmentVariable("GST_DEBUG", "*:3");

            System.Environment.SetEnvironmentVariable("GST_DEBUG_FILE", "GstreamerLog.txt");

            System.Environment.SetEnvironmentVariable("GST_DEBUG_DUMP_DOT_DIR", apppath);











            // Initialize Gstreamer
            Gst.Application.Init();

            // Build the pipeline
            var source = ElementFactory.Make("videotestsrc", "source");
            var sink = ElementFactory.Make("autovideosink", "sink");

            // Create the empty pipeline
            var pipeline = new Pipeline("test-pipeline");

            if (pipeline == null || source == null || sink == null)
            {
                Console.WriteLine("Not all elements could be created");
                return;
            }

            // Build the pipeline
            pipeline.Add(source, sink);
            if (!source.Link(sink))
            {
                Console.WriteLine("Elements could not be linked");
                return;
            }

            // Modify the source's properties
            source["pattern"] = 0;

            // Start playing
            var ret = pipeline.SetState(State.Playing);
            if (ret == StateChangeReturn.Failure)
            {
                Console.WriteLine("Unable to set the pipeline to the playing state");
                return;
            }

            // Wait until error or EOS
            var bus = pipeline.Bus;
            //var msg = bus.TimedPopFiltered(Constants.CLOCK_TIME_NONE, MessageType.Eos | MessageType.Error);

            // Free resources
          /*  if (msg != null)
            {
                switch (msg.Type)
                {
                    case MessageType.Error:
                        GLib.GException exc;
                        string debug;
                        msg.ParseError(out exc, out debug);
                        Console.WriteLine(String.Format("Error received from element {0}: {1}", msg.Src.Name, exc.Message));
                        Console.WriteLine(String.Format("Debugging information {0}", debug));
                        break;
                    case MessageType.Eos:
                        Console.WriteLine("End-Of-Stream reached");
                        break;
                    default:
                        // We should not reach here because we only asked for ERRORs and EOS
                        Console.WriteLine("Unexpected messag received");
                        break;
                }
            }*/

            pipeline.SetState(State.Null);

          //  return;


          

            Gst.Application.Init(); 



            InitializeComponent();



            // Create a main loop for GLib, run it in a separate thread

            m_GLibMainLoop = new Gst.GLib.MainLoop();

            m_GLibThread = new System.Threading.Thread(m_GLibMainLoop.Run);

            m_GLibThread.IsBackground = true;

            m_GLibThread.Name = "GLibMainLoop";

            m_GLibThread.Start();



           // System.Threading.Thread.CurrentThread.Name = "WinForms";



            CreatePipeline();

        }


        private System.ComponentModel.IContainer components = null;



        /// <summary>

        /// Clean up any resources being used.

        /// </summary>

        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>

        protected override void Dispose(bool disposing)
        {

            if (disposing && (components != null))
            {

                components.Dispose();

            }

            base.Dispose(disposing);

        }



        #region Windows Form Designer generated code



        /// <summary>

        /// Required method for Designer support - do not modify

        /// the contents of this method with the code editor.

        /// </summary>

        private void InitializeComponent()
        {

            this.btnStart = new System.Windows.Forms.Button();

            this.btnStop = new System.Windows.Forms.Button();

            this.videoPanel = new System.Windows.Forms.Panel();

            this.SuspendLayout();

            // 

            // btnStart

            // 

            this.btnStart.Location = new System.Drawing.Point(12, 12);

            this.btnStart.Name = "btnStart";

            this.btnStart.Size = new System.Drawing.Size(70, 37);

            this.btnStart.TabIndex = 0;

            this.btnStart.Text = "Start";

            this.btnStart.UseVisualStyleBackColor = true;

            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);

            // 

            // btnStop

            // 

            this.btnStop.Location = new System.Drawing.Point(99, 12);

            this.btnStop.Name = "btnStop";

            this.btnStop.Size = new System.Drawing.Size(64, 36);

            this.btnStop.TabIndex = 1;

            this.btnStop.Text = "Stop";

            this.btnStop.UseVisualStyleBackColor = true;

            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);

            // 

            // videoPanel

            // 

            this.videoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)

            | System.Windows.Forms.AnchorStyles.Left)

            | System.Windows.Forms.AnchorStyles.Right)));

            this.videoPanel.Location = new System.Drawing.Point(3, 110);

            this.videoPanel.Name = "videoPanel";

            this.videoPanel.Size = new System.Drawing.Size(575, 331);

            this.videoPanel.TabIndex = 2;

            // 

            // MainForm

            // 

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            this.ClientSize = new System.Drawing.Size(579, 443);

            this.Controls.Add(this.videoPanel);

            this.Controls.Add(this.btnStop);

            this.Controls.Add(this.btnStart);

            this.Name = "MainForm";

            this.Text = "MainForm";

            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);

            this.ResumeLayout(false);



        }



        #endregion



        private System.Windows.Forms.Button btnStart;

        private System.Windows.Forms.Button btnStop;

        private System.Windows.Forms.Panel videoPanel;





        private void CreatePipeline()
        {

            m_Pipeline = new Pipeline("test-pipeline");

            //m_Source = Gst.Parse.Launch("playbin uri=http://ftp.nluug.nl/ftp/graphics/blender/apricot/trailer/Sintel_Trailer1.1080p.DivX_Plus_HD.mkv"); 



            m_Source = Gst.ElementFactory.Make("videotestsrc","source");

            m_Source["pattern"] = 18; // Example of setting element properties



            m_Sink = Gst.ElementFactory.Make("autovideosink", "sink");



            m_Pipeline.Add(m_Source, m_Sink);

            m_Source.Link(m_Sink);

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            m_GLibMainLoop.Quit();

        } 


        private void btnStart_Click(object sender, EventArgs e)
        {

            // Tell d3dvideosink to render into our window.

            var overlay = new Gst.Interfaces.XOverlayAdapter(m_Sink.Handle);

            overlay.XwindowId = (ulong)videoPanel.Handle;



            m_Pipeline.SetState(State.Playing);



          //  m_Pipeline.DebugToDotFile("graph"); // Save pipeline to graph.dot

        }



        private void btnStop_Click(object sender, EventArgs e)
        {

            m_Pipeline.SetState(State.Ready);

        }


    }
}