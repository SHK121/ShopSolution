
$ErrorActionPreference = "Stop"

$baselinePath = Join-Path $PSScriptRoot "baseline.json"
$repoPath = Join-Path (Split-Path $PSScriptRoot -Parent) "Shop.Client"

# Never overwrite an existing baseline.
if (Test-Path $baselinePath) {
    throw "Baseline already exists: $baselinePath"
}

if (-not (Test-Path (Join-Path $repoPath ".git"))) {
    throw "Git repository not found: $repoPath"
}

Write-Host "Capturing Git status..."

$gitStatus = @(git -C $repoPath status --porcelain=v1 --untracked-files=all)

if ($LASTEXITCODE -ne 0) {
    throw "Failed to retrieve Git status."
}

Write-Host "Collecting file hashes..."

$files = Get-ChildItem $repoPath -Recurse -File -Force |
    Where-Object {
        $_.FullName -notmatch '[\\/]\.git[\\/]' -and
        $_.FullName -notmatch '[\\/]node_modules[\\/]' -and
        $_.FullName -notmatch '[\\/]\.angular[\\/]' -and
        $_.FullName -notmatch '[\\/]dist[\\/]'
    }

$hashes = @(
    $files | ForEach-Object {
        [PSCustomObject]@{
            Path = $_.FullName.Substring($repoPath.Length + 1)
            SHA256 = (Get-FileHash $_.FullName -Algorithm SHA256).Hash
        }
    }
)

$baseline = [PSCustomObject]@{
    CapturedAt = (Get-Date).ToString("o")
    GitStatus = $gitStatus
    Files = $hashes
}

$baseline |
    ConvertTo-Json -Depth 5 |
    Set-Content -Path $baselinePath -Encoding UTF8

Write-Host "Baseline captured: $baselinePath" -ForegroundColor Green
Write-Host "Files recorded: $($hashes.Count)"
Write-Host "Git status entries: $($gitStatus.Count)"
