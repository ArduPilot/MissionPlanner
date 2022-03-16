import clr
import MissionPlanner
clr.AddReference("MAVLink")
from System import Func, Action
import MAVLink

def OtherMethod(message):
    print "got HB";
    return True

def MyMethod(message):
    print("STATUSTEXT from MAV.SubscribeToPacketType")
    print(message.data.text)
    return True

def MyPacketHandler(o, message):
    try:
        if message.msgid == MAVLink.MAVLINK_MSG_ID.STATUSTEXT.value__:
            print "STATUSTEXT from MyPacketHandler " + str(message.sysid) + " " + str(message.compid)
            print dir(message)
            print(bytes(message.data.text))
    except Exception as inst:
        print inst

sub = MAV.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.HEARTBEAT.value__, Func[MAVLink.MAVLinkMessage, bool] (OtherMethod))
sub2 = MAV.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.STATUSTEXT, Func[MAVLink.MAVLinkMessage,
                                 bool] (MyMethod))
sub3 = MAV.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.STATUSTEXT_LONG, Func[MAVLink.MAVLinkMessage,
                                 bool] (MyMethod))

MAV.OnPacketReceived += MyPacketHandler

import time
while True:
    time.sleep(1)