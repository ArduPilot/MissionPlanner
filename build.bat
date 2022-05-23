call _vscmd.bat

del bin\release\MissionPlannerBeta.zip

.nuget\nuget.exe restore MissionPlanner.sln

MSBuild.exe MissionPlanner.sln /restore /m /p:Configuration=Release /verbosity:n

echo create appx?
pause

cd bin\release\net461

powershell -command "ls plugins -recurse | ForEach-Object { if (Test-Path ($_.fullname -replace '\\plugins\\','\') -PathType Leaf) { $_.fullname }} | ForEach-Object { del $_ }"

cd ..
cd ..
cd ..

makeappx pack /d bin\release\net461 /p MissionPlanner.appx

signtool sign /a /v /fd SHA256 /t http://timestamp.verisign.com/scripts/timestamp.dll /n "michael oborne" MissionPlanner.appx

c:\cygwin\bin\rsync.exe -Pv -e '/usr/bin/ssh -o StrictHostKeyChecking=no -o UserKnownHostsFile=/dev/null -i /cygdrive/c/Users/michael/sitl' MissionPlanner.appx michael@mega.ardupilot.org:MissionPlanner/

pause