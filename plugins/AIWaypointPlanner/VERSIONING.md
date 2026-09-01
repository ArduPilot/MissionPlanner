# Versioning

The project uses `MAJOR.MINOR.PATCH` version numbers.

## Patch releases

Increment `PATCH` for bug fixes, wording changes, compatibility fixes, test additions and other changes that do not alter the principal workflow.

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
- the version returned by `AIWaypointPlannerPlugin`;
- the version in the plugin window title;
- the release entry in `CHANGELOG.md`;
- `README.md` when behavior, limits or dependencies change.

The version identifies software scope only. It is not a safety rating, mission-quality rating or model-capability rating.
