@echo off


set "RELEASEDIR=%tmp%\PEDollRelease"


:main
	pushd %~dp0
	msbuild -ver >nul 2>nul
	if %ERRORLEVEL% neq 0 (
		echo Run this batch file from a Developer Command Prompt!
		goto :eof
	)

	if exist "%RELEASEDIR%" (
		rd /s /q "%RELEASEDIR%"
	)

	md %RELEASEDIR%

	::call :buildAll Debug
	call :buildAll Release

	start explorer %RELEASEDIR%
	popd
goto :eof


:: call :buildAll Debug
:buildAll
	call :buildController %1 ^
	&& call :buildMonitor x86 %1 ^
	&& call :buildMonitor x64 %1
goto :eof


:: call :buildController Debug
:buildController
	set PLATFORMDIR=PEDollController\bin

	msbuild PEDoll.sln -t:PEDollController -p:Platform="Any CPU";Configuration=%1
	if %ERRORLEVEL% neq 0 (
		goto :eof
	)

	if not exist "%RELEASEDIR%\%1" (
		md "%RELEASEDIR%\%1"
	)

	xcopy /e %PLATFORMDIR%\%1 "%RELEASEDIR%\%1\"
	xcopy /e /i Scripts "%RELEASEDIR%\%1\Scripts"

	wsl ./GenerateAPIx64.sh "%RELEASEDIR%\%1\Scripts\API"
goto :eof


:: call :buildMonitor x64 Debug
:buildMonitor

	if %1 equ x86 (
		set PLATFORMDIR=.
	) else (
		set PLATFORMDIR=x64
	)

	msbuild PEDoll.sln -t:PEDollMonitor,libDoll -p:Platform=%1;Configuration=%2
	if %ERRORLEVEL% neq 0 (
		goto :eof
	)

	if not exist "%RELEASEDIR%\%2" (
		md "%RELEASEDIR%\%2"
	)
	
	md "%RELEASEDIR%\%2\Monitor_%1"

	copy %PLATFORMDIR%\%2\PEDollMonitor.exe "%RELEASEDIR%\%2\Monitor_%1\"
	copy %PLATFORMDIR%\%2\PEDollMonitor.pdb "%RELEASEDIR%\%2\Monitor_%1\"
	copy %PLATFORMDIR%\%2\libDoll.dll "%RELEASEDIR%\%2\Monitor_%1\"
	copy %PLATFORMDIR%\%2\libDoll.pdb "%RELEASEDIR%\%2\Monitor_%1\"

goto :eof