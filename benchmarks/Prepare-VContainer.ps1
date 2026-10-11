$ErrorActionPreference = 'Stop'

# 按官方 Standalone 项目的文件范围构建固定版本核心，不包含 Unity 或源码生成器。
$revision = '5401e5a7ebc4980a2b82141ffc26391a6547edd7'
$dependencyDirectory = Join-Path $PSScriptRoot '.dependencies/VContainer-1.19.0'
$sourceDirectory = Join-Path $dependencyDirectory 'Runtime'
$assemblyPath = Join-Path $dependencyDirectory 'bin/Release/net10.0/VContainer.Standalone.dll'
if (Test-Path -LiteralPath $assemblyPath) {
    Write-Output $assemblyPath
    return
}

New-Item -ItemType Directory -Path $sourceDirectory -Force | Out-Null
$tree = Invoke-RestMethod "https://api.github.com/repos/hadashiA/VContainer/git/trees/${revision}?recursive=1"
$prefix = 'VContainer/Assets/VContainer/Runtime/'
$sources = $tree.tree | Where-Object { $_.path.StartsWith($prefix) -and $_.path.EndsWith('.cs') -and $_.path -notlike '*/Unity/*' }
foreach ($source in $sources) {
    $relativePath = $source.path.Substring($prefix.Length)
    $destination = Join-Path $sourceDirectory $relativePath
    New-Item -ItemType Directory -Path (Split-Path $destination) -Force | Out-Null
    Invoke-WebRequest "https://raw.githubusercontent.com/hadashiA/VContainer/$revision/$($source.path)" -OutFile $destination
}
Invoke-WebRequest "https://raw.githubusercontent.com/hadashiA/VContainer/$revision/LICENSE" -OutFile (Join-Path $dependencyDirectory 'LICENSE')

$project = @'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <Nullable>disable</Nullable>
    <IsPackable>false</IsPackable>
    <GenerateDocumentationFile>false</GenerateDocumentationFile>
  </PropertyGroup>
</Project>
'@
$projectPath = Join-Path $dependencyDirectory 'VContainer.Standalone.csproj'
[IO.File]::WriteAllText($projectPath, $project.Replace("`r`n", "`n") + "`n", [Text.UTF8Encoding]::new($false))
dotnet build $projectPath -c Release -p:NuGetAudit=false
if ($LASTEXITCODE -ne 0) { throw 'VContainer standalone build failed.' }
Write-Output $assemblyPath
