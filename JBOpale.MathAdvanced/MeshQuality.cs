// SPDX-License-Identifier: MIT
// Copyright (C) 2026 Jérôme FRANCOIS / JB-Opale — voir LICENSE / see LICENSE
using System;
using System.Collections.Generic;
using System.Linq;

namespace JBOpale.MathAdvanced
{
    /// <summary>
    /// <para><b>[FR]</b> Résumé statistique d'une série de mesures de qualité de maillage.</para>
    /// <para><b>[EN]</b> Statistical summary of a series of mesh quality measurements.</para>
    /// </summary>
    public sealed class MeshQualityStatistics
    {
        /// <summary><b>[FR]</b> Nombre de cellules. / <b>[EN]</b> Number of cells.</summary>
        public int Count { get; internal set; }
        /// <summary><b>[FR]</b> Valeur minimale. / <b>[EN]</b> Minimum value.</summary>
        public double Min { get; internal set; }
        /// <summary><b>[FR]</b> Valeur maximale. / <b>[EN]</b> Maximum value.</summary>
        public double Max { get; internal set; }
        /// <summary><b>[FR]</b> Moyenne. / <b>[EN]</b> Mean.</summary>
        public double Mean { get; internal set; }
        /// <summary><b>[FR]</b> Écart-type de population. / <b>[EN]</b> Population standard deviation.</summary>
        public double StandardDeviation { get; internal set; }
    }

    /// <summary>
    /// <para><b>[FR]</b> Métriques de qualité de maillage pour la simulation numérique (éléments finis / volumes finis) :
    /// triangles, quadrilatères, tétraèdres, hexaèdres (numérotation VTK), métriques de volumes finis
    /// (non-orthogonalité, obliquité de face, rapport d'expansion) et statistiques globales.
    /// Les angles sont en degrés. Une cellule dégénérée donne <c>+∞</c> pour les métriques « à minimiser »
    /// (rapports, conditionnement) et 0 pour celles « à maximiser » (forme, Jacobien).</para>
    /// <para><b>[EN]</b> Mesh quality metrics for numerical simulation (finite elements / finite volumes):
    /// triangles, quadrilaterals, tetrahedra, hexahedra (VTK node ordering), finite-volume metrics
    /// (non-orthogonality, face skewness, expansion ratio) and global statistics.
    /// Angles are in degrees. A degenerate cell yields <c>+∞</c> for "lower is better" metrics
    /// (ratios, condition number) and 0 for "higher is better" ones (shape, Jacobian).</para>
    /// </summary>
    public static class MeshQuality
    {
        #region Vector helpers

        private struct V
        {
            public readonly double X, Y, Z;
            public V(double x, double y, double z) { X = x; Y = y; Z = z; }
            public static V From(Point p, string name)
            {
                if (ReferenceEquals(p, null)) throw new ArgumentNullException(name);
                return new V(p.X, p.Y, p.Z);
            }
            public static V operator +(V a, V b) { return new V(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }
            public static V operator -(V a, V b) { return new V(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }
            public static V operator *(V a, double s) { return new V(a.X * s, a.Y * s, a.Z * s); }
            public double Dot(V b) { return X * b.X + Y * b.Y + Z * b.Z; }
            public V Cross(V b) { return new V(Y * b.Z - Z * b.Y, Z * b.X - X * b.Z, X * b.Y - Y * b.X); }
            public double Norm2 { get { return Dot(this); } }
            public double Norm { get { return System.Math.Sqrt(Norm2); } }
        }

        private const double Eps = 1e-300;
        private static readonly double RadToDeg = 180.0 / System.Math.PI;

        private static double Ratio(double max, double min)
        {
            return min <= Eps ? double.PositiveInfinity : max / min;
        }

        private static double Clamp1(double x)
        {
            return x < -1 ? -1 : (x > 1 ? 1 : x);
        }

        // Angle non orienté entre u et v, en degrés.
        private static double Angle(V u, V v)
        {
            double d = u.Norm * v.Norm;
            if (d <= Eps) return 0;
            return System.Math.Acos(Clamp1(u.Dot(v) / d)) * RadToDeg;
        }

        // Angle orienté de u vers v autour de la normale n, dans [0, 360[.
        private static double SignedAngle(V u, V v, V n)
        {
            double a = System.Math.Atan2(u.Cross(v).Dot(n), u.Dot(v)) * RadToDeg;
            return a < 0 ? a + 360.0 : a;
        }

        private static double Det(V a, V b, V c)
        {
            return a.Dot(b.Cross(c));
        }

        // Conditionnement de Frobenius de la matrice [a b c] (1 pour un cube unité).
        private static double Condition3(V a, V b, V c)
        {
            double det = Det(a, b, c);
            if (System.Math.Abs(det) <= Eps) return double.PositiveInfinity;
            double f2 = a.Norm2 + b.Norm2 + c.Norm2;
            double inv2 = (b.Cross(c).Norm2 + c.Cross(a).Norm2 + a.Cross(b).Norm2) / (det * det);
            return System.Math.Sqrt(f2 * inv2) / 3.0;
        }

        private static void Require(Point[] pts, int n, string name)
        {
            if (pts == null) throw new ArgumentNullException(name);
            if (pts.Length != n) throw new ArgumentException("Exactement " + n + " nœuds requis. / Exactly " + n + " nodes required.", name);
        }

        #endregion

        #region Triangle

        /// <summary>
        /// <para><b>[FR]</b> Aire d'un triangle.</para>
        /// <para><b>[EN]</b> Triangle area.</para>
        /// </summary>
        public static double TriangleArea(Point a, Point b, Point c)
        {
            V pa = V.From(a, nameof(a)), pb = V.From(b, nameof(b)), pc = V.From(c, nameof(c));
            return 0.5 * (pb - pa).Cross(pc - pa).Norm;
        }

        /// <summary>
        /// <para><b>[FR]</b> Angles intérieurs (degrés) d'un triangle, aux nœuds a, b, c.</para>
        /// <para><b>[EN]</b> Triangle interior angles (degrees), at nodes a, b, c.</para>
        /// </summary>
        public static double[] TriangleAngles(Point a, Point b, Point c)
        {
            V pa = V.From(a, nameof(a)), pb = V.From(b, nameof(b)), pc = V.From(c, nameof(c));
            return new double[]
            {
                Angle(pb - pa, pc - pa),
                Angle(pa - pb, pc - pb),
                Angle(pa - pc, pb - pc)
            };
        }

        /// <summary>
        /// <para><b>[FR]</b> Angle minimal d'un triangle (degrés). Idéal : 60.</para>
        /// <para><b>[EN]</b> Triangle minimum angle (degrees). Ideal: 60.</para>
        /// </summary>
        public static double TriangleMinAngle(Point a, Point b, Point c)
        {
            return TriangleAngles(a, b, c).Min();
        }

        /// <summary>
        /// <para><b>[FR]</b> Angle maximal d'un triangle (degrés). Idéal : 60.</para>
        /// <para><b>[EN]</b> Triangle maximum angle (degrees). Ideal: 60.</para>
        /// </summary>
        public static double TriangleMaxAngle(Point a, Point b, Point c)
        {
            return TriangleAngles(a, b, c).Max();
        }

        /// <summary>
        /// <para><b>[FR]</b> Rapport des arêtes (plus longue / plus courte). Idéal : 1.</para>
        /// <para><b>[EN]</b> Edge ratio (longest / shortest). Ideal: 1.</para>
        /// </summary>
        public static double TriangleEdgeRatio(Point a, Point b, Point c)
        {
            V pa = V.From(a, nameof(a)), pb = V.From(b, nameof(b)), pc = V.From(c, nameof(c));
            double l1 = (pb - pa).Norm, l2 = (pc - pb).Norm, l3 = (pa - pc).Norm;
            return Ratio(System.Math.Max(l1, System.Math.Max(l2, l3)), System.Math.Min(l1, System.Math.Min(l2, l3)));
        }

        /// <summary>
        /// <para><b>[FR]</b> Rapport de forme : arête la plus longue × périmètre / (4√3 · aire). Idéal : 1 (équilatéral).</para>
        /// <para><b>[EN]</b> Aspect ratio: longest edge × perimeter / (4√3 · area). Ideal: 1 (equilateral).</para>
        /// </summary>
        public static double TriangleAspectRatio(Point a, Point b, Point c)
        {
            V pa = V.From(a, nameof(a)), pb = V.From(b, nameof(b)), pc = V.From(c, nameof(c));
            double l1 = (pb - pa).Norm, l2 = (pc - pb).Norm, l3 = (pa - pc).Norm;
            double area = 0.5 * (pb - pa).Cross(pc - pa).Norm;
            if (area <= Eps) return double.PositiveInfinity;
            return System.Math.Max(l1, System.Math.Max(l2, l3)) * (l1 + l2 + l3) / (4.0 * System.Math.Sqrt(3.0) * area);
        }

        /// <summary>
        /// <para><b>[FR]</b> Rapport des rayons : R / (2r), rayon du cercle circonscrit sur celui du cercle inscrit. Idéal : 1.</para>
        /// <para><b>[EN]</b> Radius ratio: R / (2r), circumradius over inradius. Ideal: 1.</para>
        /// </summary>
        public static double TriangleRadiusRatio(Point a, Point b, Point c)
        {
            V pa = V.From(a, nameof(a)), pb = V.From(b, nameof(b)), pc = V.From(c, nameof(c));
            double l1 = (pb - pa).Norm, l2 = (pc - pb).Norm, l3 = (pa - pc).Norm;
            double area = 0.5 * (pb - pa).Cross(pc - pa).Norm;
            if (area <= Eps) return double.PositiveInfinity;
            double circum = l1 * l2 * l3 / (4.0 * area);
            double inr = 2.0 * area / (l1 + l2 + l3);
            return circum / (2.0 * inr);
        }

        /// <summary>
        /// <para><b>[FR]</b> Rapport de forme moyen (« mean ratio » de Knupp) : 4√3 · aire / (a² + b² + c²), dans [0, 1]. Idéal : 1.</para>
        /// <para><b>[EN]</b> Mean ratio shape metric (Knupp): 4√3 · area / (a² + b² + c²), in [0, 1]. Ideal: 1.</para>
        /// </summary>
        public static double TriangleShape(Point a, Point b, Point c)
        {
            V pa = V.From(a, nameof(a)), pb = V.From(b, nameof(b)), pc = V.From(c, nameof(c));
            double sum2 = (pb - pa).Norm2 + (pc - pb).Norm2 + (pa - pc).Norm2;
            if (sum2 <= Eps) return 0;
            double area = 0.5 * (pb - pa).Cross(pc - pa).Norm;
            return 4.0 * System.Math.Sqrt(3.0) * area / sum2;
        }

        /// <summary>
        /// <para><b>[FR]</b> Conditionnement (norme de Frobenius) : inverse de <see cref="TriangleShape"/>. Idéal : 1.</para>
        /// <para><b>[EN]</b> Condition number (Frobenius norm): inverse of <see cref="TriangleShape"/>. Ideal: 1.</para>
        /// </summary>
        public static double TriangleCondition(Point a, Point b, Point c)
        {
            double s = TriangleShape(a, b, c);
            return s <= 0 ? double.PositiveInfinity : 1.0 / s;
        }

        /// <summary>
        /// <para><b>[FR]</b> Obliquité équiangulaire dans [0, 1] : max((θmax − 60)/120, (60 − θmin)/60). Idéal : 0.</para>
        /// <para><b>[EN]</b> Equiangular skewness in [0, 1]: max((θmax − 60)/120, (60 − θmin)/60). Ideal: 0.</para>
        /// </summary>
        public static double TriangleSkewness(Point a, Point b, Point c)
        {
            double[] t = TriangleAngles(a, b, c);
            return System.Math.Max((t.Max() - 60.0) / 120.0, (60.0 - t.Min()) / 60.0);
        }

        #endregion

        #region Quadrilateral

        private static V[] QuadNodes(Point a, Point b, Point c, Point d)
        {
            return new V[] { V.From(a, nameof(a)), V.From(b, nameof(b)), V.From(c, nameof(c)), V.From(d, nameof(d)) };
        }

        // Normale (non normalisée) moyenne du quadrilatère : (c − a) × (d − b).
        private static V QuadNormal(V[] p)
        {
            return (p[2] - p[0]).Cross(p[3] - p[1]);
        }

        /// <summary>
        /// <para><b>[FR]</b> Aire d'un quadrilatère (nœuds dans l'ordre, éventuellement gauche) : ½ ‖(c − a) × (d − b)‖.</para>
        /// <para><b>[EN]</b> Quadrilateral area (nodes in order, possibly non-planar): ½ ‖(c − a) × (d − b)‖.</para>
        /// </summary>
        public static double QuadArea(Point a, Point b, Point c, Point d)
        {
            return 0.5 * QuadNormal(QuadNodes(a, b, c, d)).Norm;
        }

        /// <summary>
        /// <para><b>[FR]</b> Angles intérieurs (degrés) aux 4 nœuds. Un angle &gt; 180 indique un quadrilatère non convexe.</para>
        /// <para><b>[EN]</b> Interior angles (degrees) at the 4 nodes. An angle &gt; 180 indicates a non-convex quadrilateral.</para>
        /// </summary>
        public static double[] QuadAngles(Point a, Point b, Point c, Point d)
        {
            V[] p = QuadNodes(a, b, c, d);
            V n = QuadNormal(p);
            double[] angles = new double[4];
            for (int i = 0; i < 4; i++)
            {
                V cur = p[i], next = p[(i + 1) % 4], prev = p[(i + 3) % 4];
                angles[i] = SignedAngle(next - cur, prev - cur, n);
            }
            return angles;
        }

        /// <summary>
        /// <para><b>[FR]</b> Angle minimal d'un quadrilatère (degrés). Idéal : 90.</para>
        /// <para><b>[EN]</b> Quadrilateral minimum angle (degrees). Ideal: 90.</para>
        /// </summary>
        public static double QuadMinAngle(Point a, Point b, Point c, Point d)
        {
            return QuadAngles(a, b, c, d).Min();
        }

        /// <summary>
        /// <para><b>[FR]</b> Angle maximal d'un quadrilatère (degrés). Idéal : 90.</para>
        /// <para><b>[EN]</b> Quadrilateral maximum angle (degrees). Ideal: 90.</para>
        /// </summary>
        public static double QuadMaxAngle(Point a, Point b, Point c, Point d)
        {
            return QuadAngles(a, b, c, d).Max();
        }

        /// <summary>
        /// <para><b>[FR]</b> Rapport des arêtes (plus longue / plus courte). Idéal : 1.</para>
        /// <para><b>[EN]</b> Edge ratio (longest / shortest). Ideal: 1.</para>
        /// </summary>
        public static double QuadEdgeRatio(Point a, Point b, Point c, Point d)
        {
            V[] p = QuadNodes(a, b, c, d);
            double[] l = new double[4];
            for (int i = 0; i < 4; i++) l[i] = (p[(i + 1) % 4] - p[i]).Norm;
            return Ratio(l.Max(), l.Min());
        }

        // Axes principaux X1 = (b − a) + (c − d), X2 = (c − b) + (d − a).
        private static void QuadAxes(V[] p, out V x1, out V x2)
        {
            x1 = (p[1] - p[0]) + (p[2] - p[3]);
            x2 = (p[2] - p[1]) + (p[3] - p[0]);
        }

        /// <summary>
        /// <para><b>[FR]</b> Rapport de forme : rapport des longueurs des deux axes principaux
        /// X1 = (b − a) + (c − d) et X2 = (c − b) + (d − a), plus grand sur plus petit. Idéal : 1.</para>
        /// <para><b>[EN]</b> Aspect ratio: ratio of the two principal axes lengths
        /// X1 = (b − a) + (c − d) and X2 = (c − b) + (d − a), larger over smaller. Ideal: 1.</para>
        /// </summary>
        public static double QuadAspectRatio(Point a, Point b, Point c, Point d)
        {
            V x1, x2;
            QuadAxes(QuadNodes(a, b, c, d), out x1, out x2);
            double l1 = x1.Norm, l2 = x2.Norm;
            return Ratio(System.Math.Max(l1, l2), System.Math.Min(l1, l2));
        }

        /// <summary>
        /// <para><b>[FR]</b> Obliquité : |cos| de l'angle entre les axes principaux X1 et X2, dans [0, 1]. Idéal : 0.</para>
        /// <para><b>[EN]</b> Skew: |cos| of the angle between principal axes X1 and X2, in [0, 1]. Ideal: 0.</para>
        /// </summary>
        public static double QuadSkew(Point a, Point b, Point c, Point d)
        {
            V x1, x2;
            QuadAxes(QuadNodes(a, b, c, d), out x1, out x2);
            double den = x1.Norm * x2.Norm;
            return den <= Eps ? 1.0 : System.Math.Abs(x1.Dot(x2)) / den;
        }

        /// <summary>
        /// <para><b>[FR]</b> Effilement (« taper ») : ‖(a − b) + (c − d)‖ / min(‖X1‖, ‖X2‖). Idéal : 0.</para>
        /// <para><b>[EN]</b> Taper: ‖(a − b) + (c − d)‖ / min(‖X1‖, ‖X2‖). Ideal: 0.</para>
        /// </summary>
        public static double QuadTaper(Point a, Point b, Point c, Point d)
        {
            V[] p = QuadNodes(a, b, c, d);
            V x1, x2;
            QuadAxes(p, out x1, out x2);
            V x12 = (p[0] - p[1]) + (p[2] - p[3]);
            double m = System.Math.Min(x1.Norm, x2.Norm);
            return m <= Eps ? double.PositiveInfinity : x12.Norm / m;
        }

        /// <summary>
        /// <para><b>[FR]</b> Étirement : √2 · arête la plus courte / diagonale la plus longue. Idéal : 1 (carré).</para>
        /// <para><b>[EN]</b> Stretch: √2 · shortest edge / longest diagonal. Ideal: 1 (square).</para>
        /// </summary>
        public static double QuadStretch(Point a, Point b, Point c, Point d)
        {
            V[] p = QuadNodes(a, b, c, d);
            double minEdge = double.MaxValue;
            for (int i = 0; i < 4; i++) minEdge = System.Math.Min(minEdge, (p[(i + 1) % 4] - p[i]).Norm);
            double maxDiag = System.Math.Max((p[2] - p[0]).Norm, (p[3] - p[1]).Norm);
            return maxDiag <= Eps ? 0 : System.Math.Sqrt(2.0) * minEdge / maxDiag;
        }

        /// <summary>
        /// <para><b>[FR]</b> Gauchissement (« warpage ») : angle maximal (degrés) entre les normales aux 4 coins.
        /// 0 pour un quadrilatère plan.</para>
        /// <para><b>[EN]</b> Warpage: maximum angle (degrees) between the 4 corner normals. 0 for a planar quadrilateral.</para>
        /// </summary>
        public static double QuadWarpage(Point a, Point b, Point c, Point d)
        {
            V[] p = QuadNodes(a, b, c, d);
            V[] n = new V[4];
            for (int i = 0; i < 4; i++)
                n[i] = (p[(i + 1) % 4] - p[i]).Cross(p[(i + 3) % 4] - p[i]);
            double max = 0;
            for (int i = 0; i < 4; i++)
                for (int j = i + 1; j < 4; j++)
                    max = System.Math.Max(max, Angle(n[i], n[j]));
            return max;
        }

        /// <summary>
        /// <para><b>[FR]</b> Jacobien normalisé minimal aux 4 coins (sin de l'angle intérieur), dans [−1, 1].
        /// Idéal : 1 ; ≤ 0 : élément dégénéré ou inversé.</para>
        /// <para><b>[EN]</b> Minimum scaled Jacobian over the 4 corners (sine of the interior angle), in [−1, 1].
        /// Ideal: 1; ≤ 0: degenerate or inverted element.</para>
        /// </summary>
        public static double QuadScaledJacobian(Point a, Point b, Point c, Point d)
        {
            V[] p = QuadNodes(a, b, c, d);
            V n = QuadNormal(p);
            double nn = n.Norm;
            if (nn <= Eps) return 0;
            n = n * (1.0 / nn);
            double min = double.MaxValue;
            for (int i = 0; i < 4; i++)
            {
                V u = p[(i + 1) % 4] - p[i], v = p[(i + 3) % 4] - p[i];
                double den = u.Norm * v.Norm;
                min = System.Math.Min(min, den <= Eps ? 0 : u.Cross(v).Dot(n) / den);
            }
            return min;
        }

        #endregion

        #region Tetrahedron

        private static V[] TetNodes(Point a, Point b, Point c, Point d)
        {
            return new V[] { V.From(a, nameof(a)), V.From(b, nameof(b)), V.From(c, nameof(c)), V.From(d, nameof(d)) };
        }

        private static double TetVolume(V[] p)
        {
            return Det(p[1] - p[0], p[2] - p[0], p[3] - p[0]) / 6.0;
        }

        private static double[] TetEdgeLengths(V[] p)
        {
            return new double[]
            {
                (p[1] - p[0]).Norm, (p[2] - p[0]).Norm, (p[3] - p[0]).Norm,
                (p[2] - p[1]).Norm, (p[3] - p[1]).Norm, (p[3] - p[2]).Norm
            };
        }

        /// <summary>
        /// <para><b>[FR]</b> Volume signé d'un tétraèdre : (b − a) · ((c − a) × (d − a)) / 6. Négatif si l'élément est inversé.</para>
        /// <para><b>[EN]</b> Signed tetrahedron volume: (b − a) · ((c − a) × (d − a)) / 6. Negative when the element is inverted.</para>
        /// </summary>
        public static double TetrahedronVolume(Point a, Point b, Point c, Point d)
        {
            return TetVolume(TetNodes(a, b, c, d));
        }

        /// <summary>
        /// <para><b>[FR]</b> Rapport des arêtes (plus longue / plus courte). Idéal : 1.</para>
        /// <para><b>[EN]</b> Edge ratio (longest / shortest). Ideal: 1.</para>
        /// </summary>
        public static double TetrahedronEdgeRatio(Point a, Point b, Point c, Point d)
        {
            double[] l = TetEdgeLengths(TetNodes(a, b, c, d));
            return Ratio(l.Max(), l.Min());
        }

        // Rayon inscrit r = 3V / somme des aires des faces.
        private static double TetInradius(V[] p, double volume)
        {
            double area = 0.5 * ((p[1] - p[0]).Cross(p[2] - p[0]).Norm
                               + (p[1] - p[0]).Cross(p[3] - p[0]).Norm
                               + (p[2] - p[0]).Cross(p[3] - p[0]).Norm
                               + (p[2] - p[1]).Cross(p[3] - p[1]).Norm);
            return area <= Eps ? 0 : 3.0 * System.Math.Abs(volume) / area;
        }

        // Rayon circonscrit.
        private static double TetCircumradius(V[] p, double volume)
        {
            V u = p[0] - p[3], v = p[1] - p[3], w = p[2] - p[3];
            double det = Det(u, v, w);
            if (System.Math.Abs(det) <= Eps) return double.PositiveInfinity;
            V num = v.Cross(w) * u.Norm2 + w.Cross(u) * v.Norm2 + u.Cross(v) * w.Norm2;
            return num.Norm / (2.0 * System.Math.Abs(det));
        }

        /// <summary>
        /// <para><b>[FR]</b> Rapport de forme : arête la plus longue / (2√6 · rayon inscrit). Idéal : 1 (régulier).</para>
        /// <para><b>[EN]</b> Aspect ratio: longest edge / (2√6 · inradius). Ideal: 1 (regular).</para>
        /// </summary>
        public static double TetrahedronAspectRatio(Point a, Point b, Point c, Point d)
        {
            V[] p = TetNodes(a, b, c, d);
            double r = TetInradius(p, TetVolume(p));
            return r <= Eps ? double.PositiveInfinity : TetEdgeLengths(p).Max() / (2.0 * System.Math.Sqrt(6.0) * r);
        }

        /// <summary>
        /// <para><b>[FR]</b> Rapport des rayons : R / (3r), rayon circonscrit sur rayon inscrit. Idéal : 1.</para>
        /// <para><b>[EN]</b> Radius ratio: R / (3r), circumradius over inradius. Ideal: 1.</para>
        /// </summary>
        public static double TetrahedronRadiusRatio(Point a, Point b, Point c, Point d)
        {
            V[] p = TetNodes(a, b, c, d);
            double vol = TetVolume(p);
            double r = TetInradius(p, vol);
            return r <= Eps ? double.PositiveInfinity : TetCircumradius(p, vol) / (3.0 * r);
        }

        /// <summary>
        /// <para><b>[FR]</b> Rapport de forme moyen (Knupp) : 12 (3|V|)^(2/3) / Σ longueurs², dans [0, 1]. Idéal : 1.</para>
        /// <para><b>[EN]</b> Mean ratio shape metric (Knupp): 12 (3|V|)^(2/3) / Σ edge², in [0, 1]. Ideal: 1.</para>
        /// </summary>
        public static double TetrahedronMeanRatio(Point a, Point b, Point c, Point d)
        {
            V[] p = TetNodes(a, b, c, d);
            double sum2 = TetEdgeLengths(p).Sum(l => l * l);
            if (sum2 <= Eps) return 0;
            return 12.0 * System.Math.Pow(3.0 * System.Math.Abs(TetVolume(p)), 2.0 / 3.0) / sum2;
        }

        /// <summary>
        /// <para><b>[FR]</b> Jacobien normalisé minimal aux 4 sommets (√2 · déterminant / produit des arêtes), signé.
        /// Idéal : 1 ; ≤ 0 : élément dégénéré ou inversé.</para>
        /// <para><b>[EN]</b> Minimum scaled Jacobian over the 4 vertices (√2 · determinant / product of edges), signed.
        /// Ideal: 1; ≤ 0: degenerate or inverted element.</para>
        /// </summary>
        public static double TetrahedronScaledJacobian(Point a, Point b, Point c, Point d)
        {
            V[] p = TetNodes(a, b, c, d);
            int[][] corners = new int[][]
            {
                new[] { 0, 1, 2, 3 }, new[] { 1, 2, 0, 3 }, new[] { 2, 0, 1, 3 }, new[] { 3, 0, 2, 1 }
            };
            double min = double.MaxValue;
            foreach (int[] k in corners)
            {
                V e1 = p[k[1]] - p[k[0]], e2 = p[k[2]] - p[k[0]], e3 = p[k[3]] - p[k[0]];
                double den = e1.Norm * e2.Norm * e3.Norm;
                min = System.Math.Min(min, den <= Eps ? 0 : System.Math.Sqrt(2.0) * Det(e1, e2, e3) / den);
            }
            return min;
        }

        /// <summary>
        /// <para><b>[FR]</b> Angles dièdres intérieurs (degrés) aux 6 arêtes (ab, ac, ad, bc, bd, cd). Idéal : ≈ 70,53.</para>
        /// <para><b>[EN]</b> Interior dihedral angles (degrees) at the 6 edges (ab, ac, ad, bc, bd, cd). Ideal: ≈ 70.53.</para>
        /// </summary>
        public static double[] TetrahedronDihedralAngles(Point a, Point b, Point c, Point d)
        {
            V[] p = TetNodes(a, b, c, d);
            int[][] edges = new int[][]
            {
                new[] { 0, 1, 2, 3 }, new[] { 0, 2, 1, 3 }, new[] { 0, 3, 1, 2 },
                new[] { 1, 2, 0, 3 }, new[] { 1, 3, 0, 2 }, new[] { 2, 3, 0, 1 }
            };
            double[] result = new double[6];
            for (int e = 0; e < 6; e++)
            {
                int[] k = edges[e];
                V axis = p[k[1]] - p[k[0]];
                double a2 = axis.Norm2;
                if (a2 <= Eps) { result[e] = 0; continue; }
                V u = p[k[2]] - p[k[0]], w = p[k[3]] - p[k[0]];
                V pu = u - axis * (u.Dot(axis) / a2);
                V pw = w - axis * (w.Dot(axis) / a2);
                result[e] = Angle(pu, pw);
            }
            return result;
        }

        #endregion

        #region Hexahedron

        // Ordre VTK : 0-3 face inférieure (anti-horaire vue d'en haut), 4-7 face supérieure au-dessus de 0-3.
        private static readonly int[][] HexCornerNeighbours = new int[][]
        {
            new[] { 1, 3, 4 }, new[] { 2, 0, 5 }, new[] { 3, 1, 6 }, new[] { 0, 2, 7 },
            new[] { 7, 5, 0 }, new[] { 1, 4, 6 }, new[] { 2, 5, 7 }, new[] { 3, 6, 4 }
        };

        private static readonly int[][] HexEdges = new int[][]
        {
            new[] { 0, 1 }, new[] { 1, 2 }, new[] { 2, 3 }, new[] { 3, 0 },
            new[] { 4, 5 }, new[] { 5, 6 }, new[] { 6, 7 }, new[] { 7, 4 },
            new[] { 0, 4 }, new[] { 1, 5 }, new[] { 2, 6 }, new[] { 3, 7 }
        };

        private static readonly int[][] HexDiagonals = new int[][]
        {
            new[] { 0, 6 }, new[] { 1, 7 }, new[] { 2, 4 }, new[] { 3, 5 }
        };

        private static V[] HexNodes(Point[] nodes)
        {
            Require(nodes, 8, nameof(nodes));
            return nodes.Select((n, i) => V.From(n, nameof(nodes))).ToArray();
        }

        /// <summary>
        /// <para><b>[FR]</b> Volume d'un hexaèdre (nœuds en ordre VTK) par quadrature de Gauss 2×2×2 de la
        /// transformation trilinéaire (exact). Négatif si l'élément est inversé.</para>
        /// <para><b>[EN]</b> Hexahedron volume (VTK node order) using 2×2×2 Gauss quadrature of the
        /// trilinear mapping (exact). Negative when the element is inverted.</para>
        /// </summary>
        public static double HexahedronVolume(Point[] nodes)
        {
            V[] p = HexNodes(nodes);
            double[] sx = { -1, 1, 1, -1, -1, 1, 1, -1 };
            double[] sy = { -1, -1, 1, 1, -1, -1, 1, 1 };
            double[] sz = { -1, -1, -1, -1, 1, 1, 1, 1 };
            double g = 1.0 / System.Math.Sqrt(3.0);
            double volume = 0;
            for (int ix = 0; ix < 2; ix++)
                for (int iy = 0; iy < 2; iy++)
                    for (int iz = 0; iz < 2; iz++)
                    {
                        double xi = ix == 0 ? -g : g, eta = iy == 0 ? -g : g, zeta = iz == 0 ? -g : g;
                        V dxi = new V(0, 0, 0), deta = new V(0, 0, 0), dzeta = new V(0, 0, 0);
                        for (int i = 0; i < 8; i++)
                        {
                            double fx = 1 + sx[i] * xi, fy = 1 + sy[i] * eta, fz = 1 + sz[i] * zeta;
                            dxi = dxi + p[i] * (sx[i] * fy * fz / 8.0);
                            deta = deta + p[i] * (sy[i] * fx * fz / 8.0);
                            dzeta = dzeta + p[i] * (sz[i] * fx * fy / 8.0);
                        }
                        volume += Det(dxi, deta, dzeta);
                    }
            return volume;
        }

        /// <summary>
        /// <para><b>[FR]</b> Rapport des arêtes (plus longue / plus courte, sur 12 arêtes). Idéal : 1.</para>
        /// <para><b>[EN]</b> Edge ratio (longest / shortest over 12 edges). Ideal: 1.</para>
        /// </summary>
        public static double HexahedronEdgeRatio(Point[] nodes)
        {
            V[] p = HexNodes(nodes);
            double[] l = HexEdges.Select(e => (p[e[1]] - p[e[0]]).Norm).ToArray();
            return Ratio(l.Max(), l.Min());
        }

        /// <summary>
        /// <para><b>[FR]</b> Rapport des diagonales : plus courte / plus longue des 4 diagonales principales, dans [0, 1]. Idéal : 1.</para>
        /// <para><b>[EN]</b> Diagonal ratio: shortest / longest of the 4 main diagonals, in [0, 1]. Ideal: 1.</para>
        /// </summary>
        public static double HexahedronDiagonalRatio(Point[] nodes)
        {
            V[] p = HexNodes(nodes);
            double[] l = HexDiagonals.Select(e => (p[e[1]] - p[e[0]]).Norm).ToArray();
            return l.Max() <= Eps ? 0 : l.Min() / l.Max();
        }

        /// <summary>
        /// <para><b>[FR]</b> Étirement : √3 · arête la plus courte / diagonale la plus longue. Idéal : 1 (cube).</para>
        /// <para><b>[EN]</b> Stretch: √3 · shortest edge / longest diagonal. Ideal: 1 (cube).</para>
        /// </summary>
        public static double HexahedronStretch(Point[] nodes)
        {
            V[] p = HexNodes(nodes);
            double minEdge = HexEdges.Min(e => (p[e[1]] - p[e[0]]).Norm);
            double maxDiag = HexDiagonals.Max(e => (p[e[1]] - p[e[0]]).Norm);
            return maxDiag <= Eps ? 0 : System.Math.Sqrt(3.0) * minEdge / maxDiag;
        }

        /// <summary>
        /// <para><b>[FR]</b> Jacobien normalisé minimal aux 8 coins (déterminant / produit des 3 arêtes), dans [−1, 1].
        /// Idéal : 1 ; ≤ 0 : élément dégénéré ou inversé.</para>
        /// <para><b>[EN]</b> Minimum scaled Jacobian over the 8 corners (determinant / product of the 3 edges), in [−1, 1].
        /// Ideal: 1; ≤ 0: degenerate or inverted element.</para>
        /// </summary>
        public static double HexahedronScaledJacobian(Point[] nodes)
        {
            V[] p = HexNodes(nodes);
            double min = double.MaxValue;
            for (int i = 0; i < 8; i++)
            {
                int[] k = HexCornerNeighbours[i];
                V e1 = p[k[0]] - p[i], e2 = p[k[1]] - p[i], e3 = p[k[2]] - p[i];
                double den = e1.Norm * e2.Norm * e3.Norm;
                min = System.Math.Min(min, den <= Eps ? 0 : Det(e1, e2, e3) / den);
            }
            return min;
        }

        /// <summary>
        /// <para><b>[FR]</b> Conditionnement de Frobenius maximal aux 8 coins. Idéal : 1 (cube) ; +∞ si dégénéré.</para>
        /// <para><b>[EN]</b> Maximum Frobenius condition number over the 8 corners. Ideal: 1 (cube); +∞ if degenerate.</para>
        /// </summary>
        public static double HexahedronCondition(Point[] nodes)
        {
            V[] p = HexNodes(nodes);
            double max = 0;
            for (int i = 0; i < 8; i++)
            {
                int[] k = HexCornerNeighbours[i];
                max = System.Math.Max(max, Condition3(p[k[0]] - p[i], p[k[1]] - p[i], p[k[2]] - p[i]));
            }
            return max;
        }

        #endregion

        #region Finite volume metrics / Métriques volumes finis

        /// <summary>
        /// <para><b>[FR]</b> Non-orthogonalité (degrés) : angle entre la droite joignant les centres de deux cellules
        /// voisines et la normale à leur face commune. Idéal : 0 ; au-delà de ~70, la discrétisation se dégrade.</para>
        /// <para><b>[EN]</b> Non-orthogonality (degrees): angle between the line joining two neighbouring cell centres
        /// and the normal of their shared face. Ideal: 0; above ~70, the discretisation degrades.</para>
        /// </summary>
        public static double NonOrthogonality(Point ownerCentre, Point neighbourCentre, Point faceNormal)
        {
            V d = V.From(neighbourCentre, nameof(neighbourCentre)) - V.From(ownerCentre, nameof(ownerCentre));
            V n = V.From(faceNormal, nameof(faceNormal));
            return Angle(d, n);
        }

        /// <summary>
        /// <para><b>[FR]</b> Obliquité de face : distance entre le centroïde de la face et l'intersection de la droite
        /// reliant les centres des cellules avec le plan de la face, divisée par la distance entre centres. Idéal : 0.</para>
        /// <para><b>[EN]</b> Face skewness: distance between the face centroid and the intersection of the cell-centre
        /// line with the face plane, divided by the distance between centres. Ideal: 0.</para>
        /// </summary>
        public static double FaceSkewness(Point ownerCentre, Point neighbourCentre, Point faceCentroid, Point faceNormal)
        {
            V p = V.From(ownerCentre, nameof(ownerCentre));
            V d = V.From(neighbourCentre, nameof(neighbourCentre)) - p;
            V fc = V.From(faceCentroid, nameof(faceCentroid));
            V n = V.From(faceNormal, nameof(faceNormal));
            double dn = d.Dot(n), len = d.Norm;
            if (System.Math.Abs(dn) <= Eps || len <= Eps) return double.PositiveInfinity;
            double t = n.Dot(fc - p) / dn;
            V x = p + d * t;
            return (fc - x).Norm / len;
        }

        /// <summary>
        /// <para><b>[FR]</b> Rapport d'expansion (de progression) entre deux cellules adjacentes de tailles
        /// <paramref name="sizeA"/> et <paramref name="sizeB"/> : plus grande / plus petite. Idéal : 1 ; conseillé &lt; 1,3.</para>
        /// <para><b>[EN]</b> Expansion (growth) ratio between two adjacent cells of sizes
        /// <paramref name="sizeA"/> and <paramref name="sizeB"/>: larger / smaller. Ideal: 1; recommended &lt; 1.3.</para>
        /// </summary>
        public static double ExpansionRatio(double sizeA, double sizeB)
        {
            if (sizeA < 0) throw new ArgumentOutOfRangeException(nameof(sizeA));
            if (sizeB < 0) throw new ArgumentOutOfRangeException(nameof(sizeB));
            return Ratio(System.Math.Max(sizeA, sizeB), System.Math.Min(sizeA, sizeB));
        }

        /// <summary>
        /// <para><b>[FR]</b> Rapport de forme d'une cellule quelconque via sa boîte englobante alignée sur les axes :
        /// plus grande / plus petite dimension non nulle. Idéal : 1.</para>
        /// <para><b>[EN]</b> Aspect ratio of an arbitrary cell using its axis-aligned bounding box:
        /// largest / smallest non-zero extent. Ideal: 1.</para>
        /// </summary>
        public static double BoundingBoxAspectRatio(IEnumerable<Point> nodes)
        {
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));
            List<Point> pts = nodes.ToList();
            if (pts.Count < 2) throw new ArgumentException("Au moins 2 nœuds requis. / At least 2 nodes required.", nameof(nodes));
            double[] ext =
            {
                pts.Max(q => q.X) - pts.Min(q => q.X),
                pts.Max(q => q.Y) - pts.Min(q => q.Y),
                pts.Max(q => q.Z) - pts.Min(q => q.Z)
            };
            double[] nz = ext.Where(e => e > Eps).ToArray();
            return nz.Length == 0 ? double.PositiveInfinity : nz.Max() / nz.Min();
        }

        #endregion

        #region Global statistics / Statistiques globales

        /// <summary>
        /// <para><b>[FR]</b> Résume une série de valeurs de qualité (min, max, moyenne, écart-type).</para>
        /// <para><b>[EN]</b> Summarises a series of quality values (min, max, mean, standard deviation).</para>
        /// </summary>
        public static MeshQualityStatistics Summarize(IEnumerable<double> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            double[] v = values.ToArray();
            if (v.Length == 0) throw new ArgumentException("La séquence est vide. / The sequence is empty.", nameof(values));
            return new MeshQualityStatistics
            {
                Count = v.Length,
                Min = v.Min(),
                Max = v.Max(),
                Mean = Statistics.Mean(v),
                StandardDeviation = Statistics.StandardDeviation(v, false)
            };
        }

        /// <summary>
        /// <para><b>[FR]</b> Histogramme à <paramref name="bins"/> classes égales sur [<paramref name="min"/>, <paramref name="max"/>] ;
        /// les valeurs hors intervalle sont ignorées, <paramref name="max"/> appartient à la dernière classe.</para>
        /// <para><b>[EN]</b> Histogram with <paramref name="bins"/> equal bins over [<paramref name="min"/>, <paramref name="max"/>];
        /// out-of-range values are ignored, <paramref name="max"/> belongs to the last bin.</para>
        /// </summary>
        public static int[] Histogram(IEnumerable<double> values, int bins, double min, double max)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (bins <= 0) throw new ArgumentOutOfRangeException(nameof(bins));
            if (!(max > min)) throw new ArgumentException("max doit être > min. / max must be > min.");
            int[] counts = new int[bins];
            foreach (double x in values)
            {
                if (double.IsNaN(x) || x < min || x > max) continue;
                int i = (int)((x - min) / (max - min) * bins);
                counts[System.Math.Min(i, bins - 1)]++;
            }
            return counts;
        }

        /// <summary>
        /// <para><b>[FR]</b> Nombre de valeurs strictement inférieures à <paramref name="threshold"/>
        /// (ex. cellules de Jacobien &lt; 0,2) — utile pour repérer les cellules à corriger.</para>
        /// <para><b>[EN]</b> Number of values strictly below <paramref name="threshold"/>
        /// (e.g. cells with Jacobian &lt; 0.2) — handy to spot cells that need fixing.</para>
        /// </summary>
        public static int CountBelow(IEnumerable<double> values, double threshold)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            return values.Count(x => x < threshold);
        }

        #endregion
    }
}
