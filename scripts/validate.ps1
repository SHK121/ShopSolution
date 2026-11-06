
$ErrorActionPreference = "Stop"

$solution = ".\ShopSolution.slnx"

Write-Host "`n=== ShopSolution Validation ===" -ForegroundColor Cyan

if (-not (Test-Path $solution)) {
    Write-Error "Solution file not found: $solution"
    exit 1
}

Write-Host "`n=== Running Tests ===" -ForegroundColor Cyan
dotnet test $solution

if ($LASTEXITCODE -ne 0) {
    Write-Error "Tests failed."
    exit $LASTEXITCODE
}

Write-Host "`n=== Building Solution ===" -ForegroundColor Cyan
dotnet build $solution --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed."
    exit $LASTEXITCODE
}

Write-Host "`n=== Validation Succeeded ===" -ForegroundColor Green
