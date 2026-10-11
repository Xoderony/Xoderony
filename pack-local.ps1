[CmdletBinding()]
param(
    [string] $VersionSuffix = "dev.$((Get-Date).ToUniversalTime().ToString('yyyyMMddHHmmss'))"
)

$ErrorActionPreference = "Stop"

$outputPath = Join-Path $PSScriptRoot "artifacts\packages"
$projects = @(
    "tools\Xoderony.Localization.CodeGeneration\Xoderony.Localization.CodeGeneration.csproj"
    "src\Xoderony.Channels\Xoderony.Channels.csproj"
    "src\Xoderony.Collections\Xoderony.Collections.csproj"
    "src\Xoderony.DependencyInjection\Xoderony.DependencyInjection.csproj"
    "src\Xoderony.Localization\Xoderony.Localization.csproj"
    "src\Xoderony.Localization.Json\Xoderony.Localization.Json.csproj"
    "src\Xoderony.Logging\Xoderony.Logging.csproj"
    "src\Xoderony.Modding\Xoderony.Modding.csproj"
    "src\Xoderony.Numerics\Xoderony.Numerics.csproj"
    "src\Xoderony.ObjectPool\Xoderony.ObjectPool.csproj"
    "src\Xoderony.Serialization\Xoderony.Serialization.csproj"
    "src\Xoderony.Text\Xoderony.Text.csproj"
    "tools\Xoderony.Localization.SourceGeneration\Xoderony.Localization.SourceGeneration.csproj"
    "tools\Xoderony.Stride.UI.SourceGeneration\Xoderony.Stride.UI.SourceGeneration.csproj"
)

New-Item -ItemType Directory -Force -Path $outputPath | Out-Null

foreach ($project in $projects) {
    $projectPath = Join-Path $PSScriptRoot $project
    dotnet pack $projectPath -c Release -o $outputPath -p:VersionSuffix=$VersionSuffix
    if ($LASTEXITCODE -ne 0) {
        throw "Packing '$projectPath' failed with exit code $LASTEXITCODE."
    }
}

Write-Output "Packed Xoderony $VersionSuffix to '$outputPath'."
