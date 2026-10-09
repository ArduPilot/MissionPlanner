param([Parameter(Mandatory = $true)][string] $HostDirectory)
$ErrorActionPreference = 'Stop'

# Load the actual installed C# implementation. No Mission Planner process or
# flight-controller connection is created. Resolve dependencies beside its DLL.
$taskAssemblyPath = Join-Path $HostDirectory 'MissionPlanner.Utilities.dll'
$taskAssembly = [Reflection.Assembly]::LoadFrom($taskAssemblyPath)
$taskCases = [Console]::In.ReadToEnd() | ConvertFrom-Json
$taskResults = @()
foreach ($taskCase in $taskCases) {
    $taskPoint = New-Object MissionPlanner.Utilities.PointLatLngAlt -ArgumentList @(
        [double]$taskCase.home.latitude, [double]$taskCase.home.longitude, [double]0)
    $taskPoints = @()
    foreach ($taskLeg in $taskCase.legs) {
        $taskPoint = $taskPoint.newpos([double]$taskLeg.bearing_deg, [double]$taskLeg.distance_m)
        $taskPoint.Alt = [double]$taskLeg.altitude_m
        $taskPoints += @{ latitude = $taskPoint.Lat; longitude = $taskPoint.Lng; altitude_m = $taskPoint.Alt }
    }
    $taskResults += @{ points = $taskPoints }
}
$taskHasher = [Security.Cryptography.SHA256]::Create()
$taskStream = [IO.File]::OpenRead($taskAssemblyPath)
try {
    $taskHash = [BitConverter]::ToString($taskHasher.ComputeHash($taskStream)).Replace('-', '')
} finally {
    $taskStream.Dispose()
    $taskHasher.Dispose()
}
@{
    assembly_version = $taskAssembly.GetName().Version.ToString()
    assembly_sha256 = $taskHash
    host_version = [Diagnostics.FileVersionInfo]::GetVersionInfo((Join-Path $HostDirectory 'MissionPlanner.exe')).FileVersion
    host_assembly_version = [Reflection.AssemblyName]::GetAssemblyName((Join-Path $HostDirectory 'MissionPlanner.exe')).Version.ToString()
    cases = $taskResults
} | ConvertTo-Json -Depth 8 -Compress
