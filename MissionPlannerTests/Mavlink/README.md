# 32-bit system ID tests

`Sysid32Tests` is an MSTest class in `MissionPlannerTests.csproj`. It covers
independent wire fixtures for all eight flag combinations, unsigned boundary
IDs, CRC/signatures, fragmented reads, rejection/recovery, payload broadcasts,
legacy MAVLink 1, tlog playback, vehicle/inspector/subscription isolation, and
live-link target filtering. Run it through Test Explorer or filter
`vstest.console.exe` with `/TestCaseFilter:FullyQualifiedName~Sysid32Tests`.

The `SYSID32` flag widens only the source system ID. `TARGET32` independently
adds a four-byte target system ID, and is sent only when
a message has a payload target field and its destination exceeds 255. Header
lengths are 10, 13, 14 and 17 bytes; there is no capability advertisement.
MissionPlanner's system-ID APIs use `uint`; compound system/component keys use
`ulong`. Generated payload structs retain their original byte fields.
Pass the full destination ID to `sendPacket`/`generatePacket`. Small targets and
broadcast use payload fields, and messages without a target field remain
untargeted. The standalone packet generator accepts `targetSystem` and
`targetComponent` to supply the full destination for a target-bearing message;
it rejects an explicit target for messages without that field.

## Linux GUI/SITL integration

Use an isolated checkout combining this branch with `pr-linux-flightdata-layout`
and run `Linux/build.sh`. The Linux build commits are test infrastructure and
are not part of this feature branch.

Start a **disposable** Rover SITL from the 32-bit system-ID ArduPilot branch. The
test changes and restores `CRUISE_SPEED`, replaces the mission with two waypoints,
and selects HOLD. It connects only to localhost TCP port 6060 and uses an isolated
MissionPlanner settings directory. It does not arm the rover.

In a new temporary SITL directory:

```sh
printf 'MAV_SYSID -1\n' > override.parm
/path/to/ardupilot/build/sitl/bin/ardurover --model rover --speedup 5 \
    --base-port 6060 --serial1=tcp:100 \
    --home=-35.362938,149.165085,584,270 \
    --defaults /path/to/ardupilot/Tools/autotest/default_params/rover.parm,override.parm
```

`-1` is the AP_Int32 storage pattern for unsigned ID `4294967295`. The SITL
`--sysid` command-line option currently only accepts 8-bit IDs.

Start Xephyr on an unused display, then run:

```sh
Xephyr :102 -screen 1400x1000 -ac -noreset
DISPLAY=:102 MissionPlannerTests/Mavlink/run-sysid32-sitl.sh \
    /path/to/linux-checkout/bin/linux/MissionPlanner-linux-x86_64
```

The harness uses GCS ID `2147483649`, connects the actual FlightData GUI to vehicle
`4294967295`, reads/writes a parameter, downloads and decodes parameters over
MAVFTP (asserting no fallback), uploads/downloads a mission, checks a command
acknowledgement, and verifies the unsigned ID displayed in the vehicle selector.
It prints `MISSIONPLANNER_SYSID32_SITL_PASS`, keeps the window open for 20 seconds,
and exits. Stop the test SITL and Xephyr processes afterwards.

The GUI harness also exercises the Device Op ID control and the safety-toggle
button on this disposable SITL. Repeat with `MAV_SYSID 256` in `override.parm`
and `SYSID32_SITL_ID=256` in the harness environment to cover a zero low byte.
The protocol regression tests run in the release CI job via `dotnet test`.
