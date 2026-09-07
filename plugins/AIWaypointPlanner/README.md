# Mission Planner AI Waypoint Planner

An independently built Mission Planner plugin for converting a written mission description and optional reference files into locally validated candidate mission items. The plugin targets .NET Framework 4.7.2 and uses Mission Planner's public plugin host APIs. Version 3.0.0 introduces a high-contrast dark interface and keeps Mission Planner-visible plugin text synchronized with the selected language while preserving the conversation-oriented workflow and local-only safety boundary.

The plugin does not upload missions or control a vehicle. The operator must review and manually write any accepted items using Mission Planner's normal workflow.

## Scope

Supported mission templates:

- `relative_route`: calculate waypoints from the planned Home position, bearings, distances, altitude and speed.
- `survey_polygon`: generate a survey grid from a polygon already drawn in Flight Planner.
- `RETURN_TO_LAUNCH` is the only accepted completion action.

The plugin also accepts PDF, DOC/DOCX, RTF, ODT, PPT/PPTX, XLS/XLSX, PNG, JPEG, WebP, GIF and common text/data files as task references. Text documents are extracted locally where possible. Responses API requests can send supported PDF and Office files as native file inputs; Chat Completions requests use locally extracted text and image content. Scanned PDFs and Office formats without extractable text require Responses API support or an external OCR/text conversion step.

The following functions are deliberately outside the scope of this project: obstacle avoidance, terrain following, payload control, target tracking, dynamic replanning, automatic landing, flight-mode changes, arming, RC/PWM output and direct takeoff commands.

## Safety boundary

The plugin calls `PluginHost.AddWPtoList` to append candidate rows to the local Flight Planner list. It does not call Mission Planner's mission-write/upload path and does not communicate with a flight controller. `TAKEOFF` and `RETURN_TO_LAUNCH` are displayed as candidate rows only.

Before using a candidate mission, the operator must verify the Home position, altitude reference, vehicle type, airspace, geofence, failsafes, energy budget, turn radius and every generated item. The normal Mission Planner write operation remains a separate, manual action.

## Conversation and language interface

The main window is organised into Chat, Mission review and Settings views. It shows bounded task context, attachments, connection activity and local validation states such as Reading files, Connecting, Thinking, Validating and Waiting for clarification. A task prompt can be sent over several turns; previous context is truncated to fixed limits before it is sent to the model.

English is the default interface language. Simplified Chinese and Russian are also available. The selected language is saved to:

```text
%APPDATA%\\MissionPlanner\\AIWaypointPlanner\\preferences.xml
```

Changing the interface language immediately refreshes the plugin window, the Mission Planner Auto WP menu entry and its tooltip. The saved language is restored the next time Mission Planner loads the plugin. Mission Planner culture aliases such as `zh-Hans` are normalized to the corresponding supported plugin language so host-visible text does not fall back to English unexpectedly.

Model-generated summaries cannot be translated locally without risking a change to mission meaning. If the operator changes language after a model response has been displayed, the plugin clears the previous generated response and candidate mission, preserves the current task text and attachments, and asks the operator to send the task again. Plugin-generated notices, attachment states and the attachment-only task prompt are rebuilt immediately in the selected language. Free-form operator text, file names, model identifiers, URLs and protocol diagnostics are intentionally preserved verbatim.

Descriptions appended to the Mission Planner Flight Planner use the interface language that was active when the candidate items were applied. A later language change does not rewrite rows already present in Flight Planner, because those rows may have been edited by the operator. The response diagnostics window localizes its own controls while preserving raw API responses and protocol data exactly as received.

The interface uses an explicit high-contrast dark palette across conversation messages, mission review, settings, status indicators and response diagnostics. The palette is applied after Mission Planner's host theme so the plugin remains readable when the host theme changes. The workspace and diagnostics tabs use a fully plugin-painted dark strip, and the safety banner reserves its own dynamically sized row so localized text cannot obscure the tabs on first display.

The language selected in the interface is passed to the model as the requested language for human-readable fields. The schema field names and protocol enum values remain stable English identifiers. The plugin displays an activity state labelled Thinking; it does not expose or claim to expose a model's private chain of thought.

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

Saved keys are scoped to a fingerprint containing the profile name (when present), canonical endpoint, protocol, authentication mode and model. The legacy global OpenAI credential is read only for the official `api.openai.com` endpoint. Selecting another provider, or editing the endpoint, protocol, authentication mode or model, clears the current session key. A saved profile credential is reused only when every connection-affecting field still matches the profile, so it cannot be silently sent to a different host or protocol. Starting with version 2.0.0, legacy name-only profile credentials are not read automatically; re-enter and save the key once so it is stored under the connection-scoped target.

The reasoning selector offers Provider default, Low, Medium, High, Very high and ULTRA. The request mapping is Provider default (omit the parameter), `low`, `medium`, `high`, `xhigh` and `max`, respectively. `ULTRA` is a user-interface label only; the API never receives a non-standard `ultra` value. OpenAI and CC Switch presets default to Medium for `gpt-5.6-sol`; other compatibility and local presets default to Provider default until the operator selects a supported level. Gateways that reject reasoning fields should use Provider default.

## Connection recovery

API requests make up to three connection attempts when a temporary failure occurs. The client retries HTTP 408, 409, 425, 429, 500, 502, 503 and 504 responses, request timeouts and transient network errors. It honors a standard `Retry-After` response header when present; otherwise it uses bounded exponential backoff with a small delay variation. The Chat view reports Connecting, Waiting for the model, Thinking and Reconnecting activity states without exposing hidden model reasoning. Operator cancellation stops recovery immediately.

Authentication, authorization, invalid-request and missing-endpoint errors are not retried because they require a credential or configuration change. The response inspection dialog records the final status, total attempt count and automatic reconnection count. These rules follow OpenAI's published [API error guidance](https://platform.openai.com/docs/guides/error-codes), [rate-limit guidance](https://platform.openai.com/docs/guides/rate-limits) and [production best practices](https://developers.openai.com/api/docs/guides/production-best-practices).

## Repository layout

```text
AIWaypointPlanner.csproj             Plugin project (.NET Framework 4.7.2)
PluginIdentity.cs                    Shared plugin version identity
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

The main output is `bin\\Release\\net472\\AIWaypointPlanner.dll`. The assembly deliberately avoids a `MissionPlanner.` filename prefix because Mission Planner 1.3.83 filters host assemblies with that prefix out of its plugin loader.

When the project is built from the full Mission Planner repository without `MissionPlannerHostDir`, the project is included in `MissionPlanner.sln` and outputs to the solution plugin folders (`bin\\Debug\\net461\\plugins` or `bin\\Release\\net461\\plugins`). Supplying `MissionPlannerHostDir` continues to build against an installed Mission Planner and outputs under the plugin project's `bin\\{Configuration}\\net472` directory.

Run the offline regression tests:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe' `
  '.\SelfTests\AIWaypointPlanner.SelfTests.csproj' /t:Rebuild /p:Configuration=Release /p:Platform=AnyCPU `
  '/p:MissionPlannerHostDir=C:\Program Files (x86)\Mission Planner'
& '.\SelfTests\bin\Release\net472\AIWaypointPlanner.SelfTests.exe'
```

The tests do not require an API key or a flight controller. They cover version consistency, localized host presentation, dark-palette contrast, endpoint and credential-scope validation, route and survey compilation, RTL enforcement, cancellable attachment extraction, protocol-specific native-file limits, multimodal request formatting, large-response extraction, response capture, temporary-error recovery, permanent-error handling and local-proxy diagnostics.

## Installation

1. Build the plugin against the same Mission Planner installation that will load it.
2. Copy `AIWaypointPlanner.dll` to Mission Planner's `plugins` directory.
3. Copy the `UglyToad.PdfPig*.dll` and `Microsoft.Bcl.HashCode.dll` dependencies if they are not already present in that directory.
4. Do not overwrite Mission Planner's existing `System.Memory.dll`, `System.Buffers.dll` or other shared runtime assemblies with files from the build output.
5. Restart Mission Planner, open Flight Planner, and select the plugin from the Auto WP menu.

## Repository integration and CI

The plugin project is included in the Mission Planner solution for reproducible .NET build coverage; the offline `SelfTests` project remains separate. The `DotNet Build` workflow runs the full solution on Windows and is the only current workflow that compiles this desktop plugin. The `OSX Build` and `Android Build` workflows invoke only their Xamarin iOS/macOS/Android project files and do not include this WinForms/.NET Framework plugin.

This project uses Windows desktop APIs (WinForms, System.Drawing and Windows Credential Manager) and is not a macOS or Android target. Adding it to `MissionPlanner.sln` therefore affects the Windows solution build only; it does not add plugin code to the mobile projects. The main package cleanup also removes files under `bin\\{Configuration}\\net461\\plugins`, so the plugin DLL and its dependencies must be distributed separately when the main Mission Planner artifact is produced.

GitHub still requires approval for workflows from a public first-time contributor fork. A run shown as `No jobs were run` with `action_required` (or `Awaiting approval`) means that an upstream maintainer must approve the workflow from the pull-request page; it is not a plugin test result.

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

The current plugin version is `3.0.0`. Versioning rules and release history are documented in `VERSIONING.md` and `CHANGELOG.md`.
