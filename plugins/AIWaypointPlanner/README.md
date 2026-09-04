# Mission Planner AI Waypoint Planner

An independently built Mission Planner plugin for converting a written mission description and optional reference files into locally validated candidate mission items. The plugin targets .NET Framework 4.7.2 and uses Mission Planner's public plugin host APIs.

The plugin does not upload missions or control a vehicle. The operator must review and manually write any accepted items using Mission Planner's normal workflow.

## Scope

Supported mission templates:

- `relative_route`: calculate waypoints from the planned Home position, bearings, distances, altitude and speed.
- `survey_polygon`: generate a survey grid from a polygon already drawn in Flight Planner.
- `RETURN_TO_LAUNCH` is the only accepted completion action.

The plugin also accepts PDF, DOCX, PNG, JPEG, WebP, GIF and common text/data files as task references. Text documents are extracted locally where possible; images and scanned PDFs are sent only when the selected API supports the required input format.

The following functions are deliberately outside the scope of this project: obstacle avoidance, terrain following, payload control, target tracking, dynamic replanning, automatic landing, flight-mode changes, arming, RC/PWM output and direct takeoff commands.

## Safety boundary

The plugin calls `PluginHost.AddWPtoList` to append candidate rows to the local Flight Planner list. It does not call Mission Planner's mission-write/upload path and does not communicate with a flight controller. `TAKEOFF` and `RETURN_TO_LAUNCH` are displayed as candidate rows only.

Before using a candidate mission, the operator must verify the Home position, altitude reference, vehicle type, airspace, geofence, failsafes, energy budget, turn radius and every generated item. The normal Mission Planner write operation remains a separate, manual action.

## API configuration

The API page supports OpenAI-compatible Responses and Chat Completions endpoints. The following connection types are represented by editable presets:

- CC Switch or another local gateway;
- OpenAI-compatible remote services over HTTPS;
- OpenRouter, LiteLLM, LM Studio, Ollama, New API/One API and Azure OpenAI-compatible endpoints;
- a custom endpoint with an explicit base URL, protocol, authentication mode and model identifier.

Named connection profiles can be saved, selected, overwritten and deleted. The most recently saved or used profile is restored when the plugin opens. Non-sensitive fields are stored at:

```text
%APPDATA%\\MissionPlanner\\AIWaypointPlanner\\api-profiles.xml
```

API keys are never written to that file. When enabled, each profile stores its key in a separate Windows Credential Manager entry. The legacy global credential target is retained only for backward compatibility.

Remote endpoints must use HTTPS. HTTP is accepted only for loopback addresses such as `127.0.0.1`, `localhost` and `::1`.

The plugin does not read credentials, cookies or OAuth data belonging to CC Switch, Codex, ChatGPT or another application. A gateway must expose an OpenAI-compatible endpoint; native Anthropic or Gemini protocols are not handled directly.

## Connection recovery

API requests make up to three connection attempts when a temporary failure occurs. The client retries HTTP 408, 409, 425, 429, 500, 502, 503 and 504 responses, request timeouts and transient network errors. It honors a standard `Retry-After` response header when present; otherwise it uses bounded exponential backoff with a small delay variation. Operator cancellation stops recovery immediately.

Authentication, authorization, invalid-request and missing-endpoint errors are not retried because they require a credential or configuration change. The response inspection dialog records the final status, total attempt count and automatic reconnection count. These rules follow OpenAI's published [API error guidance](https://platform.openai.com/docs/guides/error-codes), [rate-limit guidance](https://platform.openai.com/docs/guides/rate-limits) and [production best practices](https://developers.openai.com/api/docs/guides/production-best-practices).

## Repository layout

```text
AIWaypointPlanner.csproj             Plugin project (.NET Framework 4.7.2)
AIWaypointPlannerPlugin.cs           Mission Planner plugin entry point
AIWaypointPlannerForm.cs             WinForms user interface and workflow
ApiConnectionSettings.cs             Endpoint and authentication validation
ApiProfileStore.cs                   Named profile persistence
WindowsCredentialStore.cs            Windows Credential Manager wrapper
OpenAiResponsesClient.cs             Responses/Chat Completions client
AttachmentProcessor.cs               Local reference-file processing
MissionCompiler.cs                   Deterministic candidate mission compiler
MissionValidator.cs                  Local safety and range validation
SelfTests/                           Offline regression tests
```

## Requirements

- Windows 10/11;
- Mission Planner with matching host assemblies;
- Visual Studio 2022 MSBuild or the .NET Framework developer tools;
- NuGet restore access for `PdfPig` and the existing Mission Planner dependency graph.

## Build

Build against the installed Mission Planner version:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe' `
  '.\AIWaypointPlanner.csproj' /t:Rebuild /p:Configuration=Release /p:Platform=AnyCPU `
  '/p:MissionPlannerHostDir=C:\Program Files (x86)\Mission Planner'
```

The main output is `bin\\Release\\net472\\MissionPlanner.AIWaypointPlanner.dll`.

Run the offline regression tests:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe' `
  '.\SelfTests\AIWaypointPlanner.SelfTests.csproj' /t:Rebuild /p:Configuration=Release /p:Platform=AnyCPU `
  '/p:MissionPlannerHostDir=C:\Program Files (x86)\Mission Planner'
& '.\SelfTests\bin\Release\net472\AIWaypointPlanner.SelfTests.exe'
```

The tests do not require an API key or a flight controller. They cover endpoint validation, route and survey compilation, RTL enforcement, attachment extraction, multimodal request formatting, response capture, temporary-error recovery, permanent-error handling and local-proxy diagnostics.

## Installation

1. Build the plugin against the same Mission Planner installation that will load it.
2. Copy `MissionPlanner.AIWaypointPlanner.dll` to Mission Planner's `plugins` directory.
3. Copy the `UglyToad.PdfPig*.dll` and `Microsoft.Bcl.HashCode.dll` dependencies if they are not already present in that directory.
4. Do not overwrite Mission Planner's existing `System.Memory.dll`, `System.Buffers.dll` or other shared runtime assemblies with files from the build output.
5. Restart Mission Planner, open Flight Planner, and select the plugin from the Auto WP menu.

## Current loader limitation

The current Mission Planner loader automatically compiles top-level `plugins\\*.cs` scripts. This project is intentionally kept as a separate multi-file `net472` project because it uses external dependencies and a test assembly. An upstream maintainer may choose to integrate the project into the main solution, adapt it to the script loader, or define another packaging method.

## Contribution

Patches intended for the official Mission Planner repository should follow the ArduPilot contribution guidance:

1. Fork `ArduPilot/MissionPlanner` and update the fork's `master` before branching.
2. Use a new branch containing one focused change and no unrelated files.
3. Keep the commit title below 72 characters and prefix it with the affected subsystem, for example `MissionPlanner: ...`.
4. Include reproducible build and test results in the pull request description.
5. Open the pull request against `ArduPilot:master` and respond to maintainer review and CI results.

See `CONTRIBUTING.md` and the [ArduPilot patch submission guide](https://ardupilot.org/dev/docs/submitting-patches-back-to-master.html).

## License

This plugin is intended for inclusion in the GPLv3-licensed Mission Planner project. Unless a file states otherwise, contributions to this directory are provided under GPLv3. See the parent repository's `COPYING.txt` for the full license text.

## Version

The current plugin version is `1.5.0`. Versioning rules and release history are documented in `VERSIONING.md` and `CHANGELOG.md`.
