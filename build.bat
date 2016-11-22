
del bin\release\MissionPlannerBeta.zip

"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" MissionPlanner.sln /m /p:Configuration=Release


rem DesktopAppConverter.exe -installer MissionPlanner-1.3.39.msi -destination f:\temp\appx -packagename "MissionPlanner" -publisher "CN=3B1842DF-8664-4A02-B840-61F61DA8A94E" -Version 1.3.39.0 -makeappx -Verbose

rem "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" MissionPlanner.sln /m /p:Configuration=Release /p:TargetFrameworkVersion=v4.6 /p:OutputPath=bin\4.6

echo create appx?
pause

"C:\Program Files (x86)\Windows Kits\10\bin\x86\makeappx" pack /d bin\release /p MissionPlanner.appx

"C:\Program Files (x86)\Windows Kits\10\bin\x86\signtool" sign /a /v /fd SHA256 /t http://timestamp.verisign.com/scripts/timestamp.dll /n "3B1842DF-8664-4A02-B840-61F61DA8A94E" MissionPlanner.appx

c:\cygwin\bin\rsync.exe -Pv -e '/usr/bin/ssh -o StrictHostKeyChecking=no -o UserKnownHostsFile=/dev/null -i /cygdrive/c/Users/michael/sitl' MissionPlanner.appx michael@bios.ardupilot.org:MissionPlanner/

pause