# Change Log

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
