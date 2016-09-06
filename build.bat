

rem "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" MissionPlanner.sln /m /p:Configuration=Release /p:TargetFrameworkVersion=v4.6

"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" MissionPlanner.sln /m /p:Configuration=Release

del bin\release\MissionPlannerBeta.zip

"C:\Program Files (x86)\Windows Kits\10\bin\x86\makeappx" pack /d bin\Release /p MissionPlanner.appx

"C:\Program Files (x86)\Windows Kits\10\bin\x86\signtool" sign /a /v /fd SHA256 /t http://timestamp.verisign.com/scripts/timestamp.dll /n "3B1842DF-8664-4A02-B840-61F61DA8A94E" MissionPlanner.appx

pause