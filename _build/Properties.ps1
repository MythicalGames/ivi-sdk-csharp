Write-Host "Start import properties"
properties {  

    $on_build_server = $false
    if ($env:ON_BUILD_SERVER -eq 'true'){
        $on_build_server = $true
    }
    
    $dotnetExe = "dotnet"
    $global:project_configuration = "release"

    $build_artifacts_dir = "$build_working_dir\BuildArtifacts"
    $test_results_dir = "$build_artifacts_dir\TestResults"
    $code_coverage_dir = "$build_artifacts_dir\CodeCoverageReport"
    $packages_root_dir = "$build_artifacts_dir\packages"

    $solution_dir = "$build_working_dir\src"
    $solution_file = "$solution_dir\IviSdk.sln"
     
    $now = Get-Date
    $logFileNameSegment = $now.ToString("yyyy_MM_dd_HH_mm_ss")

    $global:logSettings = @{
        logToFile   = $true;
        logFilePath = "$PSScriptRoot\buildlogs\log-$logFileNameSegment.txt";
    }


    if ($on_build_server -eq $false ) {   
        $global:logSettings.logFilePath = "$PSScriptRoot\buildlogs\log-$logFileNameSegment.txt";
    }

    $logFile_dir = [System.IO.Path]::GetDirectoryName($global:logSettings.logFilePath)
    if ((Test-Path $logFile_dir) -eq $False) {
        New-Item -Path $logFile_dir -ItemType "directory"
    }

}