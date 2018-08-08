@echo off

for /f %%f in ('dir /a-d /b ..\bin\Release\net461\plugins') do if exist ..\bin\Release\net461\%%f del ..\bin\Release\net461\plugins\%%f

wix.exe ..\bin\release\net461\

pause

