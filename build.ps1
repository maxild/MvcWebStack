[CmdletBinding()]
param (
    [string]$Target = "Default",
    [ValidateSet("debug", "release")]
    [Alias('config')]
    [string]$Configuration = 'Release',
    [switch]$SkipRestore,
    [switch]$CleanCache,
    [switch]$SkipTests
)

$RepoRoot = $PSScriptRoot
$NuGetExe = Join-Path $RepoRoot '.nuget\nuget.exe'

function Install-NuGet {
    [CmdletBinding()]
    param([string] $NugetVersion = "latest")
    if (-not (Test-Path $NuGetExe)) {
        Write-Host -ForegroundColor Cyan 'Downloading nuget.exe'
        $NuGetDir = Join-Path $RepoRoot '.nuget'
        if (-not (Test-Path $NuGetDir)) {
            New-Item -ItemType directory -Path $NuGetDir
        }
        Invoke-WebRequest "https://dist.nuget.org/win-x86-commandline/$NugetVersion/nuget.exe" -OutFile $NuGetExe
    }
}

function Install-PSake {
    if (-not (Test-Path (Join-Path $RepoRoot 'packages\psake'))) {
        & $NuGetExe install psake -version 4.6.0 -ExcludeVersion -o packages -nocache
    }
}

function Install-GitVersion {
    if (-not (Test-Path (Join-Path $RepoRoot 'packages\GitVersion.CommandLine'))) {
        & $NuGetExe install GitVersion.CommandLine -version 4.0.0-beta0007 -ExcludeVersion -o packages -nocache
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
Install-SourceLink

# right now it is hardcoded to full task
.\packages\psake\tools\psake.ps1 .\psakefile.ps1 $Target -properties @{configuration=$Configuration}

# report success or failure
if ($psake.build_success -eq $false) { exit 1 } else { exit 0 }
