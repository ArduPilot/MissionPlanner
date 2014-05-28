import sys
import math
import clr
import time
clr.AddReference("MissionPlanner")
import MissionPlanner
clr.AddReference("MissionPlanner.Utilities") # includes the Utilities class
from MissionPlanner.Utilities import Locationwp

def gps_distance(lat1, lon1, lat2, lon2):
	'''return distance between two points in meters,
	coordinates are in degrees
	thanks to http://www.movable-type.co.uk/scripts/latlong.html'''
	radius_of_earth = 6378100.0

	from math import radians, cos, sin, sqrt, atan2
	lat1 = radians(lat1)
	lat2 = radians(lat2)
	lon1 = radians(lon1)
	lon2 = radians(lon2)
	dLat = lat2 - lat1
	dLon = lon2 - lon1

	a = sin(0.5*dLat)**2 + sin(0.5*dLon)**2 * cos(lat1) * cos(lat2)
	c = 2.0 * atan2(sqrt(a), sqrt(1.0-a))
	return radius_of_earth * c

print __name__

# main program
print "Start script"
######Mission variables######
dist_tolerance = 15 #(m)
ber_tolerance = 45 #heading tolerance
waypoint = 1 #desired Waypoint
######Time delays (ms)######
servo_delay = 50 #To be experimentally found 
comm_delay = 50 #To be experimentally found 
######Other constants######
payload_servo = 7 #5-8
gravity = 9.81

target = (-35, 117.98) # gps pos of target in degrees

time.sleep(5) # wait 10 seconds before starting
print 'Starting Mission'
Script.ChangeMode("Guided") # changes mode to "Guided"
item = MissionPlanner.Utilities.Locationwp() # creating waypoint

alt = 60.000000 # altitude value
Locationwp.lat.SetValue(item,target[0]) # sets latitude
Locationwp.lng.SetValue(item,target[1]) # sets longitude
Locationwp.alt.SetValue(item,alt) # sets altitude
print 'Drop zone set'
MAV.setGuidedModeWP(item) # tells UAV "go to" the set lat/long @ alt
print 'Going to DZ'
Good = True
while Good == True:
	ground_speed = cs.groundspeed
	alt = cs.alt
	wp_dist = gps_distance(cs.lat ,cs.lng, math.radians(target[0]), math.radians(target[1]))
	print wp_dist 
	ber_error = cs.ber_error
	fall_time = ((2 * alt) / gravity) ** (0.5)
	fall_dist = ground_speed * fall_time
	release_time = fall_time + (servo_delay/1000) + (comm_delay/1000)
	release_dist = release_time * ground_speed
	if (math.fabs(release_dist - wp_dist) <= dist_tolerance):
		if (math.fabs(ber_error) <= ber_tolerance):
			######Payload Release######
			Script.SendRC(payload_servo,1900,True)
			print 'Bombs away!'
		else: 
			print 'Heading outside of threshold, go around!'
			Good = False 
	else: 
		print 'Outside of threshold!'
		time.sleep (1.0) #sleep for a second
	#Broken out of the loop as Bearing was not right
	print 'Bearing was out of tolerance for the Drop - Start run again' 
