using MissionPlanner;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using MissionPlanner.Comms;

namespace Forwarding
{
    public class Plugin : MissionPlanner.Plugin.Plugin
    {
        public override string Name
        {
            get { return "Forwarding"; }
        }

        public override string Version
        {
            get { return "0.10"; }
        }

        public override string Author
        {
            get { return "Michael Oborne"; }
        }

        public override bool Init()
        {
            this.loopratehz = 1;
            // to enable change this
            return false;
        }

        public override bool Loaded()
        {
            return true;
        }
        
        static TcpListener listener;
        bool started = false;

        public override bool Loop()
        {
            {
                MainV2.comPort.MirrorStreamWrite = true;
                
                if (!started)
                {
                    started = true;
                    MainV2.comPort.MirrorStream = new TcpSerial();
                    var port = 14550;
                    listener = new TcpListener(System.Net.IPAddress.Any, port);
                                listener.Start(0);
                                listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
                }
            }
            return true;
        }
        
        void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener)ar.AsyncState;

            // End the operation and display the received data on  
            // the console.
            TcpClient client = listener.EndAcceptTcpClient(ar);

            ((TcpSerial)MainV2.comPort.MirrorStream).client = client;

            listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
        }

        public override bool Exit()
        {
            return true;
        }
    }
}