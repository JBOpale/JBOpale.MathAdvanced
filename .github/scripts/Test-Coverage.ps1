<#
.SYNOPSIS
  Contrôle de couverture de code (règle « couverture de code » de CLAUDE.md).

.DESCRIPTION
  1. Fusionne tous les rapports Cobertura trouvés sous -ReportsDir : une ligne est couverte si hits > 0
     dans au moins un rapport (ex. net461 + net10.0). Les chemins sont ramenés au dépôt via
     <sources>/<source> + filename.
  2. Code modifié (si -BaseRef est fourni, cas d'une PR) : lignes ajoutées/modifiées des fichiers *.cs
     (git diff --unified=0 --diff-filter=AMR <BaseRef>...HEAD), hors UnitTest/, tests/ et obj/.
     Seules les lignes couvrables (présentes dans les rapports) comptent ; il en faut au moins -Threshold %.
     PR sans ligne couvrable modifiée : OK. Fichier modifié absent des rapports : ignoré si d'autres
     fichiers de son projet y figurent (fichier sans code exécutable), sinon ses lignes de code sont
     comptées non couvertes.
  3. Global : la couverture des lignes doit être >= lineCoverage de -Baseline (jamais en baisse).
  Compatible PowerShell 7 et Windows PowerShell 5.1.
#>
[CmdletBinding()]
param(
  [string] $ReportsDir = 'TestResults',
  [string] $BaseRef = '',
  [string] $Baseline = 'tests/coverage-baseline.json',
  [double] $Threshold = 90
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 2.0

function Normalize-Path([string] $p) {
  return ($p -replace '\\', '/').TrimEnd('/')
}

function Write-Summary([string] $text) {
  if ($env:GITHUB_STEP_SUMMARY) { Add-Content -Path $env:GITHUB_STEP_SUMMARY -Value $text -Encoding UTF8 }
}

$repoRoot = Normalize-Path ((& git rev-parse --show-toplevel) | Select-Object -First 1)
if ($LASTEXITCODE -ne 0 -or -not $repoRoot) { throw 'Impossible de trouver la racine du dépôt git.' }

function Get-RepoRelative([string] $fullPath) {
  $f = Normalize-Path $fullPath
  $prefix = $repoRoot + '/'
  if ($f.StartsWith($prefix, [System.StringComparison]::OrdinalIgnoreCase)) { return $f.Substring($prefix.Length) }
  return $f
}

# --- 1. Fusion des rapports Cobertura ---------------------------------------------------------------
$reports = @()
if (Test-Path $ReportsDir) { $reports = @(Get-ChildItem -Path $ReportsDir -Recurse -Filter 'coverage.cobertura.xml') }
if ($reports.Count -eq 0) {
  Write-Host "::error::Aucun rapport de couverture Cobertura trouvé sous '$ReportsDir'."
  exit 1
}

# $coverage[fichier relatif][numéro de ligne] = $true si couverte au moins une fois
$coverage = @{}
foreach ($report in $reports) {
  Write-Host "Rapport : $($report.FullName)"
  [xml] $xml = Get-Content -Path $report.FullName -Raw -Encoding UTF8
  $sources = @()
  foreach ($s in @($xml.SelectNodes('/coverage/sources/source'))) { if ($s.InnerText) { $sources += (Normalize-Path $s.InnerText) } }
  foreach ($class in @($xml.SelectNodes('//packages/package/classes/class'))) {
    $filename = Normalize-Path $class.GetAttribute('filename')
    $full = $filename
    if (-not [System.IO.Path]::IsPathRooted($filename) -and $sources.Count -gt 0) {
      $full = $sources[0] + '/' + $filename
      foreach ($src in $sources) {
        if (Test-Path -LiteralPath ($src + '/' + $filename)) { $full = $src + '/' + $filename; break }
      }
    }
    $rel = Get-RepoRelative $full
    if (-not $coverage.ContainsKey($rel)) { $coverage[$rel] = @{} }
    $lines = $coverage[$rel]
    foreach ($line in @($class.SelectNodes('lines/line'))) {
      $n = [int] $line.GetAttribute('number')
      $hit = ([long] $line.GetAttribute('hits')) -gt 0
      if ($lines.ContainsKey($n)) { $lines[$n] = ($lines[$n] -or $hit) } else { $lines[$n] = $hit }
    }
  }
}

$total = 0; $covered = 0
foreach ($file in $coverage.Keys) {
  foreach ($n in $coverage[$file].Keys) { $total++; if ($coverage[$file][$n]) { $covered++ } }
}
$global = 0.0
if ($total -gt 0) { $global = [math]::Round(100.0 * $covered / $total, 2) }
Write-Host ("Couverture globale : {0} % ({1}/{2} lignes)" -f $global, $covered, $total)

$failed = $false
Write-Summary '## Couverture de code'
Write-Summary ''
Write-Summary ("- Couverture globale : **{0} %** ({1}/{2} lignes)" -f $global, $covered, $total)

# --- 2. Couverture du code modifié -----------------------------------------------------------------
function Get-ProjectDir([string] $relFile) {
  $dir = Split-Path -Path $relFile -Parent
  while ($dir) {
    if (@(Get-ChildItem -Path (Join-Path $repoRoot $dir) -Filter '*.csproj' -File -ErrorAction SilentlyContinue).Count -gt 0) {
      return (Normalize-Path $dir)
    }
    $dir = Split-Path -Path $dir -Parent
  }
  return ''
}

function Test-CodeLine([string] $text) {
  # Pour un fichier absent des rapports : ignore lignes vides, accolades seules, commentaires, using, namespace.
  $t = $text.Trim()
  if ($t -eq '' -or $t -match '^[{}();]+$' -or $t.StartsWith('//') -or $t.StartsWith('/*') -or $t.StartsWith('*') -or
      $t -match '^using\s' -or $t -match '^namespace\s' -or $t.StartsWith('#') -or $t.StartsWith('[')) { return $false }
  return $true
}

if ($BaseRef) {
  $diff = & git diff --unified=0 --diff-filter=AMR "$BaseRef...HEAD" -- '*.cs'
  if ($LASTEXITCODE -ne 0) { throw "git diff contre '$BaseRef' a échoué (checkout avec fetch-depth: 0 ?)." }

  # $changed[fichier] = liste des numéros de lignes ajoutées/modifiées
  $changed = @{}
  $current = $null
  foreach ($l in @($diff)) {
    if ($l -match '^\+\+\+ (?:b/)?(.+)$') {
      $path = Normalize-Path $Matches[1]
      if ($path -eq '/dev/null' -or $path -match '(^|/)(UnitTest|tests|obj)/') { $current = $null }
      else { $current = $path; if (-not $changed.ContainsKey($current)) { $changed[$current] = New-Object System.Collections.ArrayList } }
    }
    elseif ($current -and $l -match '^@@ -\d+(?:,\d+)? \+(\d+)(?:,(\d+))? @@') {
      $start = [int] $Matches[1]
      $count = 1
      if ($Matches[2]) { $count = [int] $Matches[2] }
      for ($i = 0; $i -lt $count; $i++) { [void] $changed[$current].Add($start + $i) }
    }
  }

  $reportedProjects = @{}
  foreach ($f in $coverage.Keys) { $reportedProjects[(Get-ProjectDir $f)] = $true }

  $diffTotal = 0; $diffCovered = 0
  $uncovered = New-Object System.Collections.ArrayList
  foreach ($file in ($changed.Keys | Sort-Object)) {
    $lineNumbers = $changed[$file]
    if ($lineNumbers.Count -eq 0) { continue }
    if ($coverage.ContainsKey($file)) {
      $map = $coverage[$file]
      foreach ($n in $lineNumbers) {
        if ($map.ContainsKey($n)) {
          $diffTotal++
          if ($map[$n]) { $diffCovered++ } else { [void] $uncovered.Add("${file}:$n") }
        }
      }
    }
    elseif ($reportedProjects.ContainsKey((Get-ProjectDir $file))) {
      Write-Host "Fichier sans code mesuré (ignoré) : $file"
    }
    else {
      $content = @()
      if (Test-Path -LiteralPath (Join-Path $repoRoot $file)) { $content = @(Get-Content -LiteralPath (Join-Path $repoRoot $file) -Encoding UTF8) }
      foreach ($n in $lineNumbers) {
        if ($n -le $content.Count -and (Test-CodeLine $content[$n - 1])) {
          $diffTotal++
          [void] $uncovered.Add("${file}:$n (projet sans rapport de couverture)")
        }
      }
    }
  }

  if ($diffTotal -eq 0) {
    Write-Host 'Code modifié : aucune ligne couvrable modifiée, règle des 90 % satisfaite.'
    Write-Summary '- Code modifié : aucune ligne couvrable modifiée.'
  }
  else {
    $diffPct = [math]::Round(100.0 * $diffCovered / $diffTotal, 2)
    Write-Host ("Code modifié : {0} % ({1}/{2} lignes couvrables), seuil {3} %" -f $diffPct, $diffCovered, $diffTotal, $Threshold)
    Write-Summary ("- Code modifié : **{0} %** ({1}/{2} lignes couvrables), seuil {3} %" -f $diffPct, $diffCovered, $diffTotal, $Threshold)
    if ($uncovered.Count -gt 0) {
      Write-Host 'Lignes modifiées non couvertes :'
      Write-Summary ''
      Write-Summary '<details><summary>Lignes modifiées non couvertes</summary>'
      Write-Summary ''
      foreach ($u in $uncovered) { Write-Host "  $u"; Write-Summary "- ``$u``" }
      Write-Summary ''
      Write-Summary '</details>'
    }
    if ($diffPct -lt $Threshold) {
      Write-Host ("::error::Couverture du code modifié {0} % < {1} %. Ajouter les tests manquants." -f $diffPct, $Threshold)
      $failed = $true
    }
  }
}
else {
  Write-Host 'Pas de branche de base (workflow_dispatch) : seule la règle globale est vérifiée.'
}

# --- 3. Couverture globale jamais en baisse --------------------------------------------------------
if (-not (Test-Path $Baseline)) { throw "Fichier de référence introuvable : $Baseline" }
$base = [double] ((Get-Content -Path $Baseline -Raw -Encoding UTF8 | ConvertFrom-Json).lineCoverage)
Write-Summary ("- Référence globale (``{0}``) : {1} %" -f $Baseline, $base)
if ($global -lt $base) {
  Write-Host ("::error::Couverture globale {0} % < référence {1} % : la couverture ne doit jamais baisser." -f $global, $base)
  $failed = $true
}
elseif ($global -ge $base + 1) {
  Write-Host ("::notice::Couverture globale {0} % : au moins 1 point au-dessus de la référence {1} %. Relever lineCoverage dans {2} à {3}." -f $global, $base, $Baseline, [math]::Floor($global))
}

if ($failed) { exit 1 }
Write-Host 'Contrôle de couverture réussi.'
