
set PATH=%PATH%;C:\Program Files (x86)\Microsoft Visual Studio\2019\Preview\MSBuild\Current\Bin;C:\Program Files (x86)\Microsoft Visual Studio\Preview\Community\MSBuild\15.0\Bin;C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin

del bin\release\MissionPlannerBeta.zip

.nuget\nuget.exe restore

MSBuild.exe MissionPlannerLib.sln /m /p:Configuration=Release /verbosity:n

pause
