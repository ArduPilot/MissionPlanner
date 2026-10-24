@echo off
setlocal EnableDelayedExpansion

rem Regenerates the .cat for each .inf in this folder with Inf2Cat (WDK) and signs it with signtool (SDK).
rem Only catalogs that are missing, unsigned, or no longer contain the hash of their .inf are redone;
rem set FORCE=1 to redo all of them.
rem
rem Signing certificate, pick one:
rem   set SIGN_PFX=C:\path\cert.pfx  and  set SIGN_PFX_PASSWORD=...   (certificate file)
rem   otherwise the certificate store is used, by thumbprint SIGN_SHA1 (default: the certificate in
rem   signed.cer, which the installer adds as a trusted publisher)
rem   set SIGN_ARGS=/csp "..." /kc "..." /f cert.cer                 (hardware token, overrides both)

cd /d "%~dp0"

if not defined SIGN_SHA1 set SIGN_SHA1=B6B1638E5EFC0DA254F575DF248AD0D1D227924B
if not defined TIMESTAMP_URL set TIMESTAMP_URL=http://timestamp.digicert.com

rem Inf2Cat ships with the WDK, signtool with the SDK; both live under Windows Kits\10\bin\<version>\
set "KITBIN=%ProgramFiles(x86)%\Windows Kits\10\bin"
set INF2CAT=
set SIGNTOOL=
for /f "delims=" %%d in ('dir /b /ad /o-n "%KITBIN%\10.*" 2^>nul') do (
    if not defined INF2CAT if exist "%KITBIN%\%%d\x86\Inf2Cat.exe" set "INF2CAT=%KITBIN%\%%d\x86\Inf2Cat.exe"
    if not defined SIGNTOOL if exist "%KITBIN%\%%d\x64\signtool.exe" set "SIGNTOOL=%KITBIN%\%%d\x64\signtool.exe"
)
if not defined INF2CAT (
    echo Inf2Cat.exe not found under "%KITBIN%". Install the WDK: winget install --id Microsoft.WindowsWDK.10.0.26100 -e
    goto :fail
)
if not defined SIGNTOOL (
    echo signtool.exe not found under "%KITBIN%". Install the Windows SDK.
    goto :fail
)
echo Inf2Cat:  %INF2CAT%
echo signtool: %SIGNTOOL%

if defined SIGN_ARGS (
    set "CERTARGS=%SIGN_ARGS%"
) else if defined SIGN_PFX (
    set CERTARGS=/f "%SIGN_PFX%"
    if defined SIGN_PFX_PASSWORD set CERTARGS=!CERTARGS! /p "%SIGN_PFX_PASSWORD%"
) else (
    set CERTARGS=/sha1 %SIGN_SHA1%
)

rem SHA-256 signatures only validate on Windows 7 (with KB3033929) and later
set OSLIST=7_X86,7_X64,Server2008R2_X64,8_X86,8_X64,Server8_X64,6_3_X86,6_3_X64,10_X86,10_X64,Server10_X64

rem These catalogs are signed by their vendors or by Microsoft (WHQL); re-signing them would replace that
rem signature with ours, so leave them alone. ubloxusb also ships kernel-mode .sys files.
set "SKIP= arduino.inf stm32bootloader.inf stmcdc.inf ubloxusb.inf "

set FAILED=0
for %%v in (*.inf) do (
    echo.
    echo "%SKIP%" | findstr /i /c:" %%v " >nul
    if errorlevel 1 (
        call :catalog "%%v"
    ) else (
        echo === %%v skipped, not our signature
    )
)

echo.
if "%FAILED%"=="1" goto :fail
echo Done.
pause
exit /b 0

:catalog
set "INF=%~1"
if not defined FORCE (
    rem up to date when the catalog named by CatalogFile= exists, is signed and holds the .inf's current hash
    powershell -NoProfile -ExecutionPolicy Bypass -Command "$i=$env:INF; $m=Select-String -LiteralPath $i -Pattern '^\s*CatalogFile\s*=\s*([^\s;]+)' | Select-Object -First 1; if (-not $m) {exit 1}; $c=$m.Matches[0].Groups[1].Value; if (-not (Test-Path -LiteralPath $c)) {exit 1}; if ((Get-AuthenticodeSignature -LiteralPath $c).Status -in 'NotSigned','HashMismatch') {exit 1}; $b=[BitConverter]::ToString([IO.File]::ReadAllBytes((Resolve-Path -LiteralPath $c).Path)) -replace '-',''; foreach ($a in 'SHA1','SHA256') { if ($b.Contains((Get-FileHash -LiteralPath $i -Algorithm $a).Hash)) {exit 0} }; exit 1"
    if not errorlevel 1 (
        echo === %INF% up to date
        exit /b 0
    )
)
echo === %INF%
rem Inf2Cat catalogs every .inf in the folder it is given, so stage each one on its own
if exist "%INF%-t" rmdir /s /q "%INF%-t"
mkdir "%INF%-t"
copy /y "%INF%" "%INF%-t\" >nul
"%INF2CAT%" /driver:"%INF%-t" /os:%OSLIST%
if errorlevel 1 (
    echo Inf2Cat failed for %INF%
    set FAILED=1
    rmdir /s /q "%INF%-t"
    exit /b 1
)
for %%c in ("%INF%-t\*.cat") do (
    move /y "%%c" . >nul
    "%SIGNTOOL%" sign /fd sha256 /tr %TIMESTAMP_URL% /td sha256 !CERTARGS! "%%~nxc"
    if errorlevel 1 (
        echo signtool failed for %%~nxc
        set FAILED=1
    )
)
rmdir /s /q "%INF%-t"
exit /b 0

:fail
echo One or more drivers failed, see above.
pause
exit /b 1
