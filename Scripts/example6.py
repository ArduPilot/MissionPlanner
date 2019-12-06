# from http://diydrones.com/forum/topics/mission-planner-python-script?commentId=705844%3AComment%3A2035437&xg_source=msg_com_forum

import socket
import sys
import math
from math import sqrt
import clr
import time
import re, string
clr.AddReference("MissionPlanner.Utilities")
import MissionPlanner #import *
clr.AddReference("MissionPlanner.Utilities") #includes the Utilities class
from MissionPlanner.Utilities import Locationwp


HOST = 'localhost'   # Symbolic name meaning all available interfaces
#SPORT = 5000 # Arbitrary non-privileged port  
RPORT = 4000 # Arbitrary non-privileged port

REMOTE = ''
# Datagram (udp) socket 
 

rsock = socket.socket(socket.AF_INET,socket.SOCK_DGRAM)
print 'Sockets created'

# Bind socket to local host and port 
try:     
   rsock.bind((HOST,RPORT)) 
except socket.error, msg:
   #print 'Bind failed. Error Code:'
   sys.stderr.write("[ERROR] %s\n" % msg[1])
   rsock.close()
   sys.exit()       

print 'Receive Socket bind complete on ' + str(RPORT)

print 'Starting Follow'
Script.ChangeMode("Guided")                     # changes mode to "Guided"
print 'Guided Mode'

#keep talking with the Mission Planner server 
while 1:     

    msg = rsock.recv(1024)
    pattern = re.compile("[ ]")
    parameters = pattern.split(msg)

    latData = parameters[0]
    lngData = parameters[1]
    headingData = parameters[2]
    altData = parameters[3]
    
    float_lat = float(latData)
    float_lng = float(lngData)
    float_heading = float(headingData)
    float_alt = float(altData)


    """Safety Manual Mode Switch"""
    #while True:
       
    if cs.mode == 'MANUAL':
       Script.ChangeMode("Manual")
       rsock.close()
    else:
       
       #print cs.mode
       """Follower Offset"""
       XOffset= float(0) #User Input for x axis offset
       YOffset= float(-2) #User Input for y axis offset
       brng = math.radians(float_heading)
      # brng = float_heading*math.pi/180 #User input heading angle of follower in relation to leader.  0 degrees is forward.

       d = math.sqrt((XOffset**2)+(YOffset**2)) #Distance in m

       MperLat = 69.172*1609.34 #meters per degree of latitude. Length of degree (miles) at equator * meters in a mile
       MperLong = math.cos(float_lat)*69.172*1609.34 #meters per degree of longitude

       Lat_Offset_meters = YOffset/MperLat #lat distance offset in meters
       Long_Offset_meters = XOffset/MperLong #long distance offset in meters

       Follower_lat = float_lat + (Long_Offset_meters*math.sin(brng)) + (Lat_Offset_meters*math.cos(brng)) #rotates lat follower offset in relation to heading of leader
       Follower_long = float_lng - (Long_Offset_meters*math.cos(brng)) + (Lat_Offset_meters*math.sin(brng)) #rotates long follower offset in relation to heading of leader
       Follower_alt = float_alt + 10
       #Follower_alt = 10
   
       float_lat = float(Follower_lat)
       float_lng = float(Follower_long)
       float_alt = float(Follower_alt) #4-5 second lag induced on altitude waypoint line, unless alt is set to 0
    
       print(float_lat)
       print(float_lng)
       print(float_heading)
       print(float_alt)

       """Writing Waypoints"""
       item = MissionPlanner.Utilities.Locationwp() # creating waypoint
       MissionPlanner.Utilities.Locationwp.lat.SetValue(item,float_lat)
       MissionPlanner.Utilities.Locationwp.lng.SetValue(item,float_lng)
       #MissionPlanner.Utilities.Locationwp.groundcourse.SetValue(item,float_heading)
       MissionPlanner.Utilities.Locationwp.alt.SetValue(item,float_alt) #Can only use lat,lng, or alt
       MAV.setGuidedModeWP(item) #set waypoint
       print 'Waypoint Sent'
       print time.strftime('%X %x %Z')
# exit
rsock.close()
print 'Script End'
