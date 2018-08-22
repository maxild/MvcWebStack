[CmdletBinding()]
param (
    [string]$Target = "Default",
    [ValidateSet("debug", "release")]
    [Alias('config')]
    [string]$Configuration = 'Release',
    [Alias('h')]
    [switch]$ShowHelp,
    [Alias('t')]
    [switch]$ShowTargets,
    [switch]$SkipRestore,
    [switch]$CleanCache,
    [switch]$SkipTests,
    [switch]$UpdateNuget
)

$RepoRoot = $PSScriptRoot
$ToolsDir = Join-Path $RepoRoot 'tools'
$NuGetExe = Join-Path $ToolsDir 'nuget.exe'

if (-not (Test-Path $ToolsDir)) {
    New-Item -ItemType directory -Path $ToolsDir -ErrorAction SilentlyContinue | out-null
}

function Install-NuGet {
    [CmdletBinding()]
    param([string] $NugetVersion = "latest")
    if (-not (Test-Path $NuGetExe)) {
        Write-Host -ForegroundColor Cyan 'Downloading nuget.exe'
        Invoke-WebRequest "https://dist.nuget.org/win-x86-commandline/$NugetVersion/nuget.exe" -OutFile $NuGetExe
    }
    elseif ($UpdateNuget.IsPresent) {
        # Update Nuget client
        & $NuGetExe update -self
    }
}

function Install-PSake {
    if (-not (Test-Path (Join-Path $ToolsDir 'psake'))) {
        & $NuGetExe install psake -version 4.7.0 -ExcludeVersion -OutputDirectory `"$ToolsDir`" -nocache -Source https://api.nuget.org/v3/index.json
    }
}

function Install-GitVersion {
    if (-not (Test-Path (Join-Path $ToolsDir 'GitVersion.CommandLine'))) {
        & $NuGetExe install GitVersion.CommandLine -version 4.0.0-beta0012 -ExcludeVersion -OutputDirectory `"$ToolsDir`" -nocache -Source https://api.nuget.org/v3/index.json
    }
}

function Install-XunitCliRunner {
    if (-not (Test-Path (Join-Path $ToolsDir 'xunit.runner.console'))) {
        & $NuGetExe install xunit.runner.console -version 2.4.0 -ExcludeVersion -OutputDirectory `"$ToolsDir`" -nocache -Source https://api.nuget.org/v3/index.json
    }
}

function Install-SourceLink {
    if (-not (Test-Path (Join-Path $RepoRoot 'packages\SourceLink'))) {
        & $NuGetExe install SourceLink -version 1.1.0 -ExcludeVersion -o packages -nocache
    }
}

Install-NuGet
Install-PSake
Install-GitVersion
Install-XunitCliRunner

$PsakePath = "$ToolsDir\psake\tools\psake\psake.ps1"

if (-not (Test-Path $PsakePath)) {
    throw "PSAKE have not been installed."
}

if ($ShowHelp.IsPresent) {
    & $PsakePath -help
}
elseif ($ShowTargets.IsPresent) {
    & $PsakePath .\psakefile.ps1 -docs
}
else {
    & $PsakePath .\psakefile.ps1 $Target -properties @{configuration=$Configuration}
}

# report success or failure
if ($psake.build_success -eq $false) { exit 1 } else { exit 0 }
