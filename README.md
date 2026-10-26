# MathAdvanced

Ensemble de méthodes et de classes mathématiques (nombres complexes, matrices,
vecteurs, points, géométrie) pour .NET Framework 4.6.1 et .NET 10.

## Qualité de maillage

`MeshQuality` regroupe les métriques usuelles de simulation numérique (éléments finis / volumes finis) :
triangles, quadrilatères, tétraèdres, hexaèdres (ordre VTK) — rapport de forme, rapport des rayons,
forme moyenne, obliquité, effilement, gauchissement, Jacobien normalisé, conditionnement, angles dièdres —
ainsi que non-orthogonalité, obliquité de face, rapport d'expansion et statistiques (`Summarize`, `Histogram`).

## Skill Claude

Le paquet NuGet contient un skill Claude : `skills/mathadvanced/SKILL.md` (à la racine du
`.nupkg`). Il décrit l'API (classes, méthodes, pièges comme les indices de `Matrix` en base 1
ou les conflits de noms avec `System.Numerics`). Rien n'est copié automatiquement dans votre projet.

Pour que Claude (Claude Code) l'utilise, récupérez-le si besoin :

- sur nuget.org, page du paquet → **Download package**, puis ouvrez le `.nupkg` (c'est une archive zip) ;
- ou depuis le cache local : `~/.nuget/packages/mathadvanced/<version>/skills/mathadvanced/SKILL.md` ;
- ou directement dans ce dépôt : [`MathAdvanced/package/skills/mathadvanced/SKILL.md`](MathAdvanced/package/skills/mathadvanced/SKILL.md).

Copiez ensuite le fichier dans `<votre projet>/.claude/skills/mathadvanced/SKILL.md`.

## Licence

[MIT](LICENSE) — utilisation libre, y compris dans des applications commerciales
et propriétaires, à condition de conserver la mention de copyright.
