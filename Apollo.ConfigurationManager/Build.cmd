@echo off
set a=%cd: =:%
set a=%a:\= %
for %%j in (%a%) do (set a=%%j)
set a=%a::= %

if exist "%a%.csproj" (
	set Proj="%a%.csproj"
) else if exist "%a%.vbproj" (
	set Proj="%a%.vbproj"
) else if exist "%a%.sln" (
	set Proj="%a%.sln"
)

where dotnet >nul 2>&1
if %ERRORLEVEL% == 0 (
	if "%1" == "Debug" (
		echo dotnet build -m -v m -c Debug %Proj%
		dotnet build -m -v m -c Debug %Proj%
	) else (
		echo dotnet build -m -v m -c Release %Proj%
	    dotnet build -m -v m -c Release %Proj%
	)
	GOTO End
)

for /f "tokens=2*" %%i in ('reg query HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion^|findstr "ProgramFilesDir (x86)"') do set p=%%j

set defaultProgramDir=C:\Program Files (x86)
set driList[0]=%p:~10%
if "%driList[0]%" NEQ "%defaultProgramDir%" set driList[1]=%defaultProgramDir%

set buildList[0]=Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin
set buildList[1]=Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin
set buildList[2]=Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin
set buildList[3]=MSBuild\14.0\Bin
set buildList[4]=MSBuild\12.0\Bin
set buildList[5]=MSBuild\11.0\Bin
set buildList[6]=MSBuild\10.0\Bin

for /F "tokens=2 delims==" %%d in ('set driList[') do (
	for /F "tokens=2 delims==" %%b in ('set buildList[') do (
		set MSBuild="%%d\%%b\MSBuild.exe"
		if exist "%%d\%%b\MSBuild.exe" (
			GOTO SetMSBuild
		)
	)
)

set MSBuild=%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe

:SetMSBuild
set Nuget=nuget restore %Proj%

if "%1" == "Debug" (
	set MSBuild=%MSBuild% %Proj% /m /v:m /p:Configuration=Debug
) else (
	set MSBuild=%MSBuild% %Proj% /m /v:m /p:Configuration=Release
)

if exist "nuget.exe" (
	echo nuget source add -name Tuhu.cn -source https://nuget.tuhu.cn/nuget
	nuget source add -name Tuhu.cn -source https://nuget.tuhu.cn/nuget
	echo.
	
	echo %Nuget%
	%Nuget%
	echo.

	echo %MSBuild%
	%MSBuild%

	IF %ERRORLEVEL% == 0 (
		if exist "%a%.csproj" (
			if not exist "%a%.nuspec" (
				nuget spec
			)

			echo.
			nuget pack "%a%.csproj" -Prop Configuration=Release
		) else if exist "%a%.vbproj" (
			if not exist "%a%.nuspec" (
				nuget spec
			)

			echo.
			nuget pack "%a%.vbproj" -Prop Configuration=Release
		)
	)
) else (
	echo %MSBuild%
	%MSBuild%
)
:End
pause
