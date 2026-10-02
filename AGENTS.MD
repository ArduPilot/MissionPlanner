# AGENTS.md

This file gives repository-specific instructions for agents working in `ArduPilot/MissionPlanner`.

## Scope and priorities

- Keep changes tightly scoped to the user request.
- Prefer the smallest safe fix over broad refactors.
- Do not “modernize” frameworks, package versions, project layout, or naming conventions unless the task is explicitly about that.
- Avoid drive-by cleanup in this repo. It is large, Windows-first, and contains a mix of application code, libraries, plugins, generated/designer files, installers, scripts, drivers, and copied runtime assets.

## Environment you should assume

- Mission Planner is developed primarily on **Windows** and the README says **Visual Studio 2022** is the recommended build environment.
- Use the repo-provided `vs2022.vsconfig` rather than guessing workloads/components.
- Initialize submodules before building.
- The repository contains an `ExtLibs/mono` submodule and the solution references projects under that tree.
- VS Code may parse the codebase, but the README explicitly says it is **not sufficient for building** Mission Planner.

## Build and validation

Preferred validation path:

1. Windows machine
2. Visual Studio 2022
3. `git submodule update --init`
4. Open `MissionPlanner.sln`
5. Build `MissionPlanner`

If a properly provisioned Windows CLI environment is available, mirror CI behavior as closely as practical instead of inventing a new build flow.

Useful CI facts:

- CI builds `MissionPlanner.sln` in `Release | Any CPU` on Windows.
- CI clears NuGet locals, initializes submodules, restores/builds with VSBuild, and archives output from `bin/Release/net461`.
- Do not assume the app is a modern .NET 6 desktop app just because CI installs a .NET 6 SDK. The main app project targets .NET Framework and outputs to the legacy `net461` release folder.

Minimum expectations before finishing a code change:

- Build the solution, or at least the directly affected project(s), in the proper Windows/VS environment.
- If your change touches existing tests or testable logic, run `MissionPlannerTests` as well.
- If your change affects packaging, startup assets, plugin loading, drivers, or copied content, verify the expected files land in the output directory.

## Repository map

Treat the repo as a large Windows desktop solution with several layers:

- `MissionPlanner.sln` — main solution.
- `MissionPlanner.csproj` — primary WinForms application.
- `MainV2.cs`, `GCSViews/`, `Controls/` — core UI/shell/view logic.
- `ExtLibs/` — a large set of supporting libraries and shared components.
- `MissionPlannerTests/` — test project included in the solution.
- `Plugin/Plugin.cs` — base runtime plugin API.
- `Plugins/...` and some plugin projects under `plugins/...` — compiled plugin projects included in the solution.
- `plugins/example*.cs` and similar files — runtime plugin examples copied to output, not normal app compilation units.
- `Updater/`, `wix/`, `Msi/`, `Drivers/` — installer/update/driver related areas.
- `Scripts/`, `graphs/`, `NoFly/`, XML/JSON assets at repo root — runtime content used by the application.

## Very important plugin distinction

Do not treat every `plugins/*.cs` file as normal compiled application code.

There are two different extension patterns in this repo:

### 1) Compiled plugin projects

The solution includes plugin projects such as:

- `Plugins/Shortcuts/Shortcuts.csproj`
- `Plugins/FaceMap/FaceMap.csproj`
- `Plugins/OpenDroneID2/OpenDroneID_Plugin.csproj`
- `Plugins/TerrainMakerPlugin/TerrainMakerPlugin.csproj`
- `plugins/Dowding/Dowding.csproj`

If you are changing one of these, treat it like normal C# project code and validate it accordingly.

### 2) Runtime `.cs` plugin examples/scripts

`MissionPlanner.csproj` explicitly copies many files like `plugins/example.cs`, `plugins/example2.cs`, etc. to the output as content, while excluding them from normal compilation.

If you are changing one of these files:

- Treat it as a runtime plugin/script artifact, not as a normal compile item.
- Do not convert it into a normal app compile unit unless the task explicitly requires that architectural change.
- Preserve the expected runtime loading model.

## Plugin API expectations

Runtime plugins derive from `MissionPlanner.Plugin.Plugin`.

Important lifecycle/API facts from `Plugin/Plugin.cs`:

- Required members: `Name`, `Version`, `Author`, `Init()`, `Loaded()`, `Exit()`.
- Optional members include `SetupUI(...)`, `Loop()`, `NextRun`, and `loopratehz`.
- `Loop()` runs on a **background thread shared with other plugins**.
- `Exit()` is called on **plugin unload**, not on application exit.

Implications for agents:

- Be conservative with thread-affinity assumptions.
- Do not touch WinForms controls from plugin background-loop code without proper marshaling.
- Keep plugin load/unload behavior predictable.
- Avoid adding blocking work in `Loop()`.

## WinForms and designer files

This is a WinForms-heavy repo.

When editing UI code:

- Prefer changing logic in the main `.cs` file unless the task actually requires designer changes.
- Do not hand-edit `*.Designer.cs` or `.resx` casually.
- If you must change designer-generated files, keep the related `.cs`, `.Designer.cs`, and `.resx` in sync.
- Avoid accidental whitespace/reformat churn in designer files.

## Project and asset handling rules

The main project copies a large amount of runtime content to the output directory: drivers, XML/JSON, graphs, Python scripts, themes, KMZ files, plugin examples, and helper binaries.

Because of that:

- Do not rename, move, or delete runtime content files unless the task explicitly requires it.
- Do not assume a file is unused just because it is not a normal C# compile item.
- Be careful when touching `Drivers/`, `Scripts/`, `graphs/`, `NoFly/`, root XML/JSON, and copied helper executables.
- Preserve existing copy-to-output behavior unless the task is specifically about packaging or deployment.

## Legacy/compatibility posture

This repo contains legacy .NET Framework, WinForms, installer, updater, plugin, and platform-compatibility concerns.

Therefore:

- Prefer compatibility-preserving changes.
- Avoid introducing trendy abstractions or framework migrations as part of unrelated work.
- Avoid broad namespace, file-layout, or solution-structure reorganizations.
- Avoid replacing established project references/content behavior unless there is a clear repository-level reason.

## Code style and change discipline

- Follow the existing local style of the file you are editing.
- Match naming and patterns already used nearby.
- Keep diffs surgical.
- Do not reformat large files just because you touched them.
- Do not perform solution-wide analyzer or style cleanup as a side quest.

## What to mention in your final change summary

When you finish a change, include:

- What you changed
- Why those files were the right place to change
- What you validated
- Any remaining risk or area not validated in a real Windows/Visual Studio runtime environment

## When to stop and reconsider

Pause and reassess before making broad changes if the request would require any of the following:

- Retargeting framework versions
- Reworking build/packaging structure
- Converting runtime plugin examples into compiled project code
- Large-scale renames/moves of copied runtime assets
- Cross-cutting WinForms designer regeneration across many screens

In this repo, those are high-risk changes and should only happen when explicitly requested.
