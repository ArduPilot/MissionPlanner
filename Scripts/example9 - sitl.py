import os
import sys
import math
import clr
import time
clr.AddReference("MissionPlanner")
import MissionPlanner
clr.AddReference("MissionPlanner.Utilities") # includes the Utilities class
clr.AddReference("MissionPlanner.Comms")
clr.AddReference("System")
import MissionPlanner.Comms
import System

from System.Diagnostics import Process

for i in range(20):
	workdir = 'C:\Users\michael\Documents\Mission Planner\sitl\d' + str(i)
	if not os.path.exists(workdir):
		os.makedirs(workdir)
	proc = Process()
	proc.StartInfo.WorkingDirectory = workdir
	proc.StartInfo.FileName ='C:\Users\michael\Documents\Mission Planner\sitl\ArduCopter.exe'
	proc.StartInfo.Arguments	= ' -M+ -s1 --uartA tcp:0 --defaults ..\default_params\copter.parm --instance ' + str(i) + ' --home -35.363261,'+ str(149.165330 + 0.000001 * i) +',584,353'
	proc.Start()

	port = MissionPlanner.Comms.TcpSerial();
	port.client = System.Net.Sockets.TcpClient("127.0.0.1", 5760 + 10 * i);

	mav = MissionPlanner.MAVLinkInterface();
	mav.BaseStream = port;
	mav.getHeartBeat()
	#MissionPlanner.MainV2.instance.doConnect(mav, "preset", "0");
	MissionPlanner.MainV2.Comports.Add(mav);