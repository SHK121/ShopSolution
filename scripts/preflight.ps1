$ErrorActionPreference = "Stop"

$solution = ".\ShopSolution.slnx"
$instructions = ".\CLAUDE.md"
$validationScript = ".\scripts\validate.ps1"

Write-Host "`n=== ShopSolution Preflight Check ===" -ForegroundColor Cyan

if (-not (Test-Path $solution)) {
	Write-Error "Solution file not found. Run this script from the Solution root."
	exit 1
}

Write-Host "[OK] Solution file found." -ForegroundColor Green

if (-not (Test-Path $instructions)) {
	Write-Error "CLAUDE.md not found."
	exit 1
}

Write-Host "[OK] CLAUDE.md found." -ForegroundColor Green

if (-not (Test-Path $validationScript)) {
	Write-Error "Validation script not found."
	exit 1
}

Write-Host "[OK] Validation script found." -ForegroundColor Green

Write-Host "`n=== Git Repository Check ===" -ForegroundColor Cyan

$gitRepo = ".\Shop.Client"

if (-not (Test-Path (Join-Path $gitRepo ".git"))) {
	Write-Error "Git repository not found at $gitRepo"
	exit 1
}

$gitStatus = git -C $gitRepo status --short

if ($LASTEXITCODE -ne 0) {
	Write-Error "Failed to inspect Git status."
	exit 1
}

if ($gitStatus) {
	Write-Warning "Existing changes detected in Shop.Client."
	$gitStatus | ForEach-Object { Write-Host " $_" }
	Write-Host "Preserve these changes. Do not discard or overwrite them."
}	
else {
	Write-Host "[OK] Working tree is clean." -ForegroundColor Green
}