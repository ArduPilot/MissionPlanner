
set PATH=%PATH%;C:\Program Files (x86)\Microsoft Visual Studio\Preview\Community\MSBuild\15.0\Bin;C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin

del bin\release\MissionPlannerBeta.zip

.nuget\nuget.exe restore

MSBuild.exe MissionPlanner.sln /m /p:Configuration=Release /verbosity:d

echo create appx?
pause

"C:\Program Files (x86)\Windows Kits\10\bin\x86\makeappx" pack /d bin\release\net461 /p MissionPlanner.appx

"C:\Program Files (x86)\Windows Kits\10\bin\x86\signtool" sign /a /v /fd SHA256 /t http://timestamp.verisign.com/scripts/timestamp.dll /n "3B1842DF-8664-4A02-B840-61F61DA8A94E" MissionPlanner.appx

c:\cygwin\bin\rsync.exe -Pv -e '/usr/bin/ssh -o StrictHostKeyChecking=no -o UserKnownHostsFile=/dev/null -i /cygdrive/c/Users/michael/sitl' MissionPlanner.appx michael@mega.ardupilot.org:MissionPlanner/

pause