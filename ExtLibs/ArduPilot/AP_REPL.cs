using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MissionPlanner.ArduPilot.Mavlink;

namespace MissionPlanner.ArduPilot
{
    public class AP_REPL
    {
        private MAVLinkInterface _mavint;

        private byte _sysid;

        private byte _compid;

        private MAVFtp _mavftp;

        private Timer _timer;

        private int _outsize;
        private int _outskip = 0;

        private SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public EventHandler<string> NewResponse;

        CancellationTokenSource _cancellation = new CancellationTokenSource();

        private bool _active;
        //handle_command_int_packet
        //MAV_CMD_SCRIPTING
        //SCRIPTING_CMD_REPL_START

        public AP_REPL(MAVLinkInterface mavint)
        {
            _mavint = mavint;
            _sysid = _mavint.MAV.sysid;
            _compid = _mavint.MAV.compid;
            _mavftp = new MAVFtp(_mavint, _sysid, _compid);
        }

        public void Start()
        {
            if (_mavint.doCommandInt(_mavint.MAV.sysid, _mavint.MAV.compid, (MAVLink.MAV_CMD) 42701, 0, 0, 0, 0, 0, 0, 0))
            {
                _active = true;
                _timer = new Timer(state =>
                {
                    _semaphore.Wait();
                    try
                    {
                        if (!_active)
                            return;

                        if (!_mavint.BaseStream.IsOpen)
                            return;

                        if (_mavftp.kCmdOpenFileRO("repl/out", out _outsize, new CancellationTokenSource()))
                        {
                            var stream = _mavftp.kCmdReadFile("repl/out", _outsize, _cancellation);

                            _mavftp.kCmdTerminateSession();

                            NewResponse?.Invoke(this,
                                ASCIIEncoding.ASCII.GetString(stream.ToArray().Skip(_outskip).ToArray()));

                            _outskip = (int)stream.Length;
                        }

                        _mavftp.kCmdTerminateSession();
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }, this, 0, 1000);

                SendLine(@"""Hello World!!"""+ "\n");
            }
            else
            {
                NewResponse?.Invoke(this, "Failed to start REPL");
            }
        }

        public void Stop()
        {
            _active = false;
            _timer?.Dispose();
            if (_mavint.doCommandInt(_mavint.MAV.sysid, _mavint.MAV.compid, (MAVLink.MAV_CMD) 42701, 1, 0, 0, 0, 0, 0, 0))
            {

            }
            else
            {
                NewResponse?.Invoke(this, "Failed to stop REPL");
            }
            _cancellation.Cancel();
        }

        public void SendLine(string line)
        {
            SendLine(ASCIIEncoding.ASCII.GetBytes(line));
        }

        private int _sessionwrite = 0;
        public void SendLine(byte[] bytedata)
        {
            int createsize = 0;

            _semaphore.Wait();
            try
            {
                var list =_mavftp.kCmdListDirectory("repl", new CancellationTokenSource());

                var useopen = list.Where(a => a.Name == "in");

                if ((useopen.Count() > 0 && _mavftp.kCmdOpenFileWO("repl/in", ref createsize, new CancellationTokenSource())) || _mavftp.kCmdCreateFile("repl/in", ref createsize, new CancellationTokenSource()))
                {
                    _mavftp.kCmdWriteFile(bytedata, (uint)(useopen.FirstOrDefault()?.Size ?? 0), "REPL", _cancellation);
                    _mavftp.kCmdTerminateSession();
                    _sessionwrite += bytedata.Length;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Write(byte[] bytes, int offset, int length)
        {
            SendLine(bytes.Skip(offset).Take(length).ToArray());
        }
    }
}
