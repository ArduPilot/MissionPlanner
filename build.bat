

"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" MissionPlanner.sln /m /p:Configuration=Release /p:Mavlink10=true 

del bin\release\MissionPlannerBeta.zip

"C:\Program Files (x86)\Windows Kits\8.1\bin\x86\makeappx" pack /d bin\Release /p MissionPlanner.appx

"C:\Program Files (x86)\Windows Kits\8.1\bin\x86\signtool" sign /a /v /fd SHA256 /t http://timestamp.verisign.com/scripts/timestamp.dll /n "Michael Oborne" MissionPlanner.appx

pause