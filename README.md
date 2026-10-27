# JBOpale.MathAdvanced

[![NuGet](https://img.shields.io/nuget/v/JBOpale.MathAdvanced)](https://www.nuget.org/packages/JBOpale.MathAdvanced)

```
dotnet add package JBOpale.MathAdvanced
```

> **Migration :** ce paquet remplace `MathAdvanced` (1.x). L'espace de noms reste `JBOPaleAPI.MathAdvanced` ;
> seul l'identifiant du paquet et le nom de l'assembly (`JBOpale.MathAdvanced.dll`) changent.

Ensemble de méthodes et de classes mathématiques (nombres complexes, matrices,
vecteurs, points, géométrie) pour .NET Framework 4.6.1 et .NET 10.

## Qualité de maillage

`MeshQuality` regroupe les métriques usuelles de simulation numérique (éléments finis / volumes finis) :
triangles, quadrilatères, tétraèdres, hexaèdres (ordre VTK) — rapport de forme, rapport des rayons,
forme moyenne, obliquité, effilement, gauchissement, Jacobien normalisé, conditionnement, angles dièdres —
ainsi que non-orthogonalité, obliquité de face, rapport d'expansion et statistiques (`Summarize`, `Histogram`).

## Skill Claude

Le paquet NuGet contient un skill Claude : `skills/jbopale-mathadvanced/SKILL.md` (à la racine du
`.nupkg`). Il décrit l'API (classes, méthodes, pièges comme les indices de `Matrix` en base 1
ou les conflits de noms avec `System.Numerics`). Rien n'est copié automatiquement dans votre projet.

Pour que Claude (Claude Code) l'utilise, récupérez-le si besoin :

- sur nuget.org, page du paquet → **Download package**, puis ouvrez le `.nupkg` (c'est une archive zip) ;
- ou depuis le cache local : `~/.nuget/packages/jbopale.mathadvanced/<version>/skills/jbopale-mathadvanced/SKILL.md` ;
- ou directement dans ce dépôt : [`JBOpale.MathAdvanced/package/skills/jbopale-mathadvanced/SKILL.md`](JBOpale.MathAdvanced/package/skills/jbopale-mathadvanced/SKILL.md).

Copiez ensuite le fichier dans `<votre projet>/.claude/skills/jbopale-mathadvanced/SKILL.md`.

## Licence

[MIT](LICENSE) — utilisation libre, y compris dans des applications commerciales
et propriétaires, à condition de conserver la mention de copyright.

## Publication

Un tag `v*` lance `publish.yml` : tests (net461 et net10.0), `dotnet pack`, contrôle du contenu du paquet,
puis envoi sur nuget.org par **Trusted Publishing** (jeton OIDC éphémère, aucune clé d'API stockée).
La branche par défaut est protégée par le ruleset `.github/rulesets/protection-main.json`
(pull request obligatoire, check `test` vert, pas de suppression ni de force-push).
