Framework "4.7.1" # MSBuild 15

$psake.use_exit_on_error = $true
properties {
    # bootstrapper props
    $target = "default"
    $configuration = "Release"
    # msbuild
    $tools_version = "15.0" # MSBuild 15 == vs2017 == C#7
    # paths
    $base_dir = resolve-path .
    $source_dir = join-path $base_dir "src"
    $test_dir = join-path $base_dir "test"
    $tools_dir = join-path $base_dir "tools"
    $nuspec_dir = join-path $base_dir "nuspec"
    $artifacts_dir = join-path $base_dir "artifacts"
    # files
    $sln_file = "Maxfire.sln"
    $commonAssemblyInfoPath = join-path $base_dir "src" | join-path -ChildPath "CommonAssemblyInfo.cs"
    # deployment
    $repositoryName = "MvcWebStack"
    $repositoryOwner = "maxild"
    $cifeed_url = "https://www.myget.org/F/maxfire-ci/api/v2/package"
    $prodfeed_url = "https://www.nuget.org/api/v2/package"
}

task Default -depends pack

task ReleaseNotes -depends Create-Release-Notes

task AppVeyor -depends info, test, srcidx, pack, Upload-AppVeyor-Artifacts, publish, Publish-GitHub-Release

# TODO: kan slettes for public repo
task repairGitRemoteOnAppVeyor {
    if ($env:APPVEYOR -ne $NULL) {

        # private repo need github credentials
        $env:GITVERSION_REMOTE_USERNAME = $repositoryOwner
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

        # Running on AppVeyor, we have to patch/setup local tracking branches
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
    $global:commitSha   = &git rev-parse --verify HEAD           # full 40 char Sha
    $global:commitId   = &git rev-parse --verify --short HEAD    # 7 char commit id

    #$global:branchName = $versionInfo.BranchName
    $global:branchName = & git rev-parse --verify --abbrev-ref HEAD

    $global:commitTag = & git tag -l --points-at HEAD

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

task info -depends resolveVersion {
    Show-Info
    echoAppVeyorEnvironmentVariables
}


# Source index the pdb file (See also http://ctaggart.github.io/SourceLink/exe.html)
# SourceLink enables GIT to be the source server by indexing the pdb file with http(s) download links.
task srcidx -depends compile {
    # This should only be performed on the build server (or in a clean checkout)
    # The SourceLink.exe tool is used to insert (the version control) information
    # into the "srcsrv" stream of the target .pdb file.

    # Status should be 'clean'
    $gitStatus = (@(git status --porcelain) | Out-String)
    if ([string]::IsNullOrWhiteSpace($gitStatus)) {
        # Traverse all project files (ie. packages)
        $packages = Get-ChildItem $nuspec_dir *.nuspec -recurse
        $packages | ForEach-Object {
            $projectName = [io.path]::GetFileNameWithoutExtension($_.FullName)
            Write-Host "Source indexing PDB's from project $projectName" -ForegroundColor Yellow
            $projectfile   = "$source_dir\$projectName\$projectName.csproj"

            # URL for downloading the source files, use {0} for commit and %var2% for path
            exec {
                &"$tools_dir\SourceLink\tools\SourceLink.exe" index -pr $projectfile -pp Configuration $configuration -u 'https://raw.githubusercontent.com/maxild/MvcWebStack/{0}/%var2%' -c $commitSha
            }

        }
    }
    else {
        Write-Warning "Skipping Source Indexing: Git working tree or Git index is not clean, because 'git status --porcelain' is showing some output!!"
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
    Write-Host -NoNewline "Compiling version '" -ForegroundColor Yellow
    Write-Host -NoNewline "$semVersion" -ForegroundColor DarkGreen
    Write-Host -NoNewline "' with '" -ForegroundColor Yellow
    Write-Host -NoNewline "$configuration" -ForegroundColor DarkGreen
    Write-Host "' configuration has resulted in versions defined by" -ForegroundColor Yellow
    exec {
        # $output = &msbuild /version /nologo | Out-String
        # Write-Host $output

        #/p:TargetFrameworkVersion='v4.5.2' `
        #/tv:"$tools_version" `
        #/p:VisualStudioVersion="$tools_version" `
        msbuild /t:Clean /t:Build `
            /p:Configuration=$configuration `
            /v:minimal `
            /p:nowarn="1591" `
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
    $packages | ForEach-Object {
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

task pack -depends clean, test {

    # ensure we have an artifacts dir
    create_directory $artifacts_dir

    # Find dependency versions
    $preludeVersion         = find-dependencyVersion 'Maxfire.Core' 'Maxfire.Prelude.Core'
    $mvcVersion             = find-dependencyVersion 'Maxfire.Web.Mvc' 'Microsoft.AspNet.Mvc'
    $sparkVersion           = find-dependencyVersion 'Maxfire.Spark.Web.Mvc' 'Spark.Web.Mvc4'
    $castleWindsorVersion   = find-dependencyVersion 'Maxfire.Castle.Web.Mvc' 'Castle.Windsor'
    $xunitVersion           = find-dependencyVersion 'Maxfire.TestCommons' 'xunit'
    $rhinoMocksVersion      = find-dependencyVersion 'Maxfire.Web.Mvc.TestCommons' 'RhinoMocks'

    Show-DependencyLine 'Maxfire.Prelude.Core' $preludeVersion
    Show-DependencyLine 'Microsoft.AspNet.Mvc' $mvcVersion
    Show-DependencyLine 'Spark.Web.Mvc4' $sparkVersion
    Show-DependencyLine 'Castle.Windsor' $castleWindsorVersion
    Show-DependencyLine 'xunit' $xunitVersion
    Show-DependencyLine 'RhinoMocks' $rhinoMocksVersion

    Get-ChildItem $nuspec_dir -Filter *.nuspec | ForEach-Object {

        $nuspecPath = ($_ | Resolve-Path).Path
        $convertedPath = Convert-Path $nuspecPath

        $projectName = [io.path]::GetFileNameWithoutExtension($_.FullName)

        # All files in nuspec are relative paths, grab dll|pdb|xml files using this base path
        $basePath = "$source_dir\$projectName\bin\$configuration" # TODO: Copyfiles to temp

        exec {
            & $tools_dir\Nuget.exe pack $convertedPath -NoPackageAnalysis -BasePath $basePath -OutputDirectory $artifacts_dir -Version $pkgVersion `
                    -Properties "configuration=$configuration;package_version=$pkgVersion;prelude_version=$preludeVersion;mvc_version=$mvcVersion;spark_version=$sparkVersion;castlewindsor_version=$castleWindsorVersion;xunit_version=$xunitVersion;rhinomocks_version=$rhinomocksVersion"
         }
    }
}

function Show-DependencyLine($packageId, $version) {
    Write-Host -NoNewline 'Version ' -ForegroundColor Yellow
    Write-Host -NoNewline $version -ForegroundColor DarkGreen
    Write-Host -NoNewline ' of ' -ForegroundColor Yellow
    Write-Host -NoNewline $packageId -ForegroundColor DarkGreen
    Write-Host ' is installed/restored.' -ForegroundColor Yellow
}

function find-dependencyVersion($csprojName, $packageId) {

    $csprojPath = join-path $source_dir -ChildPath $csprojName | Join-Path -ChildPath "$csprojName.csproj"

    $csproj = [xml](Get-Content $csprojPath)

    # csproj has default namespace
    $packageReferenceVersionNode = ($csproj | Select-Xml -XPath "//a:PackageReference[@Include='$packageId']/a:Version" `
                                                         -Namespace @{a='http://schemas.microsoft.com/developer/msbuild/2003'}).Node

    if (-not $packageReferenceVersionNode) {
        throw "Could not resolve the the $packageId dependency."
    }

    $version = $packageReferenceVersionNode.InnerText

    if (-not $version) {
        throw "Could not resolve the version of the $packageId dependency."
    }

    return $version
}

task publish -depends pack {
    if (deployToCIFeed) {
        Write-Host "Deploying to CI Feed..."
        Get-ChildItem (Join-Path $artifacts_dir "*.nupkg") | ForEach-Object {
            exec { & $tools_dir\Nuget.exe push $_ -NoSymbols -Source $cifeed_url -ApiKey $env:CI_DEPLOYMENT_API_KEY }
        }
    }
    if (deployToProdFeed) {
        Write-Host "Deploying to Prod Feed..."
        Get-ChildItem (Join-Path $artifacts_dir "*.nupkg") | ForEach-Object {
            exec { & $tools_dir\Nuget.exe push $_ -NoSymbols -Source $prodfeed_url -ApiKey $env:DEPLOYMENT_API_KEY }
        }
    }
}

task Upload-AppVeyor-Artifacts {
    if (isAppVeyor) {
        Get-ChildItem (Join-Path $artifacts_dir "*.nupkg") | ForEach-Object { Push-AppveyorArtifact $_ }
    }
}

task Create-Release-Notes -depends resolveVersion {
    if ($null -eq $env:github_password) {
        Write-Warning "You must provide your github password as an environment variable: `$env:github_password = ...secret..."
        Write-Warning "No draft release was created on GitHub."
    }
    else {
        # This is both the title and tagName of the release (title can be edited on github.com)
        $milestone = $majorMinorPatch
        Write-Host "Creating draft release of version '$milestone' on GitHub"
        try {
            $gitReleaseManagerExe = Join-Path $tools_dir -ChildPath "GitReleaseManager\tools\GitReleaseManager.exe";
            exec {
                & $gitReleaseManagerExe create -c master -u $repositoryOwner -p $env:github_password -o $repositoryOwner -r $repositoryName -m $milestone -d $base_dir
            }
            Write-Output "The draft release was created successfully on GitHub."
        }
        catch {
            Write-Error $_
            Write-Warning "No draft release was created on GitHub."
        }
    }
}

# Todo: source indexing should be part of pack
task Publish-GitHub-Release -depends pack {
    if (deployToProdFeed) {
        # This is both the title and tagName of the release (title can be edited on github.com)
        $milestone = $majorMinorPatch
        Write-Host "Closing the milestone '$milestone' on GitHub"
        try {
            $gitReleaseManagerExe = Join-Path $tools_dir -ChildPath "GitReleaseManager\tools\GitReleaseManager.exe";
            exec {
                # add packages to the published release
                Get-ChildItem $artifacts_dir -Filter *.nupkg | ForEach-Object  {
                    $nugetPath = ($_ | Resolve-Path).Path;
                    $convertedPath = Convert-Path $nugetPath;
                    & $gitReleaseManagerExe addasset -a $convertedPath -t $pkgVersion -u $repositoryOwner -p $env:github_password -o $repositoryOwner -r $repositoryName -m $milestone -d $base_dir
                }
                # Close the milestone
                & $gitReleaseManagerExe close -m $milestone -u $repositoryOwner -p $env:github_password -o $repositoryOwner -r $repositoryName -d $base_dir
            }
        }
        catch {
            Write-Error $_
            Write-Warning "Milestone was closed on GitHub."
        }
    }
    else {
        Write-Host "Skipping Publish-GitHub-Release, because ShouldDeployToProdFeed is false."
    }
}

# -------------------------------------------------------------------------------------------------------------
# functions
# --------------------------------------------------------------------------------------------------------------

function IsPullRequest() {
    return $null -ne $env:APPVEYOR_PULL_REQUEST_NUMBER
}

function isTagPush() {
    if ($null -ne $env:APPVEYOR_REPO_TAG) {
        return 'true' -eq $env:APPVEYOR_REPO_TAG
    }
    else {
        return $false
    }
}

function isFeatureBranch() {
    return $branchName -match "^features?/"
}

function isHotfixBranch() {
    return $branchName -match "^hotfix(es)?/"
}

function isReleaseCandidateBranch() {
    return $branchName -match "^releases?/(0|[1-9]\d*)[.](0|[1-9]\d*)([.](0|[1-9]\d*))?"
}

function isDevelopBranch() {
    return $branchName -match "^dev(elop)?$"
}

function isMasterBranch() {
    return $branchName -eq "master"
}

function isSupportBranch() {
    return $branchName -match "^support/(0|[1-9]\d*)[.](x|0|[1-9]\d*)"
}

function isPullRequestBranch() {
    return $branchName -match "^(pull|pull\-requests|pr)[/-]"
}

function isReleaseLineBranch() {
    return (isMasterBranch) -or (isSupportBranch)
}

function configurationIs($s) {
    return $s -eq $configuration # -eq is not case-senitive
}

function deployToAnyFeed() {
    # appveyor build, PR's are not deployed
    return (isAppVeyor) -and (-not (isPullRequest))
}

function deployToCIFeed() {
    # Only Debug builds are published to CI feed
    # Any branch except master and 'support/x.y' have been pushed to GitHub
    return (deployToAnyFeed) -and (configurationIs("Debug")) -and ((-not (isTagPush)) -or (-not (isReleaseLineBranch)))
}

function deployToProdFeed() {
    # Only Release builds are published to production feed
    # A tag (i.e. a published github release) on either master or 'support/x.y' have been created on GitHub
    return (deployToAnyFeed) -and (configurationIs("Release")) -and (isTagPush) -and (isReleaseLineBranch)
}

function Show-InfoLine {

    param (
        [string]$Text,
        [string]$Value,
        [int]$Width = 32, # The width of the longext text
        [int]$Left = 2    # The number of spaces added to the left of the text
    )

    # create line with 2 spaces before 'text', and padded right into total length of 32
    Write-Host -NoNewline ((" " * $Left) + "${Text}:").PadRight($Width + $Left + 2) -ForegroundColor Yellow
    Write-Host "$Value" -ForegroundColor DarkGreen
}

function Show-InfoHeader($header) {
    Write-Host "${header}:"
}

function Show-Info {
    $w = "ShouldDeployToProdFeed".Length
    Show-InfoLine "Target" $target $w
    Show-InfoLine "Configuration" $configuration $w
    Show-InfoLine "ConfigurationIsDebug" $(configurationIs("Debug")) $w
    Show-InfoLine "ConfigurationIsRelease" $(configurationIs("Release")) $w
    Show-InfoLine "IsLocalBuild" (-not $(isAppVeyor)) $w
    Show-InfoLine "IsRunningOnAppVeyor" $(isAppVeyor) $w
    Show-InfoLine "IsPullRequest" $(isPullRequest) $w
    Show-InfoLine "IsTagPush" $(isTagPush) $w
    Show-InfoLine "ShouldDeployToAnyFeed" $(deployToAnyFeed) $w
    Show-InfoLine "CIFeed" $cifeed_url $w
    Show-InfoLine "ShouldDeployToCIFeed" $(deployToCIFeed) $w
    Show-InfoLine "ProdFeed" $prodfeed_url $w
    Show-InfoLine "ShouldDeployToProdFeed" $(deployToProdFeed) $w

    $w = "IsReleaseCandidateBranch".Length
    Show-InfoHeader "GIT Repository Information"
    Show-InfoLine "CommitId" $commitId $w
    Show-InfoLine "CommitDate" $commitDate $w
    Show-InfoLine "Sha" $commitSha $w
    Show-InfoLine "Branch" $branchName $w
    Show-InfoLine "IsFeatureBranch" $(isFeatureBranch) $w
    Show-InfoLine "IsHotfixBranch" $(isHotfixBranch) $w
    Show-InfoLine "IsReleaseCandidateBranch" $(isReleaseCandidateBranch) $w
    Show-InfoLine "IsDevelopBranch" $(isDevelopBranch) $w
    Show-InfoLine "IsMasterBranch" $(isMasterBranch) $w
    Show-InfoLine "IsSupportBranch" $(isSupportBranch) $w
    Show-InfoLine "IsReleaseLineBranch" $(isReleaseLineBranch) $w
    Show-InfoLine "IsPullRequestBranch" $(isPullRequestBranch) $w
    Show-InfoLine "Tag" $commitTag $w

    $w = "AssemblyInformationalVersion".Length
    Show-InfoHeader "Version Information"
    Show-InfoLine "MajorMinorPatch" $majorMinorPatch $w
    Show-InfoLine "Version" $semVersion $w
    Show-InfoLine "NuGetVersion" $pkgVersion $w
    Show-InfoLine "BuildVersion" $buildVersion $w
    Show-InfoLine "AssemblyVersion" $assemblyVersion $w
    Show-InfoLine "AssemblyFileVersion" $assemblyFileVersion $w
    Show-InfoLine "AssemblyInformationalVersion" $assemblyInformationalVersion $w
}

function isAppVeyor() {
    Test-Path -Path env:\APPVEYOR
}

function testEnvironmentVariable($envVariableName, $envVariableValue) {
    if ($null -ne $envVariableValue) {
        Write-Output "${envVariableName}: $envVariableValue";
    } else {
        Write-Output "${envVariableName}: Not Defined";
    }
}

function echoAppVeyorEnvironmentVariables() {
    if (isAppVeyor) {
        testEnvironmentVariable "CI" $env:CI;
        testEnvironmentVariable "APPVEYOR_API_URL" $env:APPVEYOR_API_URL;
        testEnvironmentVariable "APPVEYOR_PROJECT_ID" $env:APPVEYOR_PROJECT_ID;
        testEnvironmentVariable "APPVEYOR_PROJECT_NAME" $env:APPVEYOR_PROJECT_NAME;
        testEnvironmentVariable "APPVEYOR_PROJECT_SLUG" $env:APPVEYOR_PROJECT_SLUG;
        testEnvironmentVariable "APPVEYOR_BUILD_FOLDER" $env:APPVEYOR_BUILD_FOLDER;
        testEnvironmentVariable "APPVEYOR_BUILD_ID" $env:APPVEYOR_BUILD_ID;
        testEnvironmentVariable "APPVEYOR_BUILD_NUMBER" $env:APPVEYOR_BUILD_NUMBER;
        testEnvironmentVariable "APPVEYOR_BUILD_VERSION" $env:APPVEYOR_BUILD_VERSION;
        testEnvironmentVariable "APPVEYOR_PULL_REQUEST_NUMBER" $env:APPVEYOR_PULL_REQUEST_NUMBER;
        testEnvironmentVariable "APPVEYOR_PULL_REQUEST_TITLE" $env:APPVEYOR_PULL_REQUEST_TITLE;
        testEnvironmentVariable "APPVEYOR_JOB_ID" $env:APPVEYOR_JOB_ID;
        testEnvironmentVariable "APPVEYOR_REPO_PROVIDER" $env:APPVEYOR_REPO_PROVIDER;
        testEnvironmentVariable "APPVEYOR_REPO_SCM" $env:APPVEYOR_REPO_SCM;
        testEnvironmentVariable "APPVEYOR_REPO_NAME" $env:APPVEYOR_REPO_NAME;
        testEnvironmentVariable "APPVEYOR_REPO_BRANCH" $env:APPVEYOR_REPO_BRANCH;
        testEnvironmentVariable "APPVEYOR_REPO_TAG" $env:APPVEYOR_REPO_TAG;
        testEnvironmentVariable "APPVEYOR_REPO_TAG_NAME" $env:APPVEYOR_REPO_TAG_NAME;
        testEnvironmentVariable "APPVEYOR_REPO_COMMIT" $env:APPVEYOR_REPO_COMMIT;
        testEnvironmentVariable "APPVEYOR_REPO_COMMIT_AUTHOR" $env:APPVEYOR_REPO_COMMIT_AUTHOR;
        testEnvironmentVariable "APPVEYOR_REPO_COMMIT_TIMESTAMP" $env:APPVEYOR_REPO_COMMIT_TIMESTAMP;
        testEnvironmentVariable "APPVEYOR_SCHEDULED_BUILD" $env:APPVEYOR_SCHEDULED_BUILD;
        testEnvironmentVariable "PLATFORM" $env:PLATFORM;
        testEnvironmentVariable "CONFIGURATION" $env:CONFIGURATION;
    }
}

function Get-File-Exists-On-Path([string]$file)
{
    $results = ($env:Path).Split(";") | Get-ChildItem -filter $file -erroraction silentlycontinue
    $found = ($null -ne $results)
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
    $frameworkPathDir = Get-ChildItem "$frameworkPath"
    if ($null -eq $frameworkPathDir) {
        throw "Building Brf.Lofus.Core requires .NET 4.0, which doesn't appear to be installed on this machine"
    }
    $net4Version = $frameworkPathDir.Name
    return "$env:windir\Microsoft.NET\Framework\$net4Version"
}

function global:delete_directory($directory_name)
{
  Remove-Item $directory_name -recurse -force  -ErrorAction SilentlyContinue | out-null
}

function global:delete_file($file)
{
    if ($file) {
        Remove-Item $file -force -ErrorAction SilentlyContinue | out-null
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
