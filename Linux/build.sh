#!/usr/bin/env bash
set -euo pipefail

linux_dir=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
source_dir=$(dirname -- "$linux_dir")
if [[ ${1:-} == --help ]]; then
    echo 'Usage: Linux/build.sh [dotnet build options]'
    echo 'Builds Release and writes bin/linux/MissionPlanner-linux-x86_64.tar.gz.'
    echo 'Requires .NET SDK 10, mono-devel, rsync, tar and gzip. No submodules are needed.'
    exit 0
fi
for tool in dotnet mono mcs rsync tar gzip sha256sum; do
    if ! command -v "$tool" >/dev/null; then
        echo "Missing $tool. See Linux/README.md for build dependencies." >&2
        exit 1
    fi
done
if [[ $(uname -m) != x86_64 ]]; then
    echo 'This package currently targets Linux x86_64.' >&2
    exit 1
fi
mono_posix=${MONO_POSIX_PATH:-/usr/lib/mono/4.5/Mono.Posix.dll}
if [[ ! -f $mono_posix ]]; then
    echo "Mono.Posix not found: $mono_posix (install mono-devel or set MONO_POSIX_PATH)." >&2
    exit 1
fi

mono_facades=${MONO_FACADES_PATH:-/usr/lib/mono/4.5/Facades}
mono_mcs=${MONO_MCS_PATH:-/usr/lib/mono/4.5/mcs.exe}
mono_copyright=${MONO_COPYRIGHT_PATH:-/usr/share/doc/mono-runtime/copyright}
if [[ ! -f $mono_facades/netstandard.dll || ! -f $mono_copyright || ! -f $mono_mcs ]]; then
    echo 'Mono runtime facades/license missing. Install mono-devel and mono-runtime.' >&2
    exit 1
fi

cd -- "$source_dir"
dotnet build MissionPlanner.csproj --configuration Release \
    -p:MissionPlannerLinuxBuild=true -p:EnableWindowsTargeting=true \
    "-p:DirectoryBuildTargetsPath=$linux_dir/Build.targets" \
    "-p:MonoPosixPath=$mono_posix" "$@"

app_dir="$source_dir/bin/Release/net461"
mono "$app_dir/version.exe" "$app_dir/MissionPlanner.exe" > "$app_dir/version.txt"
cp -- ExtLibs/System.Speech.dll "$app_dir/"
# NativeLibrary is compiled into MissionPlanner.exe. Map its Linux loader name
# to the runtime soname so users do not need the libc development symlink.
sed -i '/<dllmap dll="libdl.so"/d; /<\/configSections>/a\  <dllmap dll="libdl.so" target="libdl.so.2" os="linux" />' \
    "$app_dir/MissionPlanner.exe.config"

# Include a diagnostic that can validate the runtime without a compiler.
mcs -out:"$app_dir/LinuxSmokeTest.exe" \
    -r:System.Windows.Forms -r:System.Drawing -r:"$app_dir/SkiaSharp.dll" \
    -r:"$app_dir/MissionPlanner.exe" -r:"$app_dir/Interfaces.dll" \
    -r:"$app_dir/MissionPlanner.Controls.dll" -r:"$app_dir/MissionPlanner.Utilities.dll" \
    -r:"$mono_facades/netstandard.dll" "$linux_dir/SmokeTest.cs"
cp -- "$app_dir/MissionPlanner.exe.config" "$app_dir/LinuxSmokeTest.exe.config"

package_name=MissionPlanner-linux-x86_64
output_dir="$source_dir/bin/linux"
package_dir="$output_dir/$package_name"
mkdir -p -- "$package_dir"
# Copy a runnable application, excluding Windows installers/native backends and
# build diagnostics. Keep plugins and managed dependencies, including Roslyn.
rsync -a --delete \
    --exclude='/Drivers/' --exclude='/gdal/' --exclude='/x86/' \
    --exclude='/x64/*.dll' --exclude='/arm/' --exclude='/arm64/' --exclude='*.dylib' \
    --exclude='*.pdb' --exclude='*.mdb' --exclude='*.log' \
    "$app_dir/" "$package_dir/"
# Ubuntu ships these runtime forwarding assemblies in mono-devel. Bundle them
# so the download only needs Mono's runtime packages, not its compiler/SDK.
mkdir -p -- "$package_dir/mono-facades"
cp -- "$mono_facades/"*.dll "$package_dir/mono-facades/"
# Mono uses CodeDOM for some XML serializers even in a precompiled app.
# Ship the small managed compiler helper so no system compiler install is needed.
mkdir -p -- "$package_dir/mono-tools"
cp -- "$mono_mcs" "$package_dir/mono-tools/mcs.exe"
cp -- "$linux_dir/mcs" "$package_dir/mono-tools/mcs"
chmod +x "$package_dir/mono-tools/mcs"
cp -- "$mono_copyright" "$package_dir/MONO-COPYRIGHT.txt"
# The source tree contains both casings; the plugin loader uses lowercase.
if [[ -d $package_dir/Plugins ]]; then
    mkdir -p -- "$package_dir/plugins"
    rsync -a "$package_dir/Plugins/" "$package_dir/plugins/"
    rm -r -- "$package_dir/Plugins"
fi
cp -- "$linux_dir/run.sh" "$package_dir/run.sh"
cp -- "$linux_dir/README.md" "$package_dir/README.md"
cp -- COPYING.txt "$package_dir/COPYING.txt"
chmod +x "$package_dir/run.sh"

revision=$(git rev-parse HEAD 2>/dev/null || echo unknown)
dirty=false
if [[ -n $(git status --porcelain --untracked-files=normal 2>/dev/null) ]]; then dirty=true; fi
{
    echo "Source revision: $revision"
    echo "Working tree modified: $dirty"
    echo "Source: https://github.com/${GITHUB_REPOSITORY:-ArduPilot/MissionPlanner}/tree/$revision"
    echo "Built UTC: $(date -u +%Y-%m-%dT%H:%M:%SZ)"
    echo "SDK: $(dotnet --version)"
    head -n 1 < <(mono --version)
} > "$package_dir/BUILD-INFO.txt"

tar -C "$output_dir" -czf "$output_dir/$package_name.tar.gz" "$package_name"
(cd -- "$output_dir" && sha256sum "$package_name.tar.gz" > "$package_name.tar.gz.sha256")
echo "Package: $output_dir/$package_name.tar.gz"
echo "Run: $package_dir/run.sh"
