

for /f %%f in ('dir /a-d /b ..\bin\Release\net461\plugins') do if exist ..\bin\Release\net461\%%f del ..\bin\Release\net461\plugins\%%f

powershell -command "cd..; ls plugins -recurse | ForEach-Object { if (Test-Path ($_.fullname -replace '\\plugins\\','\') -PathType Leaf) { $_.fullname }} | ForEach-Object { del $_ }"

wix.exe ..\bin\release\net461\

pause

