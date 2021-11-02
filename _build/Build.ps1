
$properties = "$PSScriptRoot\Properties.ps1"
.$properties

FormatTaskName "$([Environment]::NewLine)==================== $(Get-Date -format T) - Executing {0} ====================" 

task build -depends PrintInformation, CompileSolutions
task test -depends PrintInformation, CompileSolutions, InstallTestDependencies, RunXUnit
task codecoverage -depends PrintInformation, CreateArtifactsFolder, CompileSolutions, InstallTestDependencies, RunXUnit, OpenReport
task publish -depends PrintInformation, CreateArtifactsFolder, CompileSolutions, InstallTestDependencies, PackNugetPackages, PublishNugetPackages

task PrintInformation { 
    $timestamp = "[{0:MM/dd/yy} {0:HH:mm:ss}]" -f (Get-Date)
    Remove-Item $global:logSettings.logFilePath -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host '##########################################'
    Write-Host 'Build started'
    Write-Host $timestamp
    #Write-Host $([Environment]::NewLine)
    #Write-Host "On Build Agent: $onBuildAgent"
    Write-Host "GlobalConfig: $global:project_configuration"
    #Write-Host $([Environment]::NewLine)
    Write-Host "build_working_dir: $build_working_dir"
    Write-Host "test_results_dir: $test_results_dir"    
    Write-Host "code_coverage_dir: $code_coverage_dir"    
    Write-Host "ON_BUILD_SERVER: $on_build_server"
    Write-Host "env:CI: $env:CI"
    Write-Host "env:GITHUB_WORKFLOW: $env:GITHUB_WORKFLOW"
    Write-Host "env:GITHUB_RUN_ID: $env:GITHUB_RUN_ID"
    Write-Host "env:GITHUB_RUN_NUMBER: $env:GITHUB_RUN_NUMBER"
    Write-Host '##########################################'
}

task CreateArtifactsFolder {

    If (Test-Path $build_artifacts_dir) {
        Remove-Item $build_artifacts_dir -Recurse -Force
    }
    New-Item -Path $build_artifacts_dir -ItemType "directory"  
    New-Item -Path $packages_root_dir -ItemType "directory"  
}

task CompileSolutions {

    CompileSolution -solution_file $solution_file
}

task InstallTestDependencies{

    dotnet tool restore
}

task RunXUnit {

    New-Item -Path $code_coverage_dir -ItemType "directory" -ErrorAction SilentlyContinue

    Push-Location -Path $solution_dir     
    
    $args = @(            
            "--logger"
            ,"trx;LogFileName=TestResults.trx"
            ,"--logger"
            ,"xunit;LogFileName=TestResults.xml"
            ,"--results-directory"
            ,"$test_results_dir"
            ,"--dcReportType=HTML"
            ,"--dcOutput=$code_coverage_dir\CoverageReport.html"
            ,"--dcFilters=+:module=*Mythical*;class=*;function=*;-:module=*Tests*;"
            )

    dotnet dotcover test $args       
    
    Pop-Location
}

task OpenReport {

    Start-Process $code_coverage_dir\CoverageReport.html

}

task PackNugetPackages {
    
    $version = Get-AssemblyVersion $sdk_dll_path
    $packages_version = [Version]$version
    $global:semver = $packages_version.Major.ToString() + "." + $packages_version.Minor.ToString() + "." + $packages_version.Build.ToString() + "." + $packages_version.Revision.ToString()

    Write-Host 
    Write-Host "semver: $global:semver"
    Write-Host "sdk_csproj_path: $sdk_csproj_path"
    Write-Host 

    dotnet pack $sdk_csproj_path --no-build --include-symbols --configuration $global:project_configuration /p:AssemblyVersion=$global:semver /p:FileVersion=$global:semver /p:InformationalVersion=$global:semver /p:Version=$global:semver --output $sdk_nuget_package_output_dir
    
}

task PublishNugetPackages {
    
    $sdk_nuget_package_path = "$($sdk_nuget_package_output_dir)\$($sdk_assembly_name).$($global:semver).nupkg"

    Write-Host 
    Write-Host "sdk_nuget_package_output_path: $sdk_nuget_package_output_dir"
    Write-Host "sdk_nuget_package_path: $sdk_nuget_package_path"
    Write-Host 

    if ($on_build_server) {
        dotnet nuget push $sdk_nuget_package_path -s "github"
    }
}

function CompileSolution{
    [CmdletBinding()]
    param (        
        $solution_file
    )

    $config = $global:project_configuration

    & $dotnetExe restore $solution_file --verbosity m 
    & $dotnetExe build $solution_file --no-restore --configuration $config --verbosity m 

    if ($lastexitcode -ne 0) {
        
        Write-Host -message "Compile failed for: ${$solution_file}"
        throw "Compile failed"
    }
}

function Get-AssemblyVersion {
    param(
        $DllPath = ''
    )

    $bytes = [System.IO.File]::ReadAllBytes($DllPath)
    $assembly = [System.Reflection.Assembly]::Load($bytes)

    # Get name, version and display the results
    $assembly = $assembly.GetName()
    $version = $assembly.version

    Write-Host -Object "assembly.version: $version"
    return $version
}

