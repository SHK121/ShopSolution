$ErrorActionPreference = "Stop"

$baselinePath = Join-Path $PSScriptRoot "baseline.json"
$repoPath = Join-Path (Split-Path $PSScriptRoot -Parent) "Shop.Client"

if (-not (Test-Path $baselinePath)) {
	throw "Baseline not found: $baselinePath"
}

if (-not (Test-Path (Join-Path $repoPath ".git"))) {
	throw "Git repository not found: $repoPath"
}

$baseline = Get-Content $baselinePath -Raw | ConvertFrom-Json

if (-not $baseline.Files) {
	throw "Baseline contains no files hashes."
}

Write-Host "Baseline loaded successfully." -ForegroundColor Green
Write-Host "Captured at: $($baseline.CapturedAt)"
Write-Host "Files in baseline : $($baseline.Files.Count)"
Write-Host "Git status entries in baseline $($baseline.GitStatus.Count)"

Write-Host "`nCollecting current file hashes..."

$currentFiles = Get-ChildItem $repoPath -Recurse -File -Force |
	Where-Object {
		$_.FullName -notmatch '[\\/]\.git[\\/]' -and
		$_.FullName -notmatch '[\\/]node_modules[\\/]' -and
		$_.FullName -notmatch '[\\/]\.angular[\\/]' -and
		$_.FullName -notmatch '[\\/]dist[\\/]'
	}

$currentHashes = @(
	$currentFiles | ForEach-Object {
		[PSCustomObject]@{
			Path = $_.FullName.Substring($repoPath.Length + 1)
			SHA256 = (Get-FileHash $_.FullName -Algorithm SHA256).Hash
		}
	}
)

Write-Host "Current files: $($currentHashes.Count)"
$currentHashes | Select-Object -First 3