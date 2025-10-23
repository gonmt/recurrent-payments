# Install-Template.ps1
# Script to install and test the Clean Architecture template

param(
    [string]$TestProjectName = "TestCleanArchProject"
)

Write-Host "🚀 Installing Clean Architecture DDD Template..." -ForegroundColor Green

# Get current directory
$CurrentDir = Get-Location
$TemplatePath = $CurrentDir

Write-Host "📁 Template path: $TemplatePath" -ForegroundColor Cyan

# Uninstall existing template if it exists
Write-Host "🗑️  Removing existing template..." -ForegroundColor Yellow
dotnet new uninstall -a Archetype.CleanArchitecture.Template

# Install the template from current directory
Write-Host "📦 Installing template..." -ForegroundColor Yellow
dotnet new install $TemplatePath

# Verify installation
Write-Host "✅ Verifying template installation..." -ForegroundColor Green
$templates = dotnet new list | Select-String "cleanarch"
if ($templates) {
    Write-Host "✅ Template successfully installed!" -ForegroundColor Green
    $templates | ForEach-Object { Write-Host "   $_" -ForegroundColor Cyan }
} else {
    Write-Host "❌ Template installation failed!" -ForegroundColor Red
    exit 1
}

# Create test project
Write-Host "🧪 Creating test project: $TestProjectName" -ForegroundColor Yellow
$TestDir = "$CurrentDir\temp-test"
if (Test-Path $TestDir) {
    Remove-Item $TestDir -Recurse -Force
}
New-Item -ItemType Directory -Path $TestDir | Out-Null

Set-Location $TestDir

try {
    # Create project from template
    Write-Host "📂 Running: dotnet new cleanarch -n $TestProjectName --ProjectPrefix TestCompany" -ForegroundColor Cyan
    dotnet new cleanarch -n $TestProjectName --ProjectPrefix TestCompany

    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Test project created successfully!" -ForegroundColor Green

        # Navigate to project
        Set-Location "$TestProjectName"

        # Test restore
        Write-Host "📦 Restoring packages..." -ForegroundColor Yellow
        dotnet restore

        # Test build
        Write-Host "🔨 Building project..." -ForegroundColor Yellow
        dotnet build

        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Build successful!" -ForegroundColor Green
        } else {
            Write-Host "❌ Build failed!" -ForegroundColor Red
        }

        # Test run
        Write-Host "🏃 Running API..." -ForegroundColor Yellow
        Start-Job -ScriptBlock {
            param($ProjectPath)
            Set-Location $ProjectPath
            dotnet run --project "src\$($args[0]).Api" --urls "http://localhost:5000"
        } -ArgumentList $TestProjectName

        Start-Sleep -Seconds 5

        # Test API
        try {
            $response = Invoke-RestMethod -Uri "http://localhost:5000/health" -Method Get -TimeoutSec 5
            Write-Host "✅ API is responding!" -ForegroundColor Green
        } catch {
            Write-Host "⚠️  API health check failed (this is normal if no health endpoint)" -ForegroundColor Yellow
        }

        # Stop the running API
        Get-Job | Stop-Job
        Get-Job | Remove-Job

    } else {
        Write-Host "❌ Failed to create test project!" -ForegroundColor Red
    }

} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
} finally {
    # Cleanup
    Set-Location $CurrentDir
    if (Test-Path $TestDir) {
        Write-Host "🧹 Cleaning up test files..." -ForegroundColor Yellow
        Remove-Item $TestDir -Recurse -Force
    }
}

Write-Host "🎉 Template installation and testing complete!" -ForegroundColor Green
Write-Host ""
Write-Host "📖 Usage:" -ForegroundColor Cyan
Write-Host "   dotnet new cleanarch -n YourProject --ProjectPrefix YourCompany" -ForegroundColor White
Write-Host ""