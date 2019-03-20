
set PATH=%PATH%;C:\Program Files (x86)\Microsoft Visual Studio\Preview\Community\MSBuild\15.0\Bin;C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin

del bin\release\MissionPlannerBeta.zip

.nuget\nuget.exe restore

MSBuild.exe MissionPlanner.sln /m /p:Configuration=Release /verbosity:n

echo create appx?
pause

cd bin\release\net461

for /f %%f in ('dir /a-d /b plugins') do if exist .\%%f del .\plugins\%%f

cd ..
cd ..
cd ..

"C:\Program Files (x86)\Windows Kits\10\Tools\bin\i386\makeappx" pack /d bin\release\net461 /p MissionPlanner.appx

"C:\Program Files (x86)\Windows Kits\10\Tools\bin\i386\signtool" sign /a /v /fd SHA256 /t http://timestamp.verisign.com/scripts/timestamp.dll /n "3B1842DF-8664-4A02-B840-61F61DA8A94E" MissionPlanner.appx

c:\cygwin\bin\rsync.exe -Pv -e '/usr/bin/ssh -o StrictHostKeyChecking=no -o UserKnownHostsFile=/dev/null -i /cygdrive/c/Users/michael/sitl' MissionPlanner.appx michael@mega.ardupilot.org:MissionPlanner/

pause