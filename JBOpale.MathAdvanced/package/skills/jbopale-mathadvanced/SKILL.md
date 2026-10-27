---
name: jbopale-mathadvanced
description: Utiliser lorsque le code C# référence le paquet NuGet JBOpale.MathAdvanced (namespace JBOpale.MathAdvanced) pour des nombres complexes, matrices, vecteurs/points 3D, géométrie, arithmétique entière, statistiques ou calcul numérique. Use when C# code uses the JBOpale.MathAdvanced NuGet package.
---

# JBOpale.MathAdvanced (namespace JBOpale.MathAdvanced)

Bibliothèque mathématique MIT, cibles `net461` et `net10.0`. Namespace : `JBOpale.MathAdvanced`.
Référence : https://github.com/JBOpale/JBOpale.MathAdvanced

## Règles à respecter

- **Conflits de noms** : `Complex`, `Matrix`, `Point`, `Math` existent aussi dans `System.Numerics`, `System.Windows`, `System.Drawing`… Qualifier ou utiliser un alias :
  `using MComplex = JBOpale.MathAdvanced.Complex;`. Dans ce namespace, appeler `System.Math.X` explicitement.
- **Matrix** : indices en **base 1**, éléments de type `Complex` (pas `double`).
- Ne pas réinventer ce que la bibliothèque fournit déjà : chercher d'abord dans les classes ci-dessous.
- Les API signalent les erreurs par exception (`ArgumentException`, `ArgumentOutOfRangeException`, `OverflowException`) : ne pas les avaler.

## Classes

| Classe | Contenu |
|---|---|
| `Complex` | `Re`, `Im`, `Complex.I/Zero/One`, opérateurs `+ - * /` avec `double`, `Abs`, `Arg`, `Conj`, `Inv`, `Sqrt`, `Exp`, `Log`, `Pow`, `Sin/Cos/Tan`, `Sinh/Cosh/Tanh`, construction depuis `string` |
| `Matrix` | `Zeros/Ones/Identity/Eye/Random/Diag`, `Solve(A, b)`, `Inverse()`, décompositions LU/QR/Cholesky, valeurs propres, `HorizontalStack/VerticalStack`, `Extract`, `SwapRows/SwapColumns`, graphes (`Floyd`, `DepthFirstSearch`, `BreadthFirstSearch`) |
| `Point` | `X, Y, Z`, conversion implicite depuis/vers `double[]` |
| `Vecteur` | `X, Y, Z`, `Norm`, `Normalize()`, `Dot`, `+`, `* scalaire`, produit vectoriel `^`, constructeur `(origine, cible)` |
| `Extensions` | `DegreesToRadians`, `RadiansToDegrees`, `IsOnSegment`, `CircleSegmentIntersection`, `SegmentSegmentIntersection` |
| `NumberTheory` | `Gcd`, `Lcm`, `IsPrime`, `PrimeFactors`, `Factorial`, `Binomial`, `Fibonacci` (n ≤ 92), `PowMod` |
| `Statistics` | `Mean`, `Median`, `Percentile`, `Variance`, `StandardDeviation`, `Covariance`, `Correlation`, `LinearRegression` |
| `Numerics` | `Clamp`, `Lerp`, `AlmostEqual`, `Derivative`, `Integrate` (Simpson), `Bisection`, `Newton` |
| `MeshQuality` | Qualité de maillage (angles en degrés) : triangles (`TriangleAspectRatio`, `TriangleShape`, `TriangleSkewness`…), quadrilatères (`QuadSkew`, `QuadTaper`, `QuadWarpage`, `QuadScaledJacobian`…), tétraèdres (`TetrahedronRadiusRatio`, `TetrahedronMeanRatio`, `TetrahedronDihedralAngles`…), hexaèdres en ordre VTK (`HexahedronScaledJacobian`, `HexahedronCondition`…), volumes finis (`NonOrthogonality`, `FaceSkewness`, `ExpansionRatio`), statistiques (`Summarize`, `Histogram`, `CountBelow`) |

## Exemples

```csharp
using JBOpale.MathAdvanced;

double r2 = Statistics.LinearRegression(x, y, out double slope, out double intercept);
double area = Numerics.Integrate(System.Math.Sin, 0, System.Math.PI);       // ≈ 2
var factors = NumberTheory.PrimeFactors(360);                                // 2,2,2,3,3,5
var v = new Vecteur(1, 0, 0) ^ new Vecteur(0, 1, 0);                         // produit vectoriel
var x2 = Matrix.Solve(new Matrix(new double[,] { { 2, 0 }, { 0, 4 } }), new Matrix(new double[] { 2, 4 }));
```

Avant d'utiliser une méthode peu courante, lire sa documentation XML (bilingue FR/EN) dans l'IDE.
