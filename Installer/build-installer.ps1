$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$applicationProject = Join-Path $repositoryRoot 'WinFormsApp1\WinFormsApp1.csproj'
$installerProject = Join-Path $PSScriptRoot 'SexVip.Installer.wixproj'
$publishDirectory = Join-Path $repositoryRoot 'WinFormsApp1\bin\Release\net10.0-windows\win-x64\publish'

& dotnet publish $applicationProject -c Release -r win-x64 --self-contained true `
	-p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true `
	-p:DebugType=None -p:DebugSymbols=false
if ($LASTEXITCODE -ne 0) {
	throw 'A publicação do aplicativo falhou.'
}

& dotnet build $installerProject -c Release -p:InstallerPlatform=x64
if ($LASTEXITCODE -ne 0) {
	throw 'A compilação do instalador falhou.'
}

$installerFile = Join-Path $PSScriptRoot 'bin\x64\Release\SexVip.Installer.msi'
if (-not (Test-Path -LiteralPath $installerFile)) {
	$installerFile = Join-Path $PSScriptRoot 'bin\Release\SexVip.Installer.msi'
}
if (-not (Test-Path -LiteralPath $installerFile)) {
	throw 'O instalador MSI não foi encontrado após a compilação.'
}

Write-Host "Instalador criado: $installerFile"
Get-Item -LiteralPath $installerFile | Select-Object FullName, Length, LastWriteTime
