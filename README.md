# MathAdvanced

Ensemble de méthodes et de classes mathématiques (nombres complexes, matrices,
vecteurs, points, géométrie) pour .NET Framework 4.6.1 et .NET 10.

## Skill Claude

Le paquet NuGet embarque un skill Claude (`MathAdvanced/package/skills/mathadvanced/SKILL.md`).
À la compilation d'un projet qui référence `MathAdvanced`, il est copié dans
`<projet>/.claude/skills/mathadvanced/` pour que Claude Code connaisse l'API.
Pour le désactiver :

```xml
<PropertyGroup>
  <MathAdvancedInstallClaudeSkill>false</MathAdvancedInstallClaudeSkill>
</PropertyGroup>
```

## Licence

[MIT](LICENSE) — utilisation libre, y compris dans des applications commerciales
et propriétaires, à condition de conserver la mention de copyright.
