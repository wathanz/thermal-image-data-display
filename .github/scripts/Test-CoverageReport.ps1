param(
    [Parameter(Mandatory)][string]$Path,
    [Parameter(Mandatory)][string]$ExpectedPackage
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $Path) -or (Get-Item -LiteralPath $Path).Length -eq 0) {
    throw "Coverage report '$Path' is missing or empty"
}
try { [xml]$report = Get-Content -LiteralPath $Path -Raw }
catch { throw "Malformed coverage report '$Path': $_" }

$package = $report.coverage.packages.package | Where-Object { $_.name -eq $ExpectedPackage }
if ($null -eq $package) { throw "Coverage report has no package '$ExpectedPackage'" }

$lines = @($package.SelectNodes("classes/class/lines/line"))
if ($lines.Count -eq 0) { throw "Coverage report has no coverable lines for '$ExpectedPackage'" }

$covered = @($lines | Where-Object { [int]$_.hits -gt 0 }).Count
$percent = [math]::Round(100.0 * $covered / $lines.Count, 2)
$summary = "Core line coverage ($ExpectedPackage): $covered/$($lines.Count) = $percent%"
Write-Host $summary
if ($env:GITHUB_STEP_SUMMARY) { Add-Content -Path $env:GITHUB_STEP_SUMMARY -Value $summary }
