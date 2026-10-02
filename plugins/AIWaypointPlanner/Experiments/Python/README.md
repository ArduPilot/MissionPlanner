# Python route-geometry experiment

This developer experiment checks whether a small deterministic part of the plugin
can be expressed clearly in Python. It uses Python 3.9+ and the standard library.
It is not loaded by the production plugin and adds no production dependency.

`route_preview.py` reads JSON from a file or standard input and prints preview
coordinates as JSON. Its `legs` fields mirror `TaskSpec.RelativeLeg`; `home` is an
explicit experiment-only input corresponding to `MissionContext.Home`.

From this directory, run:

```powershell
python .\route_preview.py .\sample.json
python -m unittest -v test_preview
python .\verify_host.py --host-dir 'C:\Program Files (x86)\Mission Planner'
```

Replace `python` with the path to an installed Python 3 interpreter if needed.
The last check requires Windows PowerShell 5.1 and matching installed Mission
Planner assemblies. It loads `MissionPlanner.Utilities.dll` in a separate
PowerShell process and calls the real C# `PointLatLngAlt.newpos` method for each
leg. It does not launch Mission Planner. It compares fixed edge cases and 64
seeded random routes, checks unchanged altitudes, and prints the reference
assembly version and SHA-256 for reproducibility. A failed check returns a
non-zero exit code.

Comparison tolerances are `1e-9` degrees for latitude/longitude and `1e-9` metres
for altitude. Both runtimes parse and serialize JSON doubles, so exact equality
of every final bit is not expected. The runner reports measured maximum errors.

The reference runner starts only its own PowerShell child with process-scoped
`-ExecutionPolicy Bypass` so the checked-in adapter works when local scripts are
disabled by default. It does not change the machine or user execution policy.

## What is equivalent

The geographic calculation follows
`ExtLibs/Utilities/PointLatLngAlt.cs::newpos`: spherical direct calculation using
radius **6,378,100 metres**. The plugin calls this method sequentially from
`MissionCompiler.AddRelativeWaypoints` and assigns each leg's altitude.
The experiment follows those same two operations. It deliberately preserves
the host's unwrapped longitude when crossing the antimeridian. This is a
compatibility comparison, not a claim of geodetic accuracy or safe flight paths.

The preview accepts only `relative_route`. It checks basic data types, finite
numbers, Home coordinate bounds and non-negative distances. Other task fields
are ignored, including clarification status, takeoff, cruise speed, completion
action and safety notes. Empty routes are allowed for geometry testing.
Output always contains `preview_only: true` and `safety_validated: false`; it
contains no MAVLink commands or Mission Planner import format.

This is not a replacement for `MissionValidator`, task confirmation, survey-grid
generation, airspace/vehicle constraints or operator review. It does not access
APIs, credentials, files mentioned inside the input, or a flight controller.

## Language assessment

Python makes JSON parsing and a small numerical preview concise, and is suitable
for offline data-processing tools, fixtures and geometry experiments. This
experiment does not establish that rewriting the full application is simpler.
The plugin's production code also handles WinForms, asynchronous cancellation,
host menu integration, localization, document extraction, credential storage,
protocol compatibility and validation; none of those are replicated here.

Mission Planner loads .NET plugins and this plugin targets .NET Framework 4.7.2.
A Python version would still need a .NET/C# host bridge, a deployed Python
runtime, JSON/IPC versioning, cancellation and timeout handling, process-crash
recovery and validation of results crossing the process boundary. A second
runtime also increases packaging and test work. Retaining C# for the installed
plugin and using Python for isolated development tools is the recommended
approach for the current requirements.

## Recorded local result

On 2026-09-07, Python's 12 preview tests passed. The installed Mission Planner
1.3.83 (assembly version `1.3.9679.3011`) reference check passed for **69 routes and
399 points**, including zero distance, antimeridian crossings, polar crossings
and seeded multi-leg routes. Maximum latitude/longitude difference was
`2.84e-14` degrees; maximum altitude difference after the JSON round trip was
`1.14e-13` metres. The runner also rejects non-finite comparison results.

Reference `MissionPlanner.Utilities.dll` assembly version: `1.0.0.0`.
Reference SHA-256:

```text
8648A33EF7CD4004B87FF5FD4C78B015271EA26E0923D3839F083F0FB6E15782
```

No `.cs` file is introduced by this experiment. PowerShell is used only as an
offline adapter to the installed reference assembly; it is not an alternative
plugin implementation. Generated `__pycache__` directories must not be committed.
