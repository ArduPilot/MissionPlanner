# Mission Planner on Linux

The Linux x86_64 download contains the compiled application, its managed
dependencies, Mono compatibility facades and XML serialization helper, native
Skia library, and a launch script. Users do not need to install the .NET SDK,
Mono compiler, NuGet, Git, or a source checkout.

## Download and run

Download `MissionPlanner-linux-x86_64.tar.gz` and its `.sha256` file from the
[Linux development release](https://github.com/ArduPilot/MissionPlanner/releases/tag/linux-latest).
Pull requests and manual builds also provide a `MissionPlanner-linux-x86_64`
artifact on the GitHub Actions **Linux Build** run page. The release becomes
available after the workflow is merged and successfully runs on master.

Install runtime dependencies on Ubuntu 22.04/24.04 or Debian:

```sh
sudo apt update
sudo apt install mono-runtime ca-certificates-mono libgdiplus fonts-dejavu-core \
  libmono-system-windows-forms4.0-cil libmono-system-web-extensions4.0-cil \
  libmono-system-management4.0-cil libmono-system-servicemodel4.0a-cil \
  libmono-system-data-entity4.0-cil libmono-system-net-http4.0-cil
sha256sum -c MissionPlanner-linux-x86_64.tar.gz.sha256
tar -xzf MissionPlanner-linux-x86_64.tar.gz
./MissionPlanner-linux-x86_64/run.sh
```

Other distributions need equivalent Mono 6.8+ WinForms/runtime assemblies,
libgdiplus, fonts, and an X11 session (or XWayland). This archive does not bundle
the Mono runtime or the system C library. No commands install dependencies automatically.
Serial connections require permission to access the device, typically membership
of the `dialout` group. Optional video and speech features need GStreamer and
speech software respectively. Windows driver installers and GDAL bindings are
not included. Update by extracting a new Linux archive.

## Build from source

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0),
then on Ubuntu/Debian:

```sh
sudo apt install mono-devel rsync
./Linux/build.sh
./Linux/run.sh
```

Run the scripts from any working directory. The build uses the desktop targets
of multi-targeted dependencies and does not require the Mono source submodule.
It does not rewrite project files or install software. Additional `dotnet build`
options such as `--no-restore` can be passed to `build.sh` (keep Release).
`MONO_POSIX_PATH` can override the installed Mono.Posix assembly location.
`MONO_FACADES_PATH`, `MONO_MCS_PATH`, and `MONO_COPYRIGHT_PATH` override the runtime
facades, bundled serializer compiler helper, and license notice when Mono is
installed in a nonstandard location.
Standard NuGet variables such as `NUGET_PACKAGES` are respected.

Output:

* `bin/linux/MissionPlanner-linux-x86_64/` — runnable application.
* `bin/linux/MissionPlanner-linux-x86_64.tar.gz` — redistributable archive.
* `bin/linux/MissionPlanner-linux-x86_64.tar.gz.sha256` — checksum.

`BUILD-INFO.txt` records the source revision, working-tree state, and toolchain.
CI builds on Ubuntu 22.04 and tests the extracted archive in an Ubuntu 22.04
container with runtime packages and a virtual X display, without a system
compiler or SDK installed. The test loads FlightData, checks its action controls,
and round-trips an XML theme without connecting
to a vehicle. Local GUI testing can use Xephyr with `DISPLAY` set to its nested
display; the launch script respects that display.

To run the packaged smoke test on a nested Xephyr display:

```sh
DISPLAY=:101 ./Linux/run.sh --self-test
```

Maintainers can also test the archive in a clean runtime-only Docker container:

```sh
MISSIONPLANNER_TEST_DISPLAY=:101 ./Linux/test-package.sh
```

Without `MISSIONPLANNER_TEST_DISPLAY`, the container uses its own Xvfb display.
