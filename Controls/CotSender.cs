using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;
using MissionPlanner.Utilities.CoT;

namespace MissionPlanner.Controls
{
    /// <summary>A row of the window's grid: what the user called one vehicle.</summary>
    public class CotRow
    {
        public string SysId;
        public string Uid;
        public bool Takv;
        public string Callsign;
        public string Endpoint;
        public string Vmf;
    }

    /// <summary>A copy of the window's settings for the sending thread, which never reads a control.</summary>
    public sealed class CotSettings
    {
        public string Type = "a-f-A-M-F-Q";
        public bool Details = true;
        public bool Indent = true;
        public double IntervalSeconds = 10;
        public IList<CotRow> Rows = new List<CotRow>();

        public CotIdentity IdentityFor(CotVehicle vehicle)
        {
            var row = Rows.FirstOrDefault(r => r.SysId == vehicle.SysId.ToString());
            var id = new CotIdentity
                { Uid = row == null || string.IsNullOrEmpty(row.Uid) ? "NOsysid" + vehicle.SysId : row.Uid };
            if (row != null)
            {
                id.Takv = row.Takv;
                id.Callsign = row.Callsign;
                id.Endpoint = row.Endpoint;
                id.Vmf = row.Vmf;
            }

            return id;
        }
    }

    /// <summary>
    /// Sends the Cursor on Target messages on its own thread. One runs at a time, and it outlives the
    /// window, which only shows it and hands it new settings. Each run has its own cancellation, so a
    /// stopped run cannot come back when the next one starts.
    /// </summary>
    public sealed class CotSender
    {
        static readonly ILog log = LogManager.GetLogger(typeof(CotSender));
        static readonly object startStop = new object();
        static CotSender current;

        readonly ICotTransport transport;
        readonly Func<IEnumerable<CotVehicle>> vehicles;
        readonly CancellationTokenSource cancel = new CancellationTokenSource();
        Thread thread;
        volatile CotSettings settings;

        /// <summary>The sender that is running, or null.</summary>
        public static CotSender Current
        {
            get { return Volatile.Read(ref current); }
        }

        public CotSettings Settings
        {
            get { return settings; }
            set
            {
                if (value != null)
                    settings = value;
            }
        }

        public string Status { get; private set; }
        public string LastRound { get; private set; } = "";
        public int Sent { get; private set; }

        /// <summary>After each round and when the run ends; raised on the sending thread.</summary>
        public event EventHandler Changed;

        CotSender(ICotTransport transport, CotSettings settings, Func<IEnumerable<CotVehicle>> vehicles)
        {
            this.transport = transport;
            this.settings = settings;
            this.vehicles = vehicles;
            Status = Strings.CotStarting;
        }

        /// <summary>Stops the sender that is running, waits for it, then starts this one.</summary>
        public static CotSender Start(ICotTransport transport, CotSettings settings,
            Func<IEnumerable<CotVehicle>> vehicles = null)
        {
            lock (startStop)
            {
                var before = Current;
                if (before != null)
                    before.Stop();

                var sender = new CotSender(transport, settings, vehicles ?? AllVehicles);
                Volatile.Write(ref current, sender);
                sender.thread = new Thread(sender.Run) { IsBackground = true, Name = "CoT output" };
                sender.thread.Start();
                return sender;
            }
        }

        /// <summary>Ends the run: the tracks it reported are sent once more, already stale, and the transport closes.</summary>
        public void Stop()
        {
            lock (startStop)
            {
                cancel.Cancel();
                if (thread != null && thread != Thread.CurrentThread)
                    thread.Join(TimeSpan.FromSeconds(10));
                Interlocked.CompareExchange(ref current, null, this);
            }
        }

        /// <summary>Each vehicle from the connection it was heard on.</summary>
        static IEnumerable<CotVehicle> AllVehicles()
        {
            return MainV2.Comports.ToArray()
                .SelectMany(port => port.MAVlist.Select(mav => CotVehicles.Read(mav, port)))
                .ToList();
        }

        void Run()
        {
            try
            {
                transport.Open();
            }
            catch (Exception ex)
            {
                log.Warn("CoT output could not start", ex);
                Status = string.Format(Strings.CotCouldNotStart, CotNetwork.Reason(ex));
                Interlocked.CompareExchange(ref current, null, this);
                transport.Dispose();
                OnChanged();
                return;
            }

            var round = new CotRound();
            while (!cancel.IsCancellationRequested)
            {
                var now = settings;
                try
                {
                    var events = round.Build(vehicles(), now.IdentityFor, now.Type, now.Details, DateTime.UtcNow,
                        CotVehicles.StaleSeconds(now.IntervalSeconds));

                    var shown = new StringBuilder();
                    foreach (var cotevent in events)
                    {
                        var xml = CotXml.Serialize(cotevent, now.Indent);
                        shown.Append(xml).Append("\r\n");
                        if (Send(xml))
                            Sent++;
                    }

                    LastRound = shown.ToString();
                    Status = string.Format(Strings.CotSentSoFar, transport.Status, Sent);
                }
                catch (Exception ex)
                {
                    log.Warn("CoT output round failed", ex);
                    Status = string.Format(Strings.CotSendFailed, CotNetwork.Reason(ex));
                }

                OnChanged();
                cancel.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(Math.Max(1, now.IntervalSeconds)));
            }

            // stopped: TAK greys the tracks out now instead of at their stale time
            try
            {
                foreach (var cotevent in round.ExpireAll(DateTime.UtcNow))
                    Send(CotXml.Serialize(cotevent, settings.Indent));
            }
            catch (Exception ex)
            {
                log.Warn("CoT output could not expire its tracks", ex);
            }

            transport.Dispose();
            Status = Strings.CotStopped;
            OnChanged();
        }

        /// <summary>The message as sent: UTF-8, without carriage returns, ending in a line feed.</summary>
        bool Send(string xml)
        {
            return transport.Send(Encoding.UTF8.GetBytes(xml.Replace("\r", "") + "\n"));
        }

        void OnChanged()
        {
            try
            {
                Changed?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                log.Warn(ex);
            }
        }
    }
}
