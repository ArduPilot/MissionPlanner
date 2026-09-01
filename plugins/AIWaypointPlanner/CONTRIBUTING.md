# Contributing

## Before opening a patch

Check the Mission Planner issue tracker and open pull requests for related work. A patch should address one focused problem and should not include generated build output, local configuration, credentials or unrelated formatting changes.

## Branch and commit

Use a branch based on the current `ArduPilot/MissionPlanner:master`. Keep the history minimal and free of merge commits. The subject line should be fewer than 72 characters and use an affected-subsystem prefix, for example:

```text
MissionPlanner: add AI waypoint planner plugin
```

The body should state the behavior change, compatibility impact and exact tests that were run.

## Validation

At minimum, build the plugin against a matching Mission Planner host and run the offline self-tests in `SelfTests`. Do not test by connecting to a live vehicle, uploading a mission, changing flight mode or arming a vehicle. Any integration test involving a flight controller must be separately documented and performed under an appropriate safety procedure.

## Pull request

Push the branch to a personal Fork and open a pull request with base `ArduPilot:master`. Include:

- a concise summary of the change;
- user-visible behavior and limitations;
- build commands and test results;
- dependency or packaging changes;
- screenshots only when they clarify a user-interface change.

The official process is documented in the [ArduPilot patch submission guide](https://ardupilot.org/dev/docs/submitting-patches-back-to-master.html). Maintainer review takes precedence over this document.
