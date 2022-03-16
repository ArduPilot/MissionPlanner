set PATH=%PATH%;C:\Program Files (x86)\Windows Kits\8.1\bin\x86;C:\Program Files (x86)\Windows Kits\10\bin\x86

for /r %%v in (*.inf) do DPInstx64.exe /u %%v /d /s

pause