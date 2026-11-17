#Requires -Version 5.1
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string]$MSBuildPath
)

$ErrorActionPreference = 'Stop'
if (-not $MSBuildPath) {
    $command = Get-Command MSBuild.exe -ErrorAction SilentlyContinue
    if ($command) { $MSBuildPath = $command.Source }
}
if (-not $MSBuildPath) {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path -LiteralPath $vswhere) {
        $MSBuildPath = & $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
    }
}
if (-not $MSBuildPath -or -not (Test-Path -LiteralPath $MSBuildPath)) {
    throw 'MSBuild.exe was not found. Install Visual Studio Build Tools or supply -MSBuildPath.'
}

foreach ($architecture in 'x86', 'x64') {
    & $MSBuildPath "$PSScriptRoot\CaffeineV2.sln" /t:Rebuild "/p:Configuration=$Configuration" "/p:Platform=$architecture" /nologo /verbosity:minimal
    if ($LASTEXITCODE -ne 0) { throw "The $architecture build failed." }

    $output = Join-Path $PSScriptRoot "bin\$architecture\$Configuration"
    $files = @(
        (Join-Path $output "CaffeineV2-$architecture.exe")
        (Join-Path $output "CaffeineV2-$architecture.exe.config")
        (Join-Path $PSScriptRoot 'caffeinev2_fix.ps1')
    )
    $package = Join-Path $PSScriptRoot "Binary\CaffeineForCitrixWorkspace-$architecture.zip"
    Compress-Archive -LiteralPath $files -DestinationPath $package -Force
    Write-Output $package
}
