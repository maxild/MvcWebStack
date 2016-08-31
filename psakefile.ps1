# Note that using the value 4.5.1 also works for projects that target 4.5.2
# (the value 4.5.2 is not recognised by psake)
# This will make msbuild point to one of:
#    [ProgramFilesX86]\MSBuild\12.0\bin\amd64 (x64 bitness of powershell)
#    [ProgramFilesX86]\MSBuild\12.0\bin (x32 bitness of powershell)
# See also http://blogs.msdn.com/b/visualstudio/archive/2013/07/24/msbuild-is-now-part-of-visual-studio.aspx
# See also https://github.com/damianh/psake/commit/0778be759e83014f0bfdd1a0c84caac008c8112f
Framework 4.5.2

properties {
    $base_dir = resolve-path .
    $build_dir = join-path $base_dir "build"
    $artifacts_dir = join-path $base_dir "artifacts"
    $source_dir = join-path $base_dir "src"
    $test_dir = join-path $base_dir "test"
    $packages_dir = join-path $base_dir "packages"
    $sln_file = "Maxfire.sln"
    $configuration = "Release"
    $commonAssemblyInfoPath = join-path $base_dir "src" | join-path -ChildPath "CommonAssemblyInfo.cs"
    $global:version = "1.0.0-*"
    $framework_dir = Get-FrameworkDirectory
    $tools_version = "14.0" # MSBuild 14 == vs2015 == C#6
}

task default -depends dev
task verify -depends dev
task dev -depends compile, test     -description "developer build (before commits)"
task local -depends dev, pack       -description "local full build (producing local nupkg's)"
task all -depends dev, srcidx, pack -description "full build (producing source linked nupkg's)"

# For project.json as { "version": "1.0.0-*", ...}, together with label='alpha'
# and build=12345, the end result is something equivalent to (DNX_BUILD_VERSION=alpha-12345, DNX_ASSEMBLY_FILE_VERSION=12345)
#  [assembly: AssemblyVersion("1.0.0.0")]
#  [assembly: AssemblyFileVersion("1.0.0.12345")]
#  [assembly: AssemblyInformationalVersion("1.0.0-alpha-12345")]
#
# Before building (in build.ps1) do something like
#      $env:DNX_BUILD_VERSION="${PrereleaseTag}-${paddedBuildNumber}"
#      $env:DNX_ASSEMBLY_FILE_VERSION=$buildNumber
task resolveVersions {
    $commitId = Get-Git-Commit-Full
    
    if ($global:version.EndsWith('-*')) {
        $global:assemblyVersion = $global:version.Substring(0, $global:version.Length - 2)
        if ($env:DNX_BUILD_VERSION -ne $NULL) {
            $global:pkgVersion = $global:version.Substring(0, $global:version.Length - 1) + $env:DNX_BUILD_VERSION
        }
        else {
            $global:pkgVersion = $global:assemblyVersion
        }
    }
    else {
        $global:assemblyVersion = $global:version
        $global:pkgVersion = $global:version
    }

    if ($env:DNX_ASSEMBLY_FILE_VERSION -ne $NULL) {
        [int]$fileVersion = $env:DNX_ASSEMBLY_FILE_VERSION
    }
    else {
        [int]$fileVersion = 0
    }
    $global:assemblyFileVersion = "${global:assemblyVersion}.$fileVersion"
    $global:assemblyInformationalVersion = "$pkgVersion / $commitId /"
    # To access version information at run time in .NET:
    #    var executingAssembly = Assembly.GetExecutingAssembly();
    #    var fv = System.Diagnostics.FileVersionInfo.GetVersionInfo(executingAssembly.Location);
    #    Console.WriteLine(executingAssembly.GetName().Version); // AssemblyVersion
    #    Console.WriteLine(fv.FileVersion);                      // AssemblyFileVersion
    #    Console.WriteLine(fv.ProductVersion);                   // AssemblyInformationalVersion
}

task versionInfo -depends resolveVersions {
    Write-Host "Version: $pkgVersion"
    Write-Host "AssemblyVersion: $assemblyVersion"
    Write-Host "AssemblyFileVersion: $assemblyFileVersion"
    Write-Host "AssemblyInformationalVersion: $assemblyInformationalVersion"
}

# Source index the pdb file (See also http://ctaggart.github.io/SourceLink/exe.html)
# SourceLink enables GIT to be the source server by indexing the pdb file with http(s) download links.
task srcidx -depends compile {
    # This should only be performed on the build server (or in a clean checkout)
    # The SourceLink.exe tool is used to insert (the version control) information
    # into the "srcsrv" stream of the target .pdb file.
    $commitId = Get-Git-Commit-Full
    if ($commitId -ne "0000000000000000000000000000000000000000") {

        Status should be 'clean'
        $gitStatus = (@(git status --porcelain) | Out-String)
        if ( -not ([string]::IsNullOrWhiteSpace($gitStatus)) ) {
            throw ("Git working tree or Git index is not clean, because 'git status --porcelain' is showing some output!!")
        }

        # Traverse all project files (ie. packages)
        $packages = Get-ChildItem $build_dir *.nuspec -recurse
        $packages | %{
            $projectName = [io.path]::GetFileNameWithoutExtension($_.FullName)
            Write-Host "Source indexing PDB's from project $projectName" -ForegroundColor Yellow
            $projectfile   = "$source_dir\$projectName\$projectName.csproj"
            
            # URL for downloading the source files, use {0} for commit and %var2% for path
            exec {
                &"$packages_dir\SourceLink\tools\SourceLink.exe" index -pr $projectfile -pp Configuration $configuration -u 'https://raw.githubusercontent.com/maxild/MvcWebStack/{0}/%var2%' -c $commitId
            }

        }
        
    }
}

task Clean {
    delete_directory $artifacts_dir
}

task restore {
    exec {
        # always update nuget to latest release
        & $base_dir\.nuget\Nuget.exe update -self
        & $base_dir\.nuget\Nuget.exe restore $sln_file
    }
}

task compile -depends clean, restore, commonAssemblyInfo {
    Write-Host "Compiling '$sln_file' with '$configuration' configuration" -ForegroundColor Yellow
    exec { msbuild /t:Clean /t:Build /p:Configuration=$configuration /p:TreatWarningsAsErrors=true /v:minimal /tv:${tools_version} /p:VisualStudioVersion=${tools_version} /maxcpucount "$sln_file" }
}

task test -depends compile {

    $xunitConsoleRunner = join-path $packages_dir 'xunit.runner.console.2.1.0' | `
                          join-path -ChildPath 'tools' | `
                          join-path -ChildPath 'xunit.console.exe'

    $failures = @()
        
    # Traverse all project files (ie. packages)
    $packages = Get-ChildItem $build_dir *.nuspec -recurse
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

task commonAssemblyInfo -depends resolveVersions {
    create-commonAssemblyInfo $(Get-Git-Commit-Full) $commonAssemblyInfoPath
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
    $packages = Get-ChildItem $build_dir *.nuspec -recurse
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
        
        exec { & $base_dir\.nuget\Nuget.exe pack -BasePath $basePath -OutputDirectory $artifacts_dir $nuspecFilename }
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

function global:create-commonAssemblyInfo($commit, $filename)
{
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
" | out-file $filename -encoding "utf8"
}

# -------------------------------------------------------------------------------------------------------------
# generalized functions
# --------------------------------------------------------------------------------------------------------------
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

function Get-PackagePath {
    param([string]$packageName)

    $packagePath = Get-ChildItem "$packages_dir\$packageName.*" |
                        Sort-Object Name -Descending |
                        Select-Object -First 1
    Return "$packagePath"
}

function Get-DependencyPackageFiles
{
    param([string]$packageName, [string]$frameworkVersion = "net45")

    $packagePath = Get-PackagePath $packageName
    Return "$packagePath\lib\$frameworkVersion\*"
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
