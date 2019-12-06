import sys
import math
import clr
import time
clr.AddReference("MissionPlanner")
import MissionPlanner
clr.AddReference("MissionPlanner.Utilities") # includes the Utilities class

print 'Start Script'

MissionPlanner.MainV2.instance.FlightPlanner.BUT_read_Click(MissionPlanner.MainV2.instance.FlightPlanner,null)

