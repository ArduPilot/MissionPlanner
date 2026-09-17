# MissionPlanner.ArduPilot.SitlTests

Manual end-to-end harness for the MAVLink log download protocol
(`MAVLinkInterface.GetLog`) against a real ArduPilot SITL
(Software In The Loop) simulated vehicle. Not part of any automated test
run - it needs SITL running and is driven by hand.

What it verifies:

- log listing (`GetLogEntry`) against the vehicle
- full download, timed, with the byte count checked against `LOG_ENTRY.size`
- optional byte-for-byte comparison against an oracle file (the log SITL wrote
  to its own disk)
- optional mid-download cancellation, including that the link stays usable
  afterwards (`LOG_REQUEST_END` behavior)

## One-time SITL setup (WSL Ubuntu)

```bash
mkdir -p ~/sitl && cd ~/sitl
curl -fsSL -o arducopter https://firmware.ardupilot.org/Copter/stable/SITL_x86_64_linux_gnu/arducopter
curl -fsSL -o copter.parm https://raw.githubusercontent.com/ArduPilot/ardupilot/master/Tools/autotest/default_params/copter.parm
chmod +x arducopter
```

Generate a static dataflash log (boot once with disarmed logging, then exit so
the file is closed; SITL waits for a TCP client before booting, hence the
python one-liner):

```bash
cd ~/sitl
printf "LOG_DISARMED 1\n" > extra.parm
(python3 -c "import socket,time; time.sleep(3); s=socket.create_connection(('127.0.0.1',5760)); s.settimeout(35); end=time.time()+30
while time.time()<end:
    try:
        if not s.recv(4096): break
    except Exception: break" &)
timeout 35 ./arducopter --model + --speedup 1 -w --defaults copter.parm,extra.parm --home -35.363262,149.165237,584,353
ls -la logs/   # 00000001.BIN is the oracle file
```

## Running

Start the vehicle (it exits when the TCP client disconnects, so restart it for
each harness run):

```bash
cd ~/sitl && ./arducopter --model + --speedup 1 --defaults copter.parm --home -35.363262,149.165237,584,353
```

Copy the oracle out of WSL (Git Bash mangles `\\wsl$` paths, so copy first):

```bash
cp '//wsl$/Ubuntu/home/<user>/sitl/logs/00000001.BIN' /tmp/oracle.bin
```

Run the harness (from `bin/Debug/net472` after `dotnet build`):

```
SitlLogTest.exe <host> <port> [oraclePath] [cancel]

MissionPlanner.ArduPilot.SitlTests.exe 127.0.0.1 5760 C:\path\to\oracle.bin cancel
```

`cancel` also runs the mid-download cancellation check.

## Lossy-link run

`lossy_proxy.py` sits between the harness and SITL and drops every 20th
LOG_DATA frame (~5% loss), forcing the fill-in phase to recover the gaps.
In WSL, with SITL already listening on 5760:

```bash
python3 lossy_proxy.py    # listens on 5770, forwards to 127.0.0.1:5760
```

Then point the harness at port 5770. The download is expected to be slower
(one round-trip per scattered missing block) but still byte-identical to the
oracle. The proxy prints how many frames it saw and dropped on exit.

## Reference results (2026-08-29, 2.3 MB log, WSL2 loopback)

- clean link: 2,306,048 bytes in ~0.45 s (~5 MiB/s), byte-identical
- 5% LOG_DATA loss: byte-identical in ~58 s, one streaming pass
  (1,348 of 26,971 frames dropped; the extra ~3 s over the earlier ~55 s
  reference is one silence window confirming the end of log, whose packet
  arrives far past a frontier stalled at the first dropped block)

The lossy run is the reason this harness exists: a 5% loss stalls the
contiguous frontier near the start of the log, so the genuine end packet
of a large log arrives far past any near-frontier trust window - a case
the unit tests cannot represent because their logs are smaller than the
window itself. A fix that rejected such end packets outright passed the
whole unit suite and never completed here: every recovered gap forced the
vehicle to re-stream the entire log.
