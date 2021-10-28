
$properties = "$PSScriptRoot\Properties.ps1"
.$properties

FormatTaskName "$([Environment]::NewLine)==================== $(Get-Date -format T) - Executing {0} ====================" 

task build -depends PrintInformation, CompileSolutions
task test -depends PrintInformation, CompileSolutions, InstallTestDependencies, RunXUnit

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
    Write-Host '##########################################'
}

task CreateArtifactsFolder {

    If (Test-Path $build_artifacts_dir) {
        Remove-Item $build_artifacts_dir -Recurse -Force
    }
    New-Item -Path $build_artifacts_dir -ItemType "directory"  
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

function CompileSolution{
    [CmdletBinding()]
    param (        
        $solution_file
    )

    $config = $global:project_configuration

    & $dotnetExe restore $solution_file --verbosity n 
    & $dotnetExe build $solution_file --no-restore --configuration $config --verbosity n 

    if ($lastexitcode -ne 0) {
        
        Write-Host -message "Compile failed for: ${$solution_file}"
        throw "Compile failed"
    }
}

