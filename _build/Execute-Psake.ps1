param(
    [parameter(Mandatory = $true)] [string]$task_filename,     
    [parameter(Mandatory = $true)] [string]$task_name
)
Install-Module PSake -Force
Import-Module PSake

$build_working_dir = (get-item $PSScriptRoot ).Parent.FullName

write-host "build_working_dir: $build_working_dir"
write-host "PSScriptRoot: $PSScriptRoot"

$psakeArgs = @{
    build_working_dir = $build_working_dir
}

Invoke-psake -buildfile $PSScriptRoot/$task_filename.ps1 -task $task_name -parameters $psakeArgs
Write-Host "psake.build_success = $($psake.build_success)"

if ($psake.build_success) {
    $global:LastExitCode = 0
}
else {
    $global:LastExitCode = 1
}
