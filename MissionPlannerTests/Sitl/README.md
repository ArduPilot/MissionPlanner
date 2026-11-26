# SITL tests

`SitlLinkTests` and `SitlDataFlashTests` run MissionPlanner's MAVLink code against an
ArduCopter SITL vehicle. They are in the `SITL` test category and are reported as
inconclusive (skipped) unless `MP_SITL` names a SITL binary, so CI and a plain test run
are not affected.

Each test class starts its own SITL in a new temporary directory, on a free instance
(`-I`), and talks to it over TCP like a GCS. SITL exits when that connection closes. The
directory is deleted afterwards.

## Running them

Either a native SITL binary:

```bat
set MP_SITL=C:\Users\me\Documents\Mission Planner\sitl\ArduCopter.exe
vstest.console.exe MissionPlannerTests.dll /TestCaseFilter:TestCategory=SITL
```

or a Linux build in WSL, from an ArduPilot checkout (`./waf configure --board sitl && ./waf copter`):

```bat
set MP_SITL=wsl:/home/me/ardupilot/build/sitl/bin/arducopter
set MP_SITL_WSL_DISTRO=Ubuntu-24.04
vstest.console.exe MissionPlannerTests.dll /TestCaseFilter:TestCategory=SITL
```

The vehicle's default parameters are found beside the binary (`default_params\copter.parm`,
where Mission Planner keeps them) or in the build tree (`Tools/autotest/default_params`).
`MP_SITL_DEFAULTS` replaces them, as paths the binary can read, comma separated.

## What they cover

- connecting, vehicle identification, `AUTOPILOT_VERSION`, telemetry rates
- the full parameter list by `PARAM_REQUEST_LIST` and by MAVFTP, compared
- setting INT8, INT16, INT32 and REAL32 parameters, and a name that does not exist
- mission, fence and rally upload and download
- mode changes, forced arm and disarm, a refused command
- MAVFTP upload, listing, download, CRC and delete, checked against the SITL's directory
- a tlog recorded from the live link, replayed through `MAVLinkInterface`
- a DataFlash log written by the vehicle: downloaded by `LOG_REQUEST_DATA` and by MAVFTP,
  compared byte for byte with the file SITL wrote, then read by `DFLogBuffer` and converted
  to text

`SitlDataFlashTests` boots with `LOG_DISARMED 1` and waits for a GPS fix, so it takes about a
minute. ArduPilot sends MAVFTP burst reads at a fixed number of packets a second (about 170 in
SITL), so the read speed is set by the packet size: about 13 kB/s with MissionPlanner's default
80 bytes, 38 kB/s with the 239 byte maximum the log test asks for.
