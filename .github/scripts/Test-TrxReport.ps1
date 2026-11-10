param([Parameter(Mandatory)][string]$Path)

$ErrorActionPreference = "Stop"

$files = @(Get-ChildItem -Path $Path -Filter *.trx -Recurse -ErrorAction SilentlyContinue)
if ($files.Count -eq 0) { throw "No TRX report found under '$Path'" }

foreach ($file in $files) {
    try { [xml]$trx = Get-Content -LiteralPath $file.FullName -Raw }
    catch { throw "Malformed TRX report '$($file.FullName)': $_" }

    $ns = New-Object System.Xml.XmlNamespaceManager($trx.NameTable)
    $ns.AddNamespace("t", "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")
    $counters = $trx.SelectSingleNode("//t:ResultSummary/t:Counters", $ns)
    if ($null -eq $counters) { throw "TRX '$($file.Name)' has no result counters" }

    $executed = [int]$counters.executed
    $failed = [int]$counters.failed
    $errors = [int]$counters.error
    $passed = [int]$counters.passed
    Write-Host "$($file.Name): executed=$executed passed=$passed failed=$failed error=$errors"

    if ($executed -le 0) { throw "TRX '$($file.Name)' executed no tests" }
    if ($passed -le 0) { throw "TRX '$($file.Name)' has no passing tests" }
    if ($failed -gt 0 -or $errors -gt 0) { throw "TRX '$($file.Name)' reports failures or errors" }
    if ($trx.SelectSingleNode("//t:ResultSummary[@outcome='Failed']", $ns)) { throw "TRX '$($file.Name)' outcome is Failed" }
}
