
import clr
import MissionPlanner
clr.AddReference("MAVLink")
from System import Byte
from System import Array
import MAVLink

print 'Start Script'

key = Array[Byte]([0x13, 0x00, 0x00, 0x00, 0x08, 0x00])

MAV.BaseStream.Write(key,0,len(key))