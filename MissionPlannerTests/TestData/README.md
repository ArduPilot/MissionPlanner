# Test data

## copter-sitl.bin, copter-sitl.expected.json

A DataFlash log written by ArduCopter SITL (4.7.0-dev, `LOG_DISARMED 1`, about 25 seconds from
boot to a GPS fix), trimmed to 88 kB: every FMT, FMTU, UNIT, MULT, PARM, MSG, VER, MODE, EV,
ERR and ORGN message is kept, and two in every few GPS, ATT, IMU, BARO, BAT, POWR, MAG and RATE
messages. The JSON is what pymavlink's `DFReader` decodes from the trimmed log: the count of
each message type and every field of the first and last message of each type.
`DataFlashLogTests.ArduCopterLogMatchesPymavlink` checks `DFLogBuffer` against it.

To make them again, from a SITL log:

```sh
python -I MissionPlannerTests/Tools/trim_dataflash_log.py logs/00000001.BIN copter-sitl.bin copter-sitl.expected.json
```

## MAVLink frames

The frames in `Mavlink/MavlinkCodecTests.cs` are packed by pymavlink:

```sh
python -I MissionPlannerTests/Tools/mavlink_golden_frames.py
```
