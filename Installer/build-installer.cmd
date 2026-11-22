@echo off
setlocal
set "REPO=%~dp0.."

dotnet publish "%REPO%\WinFormsApp1\WinFormsApp1.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=None -p:DebugSymbols=false -o "%REPO%\WinFormsApp1\bin\Release\net10.0-windows\win-x64\installer-publish"
if errorlevel 1 exit /b %errorlevel%

dotnet build "%~dp0SexVip.Installer.wixproj" -c Release -p:InstallerPlatform=x64
if errorlevel 1 exit /b %errorlevel%

echo Installer created at:
echo %~dp0bin\Release\SexVip.Installer.msi
endlocal
