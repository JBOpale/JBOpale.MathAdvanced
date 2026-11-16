# CLAUDE.md

Ce fichier contient des règles générales pour ce dépôt, à respecter par toute session Claude (Claude Code, y compris les workflows GitHub Actions générés ou modifiés).

## Règle : exécution des GitHub Actions selon l'OS

Par défaut, pour tout workflow GitHub Actions ajouté ou modifié dans ce dépôt :

- **Jobs Windows** : doivent s'exécuter sur le runner self-hosted local `SURFACE-BOOK-3`.
  ```yaml
  runs-on: [self-hosted, Windows, SURFACE-BOOK-3]
  ```

- **Jobs macOS** : doivent être déclenchés **uniquement manuellement** (`workflow_dispatch`), jamais automatiquement sur `push`, `pull_request`, ou un autre événement, afin de limiter la consommation de minutes GitHub Actions (macOS coûte ~10x plus cher en minutes facturées que Linux).
  ```yaml
  on:
    workflow_dispatch:
  jobs:
    build-macos:
      runs-on: macos-latest
  ```
  Si un même workflow contient aussi des jobs Linux/Windows déclenchés automatiquement, séparer le job macOS avec une condition `if: github.event_name == 'workflow_dispatch'` pour qu'il ne se déclenche jamais via les autres triggers du workflow.

Cette règle s'applique à tout nouveau workflow créé dans `.github/workflows/`, ainsi qu'à toute modification d'un workflow existant.

## Règle : tests validés avant fusion d'une pull request

- Aucune pull request n'est fusionnée dans `dev` ou `main` tant que le check **« JBOpale.MathAdvanced - Tests PR »** (workflow `.github/workflows/pr-tests.yml`) n'est pas vert.
- `pr-tests.yml` ne doit **jamais** recevoir de filtre `paths` (ni de condition qui ferait sauter le job) : un check requis absent bloque la PR, et un job sauté compte comme réussi.
- Toute session lance localement les tests concernés et les voit passer **avant** d'ouvrir ou de mettre à jour une PR.
- Ne jamais désactiver, ignorer (`[Ignore]`, filtre, etc.) ou supprimer un test pour faire passer une PR.
- Plus de push direct sur les branches protégées (`dev`, `main`) : toute modification passe par une PR.
- Tout nouveau projet de tests doit être ajouté à `pr-tests.yml` (une étape `dotnet test` par projet).
- Le ruleset correspondant est dans `.github/rulesets/tests-obligatoires.json` (import via Settings → Rules → Rulesets → Import a ruleset).

## Règle : aucun paquet vulnérable avant de passer sur main

- Aucune modification n'arrive sur `main` ou `dev` avec un paquet NuGet présentant une vulnérabilité **haute ou critique**, directe ou transitive.
- Le contrôle est l'étape « Paquets vulnérables (haute / critique) » du job requis de `pr-tests.yml` (`dotnet list JBOpale.MathAdvanced.sln package --vulnerable --include-transitive --format json`) : il fait partie du check « JBOpale.MathAdvanced - Tests PR ». Les vulnérabilités faibles/modérées sont signalées en avertissement.
- Toute session lance ce contrôle localement avant d'ouvrir ou de mettre à jour une PR.
- Correction : mise à jour de la dépendance, ou épinglage d'une version corrigée pour une dépendance transitive (avec un commentaire citant l'avis GHSA).
- Ne jamais retirer, ignorer ou contourner ce contrôle, ni baisser son seuil.
- Tout nouveau projet doit être couvert par le contrôle : l'ajouter à `JBOpale.MathAdvanced.sln` (projet SDK / PackageReference). Un projet `packages.config` devra être couvert autrement (NuGetAudit au restore, `NuGetAuditLevel=high`, NU1903/NU1904 en erreur).

## Règle : suivi des actions en cours

- Jérôme suit les actions en attente de tous ses projets dans le tableau « Suivi des actions » : https://claude.ai/artifact/41ayfFGBTCRjpUJH4pgf1P
- Toute session utilise le skill `suivi-actions` pour le tenir à jour à la fin de chaque chantier, et chaque fois qu'elle rend la main avec une action qui dépend de lui (décision, validation, étape manuelle, secret à créer, PR à fusionner, ruleset à importer, réponse externe).
- Lire l'existant avant d'ajouter (pas de doublon) ; passer à `doing` / `done` ce qui avance.
- Jamais de secret dans le tableau ; ne jamais supprimer une action.

## Règle : couverture de code

- Chaque pull request doit couvrir par des tests **au moins 90 % des lignes couvrables qu'elle ajoute ou modifie** (fichiers `*.cs` hors `UnitTest/`, `tests/`, `obj/`).
- La couverture globale des lignes ne doit **jamais baisser** : elle doit rester au moins égale à `lineCoverage` dans `tests/coverage-baseline.json`. Ce seuil n'est relevé qu'à la hausse (quand le contrôle signale +1 point, le relever à la valeur mesurée arrondie à l'inférieur), jamais abaissé.
- Le contrôle est l'étape « Couverture (90 % du code modifié, global au moins au seuil) » du job requis « JBOpale.MathAdvanced - Tests PR » (script `.github/scripts/Test-Coverage.ps1`, collecte coverlet configurée par `coverage.runsettings`).
- Jamais d'exclusion de couverture (`[ExcludeFromCodeCoverage]`, filtre dans `coverage.runsettings`…), jamais de seuil abaissé, jamais de test désactivé ou sauté pour faire passer le contrôle.
- Avant d'ouvrir ou de mettre à jour une PR, toute session mesure la couverture et lance le script :
  ```
  dotnet test UnitTest/UnitTest.csproj -c Release --collect:"XPlat Code Coverage" --settings coverage.runsettings --results-directory TestResults
  pwsh ./.github/scripts/Test-Coverage.ps1 -ReportsDir TestResults -BaseRef origin/dev
  ```
  Si le code modifié est sous 90 %, écrire les tests manquants (tests significatifs, pas de simple appel sans assertion).

## Règle : tout développement part de `dev`, jamais de `main`

- Le dépôt JBOpale.MathAdvanced a deux branches permanentes : `dev` (intégration) et `main` (production). `dev` est la branche par défaut du dépôt.
- Toute branche de travail (`feature/…`, `fix/…`, `claude/…`) est créée depuis `dev` à jour (`git fetch origin dev && git checkout -b <branche> origin/dev`), jamais depuis `main`, et sa PR cible `dev`.
- `main` ne reçoit que des PR `dev` → `main`, sur validation explicite de Jérôme. Aucun commit ni aucune branche de travail ne part de `main` (seule exception : un correctif urgent de production, sur demande explicite, ensuite reporté dans `dev`).
- Si une session démarre sur `main` ou sur une branche qui n'est pas basée sur `dev`, elle se rebase/recrée sur `origin/dev` avant le premier commit.
