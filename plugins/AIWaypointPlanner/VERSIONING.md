# Versioning

The project uses `MAJOR.MINOR.PATCH` version numbers.

## Patch releases

Increment `PATCH` for bug fixes, wording changes, compatibility fixes, behavior-preserving internal refactoring, test/development tooling and other changes that do not alter the principal workflow. An isolated language experiment does not change the production architecture or require a major version; adopting a different production runtime does.

Example: `1.3.1` to `1.3.2`.

## Minor releases

Increment `MINOR` for a new self-contained capability or an extension to supported input/output behavior that does not require a fundamental workflow change. Reset `PATCH` to zero.

Example: `1.3.2` to `1.4.0`.

## Major releases

Increment `MAJOR` for a substantial user-interface or workflow redesign, a system-level architecture change, or a compatibility boundary that requires operators to relearn the main operation. Reset `MINOR` and `PATCH` to zero.

Example: `1.4.3` to `2.0.0`.

## Release checklist

Every release must update all of the following:

- `Version` in `AIWaypointPlanner.csproj`;
- the shared release identifier in `PluginIdentity.Version`;
- the version returned by `AIWaypointPlannerPlugin` and every localized plugin window title;
- the version-consistency expectation in the offline self-tests;
- the release entry in `CHANGELOG.md`;
- `README.md` when behavior, limits or dependencies change.

Before publishing, build the plugin and run the offline self-tests from a clean worktree. Verify that the generated assembly version, plugin host metadata and English, Simplified Chinese and Russian window titles all report the same `MAJOR.MINOR.PATCH` value. Do not edit generated `bin` or `obj` files to change a version.

The version identifies software scope only. It is not a safety rating, mission-quality rating or model-capability rating.
