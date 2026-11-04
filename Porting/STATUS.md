# Avalonia in-place migration status

Updated: **2026-11-04**.

## MAVLink serial throughput, DroneCAN firmware update over MAVLink CAN — 2026-11-04

- Branch `fix/mavlink-serial-throughput` from `master` `a13832b69`: `58e1cf56b` (buffered
  serial reads), `9862e933c` (DroneCAN swapped-frame reassembly, bootloader verdict, page
  status), `082e2e6dd` (opt-in live firmware trace). Full Release tests **1920/1920**, none
  skipped; `MissionPlanner.slnx` Release build **0 warnings / 0 errors**; six
  `build/porting/check-*.sh` audits and `git diff --check` pass.
- User report: the Matek L431 on the X7's CAN2 would not take a firmware update ("nothing
  happens"). Found on the bench: the first attempt had erased the node's application and left
  it in the bootloader (NodeStatus MAINTENANCE, vendor code 10 = NO_APP_SIG); later attempts
  wrote an image that failed its CRC (code 13).
- Root cause (measured, not inferred): on Linux `System.IO.Ports` costs ~1.1 ms per `Read`
  regardless of size (IO-loop thread), the MAVLink reader did three reads per packet, so this
  application parsed at most ~305 packets/s (~8.8 KB/s) with 4 KB permanently queued. The X7
  (ArduPlane 4.7.2, `SCHED_LOOP_RATE=300`) forwards ~350 CAN frames/s (ESC RawCommand at loop
  rate) plus telemetry = ~410 packets/s. The autopilot's MAVLink buffer stayed full: ~30 % of
  forwarded CAN frames were dropped and everything arrived ~600 ms late. The AP_Periph
  bootloader matches file-read answers by 5-bit transfer id only, so answers that arrived
  seconds late were accepted against later requests and wrong blocks were written. A second,
  smaller effect: the bootloader emits adjacent frames of its own transfers swapped under bus
  load, which the reassembler used to drop ("Bad Toggle").
- Fixes: `WinSerialPort` serves reads from a 16 KB buffer refilled by one base read (410
  packets/s received with an empty buffer); `DroneCAN.ProcessFrame` holds a frame that arrives
  ahead of its predecessor and validates by CRC; `DroneCAN.Update` waits for the bootloader's
  verdict and throws with the `check_fw_result_t` reason, returns whether an image was sent;
  the DroneCAN page shows the furthest block and distinguishes "installed, node restarted"
  from "already runs this image".
- Live acceptance (X7 + L431, MAVLink-CAN2, USB): stable 4.7.2 installed through "Check for
  Updates → Install" in 38 s (5.5 KB/s, 835 blocks each requested once, 170 ms per pipeline
  round trip, MAVLink loss 0); dev 4.8.0 then stable again through the same path, each 38 s,
  node restarts into the new image (`sw 4.7.FB773567`, OPERATIONAL). Before the fix the same
  transfer ran at 0.3–0.6 KB/s with 1.5 answers per block and produced a corrupt image. The
  node is left on stable 4.7.2. No autopilot parameter was changed.
- Diagnostics kept as opt-in tests: `DroneCanLiveFirmwareDiagnosticTests` (MP_LIVE_CAN_PORT,
  _BUS, _NODE, _FLASH, _CHANNEL, _FILE, _BURST, _TRACE) and `SerialPortBenchDiagnosticTests`.
  Traces of this session: `/tmp/claude-1000/-home-alex-SRC-MP-MissionPlanner/4e8a036b-b671-440c-a630-37d2e7dfd5d0/scratchpad/flash*.log`.
- Not claimed: Windows/macOS serial behaviour of the buffer (the code path is shared; the
  Windows System.IO.Ports read is synchronous so the buffer is only a small saving there);
  hosted CI for the branch; GUI checks of the page texts.
- Next executable step: open the PR, let CI run, merge only when authorized. Remaining
  idea, not started: the MAVLink reader could read whole packets instead of byte/header/payload
  now that the port is buffered, and the DroneCAN page could warn when forwarded CAN traffic
  approaches the link's capacity.

## Release 1.3.83.6 — DroneCAN, firmware catalogs, MCP DroneCAN tools, HUD — 2026-11-01

- Merged on `origin/master` with merge commits, branch history kept: PR #46 (`3348d5014`,
  DroneCAN over MAVLink CAN, X-Office-style agent window), PR #47 (`56b67bdfa`, DroneCAN and
  autopilot firmware catalogs, MCP DroneCAN tools, connect-dialog, Install Firmware and HUD
  fixes), PR #41 (`9fae128d1`, MCP session startup / Disconnect under load / access controls;
  its September macOS failure was a VLC download timeout, master was merged into the branch
  with a one-blank-line conflict), PR #48 (`aa4b05ee0`, lifecycle test accepts a raw socket
  reset), PR #49 (`65d028a19`, `dronecan_node_command` validates the command name before the
  vehicle-state checks; this was the real cause of a CI-only `McpDroneCanTests` failure).
- The single-purpose `build: bump local build number` commit `7a445a826` changed only
  `build/local-build-number.txt` from `5` to `6`. Annotated tag `v1.3.83.6-7a445a82` is on
  that commit. Release run `41335568622` passed version validation, Linux, Windows, both
  macOS builds and publishing; CI run on the commit passed. The non-prerelease GitHub Release
  https://github.com/Rouniy/MissionPlanner10/releases/tag/v1.3.83.6-7a445a82 has 19 assets
  (eight human packages, two updater ZIPs, four manifests, four signatures, `SHA256SUMS`).
- Local verification on `65d028a19` before the bump: `MissionPlanner.slnx` Release build
  0 errors; full Release tests **1909/1909**, none skipped; six `build/porting/check-*.sh`
  audits pass; `out/packages/missionplanner10_1.3.83.5-65d028a1_amd64.deb` built locally
  (needed a user-local rustup for `libdflog_ffi.so`), `lintian` clean, extracted payload
  complete, 12 s Xvfb launch without exceptions.
- CI flakes seen once each and green on rerun, not fixed: `MavlinkSerialTcpBridgeTests.
  Loopback_bridge…` (expected 7 bytes, got 0) on the PR #41 branch.
- This release record is a post-tag documentation commit and does not change released
  binaries. Next executable step: qualify the published packages on their target platforms;
  the user is to confirm the HUD now follows the autopilot without "turn, pause, turn".

## DroneCAN firmware catalog, MCP node tools — 2026-10-30

- Branch `feat/dronecan-firmware-mcp` is stacked on `fix/dronecan-mavlink-session` at
  `bb9278a7f` (PR #46, still open against `master`). Commits: `f3d291bbc` (library node-info
  sweep only asks unknown nodes; a NodeStatus with lower uptime forgets stale NodeInfo),
  `ea6f88130` ("Check for Updates…" on the DroneCAN page: manifest lookup by node name,
  stable/beta/dev list with dates and commits, operator picks a build, download to temp,
  flash through the existing updater; autopilot node labelled on an empty port),
  `f9f9ca522` (MCP tools `dronecan_nodes`, `dronecan_read_parameters`,
  `dronecan_write_parameters`, `dronecan_node_command` with disarmed/read-only/sensitive
  checks, read-back verification and audit), `117890827` (live acceptance covers the lookup,
  the stable image's app descriptor and the MCP paths).
- Live acceptance on the CUAV X7 + Matek L431 (node 125, AP_Periph 1.7 `9c6f307f`, hw 4.38):
  firmware check 4.7 s listing stable 4.7.2, beta 4.7.2 and 4.8.0-dev with dates; stable image
  212784 bytes with the signed app descriptor; `dronecan_nodes` 1.6 s; 85 parameters read in
  3.2 s; `NTF_LED_BRIGHT` 3→2→3 written and verified through the agent path (~7 s each, not
  saved). **The node was not flashed**; the user decides in the new dialog.
- Flake fixed: `Node_status_from_an_unnamed_node_requests_its_node_info_immediately` doubled
  its request when the library's ten-second sweep coincided with the test; the sweep now skips
  nodes whose info is known.
- Verification: full Release tests **1871/1871**, no skipped; `MissionPlanner.slnx` Release
  build **0 warnings / 0 errors**; six `build/porting/check-*.sh` audits and `git diff --check`
  pass. Builds ran with `-m:1`.
- Not claimed: hosted CI for this branch; flashing a node through the new dialog on hardware
  (the download and the updater call are the same code path the live test and the existing
  "Update Firmware…" button use); Windows/macOS GUI checks.
- Connect dialog (user report, 2026-10-31): closing "Connecting Mavlink" with the title-bar
  button during parameter loading cancelled the read while the autopilot kept streaming, so the
  toolbar progress under Connect froze. Closing now only hides the dialog
  (`ProgressReporter.ContinueInBackgroundOnClose`); "Skip Parameters" still cancels. Checked on
  the X7 under Xvfb+Metacity: after the close the toolbar kept showing "Got param …" with a
  growing bar and ended at "Connected. 944 params.".
- Install Firmware (user report, 2026-10-31): the page never rebooted a connected board into
  its bootloader (type check against System.IO.Ports.SerialPort), the checksum step was
  inverted so a different image was reported as "already present", and only one build per
  vehicle was offered. Now `AutopilotFirmwareCatalog` lists every manifest build for the
  board id from AUTOPILOT_VERSION (`CurrentState.board_version`, hwdef name from the banner),
  `AutopilotFirmwareDialog` lets the operator pick, and the upload goes reboot → bootloader
  scan → checksum → erase/program/verify. The Setup backstage no longer hides
  vehicle-free pages behind the parameter overlay when an idle listener becomes active.
  Verified on the X7: 4.7.0-dev → stable 4.7.2 flashed in 51 s, 933 parameters preserved;
  a second run reported the identical image. Full Release tests **1900/1900**.
- HUD horizon (user report, 2026-10-31, "turns, pauses, turns" when the autopilot is rotated
  by hand): the cause was `HudControl`'s own easing, which chased every ATTITUDE sample
  exponentially (40 % per 16 ms frame, then a pause until the next packet). The user wants the
  newest value drawn at once and accepts discrete 4 Hz steps, so the easing, its timer and
  `SnapToValues` are gone and Render draws the bound values; `FlightDataViewModel` copies
  roll/pitch/yaw every 20 ms (50 ms on SlowMachine) instead of on the 100 ms panel timer. An
  intermediate commit (`dc4e66cbb`) had raised the default stream to 10 Hz and added a
  time-based tween; both were reverted on the user's instruction, the stream default is the
  upstream 4 Hz again. Full Release tests **1904/1904**.
- Draft PR #47 (base `fix/dronecan-mavlink-session`): https://github.com/Rouniy/MissionPlanner10/pull/47
- Next executable step: watch CI on PR #47; once PR #46 merges, retarget this
  PR to `master`. If the user wants the L431 on 4.7.2, select node 125 on MAVLink-CAN2,
  Check for Updates…, choose stable, Download and flash. Merge only when authorized.

## DroneCAN over MAVLink CAN, AI agent window — 2026-10-30

- Branch `fix/dronecan-mavlink-session` is based on `origin/master` at
  `bdf6a5ac5` ("fix: accept MAV_CMD ids in MCP mavlink_command"). Commits: `8c1d5c4a0`
  (SkiaSharp PNGs, ImageSharp removed), `d897bc770` (no CAN echo, one shared forwarding
  session), `459d02026` (opt-in live CAN acceptance), `22df27980` (X-Office-style agent
  rows, case-insensitive desktop discovery), `9f47848f7` (re-adopt the Codex desktop entry).
- Hardware finding (CUAV X7 running ArduRover 4.7.0-dev `eb9a1b3c`, Matek L431 AP_Periph
  1.7 as node 125, autopilot node 10): the adapter is wired to **CAN2**. `@SYS/can0_stats.txt`
  showed zero successful transmissions and only timeouts, `can1_stats.txt` carried all
  traffic. Listening on MAVLink-CAN1 therefore showed nothing, and Mission Planner gave no
  hint. The L431's red LED is power and the blinking blue LED is AP_Periph running:
  the firmware is healthy, **no firmware was flashed**.
- Software root causes fixed in `ExtLibs/DroneCAN/DroneCAN.cs`, new
  `ViewModels/MavlinkCanTransport.cs`, `ViewModels/DroneCanBridge.cs` and
  `ConfigDroneCanViewModel.cs`: every received CAN frame was echoed back to the vehicle
  (RX equalled TX in the tlog, doubling bus load and self-processing); the DroneCAN page and
  the Inspector used exclusive subscriptions and separate `MAV_CMD_CAN_FORWARD` loops, so
  opening one broke the other; node names took up to 10 s per node because GetNodeInfo was
  only sent by the periodic sweep; the library's own node 127 appeared as a device. The page
  now says after 6 s when only the autopilot is visible and names the other bus; one
  transport per link refuses a second bus with a clear message.
- Live acceptance through Mission Planner's own code path (`DroneCanLiveHardwareTests`, env
  `MP_LIVE_CAN_PORT/BUS/NODE`): on CAN2 nodes 10 and 125 were named in 0.7 s and all 85
  parameters of node 125 arrived in 2.1–3.2 s; on CAN1 the "only the autopilot itself" hint
  appears. Node 10 can stay unnamed on the empty CAN1 bus (edge case, not pursued).
- Restore had failed on NuGet audit NU1902/NU1903 for `SixLabors.ImageSharp` 2.1.12
  (2.1.13 and 3.1.12 are flagged too; 4.x needs a licence). Spectrogram and propagation
  PNGs are rendered with the already referenced SkiaSharp; ImageSharp is gone.
- AI window (`Views/AgentToolsWindow.cs`): one row per installed agent as in X-Office's
  assistant panel (name, kind, location, Launch, Register/Unregister), Find agents,
  sessions, persistent port; task text and activity moved under Advanced. Discovery
  finds `com.anthropic.Claude.desktop` on Linux (case-insensitive file match). Verified
  under Xvfb: Codex CLI, Claude Code, ChatGPT Desktop and Claude Desktop discovered.
  Codex rewrites `~/.codex/config.toml` without comments, which turned the managed MCP
  block into a bare table that Mission Planner refused forever; such a table (only the
  keys Mission Planner writes plus a loopback `/mcp` url) is re-adopted now.
- Verification: full Release tests **1849/1849**, no skipped; `MissionPlanner.slnx`
  Release build **0 warnings / 0 errors**; six `build/porting/check-*.sh` audits and
  `git diff --check` pass. Builds ran with `-m:1` because the machine was loaded.
- Not claimed: hosted CI, packages, CodeQL for this branch; Windows/macOS GUI checks;
  launching a real agent from the new rows (the launch code is unchanged).
- Branch pushed; draft PR #46 targets `master`:
  https://github.com/Rouniy/MissionPlanner10/pull/46 (docs commit `b34aef85b`).
- Next executable step: watch CI on PR #46, then have the user select **MAVLink-CAN2** on
  the DroneCAN page with the L431 attached; merge only when explicitly authorized.
  PR #41 (`fix/mcp-session-lifecycle`) stays open; its toolbar change was re-applied here.

## MCP MAV_CMD parsing — 2026-10-04

- Branch `fix/mcp-mavlink-command` is based on `origin/master` at
  `6da955d27cfd275e0d1632e4fb59f0516d38e1b2`. Implementation commit:
  `eee8dfd2a68f552dd56ab369a1b0b17e7889c804`; precision follow-up:
  `055146104df7ccab7bb3af4d210e05b651440d30`. PR #45 targets `master`:
  https://github.com/Rouniy/MissionPlanner10/pull/45.
- `ParseMavlinkCommandId` validates integer IDs in 0..65535 before passing a `ushort`
  to `Enum.IsDefined`. It also accepts case-insensitive MAV_CMD names with or without
  `MAV_CMD_`; `vehicle_command` advertises this format. Unknown, fractional,
  out-of-range and malformed values produce an `ArgumentException` mentioning commandId.
- Independent review found that decimal conversion could round very precise fractional
  IDs into valid commands. The follow-up parses the original numeric text directly with
  `ushort.TryParse`, preserving integral decimal/exponent forms while rejecting fractions
  and underflow. The added regressions produced **12 failures** before the fix and pass
  afterward.
- .NET SDK **10.0.112**: normal test-project restore/build succeeded, including the native
  DataFlash library and the application executable. **48/48** parser tests passed
  (including 115/CONDITION_YAW, 201/DO_SET_ROI and every defined enum value).
  Full suite with `DFLOG_REQUIRE_NATIVE=1`: **1835/1835**, zero skipped. Two existing
  nullable warnings in `ParameterMetadataIndexTests.cs`. Checks used isolated XDG
  directories; `git diff --check` passed.
- Live qualification used the worktree build under Xvfb, an isolated HOME, and stock
  **ArduCopter V4.0.3 (ffd08628)** over TCP. MCP set GUIDED, armed and took off to 10 m.
  This rerun used the rebuilt precision fix after normal GPS/EKF initialization.
  Numeric commandId **115** with `p1=90,p2=30,p3=1,p4=0` was acknowledged and yaw changed
  **352.7° -> 89.9°**. `MAV_CMD_CONDITION_YAW` with `p1=180` reached **178.9°**.
  MCP then landed; final telemetry showed **disarmed**, altitude 0.037 m. The test
  session, application, SITL and Xvfb were stopped. DO_SET_ROI has parser coverage;
  its vehicle effect was not live-tested. No hardware qualification or hermes-sim changes.
- Hosted CI/package and CodeQL checks passed at `d91c5ea11b4cb694738d0189d47b1010dad45f06`,
  before the precision follow-up. Checks for the follow-up remain pending.
- The primary checkout remains on `master` at `6da955d27`, with its six original modified
  files unchanged. Five unstaged checkout line-ending differences remain in the worktree:
  `Drivers/inf2cat.bat`, `Drivers/uninstall_drivers.bat`, `ExtLibs/Mavlink/regenerate.bat`,
  `ExtLibs/Mavlink/updatexmls.bat`, `graphs/updatexmls.bat`. They have no differences under
  `git diff --ignore-space-at-eol` and were excluded from the commits.
- Remaining implementation/live-test blockers: **none**. Next executable step:
  `gh pr checks 45 --repo Rouniy/MissionPlanner10`; review the PR and its hosted checks.

## Release 1.3.83.5 — native DataFlash parser — 2026-09-29

- PR #34 merged on `origin/master` as `c644eb4964e8a89dfa1a81e1c371173044782472`.
  The single-purpose `build: bump local build number` commit
  `2c1c1f5fa9cf1345e4c7a3d6e8d99f82e53edc5a` changed only
  `build/local-build-number.txt` from `4` to `5` and was pushed to `origin/master`.
  Annotated tag `v1.3.83.5-2c1c1f5f` resolves to that exact commit.
- CI/package run `36603198987` passed all four platform jobs; its Linux job ran the full
  application suite with `DFLOG_REQUIRE_NATIVE=1`: **1787/1787 passed, zero skipped**.
  CodeQL run `36603199091` passed. Tagged release run `36604494331` passed version
  validation, Linux, Windows, both macOS builds and the publishing job.
- The non-prerelease GitHub Release is
  `https://github.com/Rouniy/MissionPlanner10/releases/tag/v1.3.83.5-2c1c1f5f`.
  It has 19 assets: eight human packages (Linux TAR/DEB, Windows ZIP/MSI, and macOS
  ZIP/DMG for both architectures), two additional Linux/Windows updater ZIPs, four
  manifests, four signatures and `SHA256SUMS` (18 entries). No local application
  build or GUI, simulator or hardware qualification was run during this release operation.
- The primary checkout was left on local
  `master` at `f65fc96e8` with its uncommitted `Porting/STATUS.md` addition and five
  `.bat` line-ending differences preserved; it was not used to build or tag the release.
  This release record is a post-tag documentation commit, so it does not change released
  binaries. Remaining release blocker: none. Next executable step: reconcile those local
  changes before updating that checkout, then qualify the published package on its target
  platform.

## Mavlink Mirror windows keep their own mirror — 2026-09-28

- Branch `fix/mavlink-mirror-windows` is based on `origin/master` at `c15c19482` (#40). Mirror
  windows wrote the legacy `MirrorStream`/`MirrorStreamWrite` properties, which address
  `Mirrors[0]`, so a second window's Start replaced the first window's live stream. For TCP Host
  the stream was attached before the 14550 bind, so a busy port then removed that shared entry
  while the first window still looked active. Toggling write-back in any window, even a stopped
  one, changed the first mirror or added an empty entry.
- Each window now binds/opens first and then adds its own entry through
  `MAVLinkInterface.AddMirror`, changes write-back only on that entry, and Stop removes only it.
  `AddMirror`/`RemoveMirror` replace the list under a lock because the read loop enumerates
  `Mirrors` without one.
- The three new `SerialPassThroughViewModelTests` cases failed against `c15c19482` and pass after
  the fix; a temporary variant also showed a stopped window's write-back toggle turning off the
  running mirror's write-back before the fix. Mirror, listener and MCP tests: **115/115**. Full
  application suite: **1702/1702**, zero skipped, .NET SDK 10.0.112, XDG paths redirected to a
  temporary directory. Native-surface validation: **1623 rows, 0 blockers**.
- Live check (2026-09-28, this build under Xvfb, stock ArduCopter 4.0.3 SITL over TCP, driven through
  MCP): one Mavlink Mirror window on TCP Host - 14550 forwarded heartbeats and position to a
  pymavlink client, write-back was switched on in the running window, and disconnect/reconnect
  worked with the mirror running. Two mirror windows at once were not run live.
- The CRLF-normalisation differences in five `.bat` files that appear on checkout are excluded.
- Next executable step: review and merge the PR, then rebuild the local application.

## MCP / X-Office comparison and lifecycle fixes — 2026-09-20

- Dedicated branch `fix/mcp-session-lifecycle` starts at fresh fork `origin/master`
  `c15c1948227fe06362d08fdbd4b0f0ee14bb2b92` (includes merged PR #40). Source fixes:
  `ea2fc5f3c` listener readiness/control capacity, `0f5141b42` fixed access toolbar;
  comparison and retry documentation: `a68b965ed`. No master merge or release.
- Compared X-Office `b534036d80d7baae86ca1917376f3fbadaef63eb` without modifying it.
  Its unrelated user-owned CRM changes continued during this review and are untouched.
  Detailed architecture/differences and deferred product features:
  [MCP_XOFFICE_REVIEW.md](MCP_XOFFICE_REVIEW.md).
- Three new regression cases failed before the fixes: both hub open methods returned an
  unready listener, and DELETE returned HTTP 429 with four blocked tools. All now pass;
  two further cases cover parallel opens and refusing to restart a revoked listener.
  An existing layout test exposed the Stop button below the minimum-size window;
  Allow/Revoke/Disconnect and global controls now remain outside the scrolling body.
- Full local Release test suite: **1704/1704**, no failures/skips. Release solution build:
  **0 warnings / 0 errors**. All six migration/source/artifact audits and `git diff --check`
  pass. Logs: `/tmp/mp-mcp-before.log`, `/tmp/mp-mcp-after.log`,
  `/tmp/mp-mcp-full.log`, `/tmp/mp-mcp-build.log`.
- Verification includes real local HTTP sessions and Avalonia headless layout checks.
  No model invocation, desktop registration change, GUI flight session, SITL/aircraft action
  or new package build. Cross-platform CI and real client/aircraft acceptance are separate.
  Claude remains paused. The old standalone Avalonia directory remains absent/untouched.
- Next: review the pushed branch/PR and its CI; merge only when authorized. Future optional
  features include concrete application-to-agent requests and reconnect-safe operation
  receipts, with exact vehicle identity/replay semantics designed before implementation.

## MAVLink Mirror TCP-host write-back — 2026-09-19

- Branch `fix/mavlink-mirror-writeback` is based on `origin/master` at
  `2aefdf98a` (`v1.3.83.4-2aefdf98`). The TCP-host mirror now owns a dedicated
  asynchronous client receive loop instead of polling client input only while vehicle data is
  being mirrored. A per-mirror input-ownership flag prevents the legacy polling path from racing
  that loop, and write-back still uses `MAVLinkInterface`'s vehicle-write lock and raw log.
- The listener test exercises client input without any outbound mirror traffic. The initial
  `--no-restore` run used stale package assets and failed to resolve MCP/Tomlyn references.
  A normal test-project restore on 2026-09-20 built and ran successfully; this was not a
  source/test-project blocker. The review follow-up below records the full passing suite.
  The original `dotnet build MissionPlanner.csproj --no-restore -m:1` passed with
  **0 warnings / 0 errors**.
- Live Linux verification used Obriy's coupled ArduCopter on TCP 5760, Mission Planner's TCP Host
  mirror on 14550 with write-back enabled, and Hermes 3.9.14 as the mirror client. Before the fix,
  the Mission Planner receive queue grew while mirror Rx stayed at zero. With the fix, mirror Tx/Rx
  counters both advance, the Mission Planner receive queue stays drained, and SITL reports neutral
  `RC_CHANNELS` values `1500,1500,1000,1500` with RSSI 255. Physical stick deflection still needs a
  separate Hermes mapping check; the vendor `RXLOSS` overlay alone is not transport evidence.
- Pre-existing user changes in five driver/MAVLink graph `.bat` files are deliberately preserved
  outside this change. No vehicle parameters, Mission Planner joystick settings, Obriy files, or
  licence data are modified by this fix.

## Mirror review follow-up — 2026-09-20

- Reviewed all five GitHub discussion comments on PR #40 and its complete diff against
  fresh `origin/master` at `2aefdf98a4e0fdcb14af3399db80fc258884f51c`. Work remains on the
  requested `fix/mavlink-mirror-writeback` branch, rather than the historical migration branch.
- Implementation commits: `497a61af0e248c8520e41cc831fe5d1636a8dd0a` moves receive callbacks
  outside the lifecycle lock, dispatches reads to the thread pool, prevents synchronous
  callback self-waits and seeds write-back from the current checkbox state;
  `99b83e93903b6e3451e1706dcc66318448cec25a` fixes an existing Linux video-device classifier
  defect exposed by the full suite on a host with `/dev/video0` present.
- Four added lifecycle cases failed against the original listener (blocked stop, blocked
  replacement acceptance, and self-wait from receive/connection callbacks). All **7/7**
  focused listener cases pass after the fix. The complete application test suite passes
  **1699/1699**, zero skipped, with .NET SDK 10.0.112 and normal package restore.
- Verification ran in a detached temporary worktree with the exact patched C# source bytes,
  not over the running application's binaries. Full-suite application paths were redirected
  to temporary XDG directories. TRX reports were retained outside the repository.
  Native-surface validation passes: **1623 rows, 0 blockers**. No GUI/simulator/vehicle
  session was launched; these are offline lifecycle and application tests, not flight evidence.
- Pre-existing edits in the five driver/MAVLink `.bat` files and the separate uncommitted
  MCP architecture note in this file are preserved and excluded from these commits.
- Next executable step: wait for PR CI, squash-merge PR #40 into `master` as requested,
  then rebuild the local application without starting it. Package release publication is
  a separate later action; no MissionPlanner10 tag or release is created by this follow-up.

## Release 1.3.83.4 — merge of feat/mcp-flight-context — 2026-09-17

- Merge, build increment and tag were explicitly requested on 2026-09-17. The local build
  number was bumped 3 → 4 on the feature branch (`ca42ac4d4`, version now **1.3.83.4**) and
  PR #39 was merged into `master` by merge commit `30b4f054d` (GitHub merge, no squash or
  rebase); all functional, test and documentation commits (`a3bba5710` … `811f0bcaf`) are
  ancestors of that merge. This post-merge status update is the only subsequent source change
  and forms the release checkpoint. Next executable step: push it to `origin/master`, require
  the complete CI/package and CodeQL runs to pass, then tag the same commit as
  `v1.3.83.4-<8-character-commit-hash>` (annotated, "Mission Planner 10 1.3.83.4") and verify
  the resulting GitHub Release assets. Local pre-merge packages for `9ff6492a` remain in
  `MissionPlanner/out/`; the workflow rebuilds every platform artifact from the tagged commit.

## Prompt exit, desktop registration at startup and robot eyes — 2026-09-17

- Continued `feat/mcp-flight-context` / PR #39 after the single-panel checkpoint below.
  Local/origin HEAD is `9ff6492a9`: three atomic commits on `dbc30df85` — `3f8b5a303`
  prompt application exit, `014ccaa48` Register/Unregister buttons and "Open at startup",
  `9ff6492a9` robot-eye animation — plus the documentation commit that follows. No merge,
  tag, release or history rewrite.
- Slow close (user report) reproduced on the packaged app under Xvfb :99 with Metacity and
  `wmctrl -c` close requests: plain close 0.26 s, AI window open 0.26 s, persistent port
  open + connected MCP client + launched terminal agent **5.3 s** — the 5 s cap in
  `MainWindow.StopAgentTools`. Root cause: `McpAgentHub.DisposeAsync` started on the UI
  thread that then blocked in `Wait(5 s)`; `_stop.Cancel()` ran the cancellation callbacks
  on that thread, the resumed request continuations captured the UI SynchronizationContext
  and could never run, so Kestrel stop and host disposal waited until the cap. Fix: hub and
  server teardown start on a worker thread (`Task.Run`). Tests: a hub-level exit test with
  open ports, a live session and a blocking wait (5.0 s / unfinished before, < 1.5 s after)
  and a server stop test (< 1 s). Acceptance after the fix, same scenario: **0.21 s**.
- Desktop application not seeing Mission Planner: Codex/ChatGPT Desktop reads
  `~/.codex/config.toml` when it starts and connects only while port 47183 is open. On this
  machine the app had been running since 12:18, before the 17:07 registration, and the port
  was never open at Mission Planner startup. Now the AI window shows **Register <app>** /
  **Unregister <app>** beside the launch buttons with the registration state, Register and
  Launch switch on **Open at startup** (`McpAgentHub.AutoOpenDesktopPort`, setting
  `mcpDesktopAutoOpen`), and `App` opens the persistent port after the main window appears
  (`MainWindow.OpenAgentPortAtStartupAsync`). The state text tells the operator to restart
  the application after registering. Acceptance in an isolated `CODEX_HOME`: Register wrote
  the managed entry, the button turned into Unregister, the checkbox switched on, and after
  a Mission Planner restart a client connected to 47183 without opening the dialog. The user
  still has to restart ChatGPT Desktop once (its entry is already present) and then Allow
  the self-connected session in the AI window, or press Launch ChatGPT Desktop.
- Robot eyes: `Views/AiRobotEyes.cs` and a drawn robot in `MainWindow.axaml` replace the
  Font Awesome glyph; the eyes cycle circles → vertical bars → horizontal bars on every
  hub `Traffic` event (each MCP request/response) in addition to the green grant colour and
  brightness pulse. Screenshots `gui/30-main-robot-zoom.png` (circles) and
  `gui/36-restart-zoom.png` (bars after traffic) in the session scratchpad.
- Validation: Release **0 warnings/0 errors**; full **1693/1693** tests; six audits and
  `git diff --check` pass. Xvfb acceptance script `acc2.sh` in the scratchpad (register →
  open port → client traffic → Allow all → timed close 0.21 s → restart with auto-open →
  timed close 0.21 s). Testing hazards recorded: `xdotool windowclose` destroys the X window
  instead of asking the application (run a WM on :99 and use `wmctrl -c`); `pkill -f` /
  `pgrep -f` patterns that appear in the tool command kill the tool shell (use PIDs or
  `ps -o comm`); `setsid` from the tool shell forks so `$!` is wrong (use `nohup … & disown`).
  Metacity is still running on :99 (pid in scratchpad `wm-pid`).
- Packages: `MissionPlanner/out/packages/missionplanner10_1.3.83.3-9ff6492a_amd64.deb` and
  `MissionPlanner10-1.3.83.3-9ff6492a-linux-x64.tar.gz` (built from a clean tree with the
  documentation stashed; a first build with uncommitted docs produced `.dirty` names and was
  discarded); portable app
  `MissionPlanner/out/MissionPlanner10-1.3.83.3-9ff6492a-linux-x64/MissionPlanner10`. Lintian
  clean; the packaged binary ran 15 s under an isolated Xvfb display (expected timeout 124,
  empty log) and, with the persistent port open, a connected client and a launched fake
  terminal agent, exited **0.26 s** after a `wmctrl -c` close request. Index and SHA256SUMS
  in `MissionPlanner/out/mcp-9ff6492a/`.
- CI for `9ff6492a9`: platform run https://github.com/Rouniy/MissionPlanner10/actions/runs/35253846452
  (build-test-linux, package-windows, package-macos x64 and arm64 all succeeded on the first
  attempt) and CodeQL https://github.com/Rouniy/MissionPlanner10/actions/runs/35253846468
  passed; open code-scanning alerts: **0**.
- `Porting/MCP_DIAGNOSTICS.md` documents Register/Unregister, Open at startup, the exit fix
  and the robot indicator. Follow-ups: acceptance with the real ChatGPT Desktop after its
  restart; SITL/real-aircraft acceptance unchanged.

## Single-panel AI agent window with per-agent launch buttons — 2026-09-17

- Session restored after a crash: the uncommitted window redesign from the previous
  session was recovered from the worktree, finished and committed. `feat/mcp-flight-context`
  / PR #39 local/origin HEAD is `dbc30df85` (two atomic commits on `04de5db2d`:
  `a3bba5710` terminal kind from the executable name, `dbc30df85` window redesign), plus
  the documentation commit that follows. Origin master `f1180f672` remains included; no
  merge, tag, release or history rewrite. Note: `gh run list` defaults to upstream
  ArduPilot here; use `-R Rouniy/MissionPlanner10`.
- User feedback addressed. The "Claude Code says there is no `--mcp-config`, only
  `--config`" failure came from the old window: a separate kind selector and an editable
  executable path let a `codex` binary be launched with the Claude flags, and Codex's
  parser answered with "a similar argument exists: --config". The kind now follows the
  executable name (`McpAgentDiscovery.TerminalKind`, `codex*`/`claude*`), a mismatch is
  rejected before the terminal opens, and custom launches infer the kind. The tabs
  (Agent, Parameter proposals, Flight logs, Connections), the agent combo box, the kind
  selector and the path field are gone. `Views/AgentToolsWindow.cs` is one X-Office-style
  page: one **Launch <agent>** button per installed agent (absent agents have no button)
  plus **Find agents**, the initial task, sessions with Allow/Revoke/Disconnect/Allow all/
  Revoke all/Stop all connections, the persistent port, an **Advanced** expander (session
  port + copy settings, working directory, custom executable, proposals review/export,
  attach log, unregister desktop apps) and the Activity log. The default initial task only
  verifies the MCP connection and summarises the tools; it no longer asks for log analysis.
  Log, parameter and mission selection stays in the application; agents use MCP tools.
- Validation: Release **0 warnings/0 errors**; local **1688/1688** tests (12 focused MCP
  layout/terminal tests, `McpLayoutTests` rewritten for the button row); six audits and
  `git diff --check` pass. Xvfb :99 acceptance with real xdotool clicks and fake `claude`/
  `codex` scripts first on PATH (no model invocation): the AI button opened the dialog with
  the "Launch Codex CLI / Launch Claude Code / Launch ChatGPT Desktop / Find agents" row;
  **Launch Claude Code** opened terminator running the fake with
  `--mcp-config <private json> --strict-mcp-config -- <task>` and `MP_MCP_TOKEN`; the
  session port opened; Advanced expanded; **Stop all connections** closed both ports (the
  external terminal stays open, as documented). Screenshots `gui/10-main.png` …
  `gui/14-stopped.png` and `gui/fake-claude.log` in the session scratchpad. The previous
  session's real Claude Code launch on the same layout (`gui/07-claude.png` of the earlier
  scratchpad) reported the server and its tools without analysing logs.
- Hazard found and repaired: the previous acceptance ran the real Claude Code with
  `XDG_DATA_HOME` pointing into a temporary scratchpad; Claude's native installer re-installed
  itself there and re-pointed the local launcher at that temporary copy. The symlink was
  restored to the installed version 2.1.274. For future acceptance use fake CLIs
  on PATH, or keep `XDG_DATA_HOME` real / set `DISABLE_AUTOUPDATER=1` when a real Claude must
  run. Leftover Xvfb processes of the crashed session (old Mission Planner, terminal broker,
  interactive claude) were terminated.
- Packages: `MissionPlanner/out/packages/missionplanner10_1.3.83.3-dbc30df8_amd64.deb` and
  `MissionPlanner10-1.3.83.3-dbc30df8-linux-x64.tar.gz`; portable app
  `MissionPlanner/out/MissionPlanner10-1.3.83.3-dbc30df8-linux-x64/MissionPlanner10`.
  Lintian clean; the packaged binary ran 15 s under an isolated Xvfb display (expected
  timeout 124, empty log). Index and SHA256SUMS in `MissionPlanner/out/mcp-dbc30df8/`.
- CI for `dbc30df85`: platform run https://github.com/Rouniy/MissionPlanner10/actions/runs/35250447672
  and CodeQL https://github.com/Rouniy/MissionPlanner10/actions/runs/35250447683 —
  all five platform jobs (Linux 1688/1688 with 0 warnings, Windows ZIP/MSI, macOS x64 and arm64) and
  CodeQL passed on the first attempt; open code-scanning alerts: **0**.
- `Porting/MCP_DIAGNOSTICS.md` "Use" and log-workflow sections were rewritten for the new
  window. Follow-ups unchanged: SITL/real-aircraft acceptance for `write_parameters`,
  `vehicle_command` and `mission_upload`.

## X-Office-style full-access AI agent connection — 2026-09-17

- Continued `feat/mcp-flight-context` / PR #39 on top of `70813c26a`. Fixed the Linux
  "terminal flashes and closes" agent launch: Codex received
  `-c mcp_servers.missionplanner10_desktop.enabled=false` even when that table was absent
  from `config.toml`, which Codex rejects as "invalid transport". The override is now added
  only when the entry exists, and a failed agent start keeps the terminal open with the exit
  code. Verified with the real Codex CLI 0.154.0 in a pty (stays running) and Claude Code.
- Moved listener/launch/session ownership out of the AI window into an application-wide
  `McpAgentHub` (like X-Office's ApplicationController-owned McpController). Closing the
  window keeps every port, launch and grant alive; **Stop all connections** or application
  exit ends them. The window is now a view over the hub and reopens with recent activity.
- Main-window **AI** navigation button (robot icon) turns green while a session holds a
  grant, brightens for ~220 ms on every MCP exchange (BrushTransition), and its tooltip
  shows the session count.
- Launch grants full control: desktop launch registers if needed, opens the fixed port
  without a token, allows its sessions and activates the app; terminal launch opens a free
  port with a one-use token and the session is allowed on connection.
- MCP grew from 46 to **55 tools**. UI: navigation to all six screens, Setup/Config page
  selection, generic inspection of every visible control and logical menu item in every
  open window (stable control IDs, labels via content/header/watermark/preceding TextBlock,
  options, bounds), native click/toggle/select via automation peers and routed Click,
  typed value entry, window capture and dialog closing. Mission: terrain elevation, 100 m
  elevation profile with clearance, Mission/Fence/Rally upload and download through the
  planner's native transfer with explicit absolute/low-altitude acknowledgements. Vehicle:
  direct verified `write_parameters` with before-snapshot/audit files, `vehicle_modes`,
  `vehicle_command` (set_mode, arm/disarm, takeoff, guided_goto, rtl/land/loiter,
  mission_start, change_speed, servo/relay, motor_test, calibrate, save_parameters, reboot,
  generic MAV_CMD/COMMAND_INT). Motor tests, calibration triggers, reboots and parameter
  writes (unless `allowArmed`) require a disarmed vehicle; commands are serialized and
  re-resolve the target before sending.
- Embedded AI_START/UI_API and this guide describe the full-access model, safety
  conventions and recovery. The AI consent window and password boxes are never targets.
- Local validation: Release solution **0 warnings / 0 errors**; **1687 passed / 0 failed /
  0 skipped** (100 MCP-focused); six migration/source/artifact audits and `git diff --check`.
  New tests: hub survives window close, Codex override gating, generic inspection
  (labels, password exclusion, static text, button Command+Click, menu Click, number
  clamping, text, checkbox, combo/tab by text), navigation to every screen and backstage
  page selection, vehicle-control argument/target validation and tool gating.
- Isolated Linux Xvfb acceptance with real button presses: AI window found 3 agents; Open
  port; a self-connected client was read-only (`permission_required`); Allow all; then over
  HTTP: `ui_get_state` (6 routes, 19 Setup pages), `ui_navigate SETUP`, `ui_select_page`,
  `ui_inspect main` (80 controls, 25 menu items), `ui_invoke` of the PLAN nav button,
  `mission_draft_replace`, `mission_elevation_profile`, `mission_upload`/`write_parameters`/
  `vehicle_command` rejected without a vehicle, and a real 123 KB `window:main` PNG showing
  the green/busy AI button and the agent-built mission. No model invocation, aircraft
  transfer or user client-registration change.
- Commit, package paths and CI outcomes are recorded in the workspace's newest
  MONOREPO_MIGRATION_HANDOFF.md checkpoint after completion.

## MCP UI and mission draft API — 2026-09-17

- Continued `feat/mcp-flight-context` / PR #39; fetched origin/master again and confirmed
  `f1180f67293469a1002f1635bc8fc153e7b00c5d` is included. Rechecked X-Office main
  snapshots `54b1ea3c` and `176877f7`, including GUI actions, inspection, receipts and
  embedded documentation. Its unrelated worktree files were preserved.
- Expanded from 29 to **46 MCP tools**: DATA/PLAN/HELP navigation, registered diagnostic
  widget inspection/actions, map centering and PNG map/plot captures, live tuning fields,
  catalogue-backed log windows/graphs, and local Mission draft get/schema/validation/
  replacement/Undo. Native replacements preserve one Undo group including home; no upload.
- Added bounded connection-local operation receipts, strict replay/conflict handling,
  access epochs and UI snapshot invalidation, shared UI mutation serialization, graph
  and draft revision checks, and cancellation before pending UI dispatch. Unknown tools
  require Allow by default. UI permissions are explained in Connections before granting.
  File reloads invalidate log snapshots; captures reject stale/replaced log views.
- Added embedded [AI_START](../docs/mcp/AI_START.md), [UI_API](../docs/mcp/UI_API.md) and
  diagnostic resources through resources/list/read; initialize instructions direct agents
  to the guide. Resource URIs are fixed, no arbitrary file reads. Reconnect limitations
  and structural-validation limits are documented explicitly.
- Local validation: **1682 passed / 0 failed / 0 skipped** (95 MCP-focused), Release
  solution **0 warnings / 0 errors**, six migration/source/artifact audits and diff check.
  New tests exercise real official HTTP clients, resources/schemas, permission gates,
  receipt recovery across revoke/allow, replay, stale/hidden/revoked controls, native
  draft CAS/Undo and serialized cancellation. Existing public log-load ABI retained.
- Isolated Linux Xvfb GUI acceptance: actual HTTP navigation, map movement/capture,
  live plot selection/capture, opening and plotting a sample DataFlash file, inspecting
  and changing numeric controls, clearing/closing graph, draft replace/replay/Undo, and
  Close all rejecting subsequent requests. Captured PNG map and graph inspected visually.
  No model invocation, real aircraft transfer or user client-registration changes.
- Commit, package paths and subsequent multi-platform CI outcomes are recorded in the
  workspace's newest MONOREPO_MIGRATION_HANDOFF.md checkpoint after completion.

## X-Office-compatible MCP connections — 2026-09-17

- Continued the existing `feat/mcp-flight-context` / [PR #39](https://github.com/Rouniy/MissionPlanner10/pull/39).
  Fetched the fork and verified current master `f1180f67293469a1002f1635bc8fc153e7b00c5d`
  is already an ancestor; no rebase, history replacement or master merge was needed.
  Reference: X-Office `feature/mcp-http` snapshot `5b525b41360aedbc3e205012ff0d13014ca35e3e`.
- Replaced the UI's headless CLI launch with an external interactive terminal. Login,
  model and normal client settings persist; process-only MCP overrides, a private
  one-use handoff and session-bound launch tokens connect it to Mission Planner.
- Unified installed CLI/desktop discovery and permanent registration status. Desktop
  Launch registers/updates, opens the fixed port and activates the client. Added Claude
  Desktop's local stdio bridge and LM Studio JSON registration alongside Codex/ChatGPT.
  Registration preserves other settings and makes an atomic backup; discovery never
  launches a model. The Connections tab and Open/Close controls are always available.
- Stateful HTTP sessions have independent Allow/Revoke/Disconnect. Self-connected
  persistent clients start read-only; Launch grants access until Revoke/Close.
  Both listeners revoke synchronously before global shutdown awaits a blocked request.
  Manual bearer clients require Allow; replay/reinitialization cannot regain a revoked
  grant. Offline UI actions no longer implicitly open a listener. All 29 tools remain;
  parameter application still requires explicit operator review.
- Validation: **1669 passed / 0 failed / 0 skipped** (82 MCP-focused), Release solution
  **0 warnings / 0 errors**, all six migration/source/artifact audits and diff checks.
  Tests cover real HTTP session isolation, token replay, revocation, blocked-request
  global stop, bridge/EOF cleanup, registration preservation and literal terminal argv.
- Linux Xvfb GUI acceptance found three installed agents and launched a fake CLI through
  the actual terminal button. It initialized MCP, read the UI mission and appeared as
  an Allowed session. Opening the second port then Close all connections closed both
  sockets. Screenshots were retained outside the repository.
  No real model, Claude delegation, aircraft operation or user client-config edit occurred.
- Platform packages and CI are being produced for this branch. Actual provider login,
  model sessions and installed Windows/macOS client activation remain manual acceptance.
  See [MCP_DIAGNOSTICS.md](MCP_DIAGNOSTICS.md) for the revised workflow and access semantics.
- The first CI attempt exposed a headless dispatcher failure in `ShellSmokeTests`.
  MCP window fixtures now await their asynchronous close/catalogue teardown before
  Avalonia resets the per-test dispatcher. The full **1669-test** local suite passed
  again; platform CI is repeated for this test-only follow-up. The application code
  and manually verified Linux behavior are unchanged.

## MCP flight-context extension — 2026-09-17

- Dedicated branch `feat/mcp-flight-context` starts from clean fetched master
  `f1180f67293469a1002f1635bc8fc153e7b00c5d`. Existing user changes were absent;
  the old standalone port directory remains absent and was not removed or archived.
- The diagnostic gap review selected six read-only additions, taking MCP from
  **23 to 29 tools**: `read_vehicle_messages`, `telemetry_packet_inventory`,
  `log_events`, `log_time_series`, `compare_log_parameters` and
  `compare_vehicle_parameters_to_log`. Both CLI and desktop endpoints expose them.
  Existing downloads, analysis and operator-reviewed proposals remain available.
- Native MAVState now offers locked packet snapshots and retains 256 STATUSTEXT
  packets per exact component independently of the UI queue. MCP preserves receipt
  times, severity, chunk IDs, cursor pagination and history-loss indicators.
- DataFlash/TLOG event evidence retains original clock/source/line information;
  TLOG state deduplication spans page boundaries. Trend envelopes scan every sample
  into bounded per-source bins, preserve spike times and report invalid samples and
  gaps without interpolation. Parameter comparisons retain unknown encodings and
  missing values, require explicit TLOG sources, and distinguish wire-type changes.
  Current-cache comparisons report completeness and do not stage or write changes.
- Local verification: **1658 passed / 0 failed / 0 skipped**, including new
  concurrent cache, cursor, event, trend, source, historical comparison and HTTP
  client coverage. Release solution build has **0 warnings / 0 errors**. All six
  migration/source/artifact audits and `git diff --check` pass.
- [PR #39](https://github.com/Rouniy/MissionPlanner10/pull/39) contains the reviewable
  extension (functional commit `27806ce8df33e21412ff163dd7ae446ff1e93a00`). The
  recorded checks above are local; platform packaging and CodeQL results are linked
  on the PR. Real aircraft messages, flight-log corpus acceptance and model-backed sessions remain
  manual checks; no aircraft writes, inference or Claude delegation occurred.
  See [MCP_DIAGNOSTICS.md](MCP_DIAGNOSTICS.md) for the gap analysis and usage.

## Published AI agent connections — 2026-09-16

- [PR #38](https://github.com/Rouniy/MissionPlanner10/pull/38) merged normally at
  `cbc0f2e03c9c03c6ccef6b3cf69bec9b7d9b3b63`, preserving functional commit
  `ea05fd1b562902562d915d41832ca3f717ad6a23`. The merge tree is identical to the
  verified PR head. This follow-up changes status documentation only and is being
  fast-forwarded to local/remote master and `port/avalonia-in-place`.
- PR platform CI [35120915147](https://github.com/Rouniy/MissionPlanner10/actions/runs/35120915147)
  passed Linux build, **1646 tests with zero failures/skips**, DEB/TAR validation
  and startup checks, Windows ZIP/MSI validation and macOS x64/arm64 ZIP/DMG builds.
  All five platform artifact bundles are retained. CodeQL
  [35120915193](https://github.com/Rouniy/MissionPlanner10/actions/runs/35120915193)
  passed; the code-scanning API reports zero open alerts. Local Release build has
  zero warnings/errors and all six migration/artifact audits pass. Automatic
  merge/documentation runs are additional to these recorded PR results.
- **AI → Agent → Run agent** discovers Codex CLI/Claude Code and creates a temporary
  authenticated HTTP session. Detected OpenAI desktop apps expose **Desktop →
  Register MCP → Launch desktop agent**, using explicit loopback port 47183 by
  default without a token. Existing parameters, telemetry and BIN/TLOG analysis
  tools are available through both modes. Claude Desktop remains deferred.
- Remaining acceptance: actual authenticated model sessions, installed desktop
  activation on supported operating systems and physical-aircraft log download.
  No real agent inference, global client registration, release/tag, archival or
  Claude delegation was performed. See [MCP_DIAGNOSTICS.md](MCP_DIAGNOSTICS.md).

## Agent discovery and desktop MCP implementation — 2026-09-16

- Started on dedicated `feat/ai-agent-discovery` from clean fetched master
  `816973a30ef9aeb6535bf7b0bfefbcf5ddf86b6f`. The old standalone port directory
  remains absent; no user changes or historical commits were removed.
- AI now discovers Codex CLI and Claude Code, offers refresh/manual executable selection,
  and launches them with process-only MCP configuration. Each run owns a private temporary
  workspace; Claude receives a private JSON, explicit MCP allowlist and 720-second tool
  timeout, with built-in tools/hooks disabled. Auth stays in the agent's existing login.
  Output supports both event formats. Completion/cancellation closes the random bearer-token
  endpoint and removes temporary files, while preserving attached logs and proposals in the UI.
- Detected OpenAI desktop apps expose a separate Desktop tab. Registration owns one marked
  `missionplanner10_desktop` TOML section, validates syntax with pinned Tomlyn 2.10.1,
  preserves other text/comments, makes a backup and atomically replaces configuration.
  Invalid/conflicting/externally modified sections fail unchanged. Registration alone opens
  no listener. Launch binds explicit loopback port 47183 (configurable), without a token,
  then activates the app. Stop closes only desktop access; remove registration is separate.
  Host/Origin, tool guards and operator-reviewed writes remain. Port conflicts fail explicitly.
- CLI and desktop share aircraft/catalogue data with independent listener ownership. Linux
  desktop entries, macOS app bundles and Windows StartApps/MSIX activation are supported.
  CLI URL handlers are not desktop apps. Claude Desktop integration is deliberately deferred.
- Local verification: complete suite **1646 passed / 0 failed / 0 skipped**; Release solution
  **0 warnings / 0 errors**; six migration/artifact audits pass, as does `git diff --check`.
  NuGet reports no known vulnerable direct/transitive application packages. Added coverage
  includes real HTTP no-token handshake, authentication separation, Host/Origin rejection,
  port conflicts, TOML preservation/backups/conflicts, private session files, failed startup,
  real Unix fake-process exit/cancellation and all four AI tabs at minimum width. The shell
  process fixture is Linux/macOS-only; other connection/configuration tests are portable.
- Next: publish branch and verify platform/CodeQL PR gates before integration. Desktop login,
  installed-app activation and actual paid model sessions remain manual acceptance; no real
  Codex/Claude inference or global desktop registration was performed. Claude was not used
  for delegation. See [MCP_DIAGNOSTICS.md](MCP_DIAGNOSTICS.md) for the new connection paths.

## Published BIN/TLOG workflow — 2026-09-16

- [PR #37](https://github.com/Rouniy/MissionPlanner10/pull/37) merged into master
  at `510892a9b0a1d29ce4ae0c832b3cd5108173cdea`, preserving functional commit
  `e6f33599186853d4683f2675b0813588d352cf1f`. The merge tree is byte-identical to
  the verified PR head. This follow-up changes documentation only and is being
  fast-forwarded to local/remote master and `port/avalonia-in-place`.
- PR platform CI [35113077516](https://github.com/Rouniy/MissionPlanner10/actions/runs/35113077516)
  passed Linux build, **1625 tests with zero failures/skips**, DEB/TAR installation
  and startup checks, Windows ZIP/MSI validation and both macOS ZIP/DMG packages.
  Build logs report zero warnings/errors; all five platform artifact bundles remain
  available. CodeQL [35113077540](https://github.com/Rouniy/MissionPlanner10/actions/runs/35113077540)
  passed and the code-scanning API reports **zero open alerts**. Automatic merge and
  documentation runs are additional to these verified functional-tree results.
- BIN downloads and local BIN/LOG/TLOG inspection are available in **AI → Flight logs**;
  MCP exposes 23 tools, including opening the analyzer. TLOG source/time/encoding
  distinctions and DataFlash-only FFT limits are documented in
  [MCP_DIAGNOSTICS.md](MCP_DIAGNOSTICS.md). The detailed local checkpoint is below.
- No remaining implementation/CI blocker. Next executable acceptance: connect a
  representative disarmed aircraft, list/download a BIN, open it, then attach a
  completed TLOG and exercise graphs and MCP reports. Physical downloads, SITL flight
  and model-backed sessions remain untested. No new release/tag, archival, history
  rewrite or Claude invocation occurred.

## BIN/TLOG flight-log workflow — 2026-09-16

- Work started on dedicated `feat/mcp-flight-log-workflow` from fetched clean
  master `db7b815d18e82907f871c25df7b58078e3f72f9c`. The old standalone port
  directory remains absent; no user changes, history or repositories were removed.
- **AI → Flight logs** now lists exact-target onboard logs, downloads DataFlash BIN
  into the local catalogue, discovers/attaches BIN/LOG/TLOG and opens the selected
  file in the existing analyzer. UI and MCP share the guarded download/retention
  pipeline. TLOG is a ground-station recording, not an onboard DataFlash format.
- Embedded Streamable HTTP now exposes **23 tools**, including `open_log_analyzer`.
  TLOG schema, paginated records, per-source statistics, historical parameters and
  VIBRATION/clipping analysis use a dedicated streaming MAVLink reader. The analyzer
  supports telemetry scalar graphs, text records, parameters, messages and track
  exports. Packet sources remain separate; receipt time is never presented as raw
  IMU sampling. FFT/batch/response analysis retains its DataFlash requirement.
- Fixed signed-packet CRC boundaries in the native MAVLink parser and its generator
  template. Offline CRC validation does not claim signature authentication. Corrupt,
  truncated, reversed-clock or unsupported-dialect recordings fail explicitly.
- Local validation: **1625 passed / 0 failed / 0 skipped**, Release solution **0
  warnings / 0 errors**, all six migration/artifact audits (1623 native rows with
  zero blockers; 708/708 source paths), and `git diff --check`. Tests cover official
  HTTP-client telemetry analysis/opening, signed packets, source isolation, encoding
  evidence, malformed files, download retention/cancellation and all three AI tabs.
  Isolated-XDG Xvfb startup stayed alive for 12 seconds (expected timeout 124);
  automatic discovery observed ambient telemetry and unknown-packet warnings, so
  this is startup evidence only, not aircraft/download acceptance. Final edits after
  these local checks only clarify two tool/error description strings.
- Next: publish this branch, verify platform/CodeQL PR gates, then integrate the
  requested master/port branches. No physical log download, SITL flight or model-backed
  session was tested; representative-aircraft acceptance remains. Claude stays disabled.
  See [MCP_DIAGNOSTICS.md](MCP_DIAGNOSTICS.md) for the complete UI/MCP workflow.

## Published MCP integration — 2026-09-16

- [PR #36](https://github.com/Rouniy/MissionPlanner10/pull/36) is merged into master
  at `e989103e7dd3eb8d55d7d4c1c9ec976b515cc884`. The merge tree is byte-identical
  to verified PR head `2ee5d7c0a56901349068540f838d1a70cf2ca989`, preserving the
  planner fix, existing MCP branch and diagnostics extension without squash/rebase.
  This documentation-only follow-up is being fast-forwarded to both master and
  `port/avalonia-in-place`; no branch or historical commit is deleted.
- PR platform CI [35108073056](https://github.com/Rouniy/MissionPlanner10/actions/runs/35108073056)
  passed Linux build/tests/DEB/TAR/smoke, Windows ZIP/MSI validation and macOS x64/arm64
  ZIP/DMG jobs. PR CodeQL [35108073057](https://github.com/Rouniy/MissionPlanner10/actions/runs/35108073057)
  passed; branch-push platform/CodeQL runs `35108044983` and `35108045027` also passed.
  GitHub confirms **1616 passed, zero failed/skipped** and zero build warnings/errors.
  The code-scanning API reports **zero open alerts**. All five platform bundles were
  retained. New automatic runs for the merge/documentation follow-up are separate
  from these verified functional-head results.
- No remaining code/test blocker. The next manual acceptance step is **AI → Start
  server**, attach a representative DataFlash log and exercise the documented analysis
  workflow with a local agent/SITL. Physical writes and flight acceptance remain untested;
  no release/tag or Claude invocation occurred. The checkpoint below records local tests.

## MCP core diagnostics integration — 2026-09-16

- User authorized integrating `fix/planner-jump-defaults` and
  `origin/port/avalonia-in-place` into master and extending the MCP implementation.
  Work started from clean fetched `origin/master` `512d9f710` on dedicated branch
  `feat/mcp-core-diagnostics`. Both histories are preserved by ordinary merges;
  the only conflict was this journal, resolved by retaining both sets of entries.
  The former `MissionPlanner-Avalonia` directory is absent in this workspace;
  no archive, removal or reference-repository change was performed. Actual origin is
  `git@github.com:Rouniy/MissionPlanner10.git`, superseding older repository URLs below.
- Functional commit `5c72bf707` expands the existing Streamable HTTP server from 18 to
  **22 tools**. `vehicle_health` reads exact-target raw MAVLink health packets with
  individual timestamps, protocol units and unknown-value handling. `log_overview`
  summarizes available messages/instances/time ranges. `log_parameters_at` returns
  flight-time values and change counts without future/live substitution.
  `log_vibration_report` separates IMUs, supports modern `Clip` and legacy `Clip0/1/2`,
  and reports counter resets without negative or invented clipping increments.
  Existing operator-reviewed parameter application remains the write boundary.
- Validation on SDK 10.0.111: Release solution **0 warnings / 0 errors**, complete suite
  **1616 passed / 0 failed / 0 skipped**, focused MCP suite **29/29**. New coverage checks
  packet age/unit/target isolation, historical parameter selection, VIBE variants and
  missing fields, resets, bounds, cancellation and HTTP tool calls. A raw HTTP test
  verifies SSE response framing, initialization, HTTP 202 notifications, stateless GET
  405 and unsupported-version 400, in addition to existing authentication/revocation tests.
  All six migration/artifact gates pass (1623 native rows, zero blockers, 708/708 source
  paths); `git diff --check` passes. An isolated XDG/Xvfb application launch stayed in
  the event loop for 12 seconds with empty output (expected timeout exit 124).
- This checkpoint precedes publication and remote CI; the next step is to push the
  integration to `port/avalonia-in-place`, run the normal PR platform/CodeQL gates and
  merge the verified history into master as requested. No new tag/release is requested.
  Physical aircraft writes, SITL and a paid model-backed agent session were not run.
  Claude remains disabled. See [MCP_DIAGNOSTICS.md](MCP_DIAGNOSTICS.md) for usage.

## Retained standalone Linux launcher — 2026-09-16

- Corrected the launch handoff: `bin/Release/net10.0/MissionPlanner` is framework-dependent
  and ordinary desktop launch cannot find an SDK installed only in a temporary directory.
  Published and **retained** the self-contained application at
  `out/linux-x64/MissionPlanner`. Launch this executable;
  keep the entire `out/linux-x64` directory together. No `DOTNET_ROOT` or system .NET
  installation is required. This supersedes the earlier temporary publish cleanup.
- Git checkpoint: `port/avalonia-in-place` HEAD
  `d8ccb13aac5570908a32755bddda87c9aa3aadf8`; this documentation-only commit follows it.
  Product sources, master and remotes are unchanged. Only the pre-existing unstaged
  `graphs/updatexmls.bat` change remains. `out/` is ignored and no binary is committed.
- Validation: Linux x64 self-contained publish succeeded with embedded .NET/ASP.NET Core
  10.0.12 and MCP payloads. A child process with all `DOTNET_ROOT*` variables removed
  loaded `libhostfxr.so` and `libcoreclr.so` from the published directory and reached
  Avalonia X11 initialization. The smoke deliberately used no display and stopped at
  `XOpenDisplay`; it verifies runtime resolution, not graphical acceptance. Logs were
  retained outside the repository.
  Restored the normal solution package graph afterwards. No source changes or test-suite
  rerun; the 1608-pass product checkpoint below still applies.
- Next executable step: run `./MissionPlanner` from `out/linux-x64` in the user's graphical
  session, open **AI**, and proceed with the log/SITL acceptance described below. Remaining
  hardware/model/native-platform acceptance limitations are unchanged.

## Embedded MCP diagnostics implementation — 2026-09-16

- Implemented the complete local diagnostics/proposal/review workflow in the existing
  MissionPlanner product. **AI** in the navigation bar opens a loopback Streamable HTTP
  server, Codex launcher, log attachment and parameter proposal review. The 18 tools expose
  connected targets, telemetry, parameter metadata/refresh, mission draft, onboard/local
  DataFlash logs, field statistics, Welch spectra, raw IMU batches and target/actual lag.
  Operating instructions and limitations: [MCP_DIAGNOSTICS.md](MCP_DIAGNOSTICS.md).
- SDK: `ModelContextProtocol.AspNetCore` 2.2.0 with `Microsoft.AspNetCore.App`. No second
  application or source dependency was introduced. Per-session tokens, Host/Origin checks,
  loopback-only binding, bounded requests and revocation are covered by real HTTP tests.
  The launcher passes credentials through the child environment and does not edit agent
  settings. Claude remains disabled and was not invoked.
- Changes require explicit review/application in Mission Planner. The apply path requires
  a fresh disarmed heartbeat, checks connection generation, serializes parameter writes,
  compares a fresh value with the reviewed value, preserves typed acknowledgements and
  refuses integer precision loss. Backup/review and incremental result files precede and
  track writes; partial failure stops the batch without automatic rollback or reboot.
  Log downloads pin system/component/signing context and cannot release another operation's
  transport ownership. No MCP tool arms, flies, deletes aircraft logs or executes code.
- Git code checkpoint: `port/avalonia-in-place` HEAD
  `1af2f1655058e4c14b2945d7cc718e76a5e39677`. This status-only handoff commit directly follows
  that checkpoint. Atomic changes: `b69a7fd7cf0d1ec241f8984379022f02192eb797` fixes the inherited
  DroneCAN test's assumption of a globally single discovered node;
  `f65023c4483b6a5030db208ff2d891298566b8b9` adds guarded MAVLink writes/download ownership;
  `a9cab1f67fd561f226481e6a754a06d397f13457` implements MCP/UI/analysis/tests/docs;
  `1af2f1655058e4c14b2945d7cc718e76a5e39677` rejects lossy C-cast integer proposals.
  `master == origin/master == 512d9f7104ccf6e211a698eaa4cf31af78b6a64d` and
  `origin/port/avalonia-in-place == 802557e623c7d4e05fd2675805af52a726011f2c` remain unchanged.
  No push or master merge occurred. Only the pre-existing unstaged `graphs/updatexmls.bat`
  line-ending change remains outside these commits. The reference source repository was
  not modified. The earlier PR #34 decision remains: defer its incomplete feature stack.
- Local verification used SDK **10.0.401**, installed outside the repository because
  the initial PATH had no SDK:
  - `dotnet restore MissionPlanner.slnx`: pass.
  - `dotnet build MissionPlanner.slnx -c Release --no-restore`: **0 warnings, 0 errors**.
  - `dotnet test MissionPlannerTests/Avalonia/MissionPlanner.Tests/MissionPlanner.Tests.csproj
    -c Release --no-restore`: **1608 passed, 0 failed, 0 skipped** (27 added tests).
    Coverage includes real Kestrel/official MCP client negotiation/auth/revocation, DataFlash
    pagination and raw-batch integrity, known-frequency/known-delay numerical signals,
    target invalidation, heartbeat checks, stale proposals, integer encodings, guarded
    protocol writes/download cancellation, launcher configuration and minimum-width UI.
  - Six migration/artifact gates pass: native surface **1623 rows / 0 blockers**, source
    resolution **708/708**, no-WinForms, project artifacts, binary artifacts and key artifacts.
    `git diff --check` passes. The optional native `--require-closed` variant still rejects
    the inherited self-replacement entry for `Program.cs`; the normal CI gate passes.
  - Self-contained publishes pass for **linux-x64, win-x64, osx-x64 and osx-arm64**,
    using `dotnet publish MissionPlanner.csproj -c Release -r <RID> --self-contained true
    -m:1 -p:DebugType=none -p:BaseOutputPath=<build-output>/ -o <publish-output>`.
    Verified apphost/main assembly, all MCP assemblies and ASP.NET Core/Kestrel runtime
    payloads, plus macOS SimpleBLE/VLC libraries/plugins. These are cross-publish/payload
    checks, not native installer/runtime acceptance.
- Packaging initially exhausted the home filesystem while copying Windows VLC
  files into test output. Removed only this task's temporary/generated outputs, restored
  the normal solution graph and moved cross-RID build outputs to temporary storage; final checks above
  supersede that failed attempt. Publish payloads were verified and removed afterwards to
  conserve disk. Test, build and per-RID publish logs were retained outside the repository.
- Remaining acceptance: no paid/model-backed external-agent run, SITL session, physical
  vehicle write or flight was performed. Native Windows/macOS execution, installer checks,
  Linux GUI smoke (no Xvfb here) and new CI/CodeQL runs remain unverified in this environment.
  Managed log indexing cannot cancel mid-constructor; live snapshots are not per-field
  freshness guarantees; legacy plugins do not share a universal transaction service.
  Those limits are explicit in the operating instructions. Flight stability is not certified.
- Next executable step: in a graphical session run
  `dotnet run --project MissionPlanner.csproj -c Release`, open **AI**, attach a
  representative flight log and launch an authenticated local Codex. Use the built-in
  **SIMULATION** workflow from [SITL-TESTING.md](../SITL-TESTING.md) to exercise discovery,
  refresh/download, proposal review, stale-value rejection and revocation before physical
  aircraft acceptance. Native platform CI/package and CodeQL gates precede any release.

## MCP Streamable HTTP feasibility — 2026-09-16

- Assessed embedded MCP hosting and locally launched external agents; findings, concrete
  integration points, source links, proposed workflow and effort estimates are in
  [MCP_INTEGRATION_ASSESSMENT.md](MCP_INTEGRATION_ASSESSMENT.md). The transport is a small
  integration; shared UI/MCP operation ownership and vehicle identity are the main work.
- Verified official C# SDK v2.2.0's net10/ASP.NET Core graph and current Codex HTTP MCP
  configuration documentation. Local `codex --version` reports `0.154.0`; `exec --help`
  confirms per-run configuration. Only version/help commands were executed: no agent
  task, listener, model request, package installation or product change was made.
- Git checkpoint before this documentation-only change:
  `port/avalonia-in-place` HEAD `37b6876add2aa6852e069407a1b2171bcd904292`;
  `master == origin/master == 512d9f7104ccf6e211a698eaa4cf31af78b6a64d`;
  `origin/port/avalonia-in-place == 802557e623c7d4e05fd2675805af52a726011f2c`.
  The assessment commit directly follows that HEAD and is not pushed. The pre-existing
  unstaged `graphs/updatexmls.bat` change remains excluded; source port stays untouched.
- Validation: documentation whitespace check only. No new build/test result is claimed;
  `dotnet` remains unavailable on the current PATH. Runtime compatibility, actual package
  growth and an end-to-end agent connection remain unmeasured.
- Next executable step if implementation is requested: validate an embedded Kestrel/MCP
  spike against a real local CLI and self-contained publishing, then deliver the complete
  read/analysis workflow with launch/stop UI and regression coverage. Remote-agent access
  and vehicle-write permissions require their own concrete scope.

## Pull-request completeness audit — 2026-09-16

- Scope: all PRs in `Rouniy/MissionPlanner10`, queried through the GitHub API. There is
  exactly one open PR, [#34](https://github.com/Rouniy/MissionPlanner10/pull/34), reviewed
  at head `5c208bfc8a21e99d502372f52c3e5fe34f8f6745`. All 34 closed PRs are merged;
  `git merge-base --is-ancestor` confirms every reported merge commit is already included
  in local `master` at `512d9f7104ccf6e211a698eaa4cf31af78b6a64d`, including #25 and #35.
- Decision: defer #34. Its description explicitly identifies it as the first of four
  planned PRs. The 26-file diff vendors the Rust parser and adds build/package plumbing,
  but changes no C# source and provides no application consumer. P/Invoke and DFLogBuffer
  fast paths belong to phase 2, log-viewer/FFT/expression consumers to phase 3, and required
  native-path tests and package-payload assertions to phase 4. This does not satisfy the
  user's requirement for a complete, independently useful feature. No PR code was imported.
- Remote verification at that exact head: Linux build/tests, Windows packaging, macOS x64
  and arm64 packaging, CodeQL analysis and the CodeQL PR check all report success
  ([CI run](https://github.com/Rouniy/MissionPlanner10/actions/runs/34002587483),
  [CodeQL run](https://github.com/Rouniy/MissionPlanner10/actions/runs/34002587485)). These
  results establish the existing phase's checks, not end-to-end native-parser acceptance.
  Reviewed the commit list, integration diff, status notes and discussion; no submitted
  reviews or completed follow-up PRs are present in this repository.
- Git handoff: audit work is on `port/avalonia-in-place`, created at the unchanged
  `master == origin/master == 512d9f7104ccf6e211a698eaa4cf31af78b6a64d`. The documentation-only
  audit commit is its direct child. The remote migration branch remains at
  `802557e623c7d4e05fd2675805af52a726011f2c`; nothing was pushed or merged to master.
  The pre-existing unstaged line-ending change in `graphs/updatexmls.bat` is preserved and
  excluded from the commit. The read-only source repository was not modified.
- Local validation: documentation diff checked with `git diff --check`. No application
  build or test suite was run because no product code was changed; `dotnet` is also absent
  from the current shell PATH. Earlier build/test results below are historical checkpoints,
  not new validation of this audit.
- Remaining integration blocker: #34 is an incomplete feature stack. Next executable step:
  query open PRs again when the remaining phases are submitted, review the combined feature
  against the then-current master, and require managed/native parity, native-required tests,
  and package validation for all four RIDs before accepting the complete implementation.

## Flight Planner DO_JUMP defaults

- Dedicated branch `fix/planner-jump-defaults` starts from clean
  `master == origin/master` `b9a62d96d83f`. Behavior and regression commit `fd92b02fd`
  restores the original Mission Planner defaults for the general Jump action: the target prompt
  starts at waypoint **1**, and the resulting `DO_JUMP` row receives `P1=target`, `P2=5` instead
  of the incorrect `P1=0`, `P2=-1`. The existing explicit Jump Start action is unchanged.
- The reference is the current upstream Mission Planner clone, specifically the Jump handlers in
  `GCSViews/FlightPlanner.cs`. The fix keeps the existing Avalonia dialog and undo boundary and
  only centralizes the two original defaults in `FlightPlannerViewModel`.
- The new focused regression passes **1/1** and the complete `PlannerPortParityTests` group passes
  **60/60**. The complete Release test project passes **1582/1582**. A Release build of
  `MissionPlanner.slnx` succeeds with **0 warnings / 0 errors**. All commands ran locally without
  restore; only the root agent launched builds, with MSBuild concurrency capped at 12.
- Remaining local code/test blocker: none. `master` and `origin/master` remain unchanged. The next
  executable step is to push this branch and run the normal Linux, Windows, both macOS and CodeQL
  PR gates before merge. Manual acceptance should open PLAN, choose Jump, confirm target **1**, and
  inspect the new row for repeat count **5**.


## PR #25: safe DFLogBuffer index cache on .NET 10

- PR `#25` (`fix/dflogbuffer-savecache-net10`, reviewed head `2cd32cfdb`) is important and
  appropriate to integrate. The path-backed `DFLogBuffer` cache is exercised for logs at or above
  300 MiB, but its upstream `BinaryFormatter` save path throws on .NET 10; loading also cannot
  restore that format. The PR replaces it with a small versioned binary format inside GZip,
  validates source length/write time, writes via a temporary file and falls back to a full scan
  when the cache is stale, truncated or foreign. The contributor's synthetic coverage and
  433.6-MiB/10.55M-record benchmark demonstrate both the failure and the useful startup gain.
- The review found integration blockers in the first revision and fixed them in follow-up commit
  `a4f24e82c` after the exact PR merge commit `55f543048`. `LastLoadFromCache` was static and was
  also read back as a control-flow decision, so concurrently constructed buffers could make one
  another skip a required scan; it is now per instance and the decision remains local. The
  predictable shared-temp name was replaced by a SHA-256 path key in the user's local data
  directory. Concurrent writers use unique sibling temp files and always clean abandoned writes.
  Source identity is captured before scanning and rechecked before publishing or accepting an
  index, covering platforms that permit an already-open log to be modified.
- Cache input is now validated as untrusted data: line and per-list counts have individual and
  aggregate bounds; offsets and line numbers must be in range and strictly increasing; paired
  lists, total binary-message count and text/binary line-offset cardinality must agree; trailing
  payload is rejected. Untrusted counts no longer cause a full-size up-front allocation. A bad
  cache resets all temporary indexes and degrades to a clean rescan without failing log open.
- Focused binary/text round-trip, stale-identity, corrupt/truncated, unreasonable-count,
  out-of-range-offset and instance-isolation coverage passes **8/8**. The complete Release suite
  passes **1581/1581**; `MissionPlanner.slnx` builds with **0 warnings / 0 errors**. All six
  migration/retirement/artifact gates pass: 1623 native rows with 0 blockers, 708/708 pinned port
  paths, and clean WinForms/project/binary/key audits.
- At the reviewed PR head, Linux, Windows, macOS x64 and CodeQL passed; macOS arm64 failed only
  after publish/sign/DMG creation because `hdiutil verify` returned runner-level `Resource
  temporarily unavailable` on all three retries. The published master checkpoint
  `afdf316733bb8b8188512182afddd5d3498eff64` then passed the complete CI/package run
  `33451960996`: Linux tests/TAR/DEB/payload smoke, Windows ZIP/MSI install validation and both
  macOS ZIP/DMG jobs all succeeded, including arm64. CodeQL run `33451960959` also succeeded.
- PR #25 is `MERGED` with merge commit `55f543048`; the review/fix summary is posted in the PR and
  `master == origin/master == afdf31673` at the verified functional handoff checkpoint. Remaining
  code blocker: none. This final status-only update follows that checkpoint; the next useful manual
  acceptance step is to open a representative path-backed log over 300 MiB twice and confirm the
  second open uses the per-user cache without changing displayed lines or message counts.

## Local ArduPilot parameter-metadata synchronization

- The authoritative source checkout remained read-only and clean on ArduPilot branch
  `codex/sitl-udpserver-local-port-20260830`, commit
  `3b2f9ac14e9da48aff58aa206ae60d80527f6edd`; its merge base with `origin/master` is
  `582a71193ad2ed2d47e6723a53380447b0038a02`. The ArduCopter pdef was regenerated locally with
  `Tools/autotest/param_metadata/param_parse.py --vehicle ArduCopter --format xml` and the exact
  fork delta was reviewed rather than copying the complete 2.8 MB generated catalogue.
- `ParameterMetaDataLocal.xml` records all **46** affected entries: six Copter flight-mode value
  lists, thirteen changed/new dead-reckoning entries, Copter's changed `ARSPD_USE`, six generic
  EKF3 entries and twenty ModelCal entries. This covers the new `DR_OBS_*`, `DR_EKF_*`,
  `EK3_ARSP_MODE`, `EK3_GPS_Q_*` and `MCAL_*` descriptions, ranges, units, values, reboot and
  read-only flags, plus the new `31:ModelCal` mode value and the changed `DR_NEXT_MODE` /
  `EK3_OPTIONS` descriptions.
- The 29,318-byte pdef overlay is loaded before downloaded upstream metadata, so fork changes to
  existing fields are visible as well as entirely new parameters. Copter-only library entries are
  explicitly qualified as `ArduCopter:*`, preventing ModelCal, dead-reckoning and the Copter pitot
  guidance from leaking into Plane/Rover/Sub; generic EKF3 additions remain available to every
  vehicle that exposes them. Missing overlay fields still fall through to downloaded pdef and then
  to the legacy backup. The overlay is copied byte-for-byte into normal output and publish payloads.
- Functional/data/test commit `839659c7f` was developed and published on
  `port/avalonia-in-place`. The user explicitly authorized master integration on 2026-09-01;
  local `master` was first fast-forwarded through the eight newer origin changes ending at
  `ecf886a19`, then the complete metadata branch was integrated by merge commit `185fc9065`.
  The tracked local build sequence was intentionally incremented from **2 to 3** in the separate
  commit `6762223b1`. On this combined master, focused metadata tests pass **4/4**, the complete
  Release suite passes **1573/1573**, and `MissionPlanner.slnx` builds with **0 warnings / 0
  errors**. All six migration/retirement/artifact gates pass (1623 native rows, 0 blockers,
  708/708 pinned port paths, and clean WinForms/project/binary/key audits).
- The code and initial handoff commits were pushed to the newly published
  `origin/port/avalonia-in-place` branch. The first `make linux-deb` attempt encountered the known
  logical/physical checkout alias in stale Release
  intermediates; a standard solution clean followed by the physical-path package script resolved
  it without source changes. The resulting package is
  `out/packages/missionplanner10_1.3.83.2-6264c722_amd64.deb`: 60,143,970 bytes, Debian version
  `1:1.3.83.2+6264c722`, installed size 193,828 KiB, SHA-256
  `fa3f9e7f0f5a264a84c80bc44ae053de46d4d90c05393fd7994aa31e651bbef2`. `lintian` passes with no
  findings; the packaged `ParameterMetaDataLocal.xml` is byte-identical to the tracked 29,318-byte
  source, required launcher/resource checks pass, and forbidden Windows SimpleBLE/libusb payloads
  are absent. An isolated extracted-package Xvfb launch reaches the normal event loop for the
  complete 12-second smoke window (expected timeout 124) with empty stderr.
- The clean merged-master package after the required sequence bump is
  `out/packages/missionplanner10_1.3.83.3-6762223b_amd64.deb`: 60,142,236 bytes, Debian version
  `1:1.3.83.3+6762223b`, installed size 193,860 KiB, SHA-256
  `45f1e84556fa5c597646f83f01fd69783ecd3f7d1f2b00fac9e0bf9faa1c722f`. `lintian` has no findings;
  the packaged overlay is byte-identical, required launcher/resource checks pass, and forbidden
  Windows SimpleBLE/libusb payloads are absent. Its isolated Xvfb smoke reaches the normal event
  loop for the full 12-second window (expected timeout 124) with empty stderr.
- Remaining blocker: none. Next executable acceptance step is to connect the matching custom
  firmware and visually verify the new descriptions/editors in Full Parameter List; regenerate
  and review this small overlay whenever the pinned ArduPilot branch advances. This status-only
  handoff follows the merged-master package checkpoint; after it is pushed, `master`,
  `origin/master` and the worktree are aligned and clean.

## Flight Planner waypoint display numbering

- Dedicated branch `fix/flight-planner-waypoint-numbering` starts from merged `master`
  `1cbe17b87`. Behavior commit `03fca7c64` makes human-facing Planner waypoint numbers
  one-based; regression commit `054f9c7f1` covers the row model and map markers.
- Official Mission Planner removes mission item zero (home) before building the Flight Data
  waypoint overlay, labels the remaining items with `a + 1`, and numbers Flight Planner row
  headers with `a + 1`. The Avalonia Plan grid and map instead exposed zero-based `WpRow.Seq`, so
  its first planned point appeared as 0 while Flight Data correctly showed 1.
- `WpRow.Seq` remains zero-based for editing, plugin APIs, mission transfer, `DO_JUMP`, map dragging
  and mission files. New read-only `DisplayNumber` returns `Seq + 1` and raises a dependent property
  notification whenever rows are renumbered. Only human-facing Plan grid, map-marker and KML labels
  use the one-based value.
- Focused `WpRowTests` plus `FlightPlannerViewportTests` pass **18/18**. The Release build succeeds
  with **0 warnings / 0 errors**. The complete Avalonia suite rerun executed **1569** tests:
  **1568 passed** and only the existing host-sensitive
  `VideoSourceResolverTests.NormalizesCommonStreamSources` `/dev/video0` case failed. A serial/TCP
  loopback timing test failed once during the first full run, then passed alone and in the complete
  rerun; no bridge source or test changes on this branch.
- Behavior, integration and test reviewers approve exact code/test head `054f9c7f1` with no
  protocol, file-format, plugin or map-interaction blocker.
- The worktree still contains only the user's five pre-existing modifications in
  `Drivers/inf2cat.bat`, `Drivers/uninstall_drivers.bat`, `ExtLibs/Mavlink/regenerate.bat`,
  `ExtLibs/Mavlink/updatexmls.bat` and `graphs/updatexmls.bat`; none is staged or included. Claude
  remains disabled.
- Manual acceptance: rebuild and relaunch, add at least two Plan points and confirm the grid and map
  show 1 and 2; upload/read the mission and confirm Flight Data uses the same numbers while current
  mission status may still correctly report sequence 0 when the vehicle is idle.

## Flight Data bearing-overlay zoom stability

- Dedicated branch `fix/flight-data-bearing-overlay-zoom` starts from merged `master`
  `47fc0de85`. The published branch first attempted screen-space parity in commits `005462291` and
  `40de2497f`; live testing showed that keeping the default 500-pixel vectors constant made them
  span countries at world zoom. Corrective commit `5acf3d98e` removes that viewport callback/cache
  and keeps bearing vectors at a stable map distance instead. Published history was not rewritten.
- Official Mission Planner draws heading, navigation, course and target bearings at a constant
  screen-pixel length. This port now deliberately differs: `GMapMarkerBase_Length` is a map distance
  in metres, and Planner Settings labels it **Line Length (m)**. The vector therefore shrinks with
  the aircraft when the map is zoomed out instead of acquiring a continental geographic extent on
  every telemetry refresh. Radius geometry retains its existing physical-distance behavior.
- `FlightMapOverlayTests` passes **29/29**. The corrected regression verifies that a configured
  500m bearing renders as 250px at resolution 2, shrinks to 62.5px after zooming to resolution 8,
  and remains 62.5px after the next `PopulateVehicleLayer` telemetry redraw. The Release build
  succeeds with **0 warnings / 0 errors**. The complete Avalonia suite ran **1567** cases: **1566
  passed** and only the existing environment-sensitive
  `VideoSourceResolverTests.NormalizesCommonStreamSources` `/dev/video0` case failed; neither the
  resolver nor its test changes on this branch.
- The worktree still contains only the user's five pre-existing modifications in `Drivers/inf2cat.bat`,
  `Drivers/uninstall_drivers.bat`, `ExtLibs/Mavlink/regenerate.bat`,
  `ExtLibs/Mavlink/updatexmls.bat` and `graphs/updatexmls.bat`; none is staged or included. Claude
  remains disabled.
- No code or automated-test blocker remains. Manual acceptance requires rebuilding and relaunching
  this branch (the already running process predates it), connecting the live vehicle, and zooming
  Flight Data out to a regional/world view: all enabled bearing lines must stay local to the
  aircraft, shrink with the map and remain free of duplicated aircraft symbols.

## Choice-dialog action layout

- Dedicated branch `fix/dialog-choice-button-layout` starts from merged `master` `4700897ad`.
  Commit `5e8d86341` makes choice dialogs size to their content within a 380px minimum and 760px
  maximum and lets action buttons wrap when the window is constrained; commit `664f4d5e2` adds the
  exact four-button updater-dialog regression.
- `Dialogs.Choice` previously reused the fixed 380px generic dialog frame with a single horizontal
  `StackPanel`. The updater actions required more width, so the final **Later** button painted past
  the client edge and was clipped. The choice dialog now expands enough to keep those actions on
  one row under normal sizing, while a `WrapPanel` keeps every action inside the window if the
  platform constrains it to the minimum. Updater/version behavior is unchanged on this branch.
- `DialogLayoutTests` passes **1/1** and renders the reported labels at both automatic width and a
  verified 380px window width, checking horizontal and vertical containment. The automatic layout
  also must retain one action row. `dotnet build MissionPlanner.csproj -c Release -m:1 --no-restore`
  succeeds with **0 warnings / 0 errors**. The complete Avalonia suite ran **1566** cases: **1565
  passed** and only the existing environment-sensitive
  `VideoSourceResolverTests.NormalizesCommonStreamSources` `/dev/video0` case failed; neither the
  resolver nor its test changes on this branch.
- Behavior, integration and test reviewers approve the code/test tuple `5e8d86341` + `664f4d5e2`.
  The worktree still contains only the user's five pre-existing modifications in
  `Drivers/inf2cat.bat`, `Drivers/uninstall_drivers.bat`, `ExtLibs/Mavlink/regenerate.bat`,
  `ExtLibs/Mavlink/updatexmls.bat` and `graphs/updatexmls.bat`; none is staged or included. Claude
  remains disabled.
- PR #31's first CI run `33410216298` published, signed and verified the x64 app, then failed while
  creating its DMG; the arm64 package passed. An earlier run `33381702003` failed at the same
  boundary on arm64 with `Resource temporarily unavailable`, while its x64 package passed. Commit
  `f04397be3` gives DMG creation and immediate verification three bounded attempts with 2s/4s
  backoff, removes a partial image before recreating it, and exposes `hdiutil` diagnostics. A
  persistent packaging error still fails the build. `bash -n build/macos/make-dmg.sh` and
  `git diff --check` pass; integration and test reviewers approve the CI hardening.
- Follow-up CI run `33411440256` passed on pushed head `719b1858b`: the complete Linux build/test
  and package job, Windows package build/install validation, and both macOS x64 and arm64 signed
  app/DMG builds, mount checks and artifact uploads are green. No automated blocker remains. Manual
  UI acceptance requires rebuilding and relaunching this branch and opening a four-action choice
  such as the update prompt for a genuinely newer signed release: all four buttons must be visible
  and clickable. After this UI fix lands, the next separate task is the Flight Data bearing/target
  overlay behavior while zooming.

## Updater equal-version metadata precedence

- Dedicated branch `fix/updater-same-version-prompt` starts from merged `master` `3e7ce29e1`.
  Commit `136118c40` stops Git commit and `.dirty` metadata from ordering otherwise equal builds;
  commit `2652eb316` covers the reported `1.3.83.2` clean-release versus dirty-local-build case and
  proves that a higher fourth numeric build still updates. Reviewer follow-up `a8434687f` limits
  date ordering to legacy identities without an explicit fourth field, and `3111d8ebe` covers that
  equal-four-part/different-date case.
- `UpdateEngine.IsNewer` previously treated different hashes as newer after the official version,
  local build number and legacy build date compared equal. This produced an update prompt whose
  message showed the same `1.3.83.2` version on both sides. Hashes now remain display and artifact
  identity metadata only. The four-part numeric version remains authoritative, while legacy
  three-part dated builds and the one-time CalVer migration retain their existing ordering. The
  release process already requires incrementing the repository-global local build number before
  every materially new stable or beta release.
- The combined `UpdaterTests` and `AppVersionTests` pass **51/51**, and
  `dotnet build MissionPlanner.csproj -c Release -m:1 --no-restore` succeeds with **0 warnings / 0
  errors**. The complete Avalonia suite ran **1565** cases: **1564 passed** and only the existing
  environment-sensitive `VideoSourceResolverTests.NormalizesCommonStreamSources` `/dev/video0`
  case failed; neither the resolver nor its test changes on this branch.
- Behavior, integration and test reviewers approve the code/test tuple `136118c40` + `2652eb316`
  + `a8434687f` + `3111d8ebe`. The worktree still contains only the user's five pre-existing
  modifications in
  `Drivers/inf2cat.bat`, `Drivers/uninstall_drivers.bat`, `ExtLibs/Mavlink/regenerate.bat`,
  `ExtLibs/Mavlink/updatexmls.bat` and `graphs/updatexmls.bat`; none is staged or included. Claude
  remains disabled.
- No code or test blocker remains. Manual acceptance requires rebuilding and relaunching this
  branch, then checking against a signed release with the same four-part version but a different
  hash: it must report up to date and must not offer installation. A release with a higher fourth
  numeric build must still prompt. After this updater fix lands, the next separate task is the
  Flight Data bearing/target overlay behavior while zooming.

## Flight Planner vehicle-home synchronization

- Dedicated branch `fix/flight-planner-vehicle-home-sync` starts from merged `master`
  `f02eefd87`. Commit `279db6eab` refreshes Flight Planner home state whenever PLAN is selected or
  reselected; commit `d7132c4d0` adds the isolated helper and shell-navigation regressions. The
  comparison source is official `ArduPilot/MissionPlanner` commit `2b5589f40`.
- The persisted `TXT_homelat`/`TXT_homelng` values could remain at an earlier planning site after a
  different vehicle connected. Adding enough local waypoints then correctly drew the official
  `last -> home -> first` route, but against that stale home. PLAN activation now prefers the
  autopilot-reported `HomeLocation`, falls back to `PlannedHomeLocation`, and retains the saved
  planner home only when neither vehicle coordinate is valid. Route construction and its solid or
  dashed styling are unchanged.
- The focused `FlightPlannerHomeSyncTests` pass **4/4**, `PlannerPortParityTests` pass **59/59**,
  and `dotnet build MissionPlanner.csproj -c Release -m:1 --no-restore` succeeds with **0 warnings /
  0 errors**. The complete Avalonia suite ran **1562** cases: **1561 passed** and only the existing
  environment-sensitive `VideoSourceResolverTests.NormalizesCommonStreamSources` `/dev/video0`
  case failed; neither the resolver nor its test changes on this branch.
- Behavior, integration and test reviewers approve the implementation and test tuple `279db6eab`
  + `d7132c4d0`. The worktree still contains only the user's five pre-existing modifications in
  `Drivers/inf2cat.bat`, `Drivers/uninstall_drivers.bat`,
  `ExtLibs/Mavlink/regenerate.bat`, `ExtLibs/Mavlink/updatexmls.bat` and `graphs/updatexmls.bat`;
  none is staged or included. Claude remains disabled.
- No code or test blocker remains. The screenshot process loaded the older `7de82f8a`, so manual
  acceptance requires rebuilding and relaunching this branch: connect a vehicle whose reported
  home differs from the saved planner home, select and reselect PLAN, confirm the Home Location
  fields follow the vehicle home, then add local waypoints and confirm the closing route remains
  local. The separately observed Flight Data bearing-axis zoom behavior remains excluded.

## Flight Planner waypoint viewport stability

- Dedicated branch `fix/flight-planner-waypoint-viewport-stability` starts from merged `master`
  `fb7896112`. Commit `788d49d32` preserves an already initialized Flight Planner viewport across
  waypoint redraws; commit `27a4a899a` adds the SFO-home/Kyiv-mission regression and official
  closing-route style checks; reviewer follow-up `cd13dfbfd` exercises the deferred repair ordering.
- `SetWaypoints` snapshots centre, resolution and rotation only after the planner has established a
  valid viewport. It restores an inline redraw mutation immediately and posts one version-gated
  restore for an extent-reactive mutation queued by the same redraw. Initial startup and first-point
  centring remain unchanged. The official `last -> home -> first` route is still rendered: a distant
  route remains solid and a route whose two home legs are both under 5 km remains dashed.
- Behavior, integration and test reviewers approve exact HEAD `cd13dfbfd`. The focused
  `FlightPlannerViewportTests` pass **3/3**, and `PlannerPortParityTests` pass **59/59**.
  `dotnet build MissionPlanner.csproj -c Release -m:1 --no-restore` succeeds with **0 warnings / 0
  errors**. The complete Avalonia suite ran **1558** cases: **1557 passed** and only the existing
  environment-sensitive `VideoSourceResolverTests.NormalizesCommonStreamSources` `/dev/video0`
  case failed; neither the resolver nor its test changes on this branch.
- The worktree still contains the user's five pre-existing modifications in
  `Drivers/inf2cat.bat`, `Drivers/uninstall_drivers.bat`, `ExtLibs/Mavlink/regenerate.bat`,
  `ExtLibs/Mavlink/updatexmls.bat` and `graphs/updatexmls.bat`; none is staged or included in these
  commits. Claude remains disabled.
- The currently running Debug process loaded older commit `8f63d8d1`, so it must be stopped and
  relaunched from the repository root before manual acceptance. Keep a connected SFO home, pan Plan
  to Kyiv, add at least three waypoints and confirm the local centre/zoom does not change; the long
  solid closing route must remain rendered. The separately observed Flight Data bearing-axis
  behavior while zooming remains excluded and is the next independent bug.

## Flight Data vehicle-overlay marker rendering

- Dedicated branch `fix/flight-data-vehicle-overlay-rendering` starts from pulled `master`
  `5a1c11ea3`. Commit `7379e6dde` scopes the active aircraft triangle to its point feature;
  commit `109872c5e` adds the rendering regression test, and reviewer follow-up `a4f218da0`
  exercises the complete live fixed-wing layer with all four bearing lines and its radius arc.
- Flight Data previously assigned `MavMarker.Vehicle` as the style of the complete `Vehicle`
  layer, then added heading, course, navigation-bearing, target-bearing and turn-radius geometries
  to that same layer. Mapsui consequently painted the aircraft symbol on those geometries too,
  producing duplicated arrowheads and a striped fan along the turn-radius arc. The layer now has
  no shared symbol style, while its aircraft point owns the triangle style; bearing and radius
  features retain only their vector styles. Log Browse sample markers use the same safe path.
- `dotnet build MissionPlanner.csproj -c Release -m:1 --no-restore` succeeds with **0 warnings / 0
  errors**, and the focused `FlightMapOverlayTests` pass **28/28**. The complete Avalonia suite ran
  **1555** cases: **1554 passed** and only the existing environment-sensitive
  `VideoSourceResolverTests.NormalizesCommonStreamSources` `/dev/video0` case failed. Neither the
  resolver nor its test changes on this branch.
- The worktree still contains the user's five pre-existing modifications in
  `Drivers/inf2cat.bat`, `Drivers/uninstall_drivers.bat`, `ExtLibs/Mavlink/regenerate.bat`,
  `ExtLibs/Mavlink/updatexmls.bat` and `graphs/updatexmls.bat`; none is staged or included in these
  commits. Claude remains disabled.
- No code blocker remains. The next executable acceptance step is a fixed-wing SITL flight with
  bearing and turn-radius overlays enabled: confirm exactly one aircraft triangle is rendered and
  the colored guide lines and radius arc remain clean. The separately observed Flight Planner
  viewport jump between a distant persisted home and new waypoints is intentionally excluded from
  this branch and should be handled after this fix lands.

## Flight Data Auto Pan settings parity

- Dedicated branch `fix/flight-data-autopan-settings` carries four granular code/test commits:
  `8540f8b74` restores the official setting behavior, `adea69d75` adds its initial regression
  tests, `e6cf79aee` matches official handling of a malformed present value, and `7d570b224`
  covers the complete absent/true/false/malformed settings matrix.
  The comparison source is the official `ArduPilot/MissionPlanner` repository at commit
  `2b5589f40` (`latest`).
- PR #26 (`https://github.com/Rouniy/MissionPlanner10/pull/26`) tracks this branch. Merge commit
  `cbe4c4db8` brings it onto current `origin/master` `2f99868f2`; its only textual conflict was
  this status file, where both the Auto Pan record and upstream's dual-listener release record
  were retained.
- Official Mission Planner starts `CHK_autopan` enabled, restores an existing preference and
  updates that preference when the checkbox changes. Mission Planner 10 instead initialized the
  generated `AutoPan` property to false and never loaded or stored `CHK_autopan`. A restored
  `maplast_lat`/`maplast_lng` viewport was therefore marked centred and stayed at an old location
  while the live aircraft marker updated off-screen. The port now defaults Auto Pan on without
  manufacturing a setting, restores an explicit true/false value, treats a malformed present
  value as false like official Mission Planner, and persists later changes.
- On the merged result, `dotnet build MissionPlanner.csproj -c Release -m:1 --no-restore` succeeds
  with **0 warnings / 0 errors**. The focused Auto Pan tests pass **3/3**, and the complete
  `PlannerPortParityTests` class passes **59/59**.
- The complete merged Avalonia suite ran **1553** cases: **1552 passed** and one unrelated existing
  `VideoSourceResolverTests.NormalizesCommonStreamSources` case failed because this workstation has
  a real `/dev/video0`. `VideoSourceResolver.Resolve` treats an existing filesystem path as
  `FromPath` before its later V4L2 normalization, while the test unconditionally expects
  `v4l2:///dev/video0`. Neither file is changed on this branch; this is an environment-sensitive
  pre-existing test/implementation mismatch rather than an Auto Pan regression.
- The worktree still contains the user's five pre-existing modifications in
  `Drivers/inf2cat.bat`, `Drivers/uninstall_drivers.bat`, `ExtLibs/Mavlink/regenerate.bat`,
  `ExtLibs/Mavlink/updatexmls.bat` and `graphs/updatexmls.bat`; none is staged or included in these
  commits. Claude remains disabled.
- No Auto Pan code blocker remains. The next executable acceptance step is a fresh WP10 SITL
  launch: confirm
  Flight Data starts with Auto Pan checked and centres on the live aircraft, then uncheck it,
  restart WP10 and confirm the explicit false preference is restored. Run the full suite on a host
  without `/dev/video0`, or address that video test separately, before claiming an entirely green
  repository suite.

## Dual startup MAVLink UDP listeners and Debian handoff

- Work is isolated on `port/avalonia-in-place`, as required by the repository handoff policy.
  Functional commit `ccbfe7740` restores the two upstream default inbound MAVLink listeners at
  application startup. UDP 14550 and 14551 bind immediately without waiting for a heartbeat and
  are registered as two independent `MAVLinkInterface`/secondary-runtime connections. Their
  vehicle lists, reads, writes, disconnect handling and active-link selection therefore remain
  isolated; multiple systems received on one port continue to use the existing per-component
  selector. A silent passive listener no longer makes the primary connection button report a
  false connected session.
- Planner Settings has a separate **Startup UDP Listeners** group with an enabled-by-default
  toggle and validated primary/alternate ports (14550/14551). Invalid persisted values fall back
  to the documented defaults, equal port values create one listener, and changes explicitly take
  effect after restart. Each listener opens its telemetry log only when its first datagram is
  ready, so launches without telemetry do not create empty `.tlog`/`.rlog` files. Binding failure
  on one configured port is contained and does not prevent the other port or the application from
  starting.
- Regression coverage sends two real MAVLink heartbeat streams with different sysids to two real
  loopback UDP sockets and proves that both remain open and that neither vehicle appears on the
  other link. Default, disabled, duplicate-port and invalid-port behavior is also pinned. The
  complete final Release suite passes **1550/1550**; `MissionPlanner.slnx` and the standalone SITL
  harness build with **0 warnings / 0 errors**. All six migration/retirement/artifact gates pass
  (1623 native rows, 0 blockers, 708/708 pinned port paths, no WinForms dependency, and clean
  project/binary/key audits).
- The intentional local sequence increment is isolated in commit `3b88492d1` (`1 -> 2`). The
  clean-tree Debian artifact is
  `out/packages/missionplanner10_1.3.83.2-3b88492d_amd64.deb`: 60,128,072 bytes, Debian version
  `1:1.3.83.2+3b88492d`, installed size 193,792 KiB, SHA-256
  `a8f111e1eb8bbb44f0a73618ecc1275b194fc355168403a7d0968a26b06bbfd1`. `lintian`, launcher,
  executable, airport-resource and forbidden Windows-native-library payload checks pass. An
  isolated extracted-package Xvfb launch reaches the normal event loop (expected timeout 124),
  emits no stdout/stderr or crash log, creates no empty telemetry logs, and `ss` confirms the
  packaged process owns both `0.0.0.0:14550` and `0.0.0.0:14551` simultaneously. This is the
  pre-merge local acceptance package; the GitHub workflow rebuilds every platform artifact from
  the exact final tagged commit and therefore uses that commit's hash in its filenames.
- Release was explicitly requested on 2026-08-31. `port/avalonia-in-place` was merged without
  conflict into `master` by merge commit `55bdaf2b1`; release checkpoint `22ec12fef` is published
  on `origin/master`. Complete CI/package run `33370869010` passes Linux tests/DEB/TAR smoke,
  Windows ZIP/MSI build-install-uninstall and both macOS app/DMG jobs. CodeQL run `33370869031`
  passes on the same commit and the code-scanning API reports zero open alerts.
- Annotated tag `v1.3.83.2-22ec12fe` resolves exactly to release checkpoint `22ec12fef`. Release
  workflow `33371674803` passes tag-contract validation, all four fresh platform builds, signed
  update-manifest generation, checksum aggregation and publication. The stable, non-draft,
  non-prerelease GitHub Release is
  `https://github.com/Rouniy/MissionPlanner10/releases/tag/v1.3.83.2-22ec12fe` and contains 19
  uploaded assets: Linux TAR/DEB/update ZIP, Windows ZIP/MSI/update ZIP, both macOS ZIP/DMG pairs,
  four manifest/signature pairs and `SHA256SUMS`. The checksum file names all other 18 assets;
  every manifest reports `1.3.83.2+22ec12fe`, its published bundle URL/hash/size, and every Ed25519
  signature independently verifies with `build/update-public-key.txt`.
- This final status-only handoff is the sole commit after the immutable release tag; application
  source and released binaries remain at `22ec12fef`. Remaining blocker: none for the release.
  Next executable steps are physical simultaneous-input acceptance on UDP 14550/14551 and, before
  any later release, an intentional `make bump-local-build` from **2 to 3**.

## Native dataflash log core: combined delivery on PR #34 (all four phases on feature/dflog-native-log-core)

- Why one PR: the 2026-09-16 completeness audit above deferred #34 as a phase-1-only change
  with no application consumer and asked for the complete feature, reviewed against the
  then-current master, with managed/native parity, native-required tests and package
  validation for all four RIDs. The stacked branches were propagated 1 -> 2 -> 3 -> 4 on master
  through #38 (`f1180f672`) and merged onto the PR's head branch, so #34 now carries the whole
  feature. The four phase sections below stay as the detailed record of each part.
- Merge notes: two content conflicts, both where #37's TLOG dispatch met phase 3's native
  DataFlash paths (`Services/DataFlashLog.cs`, `ViewModels/LogBrowseViewModel.cs`), resolved by
  keeping the TLOG early return ahead of the DataFlash path in both. `DataFlashLog.ReadFields`
  (phase 3, public) gained the same TLOG guard: a tlog must never reach `DFLogBuffer`, which is
  the bug #37 fixed. Everything else merged automatically.
- The audit's criteria, by name:
  - Managed/native parity: `DflogNativeTests` (index and column parity, 23 cases),
    `DflogNativeConsumerTests` (the converted consumers, 16 cases) and the new
    `DflogNativeMcpParityTests` (4 cases) for the consumer master gained in #36/#37: every
    DataFlash MCP tool - log schema, flight overview, record pages (two pages, since
    pagination rides on the line numbers the index assigns), field series plain and instanced,
    the PARM snapshot, the VIBE report and the ISBH/ISBD spectrum - runs through
    `McpLogCatalog.Read` (the server's cached-reader path) on all four corpus logs, managed
    scan then native scan, and the JSON must be byte-identical. Exact equality is the right bar
    there: those tools read rows through the enumerator and the native scan replaces only the
    index, so any difference is a defect. Every output is real (1.2-1.3k PARM entries per
    log, VIBE sensors, batches on rover and copter-isbd), not a rejection both scanners repeat.
  - Native-required tests: `DFLOG_REQUIRE_NATIVE=1` on the Linux test step turns every parity
    skip into a failure, and the ABI-drift test fails a built-but-unloadable library.
  - Package validation for all four RIDs: the linux-x64 `.deb` payload, the win-x64 ZIP/MSI
    required files, and the Mach-O architecture assert for both macOS RIDs, in the CI and
    release workflows and in both local package scripts.
- Verified on the combined tree (Windows host, Rust toolchain present): `cargo fmt --check`,
  `cargo clippy --workspace --all-targets` and `cargo test` (24+2) clean; Release solution
  build 0 errors, 3 warnings (the pre-existing `PInvoke005` lines in
  `ExtLibs/WinUSBNet/NativeMethods.txt`, AnyCPU on a Windows host, none from this work);
  dflog and MCP test groups 111/111 with `DFLOG_REQUIRE_NATIVE=1`; full suite 1677/1689, the
  12 being the 11 known environment-dependent failures plus `PluginRuntimeTests`, which
  passes on rerun (13/13); all six migration/artifact gates pass on the branch content -
  five in this checkout, and `check-port-source-resolution` (708/708) in a fresh worktree,
  because this checkout's stale CRLF copies of LF blobs trip its byte-identity checks;
  `git diff --check` clean apart from upstream's `MavlinkParse.cs` lines, byte-identical to
  master. Not exercised here: `lipo` - the first CI run on the pushed branch is the proof for
  the two macOS legs.
- 2026-09-16 review of the combined diff (eight angles, every finding verified against the
  code, one empirically). Fixed here: the log browser's rows view had been rebuilt in phase 3 by
  zipping per-field numeric series from `ReadFields`, which emptied the grid and the CSV export
  for any type with a text column (PARM/MSG/MODE showed 0 rows on the corpus against 1373 PARM
  rows on master), formatted every cell as `0.###` (GPS.Lat -35.3632621 became -35.363), and
  decoded the whole log for a 5000-row preview; it is the row-oriented enumerator again (one
  record per row, the decoder's display strings, `Take(maxRows)`), pinned by
  `Rows_view_keeps_text_columns_and_decoder_strings` with and without the library. The
  per-instance `SaveCache` decision read the process-wide `LastScanNative` mirror, which a
  concurrent construction (LogIndexService's parallel opens, the MCP catalog's worker) can
  rewrite; it now reads a method-local. The FFI column offsets were 32-bit products (overflow
  past ~45M rows of a 7-field query); they are 64-bit. The column fast path is gated on `binary`
  like the scan, so a text .log no longer gets an extra native scan and a WARN per field. The
  timed track reads GPS Lat/Lng through one `ReadFields` open instead of two `ReadField` opens.
  Deferred, recorded here: every column-querying buffer still reads and scans the file natively
  twice, once for the index and once for the column reader (since the 2026-09-28 review fixes
  below, both reads go through the buffer's stream and the second is verified against the
  first; keeping the construction reader for short-lived column consumers would remove it);
  the column reader holds a private copy of the file for the buffer's lifetime (a
  positioned-read block reader would bound it; see the 2026-09-28 review bullets below);
  the ISBH/ISBD merge is written in
  both FFT and spectrogram, mirroring master's duplicated enumeration loops. `ReadField` is now
  `ReadFields` with one field: the private per-field core that duplicated the native block is
  gone, and the managed fallback enumerates the log once for every requested field instead of
  once per field, pinned against a direct decoder walk by
  `Read_fields_fallback_matches_the_enumeration_path`. After the fixes:
  dflog/MCP/log-browser/expression groups 139/139 with `DFLOG_REQUIRE_NATIVE=1`; full suite
  1679/1691, the same 11 environment-dependent failures plus the flaky `PluginRuntimeTests` case.
- 2026-09-28 review by obazna (changes requested, three threads). Two were one root cause: the
  native side opened the log by path and memory-mapped it, independently of the managed
  `basestream`. Fixed in two commits:
  - The mapping: on Linux and macOS a log truncated while mapped raised SIGBUS, which no panic
    barrier catches, so the app died before any fallback. dflog-core now reads logs into
    memory, sampling the length at open into one exact, fallible allocation; memmap2 is gone
    (crates 0.7.2).
  - The path reopen: the index scan and the lazy column reader each reopened the path, so a
    path replaced after the stream was opened gave the index or the columns another file's
    records (the reviewer's reproduction: 20,885 plane.bin records for a 31,867-record
    copter.bin stream). Both now read the buffer's own `basestream` into a caller-filled
    native image (`dflog_image_new`, `dflog_open_image`, `dflog_file_index`; ABI 6, crates
    0.8.0; `dflog_scan_file` and `dflog_index_free` removed) over the length sampled when the
    index was built, and the column reader is kept only if its index matches the buffer's
    record for record. A shrunk file or a moved record boundary refuses native columns.
  - Trade-off against the mapping, measured on a 1 GB log (copter.bin concatenated, 26.3 M
    records; Windows, Release, index cache off, medians of 10; mapped / stream read / managed
    only): open 2,754 / 2,952 / 3,704 ms, first column query 565 / 731 / 1,434 ms, later
    queries 54 / 54 / 1,337 ms. The open's temporary copy of the file is freed before the
    managed index peaks, so memory at open does not grow. The cost is the column reader: once
    a buffer has queried columns it holds a private copy of the file until disposed (held
    after the first query, medians of 6 after forced collections: 1,869 / 2,836 / 1,402 MB),
    where the mapped pages were file-backed and reclaimable. A positioned-read block reader
    would bound that, but `Record` borrows payload slices from the whole image, so it is
    follow-up work. The image allocation fails cleanly; the index vectors the scan builds
    (about 9 bytes per record) still abort if memory runs out, which the FFI docs state.
  - rust/ is now dflog's home: the userepo/MissionPlanner fork it began as a copy of is set
    aside, so the vendoring notes in rust/README.md, rust/testdata/README.md and the dflog
    notice are gone.
  - Tests: 6 FFI tests (image round trip, zero length, unopened free, truncated images,
    refused lengths, ABI 6); `DflogNativeTests` +6 (replaced path for the index and for the
    columns, truncation after and before the column reader opened, moved boundaries, a short or
    failing stream into `FromStream`; the two replaced-path ones return early on Windows,
    which refuses to rename over an open file); `DflogBufferCacheTests` +1 (native columns
    verified against a fresh and a cache-loaded managed index). Verified on Windows: `cargo fmt
    --check` and `cargo clippy --workspace --all-targets` clean, Rust 26+2+6, full suite
    1786/1786 with `DFLOG_REQUIRE_NATIVE=1`; the dflog tests, the Linux-only ones included,
    also pass on Linux (WSL, 61/61).
  - The third thread: ConfigFFT's native IMU collection filled the shared state type by
    type, so when IMU succeeded natively and IMU2 then failed, the enumeration fallback added
    IMU's samples a second time, doubling them and skewing the sample-rate estimate. The
    native samples are now staged and copied in only once every present IMU type succeeded.
    `DflogNativeConsumerTests` +1 (a synthetic log whose IMU2 declares GyrZ as `n` text, which
    the native decoder refuses and the managed path parses: native-enabled and managed results
    must be identical); on the old code its frequency table comes out doubled. Full suite
    1787/1787 with `DFLOG_REQUIRE_NATIVE=1`.
- Remaining blocker: none. Next executable step: push the combined branch to PR #34, rewrite
  its title and description as the complete feature mapped to the three criteria above, and
  read the four RID legs of the run, macOS architecture asserts included.

## Native dataflash log core, phase 4: CI and packaging wiring (part 4 of the combined delivery above)

- ci.yml: the Linux test step sets `DFLOG_REQUIRE_NATIVE=1` (the runner has a Rust toolchain,
  so a missing or unloadable native library now fails the parity tests loudly instead of letting
  every one of them skip); the Debian payload check asserts
  `usr/lib/missionplanner10/libdflog_ffi.so`; the Windows ZIP/MSI required-files list gains
  `dflog_ffi.dll`; the macOS job installs both Apple rustup targets (the arm64 runner
  cross-compiles osx-x64) and asserts that `libdflog_ffi.dylib` in the publish output is a
  Mach-O for the RID's CPU (`lipo -archs` = `x86_64` / `arm64`), not merely present.
- release.yml: the package matrix installs the Apple rustup targets for the osx rows and runs
  the same architecture assert right after the macOS publish. Linux and Windows release
  payloads are covered by the same `package.sh` scripts as CI.
- build/linux/package.sh and build/windows/package.sh assert the native library in the publish
  payload next to their existing required-file checks, so every packaging path - CI, release,
  and local - fails loudly rather than shipping a silently managed-parser-only app. A local
  developer without a Rust toolchain still builds and runs the app normally; only packaging
  demands the library.
- Verified: both workflows parse (yaml), both package scripts pass `bash -n`, a local
  `build/windows/package.sh zip` run exercises the new assert against a real publish, and the
  full dflog test set passes locally with `DFLOG_REQUIRE_NATIVE=1` set (the failure direction
  was fault-injected in phase 2).
- 2026-09-16, folded into the combined branch: the macOS checks were upgraded from a size test
  to the architecture assert above. PR #34's first live run (34002587483) had established the
  osx-x64 dylib's architecture only by construction - the library can reach the publish output
  only through the `--target x86_64-apple-darwin` path - and nothing in CI inspected the Mach-O
  header. Now it does, in both workflows, per RID.
- Remaining blocker: none. Next executable step: land the combined branch on PR #34 (the
  maintainer's 2026-09-16 completeness audit deferred the phase-1-only PR and asked for the
  full feature: managed/native parity, native-required tests, package validation for all
  four RIDs), with the measured numbers from the phase-3 checkpoint in the description.

## Native dataflash log core, phase 3: converted consumers (part 3 of the combined delivery above)

- `DataFlashLog.ReadField` takes the native columnar path (value column plus the same time field
  `DFItem.timems` resolves - TimeMS, then TimeUS, then T - with the managed enumeration loop as
  in-place fallback); this feeds LogBrowse curves and the timed track. New `ReadFields` decodes
  one type's fields in a single pass (the timed track's GPS Lat/Lng). The rows view was converted
  onto it too and reverted in the combined-delivery review above - a text table cannot be built
  from numeric series. Seconds are computed with the same two-step division as the managed path
  - a single multiplication by the combined reciprocal differs by 1 ULP and the parity tests
  catch it.
- `DataFlashExpressionEvaluator.Evaluate` (the preset/expression graphing path) gained a
  columnwise evaluation: per referenced type one native decode (fields + time field + instance
  column when referenced), then the exact latest-value merge the enumeration performs, replayed
  in global record order via native linenos - the order-sensitive lowpass/delta functions see the
  same sequence. Any fetch failure falls back to the unchanged enumeration path.
- `ConfigFFTViewModel` collects ISBH/ISBD batch samples (header/data state machine merged by line
  number) and IMU/IMU2/IMU3 series natively; `Spectrogram` gained the equivalent ISBH/ISBD fast
  path (ported from the upstream fork). Deliberately not converted: `OfflineMagFitService` and
  `LogIndexService` (their `CancellationReadStream` wrapper hides the file path by design),
  `ReadTrack`/KML export (wall-clock `item.time` semantics, cold path), `ReadOverlay`/messages/
  parameters (string-valued).
- Value semantics note, also in the code comments: the managed paths parse display strings
  (floats rounded to 7 significant digits); the native paths carry raw decoded values. Parity
  tests bound the difference (timestamps tick-exact; values within display rounding; FFT dB
  magnitudes within 0.01 dB - rounding amplified through bin cancellation and the log scale).
- 'M' (flight mode) fields are excluded from every native path (`DFLogBuffer.GetFieldFormatChar`
  gate in the field reads and the evaluator): the managed decoder renders them as
  resolver-dependent text, natively they are plain numbers, and a graph must show the same thing
  with and without the library. Making both paths numeric (which would make MODE graphable) is a
  deliberate behavior change to raise separately. The FFT/spectrogram batch paths also verify
  that the numeric and array queries describe the same row count before indexing one by the
  other, falling back to the enumeration loop otherwise.
- Tests: `DflogNativeConsumerTests` (16 cases) - field reads, multi-field reads, five expression
  shapes (single type, interleaved types, instanced reference, lowpass, delta), ISBH and IMU FFT,
  spectrogram, the mode-field gate (engagement counter must not move), and an always-on
  fallback-equality test for the multi-field read - each parity case run managed-vs-native with
  the engagement counter (`DFLogBuffer.NativeColumnHits`) proving the fast path took effect.
  Parity cases skip on hosts without the library; with the library removed, the full
  dflog/dataflash/expression set still passes (fallback verified), as does a full-suite run with
  `DFLOG_NATIVE=0`. Full suite 1568/1580; the failures are the known environment-dependent
  set, unchanged from master.
- Measured (medians of 3, warm file cache) over three logs: the 247.7 MiB concatenated SITL
  corpus (6.37M records; 39k ATT / 196k IMU rows) and two real flight logs from Andrew
  Tridgell's public archive (uav.tridgell.net/tmp): 00000145.BIN (139.8 MiB, 2024, 169k ATT
  rows) and 4.0.9Stable_AltitudeRunaway.BIN (259.4 MiB, 2021, 906k IMU rows). Native vs
  managed: open 1.5-2.4x (0.48 s -> 0.20 s on the 259 MiB log); ReadField (one curve, open +
  decode) 1.4-2.0x; rows view (7 IMU fields) 7.6 s -> 0.67 s, 3.2 s -> 0.32 s, and 10.4 s ->
  0.52 s (20x, the dense-IMU real log) - one pass and one open instead of seven, measured on the
  `ReadFields`-based rows view that the combined-delivery review later reverted, so this number
  no longer describes the shipped rows view (a bounded row enumerator); decode alone on
  an open buffer 3-10x; expression evaluation 1.4-1.9x. Row counts cross-checked managed vs
  native before timing on every log. Observation for a later change: every consumer re-opens the
  DFLogBuffer per call, so the open dominates single-curve numbers - a shared buffer per loaded
  log would compound these wins. Logs over 300 MiB currently cannot A/B at all: the managed
  path's unguarded BinaryFormatter SaveCache (the phase-2 latent-bug note) throws on .NET 10.
- Remaining blocker: none for this phase. Next executable step: phase 4 - rustup targets in
  ci.yml/release.yml, `DFLOG_REQUIRE_NATIVE=1` in CI, and packaging assertions for the shipped
  library.

## Native dataflash log core, phase 2: P/Invoke bindings and DFLogBuffer fast paths (part 2 of the combined delivery above)

- `ExtLibs/Utilities/DFLogNative.cs` (ported from the upstream fork, ABI v5): internal P/Invoke
  surface over `dflog_ffi` - availability probe (`Available`, gated on the ABI version), whole-log
  index scan (`TryScan`), and a `ColumnReader` for typed column queries, instance filtering,
  int16[32] array columns and the GPS time base. Every entry point returns false instead of
  throwing; the standard .NET native probing finds the library next to the apphost (no resolver
  needed - unlike GDAL/VLC the library is bundled, not system-provided).
- `DFLogBuffer` gained the native fast paths: `setlinecount` uses the native index scan for
  path-backed binary logs (managed scanner unchanged as fallback), `TryGetColumnsNative` /
  `TryGetArrayColumnNative` (with per-instance filtering and `GetInstanceFieldName`), and
  `DFLog.GetTimeFromMs` for columnwise time axes. `UseNativeScan` precedence: explicit set >
  `DFLOG_NATIVE` env > `dflog_native` setting > on by default; `DFLogNative.Available` gates the
  actual calls, so hosts without the library keep exactly today's behavior. The native path also
  side-steps the index cache (originally the `BinaryFormatter` cache with its latent .NET 10
  throw; master has since replaced it via PR #25 and hardened it in `a4f24e82c` - see the merge
  checkpoint below for how the two compose).
- Consumers deliberately NOT converted yet (phase 3): `LogBrowseViewModel`, `ConfigFFTViewModel`,
  `SpectrogramWindow`, `OfflineMagFitService` (the last needs its `CancellationReadStream` wrapper
  reworked to expose a path before the native path can engage).
- Tests: `DflogNativeTests` (23 cases) - native-vs-managed index parity over the four vendored
  corpus logs AND over derived malformed variants (truncation, mid-record truncation, corrupt FMT
  length byte, garbage prefix flipping the binary sniff - all line-for-line string equality),
  typed-column/instance/array/time-base parity against the managed enumerator, truncated-tail
  behavior, and clean failure for unknown fields/types. All return early on hosts without the
  native library (NativeGdalMapTests pattern; verified both modes), with two guards against
  silent degradation: a built-but-unloadable library (ABI drift) fails a dedicated test, and
  `DFLOG_REQUIRE_NATIVE=1` turns the skip into a failure for hosts that must have the native
  path (set it in CI once phase 4 installs the toolchain). Both failure modes verified by fault
  injection (corrupted library file; library removed with the variable set). The corpus is copied from
  `rust/testdata` at build time - no new committed binaries. Full suite 1554/1565 locally; the
  failures are the known environment-dependent set plus one known-flaky UDP listener test,
  unchanged from master. Project/binary audit gates pass.
- 2026-08-31 merge checkpoint: merged `feature/dflog-native-log-core` (carrying master through
  `1cbe17b87`, which includes the PR #25 index-cache replacement and its `a4f24e82c` hardening)
  into this branch. One conflict, in `setlinecount`, resolved to compose both change sets: a
  native-capable open skips the hardened cache in both directions (never loads, never saves - a
  fresh native index is cheap, and the cache stays a managed-fallback optimization), the managed
  fallback keeps the working cache, and the save/load calls use the new `CacheSourceIdentity`
  signatures. `DflogBufferCacheTests.CacheScope` now pins `UseNativeScan = false` (assembly-wide
  test parallelization is disabled, so the static seams are safe) and a new
  `Native_capable_open_skips_the_cache_in_both_directions` test pins the composition with the
  native library confirmed present. Full suite 1594/1605; the 11 failures are the known
  environment-dependent set, unchanged from master.
- Remaining blocker: none for this phase. Next executable step: phase 3 - convert
  `LogBrowseViewModel` columnar reads, then the FFT/spectrogram ISBD path, re-measuring against a
  large real log; phase 4 wires rustup targets into ci.yml/release.yml for the four RIDs.

## Native dataflash log core, phase 1: vendored Rust workspace and build plumbing (part 1 of the combined delivery above)

- Vendored the dflog parser core from the upstream fork (userepo/MissionPlanner
  `rust/dflog-core` branch, crates 0.7.1) into the top-level `rust/` directory: `dflog-core`
  (parser/index/columnar access, bug-for-bug compatible with `DFLogBuffer`/`BinaryLog`) and
  `dflog-ffi` (`dflog_ffi` cdylib, C ABI v5, panic-safe boundary), plus the SITL corpus in
  `rust/testdata` that the golden characterization tests pin exact values against. The CLI,
  Python bindings and fuzz targets stay upstream; parser changes land there first and are
  re-vendored. `cargo test` (24+2), `cargo fmt --check` and `cargo clippy --workspace
  --all-targets` are clean in this tree.
- `MissionPlanner.csproj` builds the cdylib from source when `cargo` is on the PATH
  (`BuildDflogNative` before `PrepareForBuild`, mirroring `FetchMacSimpleBle`): RID-mapped
  `--target` triples for win-x64/linux-x64/osx-x64/osx-arm64, host-native build when no RID is
  set, output under `obj/dflog/` so neither MSBuild nor manual cargo runs (`rust/target/` now
  gitignored) can dirty the build-identity check. Without cargo the build prints one notice and
  produces today's app unchanged - the managed parser remains the runtime fallback. The built
  library is injected as `Content` before `AssignTargetPaths`, so it flows into output, publish,
  and every package payload; verified end to end with a `win-x64` self-contained publish carrying
  `dflog_ffi.dll` (191 KB, `strip = true` release profile for the lintian gate).
- No binaries are checked in and no new project files exist: `check-project-artifacts.sh`,
  `check-binary-artifacts.sh` and `check-native-surface.sh` all pass unchanged. Third-party
  notice added as `LICENSES/dflog-NOTICE.txt` (memmap2 and the Rust standard library,
  Apache-2.0; version-free filename since the vendored source re-syncs from upstream). Full C# suite: 1532/1544 locally, the failures being the known
  environment-dependent set, unchanged from master.
- 2026-09-05, first live CI run (PR #34): the `package-macos (osx-x64)` leg failed. The macOS
  runners are Apple Silicon, so their preinstalled Rust carries only `aarch64-apple-darwin`, and
  `BuildDflogNative` probed for cargo's presence but not for its ability to build the requested
  target - so a cross-build to `x86_64-apple-darwin` reached cargo and died on `can't find crate
  for core` (MSB3073), failing the application build. The other three legs passed because each
  one's triple is its runner's native target. Two fixes: the macOS job now installs both Apple
  targets (the step phase 4 already carried, pulled forward byte-identically so the stack merge
  stays a no-op), and the skip condition is now target-aware, since a toolchain that cannot build
  for the requested RID is the same situation as no toolchain at all - the documented graceful
  degradation had a hole that also hit any Apple Silicon developer publishing `osx-x64` locally.
  The target probe reads `rustup target list --installed` from `rust/` so the toolchain file picks
  the same toolchain cargo will use; it matches by substring and treats an unreadable list as
  "attempt the build", so an unexpected reading fails loudly through cargo instead of silently
  skipping the native library. All three paths verified locally: an uninstalled target
  (`osx-x64` on this Windows host) prints the notice and exits 0, an installed target
  (`win-x64`) still builds the cdylib, and a host-native build with no RID is unchanged.
- Remaining blocker: none for this phase. Next executable step: phase 2 - port `DfLogNative`
  P/Invoke bindings into `ExtLibs/Utilities` on the `NativeGdalApi` availability pattern, add the
  `DFLogBuffer` native fast paths, and cover them with synthesized-log parity tests that skip
  when the native library is absent. The remaining CI wiring for the four RIDs (the
  `DFLOG_REQUIRE_NATIVE` test gate and the packaging payload assertions) is phase 4.

## NV4 parameter-catalog synchronization and Debian handoff

- The Hermes source checkpoint is clean GTU `master == origin/master`
  `3eebb35d6d35be5b5fb4c1a753017baff107b082` (`Support legacy NV4 refresh parameter`). Its exact
  change from `310ca309` in the three `NV5Settings` source/test files has SHA-256
  `8cb610d33eebeac454c336eafec77fd7b374fb7c85ab61561a7c9b8cb0d22d66`; Mission Planner did not
  modify the GTU tree. The canonical 56-name NV4 sketch catalog and its parameter-specific
  descriptions are now pinned locally, including the real automatic-watchdog, SBUS routing,
  network-octet, RF-statistics, radio-role and currently unused legacy-field semantics.
- Both firmware apply spellings are supported end to end. `REFRESH_SETTING` and the older
  `REFRESH_SETTINGS` are recognized as NV4 signatures, hidden/read-only controls and documented
  aliases. Ordinary saves and 32-byte key transactions write the exact parameter name and MAVLink
  type advertised by the selected modem; when unusual firmware advertises both, the newer singular
  name wins deterministically. The runtime table still follows the connected modem's advertised
  catalog rather than fabricating parameters from the pinned reference list.
- Code/documentation/test commit `f73d0ef44` and local-sequence commit `69886aeab` are published on
  `master` and `origin/master`. Focused NV modem tests pass **60/60**, focused version/updater tests
  pass **48/48**, and the complete Release suite passes **1544/1544**. `MissionPlanner.slnx` and the
  standalone SITL harness both build with **0 warnings / 0 errors**. All six
  migration/retirement/artifact gates pass (1623 native rows, 0 blockers, 708/708 pinned paths,
  clean WinForms, project, binary and key audits); the optional live-source form of the 708-path
  check could not be repeated because the pinned `MissionPlanner-Avalonia` worktree is no longer
  present locally, while its committed digest check passes.
- The first earlier `make linux-deb` attempt encountered the known local logical/physical checkout
  alias in stale MSBuild intermediates. A standard Release
  clean followed by the physical-path package script resolved it without source changes. The final
  clean-tree package is
  `out/packages/missionplanner10_1.3.83.1-69886aea_amd64.deb`: 60,129,156 bytes, Debian version
  `1:1.3.83.1+69886aea`, installed size 193,776 KiB, SHA-256
  `833c5b6536bda483d088d13e4482e7f6999acef4dd83c692280e42d0fce38cd3`. `lintian`, launcher,
  executable, resource and forbidden-native-library payload checks pass; isolated Xvfb startup
  reaches the normal event loop (status 124 is the expected 12-second timeout). Earlier local
  `.7202` packages are superseded test artifacts and were never tagged or released.
- The only change after the published code/package checkpoint is this status/provenance record.
  After its push, `HEAD == origin/master` and the worktree is clean. Remaining acceptance boundary:
  exercise both refresh spellings on representative physical NV4 hardware; GTU's per-endpoint
  direct/HUB failover remains a separate parity item because Mission Planner's shared parser does
  not expose sender/listener metadata. GitHub CI/package run `33326289139` and CodeQL run
  `33326289105` both pass on `69886aeab`; the code-scanning API reports zero open alerts. Manual
  cross-platform release build `33326726258` also passes on that exact commit and retains four
  downloadable Actions artifacts (`dist-linux-x64`, `dist-win-x64`, `dist-osx-x64` and
  `dist-osx-arm64`) with signed update manifests. Because it was a `workflow_dispatch` rather than
  a tag build, it did not create a tag or public GitHub Release. Next executable step: perform the
  physical NV4 save/key roundtrip before a tagged release.

## Explicit local build sequence and completed integration handoff

- The upstream version remains `1.3.83` from `Properties/AssemblyInfo.cs`. The tracked,
  repository-global local sequence lives in `build/local-build-number.txt` and now starts at **1**
  by explicit user decision. The provisional value `7202` came from taking the next number after
  the old implicit `git rev-list --count HEAD` result of 7201; it was a compatibility bridge, not a
  meaningful local release number, and is superseded. `make bump-local-build` performs one guarded
  atomic increment; ordinary developer builds never modify the counter, and it must not be reset
  when the upstream version changes.
- Version-contract commit `a8e060d41` and sequence correction `69886aeab` apply the contract
  consistently: product/file and UI version `1.3.83.1+<8-char-hash>`, artifact/tag identity
  `1.3.83.1-<hash>`, Debian version `1:1.3.83.1+<hash>`, MSI product version `1.3.1`, and macOS
  short/build versions `1.3.83` / `1`. Release tags are rejected unless upstream version, tracked
  local number and tagged commit hash all match. The updater compares the fourth numeric field,
  retains the date-based compatibility path and compares the full eight-character build hash.
- The guarded bump helper was exercised from a clean counter (`1 -> 2`) and restored to `1` after
  verification. The next intentional local release therefore starts by committing `2`; upstream
  version changes remain independent of this sequence. The earlier version-contract checkpoint
  passed complete CI/package run `33324097392`, CodeQL run `33324097344` and the zero-alert API
  audit; the final `69886aeab` CI, CodeQL and manually dispatched release matrix pass as recorded
  above.
- Git state for handoff: the only commit after the published code/package checkpoint is this
  status-only record; after its push, `HEAD == origin/master` and the worktree is clean. PR #24 is
  closed as merged with its review/follow-up comment, and `feature/model-calibration-support-20260830`
  is an ancestor of `master`. No tag or GitHub Release was created. Remaining blocker: none. Next
  executable release step: run and commit `make bump-local-build` (`1 -> 2`), then tag the resulting commit
  using `v<upstream>.<local-build>-<8-char-hash>`.

## Log download hardening checkpoint (branch fix/log-download-cancel-tests)

- `MAVLinkInterface.GetLog` gained cancellation (`CancellationToken`, threaded through the
  download loop and the view model, with a Cancel button in the download window), repair-request
  chaining (the next missing-range request is issued the moment the current one is satisfied,
  gated on actual coverage progress so duplicated packets cannot multiply requests), and a
  time-based silence budget (`retryLimit x LogRetryDelayMs`, so short repair windows no longer
  shrink the total tolerance a flaky link is allowed). Data beyond the known log end is ignored.
- `LogDownloadTracker` only trusts a short packet as the log end at the highest offset seen, and
  only frontier-near packets raise that bar - a stale short retransmit cannot truncate the
  download and a corrupt far offset cannot poison end inference.
- A short packet past the trusted frontier is a deferred end candidate, promoted by `GetLog`
  only once the stream goes quiet (`AcceptPendingTotalLength`): a corrupt packet both short and
  far cleared the old bar trivially and ended the download at a phantom length - below the true
  end it silently truncated the returned file. A genuine end packet still lands far past a
  frontier stalled by packet loss, so rejecting far end packets outright is not an option (that
  variant never completed the lossy SITL run - every recovered gap forced a full re-stream).
  Found via review of the equivalent upstream change (ArduPilot/MissionPlanner#3764).
- The download window toolbar wraps (`WrapPanel` with `ItemSpacing`/`LineSpacing`, window
  `MinWidth` 420): six items need ~830 px in one row and previously painted past the 540 px
  default width. A `LayoutOverflowTests` theory guards it at 420 and 540 px.
- New coverage: `GetLogProtocolTests` (15 fake-vehicle protocol tests: ordering, loss recovery,
  stray retransmits, corrupt short-far packets, duplicate storms, silence budget, beyond-end
  data, cancel, timeout), tracker unit tests, and a manual SITL end-to-end harness with a 5%
  lossy proxy (`MissionPlannerTests/Avalonia/MissionPlanner.SitlTests`, registered as retained
  tooling in `PROJECT_ARTIFACT_AUDIT.tsv`). SITL reference results: clean 2.3 MB byte-identical
  in 0.44 s; 5% loss byte-identical in 77.3 s, one streaming pass (~3 s of that is the silence
  window confirming a deferred end candidate; previously the lossy run did not complete - repair
  served one gap per 3 s silence window).
- PR #24 head `6257c6c53e5c6cef8069e7dd397e09e946634d21` was reviewed and integrated
  locally into `master` by merge commit `0ef837654`. Review follow-up `fee3c2dc3` keeps the UI
  recoverable when the native file/folder picker fails and prevents a cancellation arriving
  during the final synchronous KML export from being reported as success once export returns.
  The complete Release suite passes **1531/1531**; `MissionPlanner.slnx` and the standalone SITL
  harness both build with **0 warnings / 0 errors**; the proxy parses; and all six migration,
  retirement and artifact checks pass (1623 native rows, 0 blockers, 708/708 pinned paths).
- The fork PR workflows remained `action_required`, so they never executed on the contributor
  head. The reviewed integration was pushed instead: GitHub records PR #24 as `MERGED` with merge
  commit `0ef83765432b04db2ed3af70aa87d8081fdcfd52`, and the PR contains a review comment listing
  the accepted behavior, follow-up and verification. Master head `02e8f38f4` passed the complete
  Linux/Windows/macOS x64/macOS arm64 CI/package workflow and CodeQL. Rerun the clean + 5%-loss
  SITL harness after any further `GetLog`/tracker change, per its README.
## Custom ArduCopter model-calibration mode

- Branch `feature/model-calibration-support-20260830` is based on clean published `master`
  `58b2ea4c5a192abd6fe71a11cc67eac7837e3781`. It adds this fork's `ModelCal` flight mode to
  the Copter mode catalogue as custom mode **31**, so live mode selection, MAVLink translation,
  configuration selectors and DataFlash mode labels all resolve the same number even when the
  bundled upstream parameter metadata predates the custom firmware.
- Mode 29 was deliberately not reused: the current MAVLink `COPTER_MODE` enum assigns it to
  `RATE_ACRO`; ArduCopter reserves 30 for offboard control. Regression tests pin `ModelCal` to 31
  and protect against accidentally mapping it back onto 29.
- Source commit `715e8115e` was merged into `master` on top of the complete PR #24 integration by
  merge commit `a9788c244`. On the combined tree, focused Flight Data/DataFlash checks pass
  **58/58**, the complete Release suite passes **1533/1533**, and both `MissionPlanner.slnx` and
  the standalone log-download SITL harness build with **0 warnings / 0 errors**. All six porting
  gates pass (1623 native rows, 0 blockers, 708/708 pinned paths, clean WinForms and artifact
  audits). No package, tag or release was created.
- The combined merge/status commits were pushed to `origin/master`. CI/package run `33323244170`
  and CodeQL run `33323244173` both passed on `adedcf62b`; the later version-contract checkpoint
  above provides the final combined verification.
## Ten-second Flight Data action bounds and compact Actions layout

- The Flight Data `Arm / Disarm` action now uses an explicit ten-second total MAVLink ACK bound
  for ordinary and forced requests. The timed low-level path releases `giveComport` on expiry, so
  a silent vehicle cannot leave the action disabled for the former four ten-second attempts.
- `Resume Mission` applies the same bound to waypoint reads and mission-current confirmation and
  keeps one captured link/system/component for the complete sequence. GUIDED, ARM and TAKEOFF are
  each sent once and then state is polled within the remaining ten-second step budget; the former
  loop could resend a blocking ARM or TAKEOFF command every second and greatly exceed its nominal
  timeout. The already-read resume waypoint is also reused instead of requesting it twice.
- The Actions grid remains faithful to the official five-column placement but can shrink further:
  action buttons use smaller font/padding/minimum height, inter-column spacing is reduced, and the
  right-hand speed/altitude/loiter pairs are proportional two-column grids instead of fixed-width
  wrapping panels. Numeric editors and their buttons now contract together rather than moving the
  right-hand button onto a second row while unused horizontal padding remains.
- Focused Flight Data, MAVLink timeout and mission-protocol tests pass **50/50**. The complete
  suite passes **1492/1492** and the Release solution builds with **0 warnings / 0 errors**. One
  first full run hit the pre-existing timing-sensitive DroneCAN empty-response assertion; it then
  passed alone ten consecutive times and the complete retry was green. Claude remains disabled.

## Cancellable/partial parameter loading and Open Drone ID shutdown checkpoint

- Primary connection initialization now distinguishes transport opening from parameter loading.
  Cancel/closing the progress window still aborts a port that has not opened, but once MAVLink is
  open the action becomes `Skip Parameters`: the parameter reader is cancelled and awaited while
  the transport, logs and telemetry connection remain active. A parameter timeout/failure likewise
  no longer converts a valid connection into a disconnect. The configuration loading page also
  provides `Stop Loading` and retains `Retry Now`.
- Fresh parameters for the exact selected target are now visible incrementally in `Full Parameter
  List`. That page bypasses the complete-list overlay, refreshes its safe snapshot twice per second,
  preserves user-staged edits as later packets arrive, and shows a red bottom warning with the
  received/expected counts until complete. Cancelling retains received values; a new session,
  target switch or explicit retry still clears old values first. Specialized configuration pages
  remain gated until the complete list because many treat a missing parameter as a default value.
- Writing one already-received parameter from a partial list is supported and no longer forces an
  automatic full refresh afterward. The confirmation/result explicitly says that parameters not
  displayed were not changed and leaves a manual full refresh available.
- Open Drone ID shutdown now treats a serial/network `ReadLine` interrupted by closing the port as
  normal cancellation. `StopCoreAsync` also observes and contains faulted background tasks instead
  of allowing `ObjectDisposedException: The port is closed` to escape through `AsyncRelayCommand`
  and terminate Mission Planner 10. A deterministic blocking-read/closed-port regression test
  covers the reported stack.
- Focused connection/parameter/OpenDroneID tests pass 105/105; the full suite passes 1489/1489 and
  the Release solution builds with 0 warnings and 0 errors.

## SITL AutoTune feedback and Copter Circle-overlay checkpoint

- The reported Copter SITL `AutoTune` failure was traced through the live telemetry log rather
  than inferred from the UI. MissionPlanner sent the correct ArduCopter custom mode `15`; SITL
  `ArduCopter V4.8.0-dev (01a504b4)` answered `Mode change to Autotune failed: init failed`.
  That firmware permits AutoTune entry only while armed/airborne with non-zero throttle and from
  Stabilize, AltHold, PosHold or Loiter; Circle deliberately does not opt into AutoTune entry.
- Set Mode now preserves a stable link/target for the request, watches the target's `STATUSTEXT`
  and current mode for two seconds, and reports confirmed success, explicit vehicle rejection or
  missing confirmation instead of always claiming only `Requested`. A Copter AutoTune request
  whose known current state cannot satisfy the firmware contract is stopped with actionable
  guidance before sending; ready airborne requests and Plane AutoTune remain unaffected.
- The moving pink arcs seen behind a Copter in Circle mode were a real Avalonia parity bug. The
  port applied `CurrentState.radius` to every vehicle, whereas official Mission Planner gives its
  dynamic turn-radius arc only to fixed-wing/VTOL markers. Quad, helicopter and rover markers no
  longer receive it. The retained fixed-wing/VTOL arc now also converts display-distance units
  back to metres before creating Mapsui world geometry.
- Focused mode/map checks pass 71/71; the complete suite passes 1478/1478 and the Release solution
  builds with 0 warnings and 0 errors.

## Flight Data Actions and joystick-dialog parity checkpoint

- Flight Data hardware Actions no longer terminate the process on a transport failure. `Message`
  treats the legal default/disconnected `MAVLinkInterface` state (`BaseStream == null`) as closed;
  arm/disarm, mission restart/resume, waypoint, mode, and abort-landing operations convert timeout
  or connection-closure exceptions into a visible error and return control to the UI. Resume
  Mission also validates its waypoint and no longer reports success after an incomplete sequence.
  Regression tests cover null transport, timeout reporting, and initial command availability.
- Branch `fix/actions-panel-parity` was compared directly with the current official Mission
  Planner checkpoint `67a3c4f22bd1b38ac499f9756902e04fa4ed8444`. Upstream has two distinct
  controls: `BUT_joystick` in the five-column Actions table opens
  `new JoystickSetup().ShowUserControl()`, while `but_disablejoystick` is a separate map overlay
  that appears only while joystick output is active. The early Avalonia port incorrectly replaced
  the first role with an always-visible `Disable Joystick` action.
- The Actions tab now follows the official five-column/five-row placement for its action, waypoint,
  mode and mount selectors; Auto/Loiter/RTL, home/restart/raw-sensor/arm, joystick/message/resume,
  clear-track/abort and numeric speed/altitude/loiter controls. The port-specific MAVLink message
  rate tool remains available in a collapsed expander below the official surface.
- The layout follow-up also restores upstream's five equal 20-percent columns. Avalonia buttons
  explicitly stretch to fill each table cell, the speed/altitude/loiter value editors use bounded
  controls that wrap like upstream's `FlowLayoutPanel`, and the surface shrinks with the resizable
  Flight Data pane without horizontal scrolling. On a wide pane the table and message-rate
  expander use the full available surface, while each command button is capped at 110 pixels;
  excess room is distributed between the five columns instead of leaving a dead strip at the
  right. This removes the irregular gaps, mixed button widths and displaced numeric fields seen
  in the first parity implementation.
- `Joystick` now opens a separate modeless `JoystickSetupWindow`, matching upstream
  `ShowUserControl()` behavior instead of navigating to Setup. The window directly hosts the full
  native port of `JoystickSetup` (`ConfigJoystickView` plus `ConfigJoystickViewModel`), keeps an
  enabled joystick active after close just as Mission Planner does, and disposes only its UI
  subscriptions/timers. Repeated clicks activate the existing window rather than creating
  competing setup sessions. The separate map-overlay `Disable Joystick` button releases control
  and RC overrides and is visible only while the global joystick service is active.
- A real Xvfb/XTest run opened Flight Data -> Actions and the modeless Joystick window, confirmed
  the table layout and full axis/device controls, and closed both windows normally. Release build
  passes with **0 warnings / 0 errors**; focused Flight Data tests pass **32/32** and the complete
  suite passes **1459/1459**. Headless UI regressions lock the upstream grid/button roles,
  direct window content, modeless visibility and single-window behavior. Claude remains disabled
  by user instruction. The initial parity checkpoint is merged into local `master` at `6f983d21c`;
  no push, tag or GitHub release has been performed for it.

## Live UDP/NV, SITL/X-Plane and Mission Planner 10 checkpoint

- Local branch `fix/udp-nv-connect-state` is at code checkpoint
  `5e6c1c8fc0e5234e243e7e914c56e63190757315`. The new atomic commits retain the first SITL TCP
  connection (`f0e407dca`), drain short packets from persistent UDP listeners (`1813353ef`),
  restore and filter the complete NV parameter cache (`2d4205e6e`), apply the Mission Planner 10
  product/installer identity (`8caed8f30`), migrate legacy SITL/SRTM caches while preserving Unix
  executable modes (`4145b5bb1`) and reject reflected GCS RTSP requests as modem identities
  (`5e6c1c8fc`). The managed assembly intentionally remains
  `MissionPlanner.dll` for plugin ABI compatibility; apphosts, install directories and package
  names are Mission Planner 10. The official Mission Planner filesystem map-tile location remains
  shared and is not migrated or renamed.
- The final UDP-reader stall was traced backwards to the initial native Avalonia import
  `c68d7f366`: its generic stream loop required `BytesToRead > 10`. `UdpSerial` exposes datagram
  availability, so a short packet at the head of a bound listener could prevent every later
  broadcast from being consumed. `402548abc` fixed the separate initial parameter-load ordering,
  `8519ff5b9` correctly made inbound UDP persistent but exposed the inherited reader starvation as
  an open-yet-dead socket, and `c13d14962` fixed a separate successful-progress-dialog cancellation
  that released a newly opened transport. The current policy uses any positive byte count for a
  bound UDP listener while retaining the legacy threshold for TCP/UART/UDP client streams; it is
  used by both primary and secondary connection runtimes.
- Real hardware verification used the actual shared UDP port 14565 under Xvfb and physical XTest
  clicks through `SETUP -> NV Modem -> Refresh selected`. The socket stayed `Recv-Q=0`, `drops=0`
  for the 30-second observation and through repeated parameter refreshes. The page listed exactly
  five vendor-confirmed broadcasts and no autopilot: NV5 `10:18` (91/91), `255:11` (91/91), `6:6`
  (77/77), `255:10` (59/59) and `1:5` (77/77). Cached `MAVState.param` values are replayed only
  after an NV custom message, valid modem-info payload or strict legacy NV4 CAN passport confirms
  the exact system/component endpoint; parameter names alone can no longer classify an autopilot
  as a modem. Closing the live window produces no former `CancellationTokenSource` disposal stack
  or core dump and the final Xvfb/metacity run exits 0.
- A follow-up hardware report exposed a sixth false device at `255:190`, Mission Planner's own
  `MAV_COMP_ID_MISSIONPLANNER` endpoint. The captured tlog proved that the reflected custom frame
  was an outbound `NV5_RTSP_CONFIG` read request (`operation=0`), not modem status. Read/write
  operations 0/1 can no longer establish identity; only the modem report operation 2 can. This is
  payload-direction filtering rather than an ID blacklist: a real modem at `255:190` is still
  accepted when it sends a valid report. A second live dropdown check shows only the five physical
  modems and keeps `Recv-Q=0`.
- The X-Plane/SITL regression came from using a temporary readiness connection and then opening a
  second TCP session. External simulator models can accept/associate the first session, so the
  launcher now retains that proven connection and hands it directly to Mission Planner. The child
  PATH includes the SITL cache and working directory for companion runtime files. A real cached
  Linux `ArduPlane --model xplane` integration run passes, including legacy-cache migration and
  retained-stream consumption; no SITL process remains after the test.
- Verification at the code checkpoint: Release solution build **0 warnings / 0 errors**; complete
  suite **1457/1457**; focused connection/NV suite **94/94**; real X-Plane integration plus cache
  migration **2/2**; all six migration/inventory checks pass (1623 native rows with 0 blockers,
  708/708 source paths, no WinForms, and clean project/binary/key audits); all 28 active projects
  report no known vulnerable direct or transitive NuGet package. One earlier full-suite run had a
  transient Avalonia headless `Dispatcher.PushFrame` platform exception in a shapefile test; that
  test passed immediately in isolation and the following two complete builds/runs passed, so it is
  recorded as test-harness flakiness rather than hidden.
- Fresh clean local artifacts are
  `out/packages/missionplanner10_1.3.83-20260825.5e6c1c8f_amd64.deb` (60,115,816 bytes, SHA-256
  `81592f6cc4ac788fc20fdc94c459b7f97a69a7eb818459b571da51f6d147e71f`) and
  `out/packages/MissionPlanner10-1.3.83-20260825.5e6c1c8f-linux-x64.tar.gz` (75,376,646 bytes,
  SHA-256 `a6810274a4f79dd52a36cf6f3be6c707c54fc2ca995006c60face03f46cd81f0`). `lintian
  --fail-on error,warning`, gzip/TAR content checks, extracted DEB layout and a 12-second packaged
  Xvfb smoke pass; the expected smoke timeout is 124 with no stderr, unhandled exception or crash
  log.
- Publication was explicitly re-authorized after the known UDP/NV and SITL/X-Plane regressions
  were fixed and verified. Merge this checkpoint into `master`, require Linux/Windows/macOS CI
  plus CodeQL to pass on that exact merge commit, and only then tag it so the release workflow
  creates the GitHub Release and attaches the native Linux, Windows and macOS packages. Claude
  remains disabled by user instruction.

## Shared UDP/NV connection-state fix

- Stacked branch `fix/udp-nv-connect-state` contains the earlier DroneCAN fix, initial UDP
  publication fix `402548abc11141016ec65dcd0a8543e196820ec7`, and persistent-listener fix
  `8519ff5b9`. Fresh hardware traces first proved that the inbound UDP listener on port 14565
  continued receiving NV broadcasts while the main button remained `CONNECT`: synchronous initial
  parameter loading had selected an arbitrary first endpoint and delayed logical connection
  registration until that endpoint answered.
- Inbound `UdpSerial` parameter loading is now always background work. A valid MAVLink open can
  therefore mark the shared transport connected, expose `DISCONNECT`, start the reader and publish
  the link to the NV Modem page without requiring the first broadcast `sysid:compid` to implement
  or answer `PARAM_REQUEST_LIST`. The configured policy for point-to-point UDP client, TCP and UART
  connections is unchanged.
- A second hardware run exposed the independent disconnect regression. Two fresh logs contained
  681 and 673 valid frames from multiple NV endpoints over about six seconds, including valid
  traffic immediately before teardown. The cause was the generic ten-second silent-link policy
  introduced in the old Avalonia port by `51cd6ca969861fa712742c23d04afc461bc2117b`
  (`feat: expand vehicle control and telemetry parity`) and imported into the native tree by
  `c68d7f36675748fa7f80f4f880ab71e9fe42c8a4`. It treated a bound UDP discovery socket as a
  point-to-point vehicle session and closed it when an exclusive parameter read ended after the
  selected target had been silent.
- Inbound `UdpSerial` is now a persistent listener: device silence and repeated packet-reader
  errors set link quality lost but never close the bound socket. A returning modem is discovered
  without reopening the port. Explicit Disconnect/shutdown and an actually closed OS socket still
  end the logical connection. The same rule covers primary and Connection List UDP listeners;
  `UdpSerialConnect`, TCP and UART retain the existing dead-link cleanup.
- This mattered beyond the NV page: teardown reset every vehicle parameter list, closed telemetry
  logs, stopped speech, removed the link from `MavLinkConnectionManager` snapshots, raised the app
  as disconnected and therefore hid the source from NV discovery, DroneCAN, map/multi-link and
  connection-gated services. The scoped policy prevents those cascades for a merely silent inbound
  listener without weakening point-to-point failure handling.
- Three deterministic regressions cover the silent listener policy, repeated reader errors and a
  real UDP socket that remains bound while a modem disappears and then accepts it when it returns.
  A temporary live-hardware test also held the real port 14565 open for 20 seconds past the former
  failure point. Focused connection tests pass **58/58**, the complete suite passes **1445/1445**,
  and the Release solution builds with **0 warnings / 0 errors**. Clean local packages are
  `out/packages/missionplanner_1.3.83-20260825.8519ff5b_amd64.deb` (60,115,152 bytes, SHA-256
  `5ebc58e044703140492ff3dc819127fd8df47db74473640b0caad80cc5fde18f`) and
  `out/packages/MissionPlanner-1.3.83-20260825.8519ff5b-linux-x64.tar.gz` (75,373,478 bytes,
  SHA-256 `0071fe7d249dcbb1dcd28bd4c422a85394ad3d073154a4a6566800a6545d11c2`). `lintian`, extracted
  payload assertions and the 12-second Xvfb smoke pass (`timeout` exit 124).
- The fixes and packages are local only. No push, merge, tag or release was performed, and Claude
  remains disabled by user instruction.

## DroneCAN refresh and parameter-response fix

- Branch `fix/dronecan-refresh-parameters` starts from the clean released `master` checkpoint
  `294968844e7dc703f87f75724a4a5c96e52ef46b`.
- Refresh now clears both the visible nodes/selection and the protocol `NodeList`/`NodeInfo`
  discovery caches. Every fresh `uavcan.protocol.NodeStatus` upserts the visible row, so a node
  cannot remain permanently hidden merely because its ID was already present in the library cache.
- Parameter enumeration preserves the existing `GetParameters(byte)` API and adds response-aware
  diagnostics. A node that never answers the optional `uavcan.protocol.param.GetSet` service is
  reported as a timeout/unsupported service, while a valid terminal empty response is reported as
  a responding node with no configurable parameters. The temporary response handler is always
  detached, including exceptional send paths.
- Three regression scenarios cover cache reset plus rediscovery, a status-only node that does not
  implement the optional parameter service, and a valid empty parameter catalogue. Focused
  DroneCAN tests pass **13/13**; the complete suite passes **1441/1441**. The Release solution
  builds with **0 warnings / 0 errors**, and all six migration/inventory checks pass (1623 native
  rows with 0 blockers, 708/708 source paths, no WinForms, and clean project/binary/key audits).
- Claude remains disabled by user instruction.

## Post-release code-quality audit round 4

- Branch `audit/code-quality-round-4` starts from clean released `master`
  `0b1456049351420ee925d0176c76d01722febf4d` and remains deliberately unmerged in draft PR #19.
  The current reviewed implementation checkpoint is `bb90162de`; this status update follows it.
- `b3796ba50` removes a completion/start race shared by the Python and Lua hosts. An exclusive
  operation lease now owns exactly one cancellation source, so an old script cannot dispose the
  source of a newer script; disposal cancels the active run, rejects restart and prevents the local
  Lua REPL from surviving its page.
- `e7f799629` makes ESP8266 setup activation-scoped and target-stable. It waits for a complete
  parameter response with cancellation and a bounded timeout, reports missing fields instead of
  throwing from an unobserved constructor task, validates IPv4 input, captures one system ID for
  the whole write and requires every `setParam` to succeed before EEPROM persistence/reboot.
- `4ec1d5f3e` removes unsolicited compass-motor calibration ACK packets when an idle page closes,
  serializes Start/Finish transitions, stops a calibration that completed while its page was being
  disposed and always releases its MAVLink subscription independently of transport failures.
- `1d9b50305` makes closing any progress window request cancellation before disposing its source.
  The cached token remains readable during late cleanup, avoiding invisible connection/update work
  and the corresponding disposed-source race.
- `fad728f47` constrains every signed update-manifest path to the install/staging roots, rejects
  duplicate/rooted/traversal paths and invalid sizes, enforces exact signed download lengths and
  removes partial or hash-invalid files. `23acf5b74` gives the two legacy TCP output pages a
  cancellation-aware host owner: a new client closes the superseded client and no connection can
  be attached after page shutdown.
- `caa77f8a7` replaces SharpKml's direct deprecated code-page provider 5.0 with the supported
  10.0.11 package. The remaining deprecation report is intentionally limited to xUnit 2 required
  by `Avalonia.Headless.XUnit` and legacy transitive `System.*` packages below the compatible
  netstandard2.0/net472 plugin graph; none is reported vulnerable and a major test/plugin ABI
  migration is not mixed into this reliability branch.
- `5b760281c` derives three distinct Secure AP key paths even for uppercase or missing extensions,
  writes private key material atomically and restricts its Unix mode to `0600`. `bb90162de` rejects
  secure-command data/signature combinations above MAVLink's 220-byte wire limit instead of
  truncating the signature while declaring its original length; concurrent commands now receive
  atomic sequence numbers.
- `6dd060f5a` owns and awaits the CoT output/accept tasks, closes accepted TCP clients on stop,
  ignores late UDP/TCP/UI callbacks, supports immediate same-port restart and makes disposal
  idempotent. `c316b0234` rejects non-HTTPS/downgraded SITL downloads, bounds binary, metadata and
  compressed/expanded manifest input, preserves the previous cache file on failure, removes
  partial files and bounds KML/KMZ document/resource decompression. `e9df0be66` passes Linux/macOS
  external-open targets as one literal argument and releases every short-lived launcher process
  handle, including updater helpers.
- Fresh GTU `master` and `origin/master` at `bfb03b24f3180fdcba91ac7f114e7287b7b2b359`
  were compared with the previously synchronized `ab6422d4` checkpoint. Its only new production
  NV5Settings delta is mirrored here: RX/TX role presets are disabled when already selected, and
  ENABLE/SUPPRESS TX follows the live `tx_state` instead of offering a redundant command. Manual
  staged role edits refresh both controls immediately, and direct command invocation cannot bypass
  the same-state guard. The two firmware reference repositories named by GTU are not available in
  the local AgroSky workspace, so this synchronization is intentionally based on the clean fetched
  GTU repository itself.
- The active application graph was also checked for TLS validation bypasses, unsafe
  deserialization, archive/path traversal, unbounded HTTP ownership, SSH trust-all behavior,
  process/socket lifetime and primary native GDAL/SimpleBLE/libVLC ownership. No certificate
  validation bypass or active unsafe formatter was found; SFTP remains fail-closed TOFU/pinned and
  revalidates remote regular BIN files before download/delete.
- Local verification of the current branch: Release solution build
  **0 warnings / 0 errors**, **1438/1438** tests, all six migration/inventory checks pass (1623
  native rows with **0 blockers**, 708/708 pinned source paths, no WinForms, and clean project,
  binary and key audits), all 28 active projects report no known vulnerable direct or transitive
  NuGet package. Clean local Linux TAR/DEB packages
  `MissionPlanner-1.3.83-20260825.bb90162d-linux-x64.tar.gz` and
  `missionplanner_1.3.83-20260825.bb90162d_amd64.deb` build; the DEB passes `lintian`, payload
  assertions and a 12-second Xvfb event-loop smoke (`timeout` exit 124).
- Claude remains disabled. The next step is to push the updated draft PR #19 and require its
  Linux/Windows/macOS package matrix and CodeQL checks to pass. Do not merge it or create a release
  until the user reviews and explicitly requests that action.

## Current state

- The complete in-place Avalonia migration and audited cleanup were merged to `master` through
  PR #1 at merge commit `eb6cfe28f`. The later GTU `NV5Settings` synchronization was merged through
  PR #2, the independent diversity-radio key correction through PR #4, and the clean CI identity
  correction through PR #5 at master checkpoint `d273ca8aa`. PR #6 carries the final GTU
  `NV5Settings` refinement (`639a19acc`), focused CodeQL/AES follow-up (`c3265e3e8`) and explicit
  secure dependency declarations (`2e26a52a3`). PR #8 removes the obsolete MetadataExtractor build
  graph and resolves the remaining CodeQL findings, while PR #9 adds verified macOS DMG packaging;
  the resulting release checkpoint is `9ed911535`. Native baseline `67a3c4f` remains the immutable
  rollback reference in Git history.
- Upstream safety/reliability PR #13 is merged at `b24238dbc`; its complete Linux, Windows and
  macOS package matrix and CodeQL run passed, and release `v1.3.83-20260824.b24238db` contains all
  19 expected DEB/TAR/MSI/ZIP/DMG, signed update-manifest and checksum assets. Round 2 was merged
  through PR #14 at master checkpoint `0266a2878`; its master package run `32764653113` and CodeQL
  run `32764653058` passed with zero open alerts. Round 3 remains isolated on
  `fix/upstream-issues-round-3` until its own CI and review complete; no new tag or release may be
  created until every round-3 check has passed on `master`.
- The root `MissionPlanner.csproj` is now the net10 Avalonia application with assembly and product
  identity `MissionPlanner`. It builds one main `MissionPlanner.dll` and has no source, build or
  runtime dependency on an `external/MissionPlanner` tree.
- 519 application-owned files were imported directly into the canonical native paths from pinned
  port commit `8ed19081`, with source blob provenance in `IMPORTED_APPLICATION.tsv` and an explicit
  transitional compile allow-list in `ImportedApplicationItems.props`.
- The root project references the native `ExtLibs` directly. CoreCLR fixes are applied directly to
  native `Settings`, `MAVState`, Radio uploader/IHex and Grid sources; there is no generated patch
  assembly.
- The official legacy plugin ABI is compiled into the main `MissionPlanner` assembly. The separate
  `MissionPlannerAvalonia.PluginApi.dll` is only the distinctly named portable plugin contract, so
  the output does not contain a second compatibility `MissionPlanner.dll`.
- `MissionPlanner.slnx` and 115 imported test-source files now live below
  `MissionPlannerTests/Avalonia`; their five adapted projects reference the root application and
  native libraries directly. The UDP transport fixture uses an ephemeral listener port instead of
  colliding with a live modem on 14550.
- Six pinned historical audits are preserved in `Porting/Reference` with a blob manifest. They are
  migration evidence, not a copied source tree.
- A clean Release build of the complete test graph succeeds with zero warnings and zero errors
  after resolving all 156 inherited `ExtLibs` diagnostics without a repository-wide `NoWarn`; the
  decisions and reproduction commands are recorded in `WARNING_AUDIT.md`. All **1379/1379**
  Avalonia tests pass on Linux. A 12-second Xvfb launch reaches the normal Avalonia event loop with
  no console errors.
- Informational version is derived from the current native Mission Planner version and formatted as
  `1.3.83+YYYYMMDD.<commit>`; dirty developer builds append `.dirty`.
- CI packages explicitly pin that clean commit identity before compilation. Runner-local files
  created by restore/build therefore cannot add a misleading `.dirty` suffix to Linux or Windows
  package names or to the application metadata embedded in any of the four platform builds; CI
  rejects a package if that suffix reappears.
- Native-identity packaging is integrated for Linux `.tar.gz`/`.deb`, Windows portable ZIP/MSI and
  macOS x64/arm64 ZIP/DMG. All four RID publishes pass their native GitHub runners; the Linux
  packages pass `lintian` and extracted-DEB Xvfb smoke, Windows CI performs a real MSI
  install/validation/uninstall, and each macOS job verifies, mounts and inspects its DMG. Both macOS
  outputs contain architecture-correct pinned VLC/SimpleBLE runtimes. Details and signing
  boundaries are in `RELEASE.md`.
- Stable and beta auto-updates now select signed manifests directly from this fork's GitHub
  Releases. The matching Ed25519 private key is present only as the repository secret
  `UPDATE_SIGNING_KEY`; the committed public key is verified again during release.
- NV key handling is synchronized through GTU `NV5Settings` commit `77af510a` on clean GTU
  checkpoint `6c2a4b04`. NV5 accepts exactly 32 hexadecimal
  digits, displays uppercase, and maps the 16 raw bytes to four big-endian MAVLink `INT32` words.
  Ordinary Save writes edited words as exact typed `PARAM_SET` operations; explicit SET KEY uses
  the idempotent post-persistence `NV_ENCRYPTION_KEYS_SET`/`NV_ENCRYPTION_KEYS_ACK` transaction.
  Receive diversity does not mirror or couple keys: generation, staging and SET KEY target only
  the selected radio, allowing different keys on Radio 1 and Radio 2. NV4 generation now uses 32
  random bytes displayed as 64 uppercase hexadecimal digits, retains compatible printable/hex
  input, writes eight signed words plus singular `REFRESH_SETTING`, and locks ineffective
  `ENC_KEY_BITS` edits to 128.
- The firmware pages now retain the official modern and legacy safe upload paths: APJ/PX4/VRX
  bootloader upload with board-id matching, STM32 DFU/HEX/BIN, and APM1/APM2 STK500/STKv2 with
  readback verification. The Legacy manifest selector exposes platform and a functional format
  filter, including the still-published APM HEX images. Explicit target/port selection replaces the
  unsafe multi-device assumptions in old `BoardDetect`; obsolete Parrot/Solo network installers are
  reported as unsupported rather than routed to the wrong programmer.
- Flight Data local scripting now preserves the official IronPython 3.4.2 `.py` workflow and its
  live `MAV`/`cs`/`Ports`/`Script` bindings. Output is streamed into the Avalonia page and Abort uses
  a cooperative per-line trace hook instead of the original unsupported `Thread.Abort`. The local
  MoonSharp console remains available as a separate optional Lua tool.
- Flight Planner's visible live KML workflow is restored by an on-demand, loopback-only read server
  with bounded headers, responses and concurrency. It serves live vehicle/mission KML and the
  aircraft model on port 56781 while deliberately excluding the old public bind, guided-mode HTTP
  writes, raw MAVLink WebSocket, Mavelous host and WinForms/GDI MJPEG capture surface.
- CI, CodeQL, Dependabot and tag-release workflows are reconciled with the in-place tree. The EOL
  Xamarin/Uno/Blazor/Windows Store application experiments and their manual mobile workflows are
  removed on the cleanup branch; supported packaging remains Avalonia desktop for Linux, Windows
  and both macOS architectures.
- Post-merge master CI run `32713804092` passed the complete build/test graph, Linux DEB/TAR with
  lintian and extracted-payload smoke, Windows ZIP/MSI with real install/file validation/uninstall,
  and both macOS archives. Master CodeQL run `32713804084` also passed; the five open results are
  the same fully triaged findings recorded before merge, with no new NV5Settings finding.
- NV5 diversity hotfix master CI run `32716876527` and CodeQL run `32716876412` both passed, but the
  CI package run was superseded for distribution because runner-local state leaked `.dirty` into
  its Linux and Windows filenames. The clean-CI identity gate above is the corrective action; those
  superseded files are diagnostic artifacts rather than release candidates.
- Clean-identity master CI run `32719669739` and CodeQL run `32719669757` both passed. Its Linux,
  Windows and application metadata use clean `1.3.83-20260824.d273ca8a`/
  `1.3.83+20260824.d273ca8a` identities with no false `.dirty` suffix.
- PR #6 code checkpoint CI run `32721719954` passed Linux build/tests/DEB/TAR, real Windows
  MSI install/uninstall plus ZIP, and both macOS packages. CodeQL run `32721719966` passed and the
  branch-specific code-scanning API reports zero open alerts.
- PR #8 CI and CodeQL passed before merge. Master CodeQL run `32730238333` passed and the repository
  code-scanning API reports **zero open alerts**: the obsolete EXIF source is no longer compiled and
  the WinZip AES transform retains its required counter-mode output without an ECB construction.
- PR #9 CI run `32730683963` passed the complete Linux, Windows and two-architecture macOS matrix;
  CodeQL run `32730683985` passed. Cross-platform release dry-run `32730719818` independently
  produced Linux DEB/TAR, Windows ZIP/MSI and macOS x64/arm64 ZIP/DMG artifacts successfully.
- The frozen native inventory remains complete in `NATIVE_SURFACE.tsv`, while replaced WinForms
  sources are explicitly mapped to tested Avalonia artifacts and selected source files whose
  behavior is fully superseded have been removed. RESX translations remain preserved. The manifest
  now exposes **0** `unported-blocker` rows. The old WiX generator is explicitly mapped to the
  current WiX 5 packaging/version/CI implementation; its private upload commands and
  certificate/DPInst custom actions are intentionally retired. The experimental Dowding project
  was never selected by the upstream solution build; its general tracker, CoT and multi-vehicle map
  workflows are ported while the dormant proprietary integration is classified in
  `Reference/DOWDING_AUDIT.md` and removed with its generated clients/ONVIF dependency on the
  cleanup branch. Replaced standalone projects and alternate application/build-system remnants are
  classified in `PROJECT_CLEANUP_AUDIT.md` before deletion.
- The final artifact pass classifies every inactive project/solution, committed binary and
  key/certificate container. Six generated WinForms `.datasource` files, obsolete project trees,
  stale binary duplicates and unreferenced development keys are removed only through explicit,
  machine-checked audit rows. Operator scripts, `Lib.zip`, the swarm Blender authoring helper,
  generators, X-Plane bridge and conditional Windows payloads remain for documented reasons.
- The old Python 2/py2exe automatic log analyzer has been replaced in-process. Its 17 enabled
  official diagnostics now run cross-platform, have deterministic regression tests and report
  missing data independently; optical-flow recommendations no longer write a parameter file
  silently into the working directory.
- Claude remains temporarily disabled by user instruction.

## Upstream safety and issue audit

- Branch `fix/upstream-safety-reliability` adapts the applicable parts of upstream PRs #3728,
  #3740, #3710, #3715, #3724, #3679, #3705, #3222, #3603, #3752, #3750/#3722, #3250 and #3646.
  The changes preserve guided altitude frames, bound serial enumeration, harden MAVFTP/MJPEG/HTTP
  parsing and resource ownership, use current guided commands, lease camera/gimbal message rates,
  correct mission ACK behavior, snapshot proximity state safely, detect Septentrio ports, expose
  compass-calibration failures, expire stale pre-arm failures and report Windows-blocked plugins.
  Each adaptation is native to the Avalonia/CoreCLR architecture and has focused regression tests;
  obsolete WinForms-only implementation details were not copied.
- The 59 open bug-labelled upstream issues and the 100 most recently updated open issues were
  triaged against the live port, including linked commits and PRs. Two additional reports were
  confirmed in current code and fixed: #3461 now tracks actual DataFlash byte ranges, keeps progress
  monotonic, repairs bounded gaps, times out cleanly and always ends the MAVLink log session; #3694
  atomically preserves MAVLink signing keys, migrates every available legacy MAC-derived identity
  to persistent `authkeys.key` material and refuses to overwrite an unreadable `authkeys.xml`.
- Rejected transfers are recorded by reason rather than silently copied. Examples: #3736 targets
  the retired ZedGraph/WinForms viewer; #3658 and #3601 target Mono RESX/GStreamer paths absent from
  Avalonia; #3516 targets the retired WinForms internet firmware picker; #3472/#3391 target old
  MAVFTP rename/drag-drop UI not exposed by the port; #3734 is already stricter because the current
  server is loopback-only and has no guided/raw endpoints. #3746 must be corrected in ArduPilot's
  parameter metadata itself: its current `AC_AttitudeControl_Heli.cpp` still declares
  `HOVR_ROL_TRM` as `0 1000`, so overriding it only in Mission Planner would create conflicting
  safety metadata.
- The project audit exposed an incomplete earlier cleanup: only the placeholder key and empty
  assembly file had been removed from `ExtLibs/MetaDataExtractorCSharp240d`. The remaining 117-file,
  2004-era source project had no solution, source or runtime consumer; GeoRef/Survey use the pinned
  maintained `MetadataExtractor` package. The complete obsolete tree is now removed and the project
  and key audits record that decision.
- First-round checkpoint verification: Release solution build **0 warnings / 0 errors**,
  **1344/1344** tests,
  all six porting/inventory checks pass, the native manifest has **0 blockers**, and every active
  project reports no known vulnerable direct or transitive NuGet package.

## Upstream issue audit round 2

- Branch `fix/upstream-issues-round-2` contains eleven atomic changes on top of clean master
  `b24238dbc`. Confirmed open reports are fixed in the native Avalonia paths: #3761 decodes the
  complete numbered `MAVn_DEVID` range; #3600 keeps `DO_LAND_START` as a marker but removes it from
  flown routes, distances and prefetch corridors; #3492 deduplicates MAVFTP `@SYS`; #3419 preserves
  DroneCAN string parameters safely; and #3447 removes the obsolete off-spec
  `VIDEO_STREAM_INFORMATION99` interval choice.
- Upstream PR #3455 was not copied literally: its display-unit-dependent storage would still make
  travelled distance and `mAh/km` change when users switch units. The adapted implementation keeps
  metres internally and converts only at the property boundary. Likewise, PR #3723's WinForms
  float-format workaround is unnecessary because Avalonia composes bitmasks as `long`/`double`; a
  regression test proves bit 24 remains exact. From the stale mixed PR #2648, only commit
  `bb97e9004d66ec83d2b0894fca9f041e7f71bcc5` was semantically adapted, with first-delimiter
  parsing, exact string preservation and rejection of invalid numeric writes.
- #3305 now hides and never writes the Plane-style global loiter radius for Copter; a zero-radius
  Copter `LOITER_TURNS` remains a panorama and draws no false circle. #3589 renders the enabled
  legacy Copter horizontal fence (`FENCE_ENABLE`, `FENCE_TYPE` bit 1 and `FENCE_RADIUS`) around a
  valid home. #954 adds the exact `LOITER_TURNS` orbit length to mission distance while deliberately
  avoiding an unsupported estimate for `LOITER_TO_ALT`. #3717 removes obsolete Rover
  `WP_OVERSHOOT`/`NAVL1_*` fields and exposes current deceleration, steering-angle and horizontal
  position/velocity controller parameters with metadata tooltips.
- Linked changes were rejected when their assumptions no longer held. Closed PR #3706 subscribed
  to compass failure STATUSTEXT that ArduPilot ultimately never emitted; this port already has the
  final `MAG_CAL_STATUS` values 8-10, human-readable diagnostics and tests. #3595, #3335, #3394,
  #3675, #2641, #2965, #2301 and #3241 are already fixed by the current lifecycle/read-only/raw
  forwarding/bounded-value paths. #3658, #3736, #3601, #3516, #3472 and #3391 target retired
  Mono/WinForms/ZedGraph/GStreamer surfaces. #3746 belongs in authoritative ArduPilot metadata;
  #3747 is vehicle telemetry scheduling; and reports without a reproducible protocol-safe change
  such as #3408 were not patched speculatively.
- Local verification for this round: Release solution build **0 warnings / 0 errors**,
  **1379/1379** tests, all six migration/inventory checks pass with **0 blockers**, and all 28 active
  projects report no known vulnerable direct or transitive NuGet packages. Cross-platform package
  and CodeQL gates remain required on the draft PR before merge.

## Upstream issue audit round 3

- Eleven atomic changes on `fix/upstream-issues-round-3` address confirmed reports and narrowly
  applicable upstream PRs. #3535 now sends the irreversible `PARACHUTE_RELEASE` action instead of
  disabling the parachute. #2923 hides Plane-only waypoint and loiter radii for both Copter and
  Rover and prevents those pages from writing the hidden parameters. #3358 prevents a closed
  mission-command selector from changing commands through navigation keys or the mouse wheel.
- Parameter handling now preserves protocol and file precision. #3284/#2842 comparisons use
  `MAV_PARAM_TYPE`: integer values compare exactly, while REAL32 values tolerate only C
  `FLT_EPSILON`; explicit compare dialogs also expose parameters missing from either side without
  allowing a missing row to be staged. #2884 honours explicit bytewise parameter encoding ahead
  of firmware-family heuristics, so values such as `UINT32` 60180513 survive both PARAM_SET and
  parameter-list decoding exactly. The applicable validation from upstream PR #2644 rejects
  DroneCAN numeric values outside the node-reported range while leaving string parameters intact.
- #3366 retains the latest non-empty SBS transponder squawk for a bounded 30-second interval.
  #2758 changes DataFlash parameter browsing from a final-value dictionary to the complete
  chronological `PARM` history while retaining final-value `.param` export. The functional part of
  upstream PR #3735 adds Pixhawk 6C Windows USB MAVLink and SLCAN interfaces to both driver
  architectures, with packaging authoring coverage.
- #2363 needs no port change: survey speed is already a `double` edited in 0.1 m/s increments and
  emitted unchanged as `DO_CHANGE_SPEED`. The pre/post concurrent batch allocation described by
  #2986 is also already represented by separate ISBH instance slots `0/1/3/4`, with an additional
  bounds guard absent from the legacy viewer; the issue's two old Dropbox links now return HTML
  rather than the logs, so no unverified spectral rewrite was made.
- A release-time GTU comparison found newer clean checkpoint `f196ea689`. Its NV5 signal semantics
  are now mirrored: an unlocked receiver displays current channel RSSI and suppresses stale packet
  RSSI/SNR; a locked receiver prefers packet RSSI and safely falls back to channel RSSI. SX127x and
  LR11xx cases plus locked fallback are covered by regression tests.
- Round-3 local verification: Release solution build **0 warnings / 0 errors**, **1400/1400**
  tests, all six migration/inventory checks pass (708/708 pinned source paths, 1623 native-manifest
  rows and **0 blockers**), and all 28 active projects report no known vulnerable direct or
  transitive NuGet package. Clean `linux-x64` TAR/DEB packages build, `lintian` emits no diagnostic,
  payload assertions pass, and the extracted DEB reaches the normal Avalonia event loop during a
  12-second Xvfb smoke test. Native Windows and macOS packaging plus CodeQL remain mandatory CI
  gates before merge and release.

## GTU synchronization checkpoint

- NV modem behavior was most recently compared with a clean GTU checkout,
  `master == origin/master` at `3eebb35d6d35be5b5fb4c1a753017baff107b082`.
  Earlier checkpoints remain represented: `77af510a` keeps key targeting independent of
  `DIVERSITY` and supplies **Revert selected**; `f196ea689` supplies unlocked-channel RSSI
  semantics; and changes through `d74f4308` supply acquisition presets, frame validation, typed
  rejection diagnostics, attached-modem management and UID2 identity migration.
- GTU `263218e8` adds redundant per-endpoint management routes and pre-write identity/IP collision
  checks without changing the parameter catalog. Mission Planner continues to use the exact
  observed `MAVLinkInterface` as its route boundary because its shared parser does not expose GTU's
  per-datagram sender/listener metadata. The later `3eebb35d` NV4 catalog, descriptions and both
  advertised refresh spellings are synchronized and verified as recorded in the current handoff
  above. NV5 key words remain signed `INT32` values preserving the same raw bytes.
- Before each later NV modem change and before a release, recheck both committed and uncommitted
  GTU changes with `git status`, then compare every newer change to `hermes-gui/include/nv5settings.h`,
  `hermes-gui/src/nv5settings.cpp` and `hermes-gui/test/testnv5settings.cpp`. Update this commit and
  the NV regression tests whenever the source behavior advances.

## Cleanup audit

- Completed functional commit `eaf456665` passed packaging run `32685680444`: real default-path MSI
  install/file checks/uninstall, Linux DEB/TAR with lintian and extracted-payload smoke, and both
  macOS architectures all succeeded. CodeQL run `32685680428` succeeded with zero open alerts.
- `cleanup/project-audit` was the isolated review branch and was merged through PR #1 after the
  explicit user decision. It removes closed WinForms sources,
  replaced standalone projects, EOL alternate application stacks, generated proprietary API
  clients and obsolete launch/deploy/CI/binary artifacts; exact decisions and retained areas are in
  `PROJECT_CLEANUP_AUDIT.md`.
- `MissionPlanner.slnx` now names the complete active transitive graph. Its Release build has zero
  warnings/errors, analyzer verification has zero diagnostics (the .NET 10 workspace-loader notices
  are documented separately), NuGet reports no vulnerable packages, the native manifest has zero
  blockers and all 1263 tests pass after cleanup plus the later NV5Settings and security regression
  coverage.
- Clean-commit Linux TAR/DEB and Windows ZIP packaging succeeds after cleanup. The DEB passes
  `lintian`, payload assertions and a 12-second Xvfb launch; the Windows archive contains the
  expected self-contained `win-x64` application. CI run `32688021866` also passes Windows ZIP/MSI
  build, default-path install/file checks/uninstall, both macOS architectures and all Linux gates;
  all five named artifact bundles are present.
- The formerly open CodeQL findings were resolved without repository-wide suppression. The old
  unreferenced MetadataExtractor source graph containing the reported export flows was removed in
  favor of the already-used maintained NuGet dependency. SharpZipLib's WinZip AES transform now
  implements its state-cancellation primitive without constructing ECB while preserving fixed
  WinZip AES-128/AES-256 vectors and round trips. Master CodeQL run `32730238333` confirms zero open
  alerts.
- The two remaining Secret Scanning warnings were inherited Mapbox values in removed official
  Mission Planner/Xamarin/Cesium history. At the user's explicit request they are resolved as
  `wont_fix`, not falsely marked revoked; the audit comments preserve the origin and the decision
  not to rewrite published upstream-derived history.
- Every inbound UDP listener now enables address sharing/reuse before bind. This lets one modem
  broadcast reach Mission Planner and GTU/Hermes listeners bound to the same local port; a
  regression test opens two real sockets and proves that both receive the same datagram. Ordinary
  unicast fan-out remains explicitly unsupported without separate ports or a MAVLink router.
- `Scripts/`, localization RESX, NoFly data, the X-Plane/HIL bridge and independently meaningful
  remaining library/generator projects are deliberately retained; non-inclusion in the active
  solution alone is not deletion evidence. The former `ExtLibs/mono` submodule was removed only
  after every reference was shown to come from the retired WinForms project graph.

## Immediate next step

Push `fix/upstream-issues-round-3` and run its complete CI/package and CodeQL gates in a PR. Merge
only after Linux, Windows, both macOS architectures and CodeQL pass; then repeat those gates on the
actual master merge commit. Only after the master checks and zero-alert API audit pass may the
master commit be tagged and its full 19-asset release verified. The remaining hardware acceptance
work still requires
representative physical NV4/NV5 hardware: repeat UDP/TCP/UART switching, disconnect and
key-programming checks, and recheck GTU `NV5Settings` changes newer than clean checkpoint
`f196ea689` before declaring hardware acceptance complete.

## Acceptance baseline

- At least 1400 port tests retained and passing.
- Clean Release build has zero errors and zero warnings.
- `linux-x64`, `win-x64`, `osx-x64`, and `osx-arm64` publish gates pass.
- Linux `.deb`/portable archive, Windows ZIP/MSI and both macOS ZIP/DMG pairs build and pass their
  native package validation.
- CodeQL has no untriaged alerts.
- No live source/build/runtime reference to `external/MissionPlanner` remains.
- Parameter/session safety, NV modem behavior, speech serialization, airport alpha, movable Flight
  Data splitter, plugin lifecycle and localization coverage do not regress.
