# Change Log

## 3.0.1 - 2026-09-07

### Maintenance

- Split the existing form into partial-class files grouped by responsibility, preserving its public type, event handlers and workflow.
- Moved 984 interface strings into three embedded JSON catalogs and reduced the language helper to lookup, normalization, fallback and formatting.
- Preserved every translation value and the shared release placeholder; catalog files are embedded in the main plugin DLL.
- Added checks for embedded resource packaging and matching format arguments across all three languages.
- Updated source-based regression checks to read all form partials.
- Ignored generated build output and Python cache files within the plugin source directory.
- Added an isolated, standard-library Python coordinate-preview experiment to assess a simpler language without adding a production runtime dependency.

This is a patch release: operator behavior, interface layout, file/API support and the production architecture remain unchanged.

## 3.0.0 - 2026-09-07

### Changed

- Reworked the plugin presentation around a high-contrast dark palette for the conversation, mission review, settings and diagnostics views.
- Applied explicit foreground, background, selection, disabled and status colors so text remains readable independently of the active Mission Planner theme.
- Increased neutral borders and added dedicated selection and focus colors so secondary buttons, selected tabs, selected rows and keyboard focus remain distinct on dark surfaces.
- Replaced the native light tab-strip surface with a fully plugin-painted dark strip in the main workspace and response diagnostics dialog.
- Increased component-border and selected-tab contrast, including a visible keyboard-focus outline on the active tab.
- Reserved and dynamically resized the safety-banner row so localized banner text cannot cover the workspace tabs on the first frame or after a language change.
- Kept the Mission Planner Auto WP menu label and tooltip synchronized immediately with the language selected in the plugin.
- Localized Mission Planner-visible plugin identity text consistently in English, Simplified Chinese and Russian, including host culture aliases such as `zh-Hans`.
- Rebuilt attachment states, automatic attachment-only prompts and activity labels from localization keys when the interface language changes.
- Cleared earlier model-generated text and unapplied candidate items on a language change so the visible response cannot remain in the previous language; task text and attachments are preserved for resubmission.
- Displayed every unknown model mission type as the localized unsupported type instead of exposing an internal identifier.
- Centralized the release identifier in `PluginIdentity.Version` so the plugin metadata and localized window titles can share one version value.

### Compatibility and safety

- Reapplied the plugin palette after Mission Planner host theming so the host cannot silently restore low-contrast light surfaces inside the dark interface.
- Stored the successful local-apply state as a localization key so it remains correct when the interface language changes.
- Preserved raw API diagnostics and already-applied Flight Planner descriptions without silently translating or overwriting operator-visible technical data.
- Preserved the persisted language preference, English first-run default, local candidate validation and no-upload/no-flight-control safety boundary.

## 2.0.0 - 2026-09-06

### Added

- Reworked the main workflow into Chat, Mission review and Settings views with bounded multi-turn context.
- Added English, Simplified Chinese and Russian interface catalogs; English is the default and the selected language is persisted per user.
- Added task prompts, attachment status, connection activity and explicit Thinking, validation and clarification states.
- Added Provider default, Low, Medium, High, Very high and ULTRA reasoning choices. ULTRA maps to the API value `max`.
- Added native Responses API file inputs for supported PDF and Office documents while retaining Chat Completions text/image fallbacks.
- Added the plugin project to the Mission Planner solution so the official .NET workflow includes a build target.
- Added cancellable background attachment parsing with per-file duplicate-name checks and localized attachment status.
- Added live connection activity callbacks for connecting, waiting, thinking and automatic reconnection states.

### Compatibility and safety

- Moved API, language, model, authentication and reasoning controls into one settings view.
- Renamed the plugin assembly to `AIWaypointPlanner.dll` so Mission Planner 1.3.83 does not exclude it as a host assembly.
- Human-readable model fields follow the selected operator language; schema field names and enum values remain stable.
- The UI's Thinking state is an activity indicator only and does not expose private model reasoning.
- Preserved local validation, operator confirmation and no-upload/no-flight-control boundaries.
- Compatibility presets use Provider default reasoning for gateways and local models unless the preset is known to support the selected reasoning values; OpenAI and CC Switch retain the `gpt-5.6-sol` medium default.
- Cleared session keys when switching providers or editing connection fields, and isolated saved keys by profile or endpoint so a key is not silently reused across services or protocols.
- Bound every saved credential to the canonical endpoint, protocol, authentication mode and model; legacy name-only profile credentials are no longer loaded automatically.
- Defaulted legacy profiles without a reasoning setting to Provider default instead of injecting an unsupported Medium field into compatibility gateways.
- Propagated cancellation through attachment reads and PDF page parsing, and limited extracted-text totals only where text is actually sent to the selected protocol.
- Raised successful Responses and Chat Completions JSON parsing to the same 64 MB limit used by the client, preventing high-reasoning metadata from breaking otherwise valid replies.
- Made the red safety banner and settings labels wrap within the available window, including Russian and high-DPI layouts.
- Restored cancelled and invalid-result states after asynchronous operations and language changes.
- Responses PDF file inputs include the documented `detail: auto` option; same-name attachments are rejected before they can make model file confirmation ambiguous.
- Documented GitHub public-fork workflow approval requirements separately from code build results.

## 1.5.0 - 2026-09-04

### Added

- Automatic recovery for temporary API connection failures.
- Up to three connection attempts with bounded exponential backoff and delay variation.
- Support for the standard `Retry-After` response header.
- Attempt and automatic-reconnection counts in the response inspection dialog.

### Behavior

- Retry HTTP 408, 409, 425, 429, 500, 502, 503 and 504 responses.
- Retry request timeouts and transient transport failures.
- Do not retry authentication, authorization, invalid-request or missing-endpoint errors.
- Stop recovery immediately when the operator cancels the request.

## 1.4.2 - 2026-09-01

### Added

- Named persistence for multiple OpenAI-compatible API connection profiles.
- Profile selection, overwrite and deletion from the API settings page.
- Restoration of the most recently saved or used profile when the plugin opens.
- Per-profile Windows Credential Manager targets; API keys are excluded from the XML profile file.

### Security

- Disabling profile credential persistence prevents an old profile credential from being selected implicitly.
- Deleting a profile removes its corresponding stored credential.

## 1.4.1 - 2026-09-01

### Added

- Response inspection dialog with request metadata, HTTP status, request ID, raw response and structured task output.
- Diagnostics for HTTP failures, response parsing failures and local proxy refusal.
- Offline tests for response capture and local proxy diagnostics.

### Fixed

- Preserve the underlying transport exception when reporting a network failure.
- Explain when a local CC Switch proxy is not running or is listening on a different port.
- Use a relative-route default task that does not require a pre-drawn survey polygon.

### Safety and compatibility

- Never display request bodies, API keys or authorization headers in the response dialog.
- Preserve the local-only candidate mission workflow and RTL completion constraint.

## 1.3.1 - 2026-09-01

- Added PDF, DOCX, image and common text/data reference-file processing.
- Added attachment limits, path privacy and untrusted-document handling.
- Added Responses and Chat Completions multimodal request formats.
- Added operator confirmation and local validation before candidate rows can be applied.

## 1.2.0

- Added CC Switch and common OpenAI-compatible endpoint presets.
- Set the default local-gateway model to `gpt-5.6-sol`.
- Added a high-contrast red safety notice.

## 1.0.0

- Initial candidate mission generation workflow.
