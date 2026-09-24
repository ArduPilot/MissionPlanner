#!/usr/bin/env bash
set -euo pipefail
linux_dir=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
source_dir=$(dirname -- "$linux_dir")
docker_args=(--rm --mount "type=bind,src=$source_dir/bin/linux,dst=/packages,readonly"
    --mount "type=bind,src=$linux_dir,dst=/linux,readonly")
# Local tests may use an existing Xephyr display. CI uses Xvfb inside the container.
if [[ -n ${MISSIONPLANNER_TEST_DISPLAY:-} ]]; then
    docker_args+=(--env "DISPLAY=$MISSIONPLANNER_TEST_DISPLAY"
        --mount "type=bind,src=/tmp/.X11-unix,dst=/tmp/.X11-unix,readonly")
fi
docker run "${docker_args[@]}" ubuntu:22.04 bash -euo pipefail -c '
    export DEBIAN_FRONTEND=noninteractive
    apt-get update -qq
    mapfile -t packages < /linux/runtime-packages.txt
    apt-get install -y --no-install-recommends "${packages[@]}" xvfb xauth
    if command -v dotnet || command -v mcs; then
        echo "Runtime test unexpectedly has a compiler installed." >&2
        exit 1
    fi
    mkdir "/tmp/extracted package"
    tar -xzf /packages/MissionPlanner-linux-x86_64.tar.gz -C "/tmp/extracted package"
    cd "/tmp/extracted package/MissionPlanner-linux-x86_64"
    # Ask Mono for thread stacks if startup hangs before forcing termination.
    if [[ -n ${DISPLAY:-} ]]; then
        timeout --signal=QUIT --kill-after=5s 90s ./run.sh --self-test 2>&1 | tee /tmp/smoke.log
    else
        xvfb-run -a -s "-screen 0 1280x960x24" timeout --signal=QUIT --kill-after=5s 90s ./run.sh --self-test 2>&1 | tee /tmp/smoke.log
    fi
    grep -qx LINUX_SMOKE_TEST_PASS /tmp/smoke.log
'
