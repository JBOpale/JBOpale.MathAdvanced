// SPDX-License-Identifier: MIT
// Copyright (C) 2026 Jérôme FRANCOIS / JB-Opale — voir LICENSE / see LICENSE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBOpale.MathAdvanced
{
    /// <summary>
    /// <para><b>[FR]</b> Représente un point tridimensionnel (X, Y, Z) dans l'espace euclidien.
    /// Les comparaisons d'égalité sont effectuées avec un arrondi à 3 décimales.</para>
    /// <para><b>[EN]</b> Represents a three-dimensional point (X, Y, Z) in Euclidean space.
    /// Equality comparisons are performed with rounding to 3 decimal places.</para>
    /// </summary>
    public class Point
    {
        /// <summary>
        /// <para><b>[FR]</b> Coordonnée X (abscisse) du point.</para>
        /// <para><b>[EN]</b> X coordinate (abscissa) of the point.</para>
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// <para><b>[FR]</b> Coordonnée Y (ordonnée) du point.</para>
        /// <para><b>[EN]</b> Y coordinate (ordinate) of the point.</para>
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// <para><b>[FR]</b> Coordonnée Z (côte) du point.</para>
        /// <para><b>[EN]</b> Z coordinate (elevation) of the point.</para>
        /// </summary>
        public double Z { get; set; }

        #region Constructors / Constructeurs

        /// <summary>
        /// <para><b>[FR]</b> Initialise un point à l'origine (0, 0, 0).</para>
        /// <para><b>[EN]</b> Initializes a point at the origin (0, 0, 0).</para>
        /// </summary>
        public Point()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        /// <summary>
        /// <para><b>[FR]</b> Initialise un point à partir de ses trois coordonnées.</para>
        /// <para><b>[EN]</b> Initializes a point from its three coordinates.</para>
        /// </summary>
        /// <param name="x">
        /// <b>[FR]</b> Coordonnée X. /
        /// <b>[EN]</b> X coordinate.
        /// </param>
        /// <param name="y">
        /// <b>[FR]</b> Coordonnée Y. /
        /// <b>[EN]</b> Y coordinate.
        /// </param>
        /// <param name="z">
        /// <b>[FR]</b> Coordonnée Z. /
        /// <b>[EN]</b> Z coordinate.
        /// </param>
        public Point(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// <para><b>[FR]</b> Initialise un point à partir d'un tableau de trois doubles [X, Y, Z].</para>
        /// <para><b>[EN]</b> Initializes a point from an array of three doubles [X, Y, Z].</para>
        /// </summary>
        /// <param name="coordinates">
        /// <b>[FR]</b> Tableau de doubles de longueur exactement 3. /
        /// <b>[EN]</b> Array of doubles of length exactly 3.
        /// </param>
        /// <exception cref="FormatException">
        /// <b>[FR]</b> Levée si le tableau ne contient pas exactement 3 éléments. /
        /// <b>[EN]</b> Thrown if the array does not contain exactly 3 elements.
        /// </exception>
        public Point(double[] coordinates)
        {
            if (coordinates.Length != 3)
                throw new FormatException("Le tableau de point \"coordinates\" doit avoir 3 valeurs. / The coordinates array must have exactly 3 values.");
            this.X = coordinates[0];
            this.Y = coordinates[1];
            this.Z = coordinates[2];
        }

        #endregion

        #region Implicit Conversions / Conversions implicites

        /// <summary>
        /// <para><b>[FR]</b> Conversion implicite d'un <see cref="Point"/> vers un tableau de
        /// trois doubles [X, Y, Z].</para>
        /// <para><b>[EN]</b> Implicitly converts a <see cref="Point"/> to an array of
        /// three doubles [X, Y, Z].</para>
        /// </summary>
        /// <param name="point">
        /// <b>[FR]</b> Le point à convertir. /
        /// <b>[EN]</b> The point to convert.
        /// </param>
        /// <returns>
        /// <b>[FR]</b> Tableau <c>double[]</c> de longueur 3 : [X, Y, Z]. /
        /// <b>[EN]</b> <c>double[]</c> array of length 3: [X, Y, Z].
        /// </returns>
        public static implicit operator double[](Point? point)

        {
            if (point is null) return new double[3] { 0, 0, 0 };
            return new double[3] { point.X, point.Y, point.Z };
        }

        /// <summary>
        /// <para><b>[FR]</b> Conversion implicite d'un tableau de doubles vers un <see cref="Point"/>.
        /// Si le tableau contient moins de 3 éléments, Z est initialisé à 0.</para>
        /// <para><b>[EN]</b> Implicitly converts an array of doubles to a <see cref="Point"/>.
        /// If the array has fewer than 3 elements, Z defaults to 0.</para>
        /// </summary>
        /// <param name="coordinates">
        /// <b>[FR]</b> Tableau de doubles de longueur 2 ou 3. /
        /// <b>[EN]</b> Array of doubles of length 2 or 3.
        /// </param>
        /// <returns>
        /// <b>[FR]</b> Nouveau <see cref="Point"/> initialisé avec les valeurs du tableau. /
        /// <b>[EN]</b> New <see cref="Point"/> initialized from the array values.
        /// </returns>
        public static implicit operator Point(double[] coordinates)
        {
            return new Point
            {
                X = coordinates[0],
                Y = coordinates[1],
                Z = coordinates.Length == 3 ? coordinates[2] : 0
            };
        }

        #endregion

        #region Operators / Opérateurs

        /// <summary>
        /// <para><b>[FR]</b> Détermine si deux points sont égaux, avec un arrondi à 3 décimales
        /// sur chaque coordonnée. Gère les cas où l'un ou les deux opérandes sont <c>null</c>.</para>
        /// <para><b>[EN]</b> Determines whether two points are equal, rounding each coordinate to
        /// 3 decimal places. Handles cases where one or both operands are <c>null</c>.</para>
        /// </summary>
        /// <param name="a"><b>[FR]</b> Premier point. / <b>[EN]</b> First point.</param>
        /// <param name="b"><b>[FR]</b> Second point. / <b>[EN]</b> Second point.</param>
        /// <returns>
        /// <para><b>[FR]</b> <c>true</c> si les deux références sont nulles, ou si X, Y et Z sont
        /// égaux à 3 décimales près ; <c>false</c> sinon.</para>
        /// <para><b>[EN]</b> <c>true</c> if both references are null, or if X, Y and Z are equal
        /// to 3 decimal places; <c>false</c> otherwise.</para>
        /// </returns>
        public static bool operator ==(Point a, Point b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;

            return
                System.Math.Round(a.X, 3) == System.Math.Round(b.X, 3) &&
                System.Math.Round(a.Y, 3) == System.Math.Round(b.Y, 3) &&
                System.Math.Round(a.Z, 3) == System.Math.Round(b.Z, 3);
        }

        /// <summary>
        /// <para><b>[FR]</b> Détermine si deux points sont différents, avec un arrondi à 3 décimales.
        /// Gère les cas où l'un ou les deux opérandes sont <c>null</c>.</para>
        /// <para><b>[EN]</b> Determines whether two points are different, rounding to 3 decimal places.
        /// Handles cases where one or both operands are <c>null</c>.</para>
        /// </summary>
        /// <param name="a"><b>[FR]</b> Premier point. / <b>[EN]</b> First point.</param>
        /// <param name="b"><b>[FR]</b> Second point. / <b>[EN]</b> Second point.</param>
        /// <returns>
        /// <para><b>[FR]</b> <c>true</c> si au moins une coordonnée diffère (à 3 décimales près)
        /// ou si exactement l'un des deux est <c>null</c> ; <c>false</c> sinon.</para>
        /// <para><b>[EN]</b> <c>true</c> if at least one coordinate differs (to 3 decimal places)
        /// or if exactly one is <c>null</c>; <c>false</c> otherwise.</para>
        /// </returns>
        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }

        /// <summary>
        /// <para><b>[FR]</b> Détermine si ce point est égal à un autre objet.
        /// La comparaison s'effectue avec un arrondi à 3 décimales sur X, Y et Z.</para>
        /// <para><b>[EN]</b> Determines whether this point is equal to another object.
        /// Comparison is performed with rounding to 3 decimal places on X, Y and Z.</para>
        /// </summary>
        /// <param name="obj">
        /// <b>[FR]</b> Objet à comparer ; doit être un <see cref="Point"/>. /
        /// <b>[EN]</b> Object to compare; must be a <see cref="Point"/>.
        /// </param>
        /// <returns>
        /// <para><b>[FR]</b> <c>true</c> si <paramref name="obj"/> est un <see cref="Point"/> dont
        /// les coordonnées sont égales à celles de cette instance (à 3 décimales près).</para>
        /// <para><b>[EN]</b> <c>true</c> if <paramref name="obj"/> is a <see cref="Point"/> whose
        /// coordinates equal those of this instance (to 3 decimal places).</para>
        /// </returns>
        public override bool Equals(Object? obj)
        {
            if (obj == null || !(obj is Point)) return false;

            Point other = (Point)obj;
            return
                System.Math.Round(X, 3) == System.Math.Round(other.X, 3) &&
                System.Math.Round(Y, 3) == System.Math.Round(other.Y, 3) &&
                System.Math.Round(Z, 3) == System.Math.Round(other.Z, 3);
        }

        /// <summary>
        /// <para><b>[FR]</b> Retourne un code de hachage basé sur le produit des coordonnées X, Y et Z.</para>
        /// <para><b>[EN]</b> Returns a hash code based on the product of the X, Y and Z coordinates.</para>
        /// </summary>
        /// <returns>
        /// <b>[FR]</b> Code de hachage entier. /
        /// <b>[EN]</b> Integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return (int)(X * Y * Z);
        }

        #endregion
    }
}