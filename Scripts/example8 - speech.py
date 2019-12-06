import sys
import math
import clr
import time
clr.AddReference("MissionPlanner")
import MissionPlanner
clr.AddReference("MissionPlanner.Utilities") # includes the Utilities class

print 'Start Script'

MissionPlanner.MainV2.speechEnable = True

while True:
	print 'speech ...'
	MissionPlanner.MainV2.speechEngine.SpeakAsync("test " + cs.roll.ToString())
	time.sleep(1)



