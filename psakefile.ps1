Framework "4.7.1" # MSBuild 15

properties {
    $base_dir = resolve-path .
    $source_dir = join-path $base_dir "src"
    $test_dir = join-path $base_dir "test"
    $tools_dir = join-path $base_dir "tools"
    $nuspec_dir = join-path $base_dir "nuspec"
    $artifacts_dir = join-path $base_dir "artifacts"
    $sln_file = "Maxfire.sln"
    $configuration = "Release"
    $commonAssemblyInfoPath = join-path $base_dir "src" | join-path -ChildPath "CommonAssemblyInfo.cs"
    $framework_dir = Get-FrameworkDirectory
    $tools_version = "15.0" # MSBuild 15 == vs2017 == C#7
}

task default -depends dev
task verify -depends dev
task dev -depends compile, test     -description "developer build (before commits)"
task local -depends dev, pack       -description "local full build (producing local nupkg's)"
task all -depends dev, srcidx, pack -description "full build (producing source linked nupkg's)"

# TODO: kan slettes for public repo
task repairGitRemoteOnAppVeyor {
    if ($env:APPVEYOR -ne $NULL) {

        # private repo need github credentials
        $env:GITVERSION_REMOTE_USERNAME = "maxild"
        $env:GITVERSION_REMOTE_PASSWORD = $env:github_password

        # Running on AppVeyor, we have to patch private repos
        #   GitVersion (i.e. libgit2) doesn't support SSH, so to avoid 'Unsupported URL protocol'
        #   when GitVersion fetches from the remote, we have to convert to using HTTPS
        #   See http://help.appveyor.com/discussions/kb/17-getting-gitversion-to-work-with-private-bitbucketgithub-repositories

        $remoteUrl = & git remote get-url origin

        $repoHasHttpsUrl = $false;

        # AppVeyor uses https for public repos
        $httpsMatch = [System.Text.RegularExpressions.Regex]::Match($remoteUrl, "^https://github.com\/(?<RepositoryOwner>[-\w]+)/(?<RepositoryName>[-\w]+)\.git$", [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
        if ($httpsMatch.Success) {
            $repoOwner = $httpsMatch.Groups["RepositoryOwner"].Value;
            $repoName = $httpsMatch.Groups["RepositoryName"].Value;
            $repoHasHttpsUrl = $true;
            Write-Verbose "RepositoryOwner '$repoOwner' and RepositoryName '$repoName' resolved from origin remote url of the form 'https://github.com/$repoOwner/$repoName.git'."
        }
        else {
            # AppVeyor uses ssh for private repos
            $sshMatch = [System.Text.RegularExpressions.Regex]::Match($remoteUrl, "^git@github.com:(?<RepositoryOwner>[-\w]+)/(?<RepositoryName>[-\w]+)\.git$", [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
            if ($sshMatch.Success) {
                $repoOwner = $sshMatch.Groups["RepositoryOwner"].Value;
                $repoName = $sshMatch.Groups["RepositoryName"].Value;
                Write-Verbose "RepositoryOwner '$repoOwner' and RepositoryName '$repoName' resolved from origin remote url 'git@github.com:$repoOwner/$repoName.git'."
            }
            else {
                throw "Unable to resolve RepositoryOwner and RepositoryName from remote url '{$remoteUrl}'"
            }
        }

        # make libgit2 use https
        if ($repoHasHttpsUrl -eq $false) {
            &git remote set-url origin "https://github.com/$repoOwner/$repoName.git"
        }
    }
}

task resolveVersion -depends repairGitRemoteOnAppVeyor {

    $global:buildVersion = "local"

    if ($env:APPVEYOR -ne $NULL) {

        # Running on AppVeyor, we have to patch/setup local tracking branches (TODO: env vars)
        $output = & "$tools_dir\GitVersion.Commandline\tools\GitVersion.exe" /output buildserver
        if ($LASTEXITCODE -ne 0) {
            if ($output -is [array]) {
                Write-Error ($output -join "`r`n")
            } else {
                Write-Error $output
            }
            throw "GitVersion Exit Code: $LASTEXITCODE"
        }

        $global:majorMinorPatch = $env:GitVersion_MajorMinorPatch
        $global:semVersion = $env:GitVersion_SemVer
        $global:pkgVersion = $env:GitVersion_NuGetVersion
        $global:infoVersion = $env:GitVersion_InformationalVersion
        $global:buildVersion = "$($env:GitVersion_FullSemVer).build.$env:APPVEYOR_BUILD_NUMBER"

        # Update appveyor build details (-Version must be unique)
        Update-AppveyorBuild -Version $buildVersion
    }

    $output = & "$tools_dir\GitVersion.Commandline\tools\GitVersion.exe" /output json
    if ($LASTEXITCODE -ne 0) {
        if ($output -is [array]) {
            Write-Error ($output -join "`r`n")
        } else {
            Write-Error $output
        }
        throw "GitVersion Exit Code: $LASTEXITCODE"
    }
    $versionInfoJson = $output -join "`n"
    $versionInfo = $versionInfoJson | ConvertFrom-Json

    # Git (version) info
    $global:majorMinorPatch = $versionInfo.MajorMinorPatch
    $global:semVersion = $versionInfo.SemVer
    $global:pkgVersion = $versionInfo.NuGetVersion
    $global:infoVersion = $versionInfo.InformationalVersion

    #$global:commitId   = $versionInfo.Sha
    $global:commitId   = &git rev-parse --verify HEAD           # full 40 char Sha
    #$global:commitId   = &git rev-parse --verify --short HEAD   # 7 char commit id

    #$global:branchName = $versionInfo.BranchName
    $global:branchName = & git rev-parse --verify --abbrev-ref HEAD

    #$global:commitDate = $versionInfo.CommitDate

    # 2016-09-26T14:59:32+02:00 (sidste linje er stdout)
    $output = & git rev-list --format=%ad --date=iso-strict --max-count=1 HEAD
    $commitDateAsIsoWithOffset = $output.Split([System.Environment]::NewLine, [System.StringSplitOptions]::RemoveEmptyEntries) | Select-Object -Last 1
    $commitDateTimeOffset = [System.DateTimeOffset]::ParseExact($commitDateAsIsoWithOffset, "yyyy-MM-dd'T'HH:mm:ss.FFFK", [System.Globalization.CultureInfo]::InvariantCulture)
    $daDkTimeZoneId = "Romance Standard Time"
    $commitDateTimeLocalDanishTime = [System.TimeZoneInfo]::ConvertTimeFromUtc($commitDateTimeOffset.UtcDateTime, [System.TimeZoneInfo]::FindSystemTimeZoneById($daDkTimeZoneId))
    $global:commitDate = $commitDateTimeLocalDanishTime.ToString("yyyy-MM-dd'T'HH:mm:ss")

    # Assembly (version) info
    $global:assemblyVersion = $majorMinorPatch
    $global:assemblyFileVersion = ($assemblyVersion + ".0")
    $global:assemblyInformationalVersion = $infoVersion
}

task verifyGit {
    $gitFoundInPath = Get-File-Exists-On-Path "git.exe"
    Assert $gitFoundInPath "Git executable cannot be found in PATH environment variable."
    $gitVersion = &{git --version}
    Write-Host "$gitVersion is installed and found in PATH environment variable."
}

task verifyVersion -depends resolveVersion {
    Show-Configuration
}

# Source index the pdb file (See also http://ctaggart.github.io/SourceLink/exe.html)
# SourceLink enables GIT to be the source server by indexing the pdb file with http(s) download links.
task srcidx -depends compile {
    # This should only be performed on the build server (or in a clean checkout)
    # The SourceLink.exe tool is used to insert (the version control) information
    # into the "srcsrv" stream of the target .pdb file.
    $commitId = Get-Git-Commit-Full
    if ($commitId -ne "0000000000000000000000000000000000000000") {

        # Status should be 'clean'
        $gitStatus = (@(git status --porcelain) | Out-String)
        if ( -not ([string]::IsNullOrWhiteSpace($gitStatus)) ) {
            throw ("Git working tree or Git index is not clean, because 'git status --porcelain' is showing some output!!")
        }

        # Traverse all project files (ie. packages)
        $packages = Get-ChildItem $nuspec_dir *.nuspec -recurse
        $packages | %{
            $projectName = [io.path]::GetFileNameWithoutExtension($_.FullName)
            Write-Host "Source indexing PDB's from project $projectName" -ForegroundColor Yellow
            $projectfile   = "$source_dir\$projectName\$projectName.csproj"

            # URL for downloading the source files, use {0} for commit and %var2% for path
            exec {
                &"$tools_dir\SourceLink\tools\SourceLink.exe" index -pr $projectfile -pp Configuration $configuration -u 'https://raw.githubusercontent.com/maxild/MvcWebStack/{0}/%var2%' -c $commitId
            }

        }

    }
}

task clean {
    delete_directory $artifacts_dir
}

task restore {
    exec {
        & $tools_dir\nuget.exe restore $sln_file
    }
}

task compile -depends restore, resolveVersion, commonAssemblyInfo {
    Show-Configuration
    exec {
        # $output = &msbuild /version /nologo | Out-String
        # Write-Host $output

        #/p:TargetFrameworkVersion='v4.5.2' `
        msbuild /t:Clean /t:Build `
            /p:Configuration=$configuration `
            /v:minimal `
            /p:nowarn="1591" `
            /tv:"$tools_version" `
            /p:VisualStudioVersion="$tools_version" `
            /p:TreatWarningsAsErrors='true' `
            /p:MvcBuildViews='false' `
            /p:MvcPublishWebsite='false' `
            /maxcpucount `
            "$sln_file"
    }
}

task test -depends compile {

    # tfm is net452 (.NET Framework 4.5.2) (could be net461, net462, net471, net472)
    $xunitConsoleRunner = join-path $tools_dir -ChildPath 'xunit.runner.console' | `
                          join-path -ChildPath 'tools' | `
                          join-path -ChildPath 'net452' | `
                          join-path -ChildPath 'xunit.console.exe'

    $failures = @()

    # Traverse all project files (ie. packages)
    $packages = Get-ChildItem $nuspec_dir *.nuspec -recurse
    $packages | %{
        $testProjectName = ([io.path]::GetFileNameWithoutExtension($_.FullName) + ".UnitTests")
        $testAssemblyPath = "$test_dir\$testProjectName\bin\$configuration\$testProjectName.dll"

        Write-Host "Executing tests from '$testProjectName' under configuration '$configuration'" -ForegroundColor Yellow

        & $xunitConsoleRunner $testAssemblyPath
        if ($lastexitcode -ne 0) {
            $failures += (Split-Path -Leaf $testProjectName)
        }
    }

    if ($failures) {
        throw "Test failure!!! --- $failures"
    }
}

task commonAssemblyInfo -depends resolveVersion {
    $date = Get-Date
    "using System;
using System.Reflection;
using System.Runtime.InteropServices;

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: AssemblyCompany(""Maxfire"")]
[assembly: AssemblyCopyright(""Copyright Morten Maxild 2009-" + $date.Year + ". All rights reserved."")]
[assembly: AssemblyTrademark("""")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion(""$assemblyVersion"")]
[assembly: AssemblyFileVersion(""$assemblyFileVersion"")]
[assembly: AssemblyInformationalVersion(""$assemblyInformationalVersion"")]
[assembly: AssemblyProduct(""Maxfire Webstack Libraries"")]

[assembly: CLSCompliant(true)]

#if DEBUG
[assembly: AssemblyConfiguration(""Debug"")]
#else
[assembly: AssemblyConfiguration(""Release"")]
#endif
" | out-file $commonAssemblyInfoPath -encoding "utf8"
}

task pack -depends compile {

    # ensure we have an artifacts dir
    create_directory $artifacts_dir

    # Find dependency versions
    $preludeVersion = find-dependencyVersion (join-path (join-path $source_dir "Maxfire.Web.Mvc") "packages.config") 'Maxfire.Prelude.Core'
    $mvcVersion = find-dependencyVersion (join-path (join-path $source_dir "Maxfire.Web.Mvc") "packages.config") 'Microsoft.AspNet.Mvc'
    $sparkVersion = find-dependencyVersion (join-path (join-path $source_dir "Maxfire.Spark.Web.Mvc") "packages.config") 'Spark.Web.Mvc4'
    $castleCoreVersion = find-dependencyVersion (join-path (join-path $source_dir "Maxfire.Castle.Web.Mvc") "packages.config") 'Castle.Core'
    $castleWindsorVersion = find-dependencyVersion (join-path (join-path $source_dir "Maxfire.Castle.Web.Mvc") "packages.config") 'Castle.Windsor'
    $xunitVersion = find-dependencyVersion (join-path (join-path $source_dir "Maxfire.TestCommons") "packages.config") 'xunit'
    $rhinomocksVersion = find-dependencyVersion (join-path (join-path $source_dir "Maxfire.Web.Mvc.TestCommons") "packages.config") 'RhinoMocks'

    # Could use the -Version option of the nuget.exe pack command to provide the actual version.
    # _but_ the package dependency version cannot be overriden at the commandline.
    $packages = Get-ChildItem $nuspec_dir *.nuspec -recurse
    $packages | %{
        $nuspec = [xml](Get-Content $_.FullName)
        $nuspec.package.metadata.version = $global:pkgVersion
        $nuspec | Select-Xml '//dependency' | %{
            # Internal package versions
            if (($_.Node.id.StartsWith('Maxfire')) -and (-not $_.Node.id.StartsWith('Maxfire.Prelude'))) {
                $_.Node.version = $global:pkgVersion
            }
            # External package versions
            if ($_.Node.id -eq 'Maxfire.Prelude.Core') {
                $_.Node.version = $preludeVersion
            }
            if ($_.Node.id -eq 'Microsoft.AspNet.Mvc') {
                $_.Node.version = $mvcVersion
            }
            if ($_.Node.id.StartsWith('Spark')) {
                $_.Node.version = $sparkVersion
            }
            if ($_.Node.id -eq 'Castle.Core') {
                $_.Node.version = $castleCoreVersion
            }
            if ($_.Node.id -eq 'Castle.Windsor') {
                $_.Node.version = $castleWindsorVersion
            }
            if ($_.Node.id -eq 'xunit') {
                $_.Node.version = $xunitVersion
            }
            if ($_.Node.id -eq 'RhinoMocks') {
                $_.Node.version = $rhinomocksVersion
            }
        }

        $projectName = [io.path]::GetFileNameWithoutExtension($_.FullName)

        $nuspecFilename = join-path $artifacts_dir (Split-Path -Path $_.FullName -Leaf)
        $nuspec.Save($nuspecFilename)

        # All files in nuspec are relative paths, grab dll and pdb files using this base path
        $basePath = "$source_dir\$projectName\bin\$configuration"

        exec { & $tools_dir\Nuget.exe pack -BasePath $basePath -OutputDirectory $artifacts_dir $nuspecFilename }
    }
}

function find-dependencyVersion($packagesConfigPath, $packageId) {

    $dependencies = [xml](Get-Content  $packagesConfigPath)
    $dependencies | Select-Xml '//package' | %{
        if ($_.Node.id -eq $packageId) {
            $packageVersion = $_.Node.version
        }
    }

    if (-not $packageVersion) {
        throw "Could not resolve the version of the $packageId dependency."
    }

    return $packageVersion
}

# -------------------------------------------------------------------------------------------------------------
# functions
# --------------------------------------------------------------------------------------------------------------

function Show-Configuration {
    Write-Host -NoNewline "Compiling version '" -ForegroundColor Yellow
    Write-Host -NoNewline "$semVersion" -ForegroundColor DarkGreen
    Write-Host -NoNewline "' with '" -ForegroundColor Yellow
    Write-Host -NoNewline "$configuration" -ForegroundColor DarkGreen
    Write-Host "' configuration has resulted in versions defined by" -ForegroundColor Yellow

    Write-Host -NoNewline "  Version: " -ForegroundColor Yellow
    Write-Host "$semVersion" -ForegroundColor DarkGreen

    Write-Host -NoNewline "  NuGetVersion: " -ForegroundColor Yellow
    Write-Host "$pkgVersion" -ForegroundColor DarkGreen

    Write-Host -NoNewline "  BuildVersion: " -ForegroundColor Yellow
    Write-Host "$buildVersion" -ForegroundColor DarkGreen

    Write-Host -NoNewline "  CommitId: " -ForegroundColor Yellow
    Write-Host "$commitId" -ForegroundColor DarkGreen

    Write-Host -NoNewline "  CommitDate: " -ForegroundColor Yellow
    Write-Host "$commitDate" -ForegroundColor DarkGreen

    Write-Host -NoNewline "  BranchName: " -ForegroundColor Yellow
    Write-Host "$branchName" -ForegroundColor DarkGreen

    Write-Host -NoNewline "  AssemblyVersion: " -ForegroundColor Yellow
    Write-Host "$assemblyVersion" -ForegroundColor DarkGreen

    Write-Host -NoNewline "  AssemblyFileVersion: " -ForegroundColor Yellow
    Write-Host "$assemblyFileVersion" -ForegroundColor DarkGreen

    Write-Host -NoNewline "  AssemblyInformationalVersion: " -ForegroundColor Yellow
    Write-Host "$assemblyInformationalVersion" -ForegroundColor DarkGreen
}

function Get-File-Exists-On-Path([string]$file)
{
    $results = ($env:Path).Split(";") | Get-ChildItem -filter $file -erroraction silentlycontinue
    $found = ($results -ne $null)
    return $found
}

# partial sha with 7 chars (a3497c9)
function Get-Git-Commit
{
    if ((Get-File-Exists-On-Path "git.exe")){
        $gitLog = git log --oneline -1
        return $gitLog.Split(' ')[0]
    }
    else {
        return "0000000"
    }
}

# full sha with 40 chars (a3497c9f044f45b5e295f7fb9d7494df3c209a31)
function Get-Git-Commit-Full
{
    if ((Get-File-Exists-On-Path "git.exe")){
        $gitLog = git log -1 --format=%H
        return $gitLog;
    }
    else {
        return "0000000000000000000000000000000000000000"
    }
}

# directory where MSBuild.exe is to be found
function Get-FrameworkDirectory()
{
    $frameworkPath = "$env:windir\Microsoft.NET\Framework\v4.0*"
    $frameworkPathDir = ls "$frameworkPath"
    if ( $frameworkPathDir -eq $null ) {
        throw "Building Brf.Lofus.Core requires .NET 4.0, which doesn't appear to be installed on this machine"
    }
    $net4Version = $frameworkPathDir.Name
    return "$env:windir\Microsoft.NET\Framework\$net4Version"
}

function global:delete_directory($directory_name)
{
  rd $directory_name -recurse -force  -ErrorAction SilentlyContinue | out-null
}

function global:delete_file($file)
{
    if($file) {
        remove-item $file  -force  -ErrorAction SilentlyContinue | out-null
    }
}

function global:create_directory($directory_name)
{
  mkdir $directory_name  -ErrorAction SilentlyContinue  | out-null
}

function global:copy_files($source, $destination, $exclude = @()) {
    create_directory $destination
    Get-ChildItem $source -Recurse -Exclude $exclude |
        Copy-Item -Destination {Join-Path $destination $_.FullName.Substring($source.length)}
}
