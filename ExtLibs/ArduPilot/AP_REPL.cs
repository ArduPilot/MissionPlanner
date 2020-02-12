using System;
using System.Collections.Generic;
using System.IO;
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

        private SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public EventHandler<string> NewResponse;
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
            _mavint.doCommand(_mavint.MAV.sysid, _mavint.MAV.compid, (MAVLink.MAV_CMD) 42701, 0, 0, 0, 0, 0, 0, 0);
            _timer = new Timer(state =>
            {
                _semaphore.Wait();
                try
                {
                    if (_mavftp.kCmdOpenFileRO("repl/out", out _outsize))
                    {
                        var stream = _mavftp.kCmdReadFile("repl/out", _outsize, null);
                        _mavftp.kCmdTerminateSession();

                        NewResponse?.Invoke(this, ASCIIEncoding.ASCII.GetString(stream.ToArray()));
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }, this, 0, 200);
        }

        public void Stop()
        {
            _timer.Dispose();
            _mavint.doCommand(_mavint.MAV.sysid, _mavint.MAV.compid, (MAVLink.MAV_CMD) 42701, 1, 0, 0, 0, 0, 0, 0);
        }
        
        public void SendLine(string line)
        {
            int createsize = 0;

            _semaphore.Wait();
            try
            {
                if (_mavftp.kCmdCreateFile("repl/in", ref createsize))
                {
                    _mavftp.kCmdWriteFile(new MemoryStream(ASCIIEncoding.ASCII.GetBytes(line)), "REPL", null);
                    _mavftp.kCmdTerminateSession();
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
