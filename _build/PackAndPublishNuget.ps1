param(
    $configuration = "Debug",
    $output_path = "",
    $csproj_path = "",
    $assembly_name = "",
    $local_nuget
)

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

Write-Host "============================================START PackAndPublishNuget========================================================="

if (!$local_nuget){
    $local_nuget = [System.Environment]::GetEnvironmentVariable('MYTHICAL_LOCAL_NUGET')
}

Write-Host "Deleting old package"
Remove-Item -Path $local_nuget -Include "*$assembly_name*" -ErrorAction Continue -Verbose

$DllPath = "$output_path\$assembly_name.dll"

Write-Host "====================================================================================================="
Write-Host "start PackPublishNuget output_path: $output_path"
Write-Host "csproj_path: $csproj_path"
Write-Host "local_nuget: $local_nuget"
Write-Host "DllPath: $DllPath"

$version = Get-AssemblyVersion $DllPath
$packages_version = [Version]$version

$semver = $packages_version.Major.ToString() + "." + $packages_version.Minor.ToString() + "." + $packages_version.Build.ToString() + "." + $packages_version.Revision.ToString()

dotnet pack $csproj_path --no-build --include-symbols --configuration $configuration /p:AssemblyVersion=$semver /p:FileVersion=$semver /p:InformationalVersion=$semver /p:Version=$semver --output $local_nuget

Write-Host "=============================================END PackAndPublishNuget========================================================"