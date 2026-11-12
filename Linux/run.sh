#!/usr/bin/env bash
set -euo pipefail

script_dir=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
if [[ -f "$script_dir/MissionPlanner.exe" ]]; then
    app_dir=$script_dir
elif [[ -f "$script_dir/../bin/linux/MissionPlanner-linux-x86_64/MissionPlanner.exe" ]]; then
    app_dir="$script_dir/../bin/linux/MissionPlanner-linux-x86_64"
else
    echo 'MissionPlanner.exe not found. Run Linux/build.sh or extract the complete Linux archive.' >&2
    exit 1
fi
if ! command -v mono >/dev/null; then
    echo 'Mono runtime is required. See README.md for the runtime-only installation command.' >&2
    exit 1
fi
if [[ $(uname -m) != x86_64 ]]; then
    echo 'This download is built for Linux x86_64.' >&2
    exit 1
fi
if [[ -z ${DISPLAY:-} ]]; then
    echo 'An X11 display is required (use XWayland on a Wayland desktop).' >&2
    exit 1
fi

cd -- "$app_dir"
export MONO_IOMAP=drive:case
export PATH="$PWD/mono-tools:$PATH"
export MONO_PATH="$PWD/mono-facades${MONO_PATH:+:$MONO_PATH}"
# Ubuntu's legacy GTK overlay-scrollbar module can crash Mono WinForms at startup.
export LIBOVERLAY_SCROLLBAR=0
export GTK_MODULES=
export LD_LIBRARY_PATH="$PWD:$PWD/x64:$PWD/runtimes/linux-x64/native${LD_LIBRARY_PATH:+:$LD_LIBRARY_PATH}"
if [[ ${1:-} == --self-test ]]; then
    shift
    exec mono LinuxSmokeTest.exe "$@"
fi
exec mono MissionPlanner.exe "$@"
