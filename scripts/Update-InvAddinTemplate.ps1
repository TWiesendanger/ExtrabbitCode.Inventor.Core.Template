param(
    [string]$TemplateName = "ExtrabbitCode.Inventor.Core.Template",
    [string]$TemplateProjectPath = "$PSScriptRoot\..\TemplatePack\TemplatePack.csproj",
    [string]$OutDir = "$PSScriptRoot\..\.nupkgs"
)

Write-Host "⏳ Uninstalling existing template '$TemplateName'..." -ForegroundColor Yellow
dotnet new uninstall $TemplateName | Out-Null

Write-Host "🔨 Packing template project..." -ForegroundColor Cyan
Write-Host "Project Path: $TemplateProjectPath" -ForegroundColor Gray
dotnet pack $TemplateProjectPath -c Release

# Find the latest nupkg
$pkg = Get-ChildItem -Path $OutDir -Filter '*.nupkg' | Sort-Object LastWriteTime -Descending | Select-Object -First 1

if (-not $pkg) {
    Write-Host "❌ No .nupkg found. Did dotnet pack succeed?" -ForegroundColor Red
    exit 1
}

Write-Host "📦 Installing $($pkg.Name) as template..." -ForegroundColor Cyan
dotnet new install $pkg.FullName

Write-Host "✅ Template reinstalled successfully!" -ForegroundColor Green
dotnet new --list | Select-String $TemplateName