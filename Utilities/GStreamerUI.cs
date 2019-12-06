using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;
using MissionPlanner.Controls;

namespace MissionPlanner.Utilities
{
    public class GStreamerUI
    {
        public static void DownloadGStreamer()
        {
            ProgressReporterDialogue prd = new ProgressReporterDialogue();
            ThemeManager.ApplyThemeTo(prd);
            prd.DoWork += sender =>
            {
                GStreamer.DownloadGStreamer(((i, s) =>
                {
                    prd.UpdateProgressAndStatus(i, s);
                    if (prd.doWorkArgs.CancelRequested) throw new Exception("User Request");
                }));
            };
            prd.RunBackgroundOperationAsync();

            GStreamer.gstlaunch = GStreamer.LookForGstreamer();
        }
    }
}