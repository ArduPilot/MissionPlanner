#!/usr/bin/env bash
set -euo pipefail
if [[ $# != 1 ]]; then
    echo 'Usage: run-sysid32-sitl.sh /path/to/MissionPlanner-linux-x86_64' >&2
    exit 1
fi
app_dir=$(realpath -- "$1")
test_dir=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
references=(-r:System.Windows.Forms -r:System.Drawing -r:/usr/lib/mono/4.5/Facades/netstandard.dll)
for assembly in MissionPlanner.exe MAVLink.dll MissionPlanner.ArduPilot.dll MissionPlanner.Utilities.dll MissionPlanner.Comms.dll Interfaces.dll MissionPlanner.Controls.dll; do
    references+=("-r:$app_dir/$assembly")
done
mcs -langversion:latest "${references[@]}" -out:"$app_dir/Sysid32Sitl.exe" "$test_dir/Sysid32Sitl.cs"
cp -- "$app_dir/MissionPlanner.exe.config" "$app_dir/Sysid32Sitl.exe.config"
cd -- "$app_dir"
export MONO_IOMAP=drive:case
export MONO_PATH="$app_dir:$app_dir/mono-facades${MONO_PATH:+:$MONO_PATH}"
export LD_LIBRARY_PATH="$app_dir:$app_dir/x64${LD_LIBRARY_PATH:+:$LD_LIBRARY_PATH}"
export PATH="$app_dir/mono-tools:$PATH"
export LIBOVERLAY_SCROLLBAR=0
export GTK_MODULES=
exec mono Sysid32Sitl.exe
