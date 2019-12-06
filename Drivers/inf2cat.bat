set PATH=%PATH%;C:\Program Files (x86)\Windows Kits\10\Tools\bin\i386;C:\Program Files (x86)\Windows Kits\8.1\bin\x86;C:\Program Files (x86)\Windows Kits\10\bin\x86;C:\Program Files (x86)\Windows Kits\10\bin\10.0.17763.0\x64

for %%v in (*.inf) do (
mkdir "%%v-t"
copy "%%v" "%%v-t\"
inf2cat.exe /driver:"%%v-t" /os:XP_X86,XP_X64,Server2003_X86,Server2003_X64,Vista_X86,Server2008_X86,Vista_X64,Server2008_X64,7_X86,7_X64,Server2008R2_X64,8_X86,8_X64,6_3_X86,6_3_X64,Server8_X64,10_X86,10_X64,Server10_X64
)

pause

for /r %%v in (*.cat) do move "%%v" ".\"

rem cleanup
for /r %%v in (*.inf) do del "%%v-t\%%~nv%%~xv"
for /r %%v in (*.inf) do rmdir "%%v-t"

pause

for /r %%v in (*.cat) do start /min signtool sign /n "Michael Oborne" /t http://timestamp.verisign.com/scripts/timestamp.dll "%%v"

cmd

pause