// SPDX-License-Identifier: MIT
// Copyright (C) 2026 Jérôme FRANCOIS / JB-Opale — voir LICENSE / see LICENSE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBOPaleAPI.MathAdvanced
{
    /// <summary>
    /// <para><b>[FR]</b> Fournit des méthodes utilitaires mathématiques et géométriques pour les
    /// calculs en 2D et 3D : conversions d'angles, appartenance à un segment, et calculs
    /// d'intersections cercle/segment et segment/segment.</para>
    /// <para><b>[EN]</b> Provides mathematical and geometric utility methods for 2D and 3D
    /// computations: angle conversions, point-on-segment testing, and circle/segment and
    /// segment/segment intersection calculations.</para>
    /// </summary>
    public static class Extensions
    {
        #region Angle Conversions / Conversions d'angles

        /// <summary>
        /// <para><b>[FR]</b> Convertit un angle exprimé en degrés vers son équivalent en radians.</para>
        /// <para><b>[EN]</b> Converts an angle expressed in degrees to its equivalent in radians.</para>
        /// </summary>
        /// <param name="degrees">
        /// <b>[FR]</b> Angle en degrés. /
        /// <b>[EN]</b> Angle in degrees.
        /// </param>
        /// <returns>
        /// <b>[FR]</b> Valeur de l'angle en radians : <c>degrees × π / 180</c>. /
        /// <b>[EN]</b> Angle value in radians: <c>degrees × π / 180</c>.
        /// </returns>
        public static double DegreesToRadians(double degrees)
        {
            return System.Math.PI * degrees / 180.0;
        }

        /// <summary>
        /// <para><b>[FR]</b> Convertit un angle exprimé en radians vers son équivalent en degrés.</para>
        /// <para><b>[EN]</b> Converts an angle expressed in radians to its equivalent in degrees.</para>
        /// </summary>
        /// <param name="radians">
        /// <b>[FR]</b> Angle en radians. /
        /// <b>[EN]</b> Angle in radians.
        /// </param>
        /// <returns>
        /// <b>[FR]</b> Valeur de l'angle en degrés : <c>radians × 180 / π</c>. /
        /// <b>[EN]</b> Angle value in degrees: <c>radians × 180 / π</c>.
        /// </returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians * (180.0 / System.Math.PI);
        }

        #endregion

        #region Geometry / Géométrie

        /// <summary>
        /// <para><b>[FR]</b> Détermine si le point <paramref name="point"/> appartient au segment
        /// [<paramref name="segmentStart"/>, <paramref name="segmentEnd"/>]. La vérification repose
        /// sur la colinéarité des vecteurs et l'inclusion du point entre les deux extrémités.</para>
        /// <para><b>[EN]</b> Determines whether <paramref name="point"/> lies on the segment
        /// [<paramref name="segmentStart"/>, <paramref name="segmentEnd"/>]. The check is based on
        /// collinearity of the direction vectors and inclusion of the point between both endpoints.</para>
        /// </summary>
        /// <param name="segmentStart">
        /// <b>[FR]</b> Extrémité d'origine du segment. /
        /// <b>[EN]</b> Start endpoint of the segment.
        /// </param>
        /// <param name="segmentEnd">
        /// <b>[FR]</b> Extrémité d'arrivée du segment. /
        /// <b>[EN]</b> End endpoint of the segment.
        /// </param>
        /// <param name="point">
        /// <b>[FR]</b> Point à tester. /
        /// <b>[EN]</b> Point to test.
        /// </param>
        /// <param name="tolerance">
        /// <para><b>[FR]</b> Tolérance numérique appliquée sur la norme du produit vectoriel et sur
        /// le test de colinéarité. Par défaut : 0 (comparaison exacte).</para>
        /// <para><b>[EN]</b> Numerical tolerance applied to the cross-product norm and the
        /// collinearity test. Default: 0 (exact comparison).</para>
        /// </param>
        /// <returns>
        /// <b>[FR]</b> <c>true</c> si le point est sur le segment à la tolérance donnée ; <c>false</c> sinon. /
        /// <b>[EN]</b> <c>true</c> if the point lies on the segment within the given tolerance; <c>false</c> otherwise.
        /// </returns>
        public static bool IsOnSegment(Point segmentStart, Point segmentEnd, Point point, double tolerance = 0)
        {
            Vecteur v1 = new Vecteur(segmentStart, segmentEnd);
            Vecteur v2 = new Vecteur(segmentStart, point);

            Vecteur cross = v1 ^ v2;
            if (System.Math.Abs(cross.Norm) <= System.Math.Abs(tolerance))
            {
                double dotProduct = v1.Dot(v2);
                double collinearity = System.Math.Abs(1 - dotProduct / (v1.Norm * v2.Norm));
                if (collinearity <= System.Math.Abs(tolerance) && v1.Norm >= v2.Norm)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le(s) point(s) d'intersection entre un cercle et un segment
        /// dans le plan XY. Le centre du cercle peut être décalé par des tolérances sur X et Y.</para>
        /// <para><b>[EN]</b> Computes the intersection point(s) between a circle and a segment in
        /// the XY plane. The circle center can be offset by tolerances on X and Y.</para>
        /// </summary>
        /// <param name="circleCenter">
        /// <b>[FR]</b> Centre du cercle. /
        /// <b>[EN]</b> Center of the circle.
        /// </param>
        /// <param name="radius">
        /// <b>[FR]</b> Rayon du cercle. /
        /// <b>[EN]</b> Radius of the circle.
        /// </param>
        /// <param name="segmentStart">
        /// <b>[FR]</b> Première extrémité du segment. /
        /// <b>[EN]</b> First endpoint of the segment.
        /// </param>
        /// <param name="segmentEnd">
        /// <b>[FR]</b> Seconde extrémité du segment. /
        /// <b>[EN]</b> Second endpoint of the segment.
        /// </param>
        /// <param name="toleranceX">
        /// <b>[FR]</b> Décalage appliqué sur la coordonnée X du centre. /
        /// <b>[EN]</b> Offset applied to the X coordinate of the center.
        /// </param>
        /// <param name="toleranceY">
        /// <b>[FR]</b> Décalage appliqué sur la coordonnée Y du centre. /
        /// <b>[EN]</b> Offset applied to the Y coordinate of the center.
        /// </param>
        /// <param name="intersections">
        /// <para><b>[FR]</b> Tableau de deux entrées [t, X, Y] où t est le paramètre sur le segment
        /// (t ∈ [0,1] si sur le segment), et (X, Y) les coordonnées du point d'intersection.
        /// La seconde entrée vaut [-1, -1, -1] en cas d'intersection tangente.</para>
        /// <para><b>[EN]</b> Array of two entries [t, X, Y] where t is the parameter along the
        /// segment (t ∈ [0,1] if on the segment), and (X, Y) the intersection coordinates.
        /// The second entry is [-1, -1, -1] for a tangent intersection.</para>
        /// </param>
        /// <returns>
        /// <b>[FR]</b> <c>0</c> si aucune intersection ; <c>1</c> si une ou deux intersections existent. /
        /// <b>[EN]</b> <c>0</c> if no intersection; <c>1</c> if one or two intersections exist.
        /// </returns>
        public static int CircleSegmentIntersection(
            Point circleCenter, double radius,
            Point segmentStart, Point segmentEnd,
            double toleranceX, double toleranceY,
            out double[][] intersections)
        {
            intersections = new double[2][];

            double ax = segmentStart.X;
            double ay = segmentStart.Y;
            double bx = segmentEnd.X;
            double by = segmentEnd.Y;
            double ox = circleCenter.X + toleranceX;
            double oy = circleCenter.Y + toleranceY;

            double dx = bx - ax;
            double dy = by - ay;
            double a = dx * dx + dy * dy;
            double b = 2 * (dx * (ax - ox) + dy * (ay - oy));
            double c = (ax - ox) * (ax - ox) + (ay - oy) * (ay - oy) - radius * radius;

            double delta = b * b - 4 * a * c;

            double t1 = -1, x1 = -1, y1 = -1;
            double t2 = -1, x2 = -1, y2 = -1;

            if (delta < 0)
            {
                return 0;
            }
            else if (delta == 0)
            {
                t1 = -b / (2 * a);
                x1 = t1 * dx + ax;
                y1 = t1 * dy + ay;
            }
            else
            {
                t1 = (-b - System.Math.Sqrt(delta)) / (2 * a);
                x1 = t1 * dx + ax;
                y1 = t1 * dy + ay;

                t2 = (-b + System.Math.Sqrt(delta)) / (2 * a);
                x2 = t2 * dx + ax;
                y2 = t2 * dy + ay;
            }

            intersections[0] = new double[3] { t1, x1, y1 };
            intersections[1] = new double[3] { t2, x2, y2 };

            return 1;
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le point d'intersection entre deux segments [AB] et [CD]
        /// dans le plan XY, en utilisant le produit vectoriel des directions pour détecter la collision.</para>
        /// <para><b>[EN]</b> Computes the intersection point between two segments [AB] and [CD]
        /// in the XY plane, using the cross product of their directions to detect collision.</para>
        /// </summary>
        /// <param name="segmentAStart">
        /// <b>[FR]</b> Première extrémité du segment AB. /
        /// <b>[EN]</b> First endpoint of segment AB.
        /// </param>
        /// <param name="segmentAEnd">
        /// <b>[FR]</b> Seconde extrémité du segment AB. /
        /// <b>[EN]</b> Second endpoint of segment AB.
        /// </param>
        /// <param name="segmentBStart">
        /// <b>[FR]</b> Première extrémité du segment CD. /
        /// <b>[EN]</b> First endpoint of segment CD.
        /// </param>
        /// <param name="segmentBEnd">
        /// <b>[FR]</b> Seconde extrémité du segment CD. /
        /// <b>[EN]</b> Second endpoint of segment CD.
        /// </param>
        /// <param name="intersection">
        /// <para><b>[FR]</b> Point d'intersection calculé, ou <c>null</c> si les segments sont
        /// parallèles ou ne se croisent pas.</para>
        /// <para><b>[EN]</b> Computed intersection point, or <c>null</c> if the segments are
        /// parallel or do not cross.</para>
        /// </param>
        /// <returns>
        /// <b>[FR]</b> <c>0</c> si les segments sont parallèles ou ne se croisent pas ; <c>1</c> sinon. /
        /// <b>[EN]</b> <c>0</c> if segments are parallel or do not intersect; <c>1</c> otherwise.
        /// </returns>
        public static int SegmentSegmentIntersection(
            Point segmentAStart, Point segmentAEnd,
            Point segmentBStart, Point segmentBEnd,
            out Point? intersection)
        {
            intersection = null;

            double ax = segmentAStart.X, ay = segmentAStart.Y;
            double bx = segmentAEnd.X, by = segmentAEnd.Y;
            double cx = segmentBStart.X, cy = segmentBStart.Y;
            double dx = segmentBEnd.X, dy = segmentBEnd.Y;

            double[] ab = new double[2] { bx - ax, by - ay };
            double[] cd = new double[2] { dx - cx, dy - cy };
            double crossProduct = ab[0] * cd[1] - cd[0] * ab[1];

            // Vecteurs colinéaires : pas d'intersection unique
            // Collinear vectors: no unique intersection
            if (crossProduct == 0) return 0;

            bool isDenominatorPositive = crossProduct > 0;

            double[] ca = new double[2] { ax - cx, ay - cy };
            double sNumerator = ab[0] * ca[1] - ca[0] * ab[1];
            if ((sNumerator < 0) == isDenominatorPositive) return 0;

            double tNumerator = cd[0] * ca[1] - ca[0] * cd[1];
            if ((tNumerator < 0) == isDenominatorPositive) return 0;

            if (((sNumerator > crossProduct) == isDenominatorPositive) ||
                ((tNumerator > crossProduct) == isDenominatorPositive)) return 0;

            double t = tNumerator / crossProduct;

            intersection = new Point(
                ax + t * ab[0],
                ay + t * ab[1],
                0
            );

            return 1;
        }

        #endregion
    }
}