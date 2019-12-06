@echo off
rem <Compile Include="Program.cs" />
rem for /f %%f in ('dir /a-d /b plugins') do if exist .\%%f del .\plugins\%%f

rem <SubType>Control</SubType>

rem dir /b /s *.cs | find /v "ExtLibs" | for %%f in () do @echo ^<Compile Update="%%f" /^>   this crashs
rem ^| find /i /v "Plugins" ^| find /i /v "resedit" ^| find /i /v "SikRadio" ^| find /i /v "Updater" ^| find /i /v "wix" 

echo   ^<ItemGroup^>

@for /f %%f in ('dir /b /s *.cs ^| find /i /v "ExtLibs\" ^| find /i /v "Plugins\" ^| find /i /v "resedit\" ^| find /i /v "SikRadio\" ^| find /i /v "Updater\" ^| find /i /v "wix\" ^| find /i /v "\obj\" ^| find /i /v "Designer.cs" ') do (
	if exist %%~dpnf.Designer.cs ( 
		echo     ^<Compile Include="%%f" ^> ^<SubType^>Control^</SubType^> ^</Compile^>
	) else (
		echo     ^<Compile Include="%%f" /^>
	)
)

echo   ^</ItemGroup^>
echo   ^<ItemGroup^>

@for /f %%f in ('dir /b /s *.cs ^| find /i /v "ExtLibs\" ^| find /i /v "Plugins\" ^| find /i /v "resedit\" ^| find /i /v "SikRadio\" ^| find /i /v "Updater\" ^| find /i /v "wix\" ^| find /i /v "\obj\" ^| find /i /v "Designer.cs" ') do @if exist %%~dpnf.Designer.cs echo     ^<Compile Include="%%~dpnf.Designer.cs" ^>^<DependentUpon^>%%~nxf^</DependentUpon^> ^</Compile^>

echo   ^</ItemGroup^>
echo   ^<ItemGroup^>

@for /f %%f in ('dir /b /s *.cs ^| find /i /v "ExtLibs\" ^| find /i /v "Plugins\" ^| find /i /v "resedit\" ^| find /i /v "SikRadio\" ^| find /i /v "Updater\" ^| find /i /v "wix\" ^| find /i /v "\obj\" ^| find /i /v "Designer.cs" ') do (
	@for /f %%g in ('dir /b /s %%~dpnf.resx ^| find /i /v "ExtLibs\" ^| find /i /v "Plugins\" ^| find /i /v "resedit\" ^| find /i /v "SikRadio\" ^| find /i /v "Updater\" ^| find /i /v "wix\" ^| find /i /v "\obj\" ^| find /i /v "Designer.cs" ') do echo     ^<EmbeddedResource Include="%%g" ^>^<DependentUpon^>%%~nxf^</DependentUpon^> ^</EmbeddedResource^>
	@for /f %%g in ('dir /b /s %%~dpnf.*.resx ^| find /i /v "ExtLibs\" ^| find /i /v "Plugins\" ^| find /i /v "resedit\" ^| find /i /v "SikRadio\" ^| find /i /v "Updater\" ^| find /i /v "wix\" ^| find /i /v "\obj\" ^| find /i /v "Designer.cs" ') do echo     ^<EmbeddedResource Include="%%g" ^>^<DependentUpon^>%%~nxf^</DependentUpon^> ^</EmbeddedResource^>
)

echo   ^</ItemGroup^>

