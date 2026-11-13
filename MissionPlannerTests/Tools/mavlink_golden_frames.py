"""Print the MAVLink frames MavlinkCodecTests checks MissionPlanner against, packed by pymavlink.

    python -I mavlink_golden_frames.py
"""
import os
os.environ['MAVLINK20'] = '1'
from pymavlink.dialects.v20 import ardupilotmega as m2
from pymavlink.dialects.v10 import ardupilotmega as m1


def hx(b):
    return ''.join('%02X' % x for x in b)


class Sink:
    def write(self, b):
        pass


mav = m2.MAVLink(Sink(), srcSystem=1, srcComponent=1)
mav.seq = 5
print('V2Heartbeat', hx(m2.MAVLink_heartbeat_message(2, 3, 81, 4, 3, 3).pack(mav)))
mav.seq = 6
print('V2ParamValue', hx(m2.MAVLink_param_value_message(b'RTL_ALT', 1500.0, 9, 1200, 42).pack(mav)))
mav.seq = 7
# the zero tail is trimmed
print('V2CommandLong', hx(m2.MAVLink_command_long_message(1, 1, 400, 0, 1, 0, 0, 0, 0, 0, 0).pack(mav)))
mav.seq = 8
# mission_type is an extension field
print('V2MissionCount', hx(m2.MAVLink_mission_count_message(1, 1, 7, 2).pack(mav)))
mav.seq = 9
print('V2GpsRawInt', hx(m2.MAVLink_gps_raw_int_message(123456789, 3, -353632610, 1491652300, 584000, 121, 65535, 0,
                                                       65535, 10, 600000, 1000, 2000, 3000, 4000, 35999).pack(mav)))
mav.seq = 10
print('V2StatusText', hx(m2.MAVLink_statustext_message(6, b'ArduCopter V4.7.0-dev', 0, 0).pack(mav)))

mav1 = m1.MAVLink(Sink(), srcSystem=1, srcComponent=1)
mav1.seq = 11
print('V1Heartbeat', hx(m1.MAVLink_heartbeat_message(2, 3, 81, 4, 3, 3).pack(mav1)))
mav1.seq = 12
print('V1Attitude', hx(m1.MAVLink_attitude_message(1000, 0.1, -0.2, 3.0, 0.01, 0.02, -0.03).pack(mav1)))

# signed with a fixed key and timestamp
signed = m2.MAVLink(Sink(), srcSystem=1, srcComponent=1)
signed.seq = 13
signed.signing.secret_key = bytes(range(32))
signed.signing.sign_outgoing = True
signed.signing.link_id = 3
signed.signing.timestamp = 0x0102030405
print('V2SignedHeartbeat', hx(m2.MAVLink_heartbeat_message(2, 3, 81, 4, 3, 3).pack(signed)))
