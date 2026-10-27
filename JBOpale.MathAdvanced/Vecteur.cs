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
    /// <para><b>[FR]</b> Représente un vecteur tridimensionnel (X, Y, Z) dans l'espace euclidien.
    /// La norme est automatiquement recalculée à chaque modification d'une composante.</para>
    /// <para><b>[EN]</b> Represents a three-dimensional vector (X, Y, Z) in Euclidean space.
    /// The norm is automatically recomputed whenever a component is modified.</para>
    /// </summary>
    public class Vecteur : System.Object
    {
        private double _X = 0.0;

        /// <summary>
        /// <para><b>[FR]</b> Composante X (abscisse) du vecteur.
        /// La modification de cette propriété recalcule automatiquement la norme.</para>
        /// <para><b>[EN]</b> X component (abscissa) of the vector.
        /// Modifying this property automatically recomputes the norm.</para>
        /// </summary>
        public double X
        {
            get { return _X; }
            set { _X = value; ComputeNorm(); }
        }

        private double _Y = 0.0;

        /// <summary>
        /// <para><b>[FR]</b> Composante Y (ordonnée) du vecteur.
        /// La modification de cette propriété recalcule automatiquement la norme.</para>
        /// <para><b>[EN]</b> Y component (ordinate) of the vector.
        /// Modifying this property automatically recomputes the norm.</para>
        /// </summary>
        public double Y
        {
            get { return _Y; }
            set { _Y = value; ComputeNorm(); }
        }

        private double _Z = 0.0;

        /// <summary>
        /// <para><b>[FR]</b> Composante Z (côte) du vecteur.
        /// La modification de cette propriété recalcule automatiquement la norme.</para>
        /// <para><b>[EN]</b> Z component (elevation) of the vector.
        /// Modifying this property automatically recomputes the norm.</para>
        /// </summary>
        public double Z
        {
            get { return _Z; }
            set { _Z = value; ComputeNorm(); }
        }

        /// <summary>
        /// <para><b>[FR]</b> Norme euclidienne du vecteur : sqrt(X² + Y² + Z²).
        /// Recalculée automatiquement à chaque modification de X, Y ou Z.</para>
        /// <para><b>[EN]</b> Euclidean norm of the vector: sqrt(X² + Y² + Z²).
        /// Automatically recomputed whenever X, Y or Z is modified.</para>
        /// </summary>
        public double Norm { get; private set; }

        #region Constructors / Constructeurs

        /// <summary>
        /// <para><b>[FR]</b> Initialise un vecteur nul (0, 0, 0).</para>
        /// <para><b>[EN]</b> Initializes a zero vector (0, 0, 0).</para>
        /// </summary>
        public Vecteur()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }

        /// <summary>
        /// <para><b>[FR]</b> Initialise un vecteur à partir de ses trois composantes.</para>
        /// <para><b>[EN]</b> Initializes a vector from its three components.</para>
        /// </summary>
        /// <param name="x">
        /// <b>[FR]</b> Composante X. /
        /// <b>[EN]</b> X component.
        /// </param>
        /// <param name="y">
        /// <b>[FR]</b> Composante Y. /
        /// <b>[EN]</b> Y component.
        /// </param>
        /// <param name="z">
        /// <b>[FR]</b> Composante Z. /
        /// <b>[EN]</b> Z component.
        /// </param>
        /// <param name="normalize">
        /// <para><b>[FR]</b> Si <c>true</c>, le vecteur est normalisé (norme unitaire) lors de la
        /// construction, à condition que sa norme soit non nulle.</para>
        /// <para><b>[EN]</b> If <c>true</c>, the vector is normalized (unit length) during
        /// construction, provided its norm is non-zero.</para>
        /// </param>
        public Vecteur(double x, double y, double z, bool normalize = false)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            double currentNorm = Norm;
            if (normalize && currentNorm != 0)
            {
                this.X = x / currentNorm;
                this.Y = y / currentNorm;
                this.Z = z / currentNorm;
            }
        }

        /// <summary>
        /// <para><b>[FR]</b> Initialise un vecteur à partir d'un tableau de trois doubles [X, Y, Z].</para>
        /// <para><b>[EN]</b> Initializes a vector from an array of three doubles [X, Y, Z].</para>
        /// </summary>
        /// <param name="components">
        /// <b>[FR]</b> Tableau de doubles de taille 3 : [X, Y, Z]. /
        /// <b>[EN]</b> Array of three doubles: [X, Y, Z].
        /// </param>
        public Vecteur(double[] components)
        {
            this.X = components[0];
            this.Y = components[1];
            this.Z = components[2];
        }

        /// <summary>
        /// <para><b>[FR]</b> Initialise un vecteur à partir d'un tableau 2D de doubles.
        /// Seule la première ligne est utilisée : [0,0]=X, [0,1]=Y, [0,2]=Z.</para>
        /// <para><b>[EN]</b> Initializes a vector from a 2D double array.
        /// Only the first row is used: [0,0]=X, [0,1]=Y, [0,2]=Z.</para>
        /// </summary>
        /// <param name="components">
        /// <b>[FR]</b> Tableau 2D (au moins 1 ligne, 3 colonnes). /
        /// <b>[EN]</b> 2D array (at least 1 row, 3 columns).
        /// </param>
        public Vecteur(double[,] components)
        {
            this.X = components[0, 0];
            this.Y = components[0, 1];
            this.Z = components[0, 2];
        }

        /// <summary>
        /// <para><b>[FR]</b> Initialise un vecteur à partir des coordonnées d'un <see cref="Point"/>.</para>
        /// <para><b>[EN]</b> Initializes a vector from the coordinates of a <see cref="Point"/>.</para>
        /// </summary>
        /// <param name="point">
        /// <b>[FR]</b> Point source dont les coordonnées (X, Y, Z) définissent le vecteur. /
        /// <b>[EN]</b> Source point whose coordinates (X, Y, Z) define the vector.
        /// </param>
        public Vecteur(Point point)
        {
            this.X = point.X;
            this.Y = point.Y;
            this.Z = point.Z;
        }

        /// <summary>
        /// <para><b>[FR]</b> Initialise un vecteur comme le vecteur directeur allant du point
        /// <paramref name="origin"/> vers le point <paramref name="target"/> (target − origin).</para>
        /// <para><b>[EN]</b> Initializes a vector as the direction vector from
        /// <paramref name="origin"/> to <paramref name="target"/> (target − origin).</para>
        /// </summary>
        /// <param name="origin">
        /// <b>[FR]</b> Point d'origine. /
        /// <b>[EN]</b> Origin point.
        /// </param>
        /// <param name="target">
        /// <b>[FR]</b> Point d'arrivée. /
        /// <b>[EN]</b> Target point.
        /// </param>
        /// <param name="normalize">
        /// <para><b>[FR]</b> Si <c>true</c>, le vecteur résultant est normalisé à condition que
        /// sa norme soit non nulle.</para>
        /// <para><b>[EN]</b> If <c>true</c>, the resulting vector is normalized provided its
        /// norm is non-zero.</para>
        /// </param>
        public Vecteur(Point origin, Point target, bool normalize = false)
        {
            this.X = target.X - origin.X;
            this.Y = target.Y - origin.Y;
            this.Z = target.Z - origin.Z;
            double currentNorm = Norm;
            if (normalize && currentNorm != 0)
            {
                this.X = (target.X - origin.X) / currentNorm;
                this.Y = (target.Y - origin.Y) / currentNorm;
                this.Z = (target.Z - origin.Z) / currentNorm;
            }
        }

        #endregion

        #region Public Methods / Méthodes publiques

        /// <summary>
        /// <para><b>[FR]</b> Normalise ce vecteur en place : divise chaque composante par la norme
        /// courante. Sans effet si la norme est nulle.</para>
        /// <para><b>[EN]</b> Normalizes this vector in place by dividing each component by the
        /// current norm. Has no effect if the norm is zero.</para>
        /// </summary>
        public void Normalize()
        {
            double currentNorm = Norm;
            if (currentNorm != 0)
            {
                this.X = X / currentNorm;
                this.Y = Y / currentNorm;
                this.Z = Z / currentNorm;
            }
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le produit scalaire entre ce vecteur et un autre vecteur.</para>
        /// <para><b>[EN]</b> Computes the dot product between this vector and another vector.</para>
        /// </summary>
        /// <param name="other">
        /// <b>[FR]</b> Vecteur avec lequel effectuer le produit scalaire. /
        /// <b>[EN]</b> Vector with which to compute the dot product.
        /// </param>
        /// <returns>
        /// <b>[FR]</b> Scalaire X·other.X + Y·other.Y + Z·other.Z. /
        /// <b>[EN]</b> Scalar X·other.X + Y·other.Y + Z·other.Z.
        /// </returns>
        public double Dot(Vecteur other)
        {
            return X * other.X + Y * other.Y + Z * other.Z;
        }

        #endregion

        #region Private Methods / Méthodes privées

        /// <summary>
        /// <para><b>[FR]</b> Recalcule la propriété <see cref="Norm"/> : sqrt(X² + Y² + Z²).
        /// Appelée automatiquement lors de la modification de X, Y ou Z.</para>
        /// <para><b>[EN]</b> Recomputes the <see cref="Norm"/> property: sqrt(X² + Y² + Z²).
        /// Called automatically when X, Y or Z is modified.</para>
        /// </summary>
        private void ComputeNorm()
        {
            Norm = System.Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        #endregion

        #region Operators / Opérateurs

        /// <summary>
        /// <para><b>[FR]</b> Additionne deux vecteurs composante par composante.</para>
        /// <para><b>[EN]</b> Adds two vectors component by component.</para>
        /// </summary>
        /// <param name="a"><b>[FR]</b> Premier vecteur. / <b>[EN]</b> First vector.</param>
        /// <param name="b"><b>[FR]</b> Second vecteur. / <b>[EN]</b> Second vector.</param>
        /// <returns>
        /// <b>[FR]</b> Vecteur (a.X+b.X, a.Y+b.Y, a.Z+b.Z). /
        /// <b>[EN]</b> Vector (a.X+b.X, a.Y+b.Y, a.Z+b.Z).
        /// </returns>
        public static Vecteur operator +(Vecteur a, Vecteur b)
        {
            return new Vecteur(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        /// <summary>
        /// <para><b>[FR]</b> Multiplie un vecteur par un scalaire.</para>
        /// <para><b>[EN]</b> Multiplies a vector by a scalar.</para>
        /// </summary>
        /// <param name="a"><b>[FR]</b> Vecteur à multiplier. / <b>[EN]</b> Vector to scale.</param>
        /// <param name="scalar"><b>[FR]</b> Facteur d'échelle. / <b>[EN]</b> Scalar factor.</param>
        /// <returns>
        /// <b>[FR]</b> Vecteur (a.X·scalar, a.Y·scalar, a.Z·scalar). /
        /// <b>[EN]</b> Vector (a.X·scalar, a.Y·scalar, a.Z·scalar).
        /// </returns>
        public static Vecteur operator *(Vecteur a, double scalar)
        {
            return new Vecteur(a.X * scalar, a.Y * scalar, a.Z * scalar);
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le produit vectoriel (cross product) : a ∧ b.</para>
        /// <para><b>[EN]</b> Computes the cross product: a ∧ b.</para>
        /// </summary>
        /// <param name="a"><b>[FR]</b> Premier vecteur. / <b>[EN]</b> First vector.</param>
        /// <param name="b"><b>[FR]</b> Second vecteur. / <b>[EN]</b> Second vector.</param>
        /// <returns>
        /// <para><b>[FR]</b> Vecteur perpendiculaire aux deux opérandes, de norme |a|·|b|·sin(θ) :
        /// (a.Y·b.Z − a.Z·b.Y, a.Z·b.X − a.X·b.Z, a.X·b.Y − a.Y·b.X).</para>
        /// <para><b>[EN]</b> Vector perpendicular to both operands, with magnitude |a|·|b|·sin(θ):
        /// (a.Y·b.Z − a.Z·b.Y, a.Z·b.X − a.X·b.Z, a.X·b.Y − a.Y·b.X).</para>
        /// </returns>
        public static Vecteur operator ^(Vecteur a, Vecteur b)
        {
            return new Vecteur(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
            );
        }

        /// <summary>
        /// <para><b>[FR]</b> Détermine si deux vecteurs sont égaux (mêmes composantes X, Y et Z).</para>
        /// <para><b>[EN]</b> Determines whether two vectors are equal (same X, Y and Z components).</para>
        /// </summary>
        public static bool operator ==(Vecteur a, Vecteur b)
        {
            // ReferenceEquals : évite la récursion de « == null » et une NullReferenceException.
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        /// <summary>
        /// <para><b>[FR]</b> Détermine si deux vecteurs sont différents.</para>
        /// <para><b>[EN]</b> Determines whether two vectors are different.</para>
        /// </summary>
        public static bool operator !=(Vecteur a, Vecteur b)
        {
            return !(a == b);
        }

        /// <inheritdoc/>
        public override bool Equals(System.Object? obj)
        {
            if (obj == null || !(obj is Vecteur)) return false;
            Vecteur b = (Vecteur)obj;
            return this.X == b.X && this.Y == b.Y && this.Z == b.Z;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return (int)(X * Y * Z);
        }

        #endregion
    }
}