// SPDX-License-Identifier: LGPL-3.0-or-later
// Copyright (C) 2026 Jérôme FRANCOIS / JB-Opale — voir LICENSE.md / see LICENSE.md
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Diagnostics;
#pragma warning disable CS8600 // Conversion d'une valeur null possible en type non-nullable
#pragma warning disable CS8602 // Déréférencement d'une éventuelle référence null
#pragma warning disable CS8603 // Retour d'une éventuelle référence null
#pragma warning disable CS8604 // Argument de référence null possible
#pragma warning disable CS8605 // Unboxing d'une valeur null possible

namespace JBOPaleAPI.MathAdvanced
{
    /// <summary>
    /// <para><b>[FR]</b> Représente une matrice de nombres complexes de dimensions m × n.
    /// Fournit les opérations algébriques linéaires classiques : décompositions LU, QR, Cholesky,
    /// calcul de valeurs propres, résolution de systèmes, algorithmes de graphes et plus encore.
    /// Les indices sont en base 1 (à partir de 1).</para>
    /// <para><b>[EN]</b> Represents a matrix of complex numbers of dimensions m × n.
    /// Provides classic linear algebra operations: LU, QR, Cholesky decompositions,
    /// eigenvalue computation, system solving, graph algorithms and more.
    /// Indices are 1-based.</para>
    /// </summary>
    public class Matrix
    {
        /// <summary>
        /// <para><b>[FR]</b> Stockage interne des lignes de la matrice sous forme de liste de listes.</para>
        /// <para><b>[EN]</b> Internal storage of matrix rows as a list of lists.</para>
        /// </summary>
        private ArrayList Values;

        /// <summary>
        /// <para><b>[FR]</b> Nombre de lignes de la matrice.</para>
        /// <para><b>[EN]</b> Number of rows in the matrix.</para>
        /// </summary>
        public int RowCount
        {
            get { return rowCount; }
        }

        /// <summary>
        /// <para><b>[FR]</b> Nombre de colonnes de la matrice.</para>
        /// <para><b>[EN]</b> Number of columns in the matrix.</para>
        /// </summary>
        public int ColumnCount
        {
            get { return columnCount; }
        }

        private int rowCount;
        private int columnCount;

        #region Constructors / Constructeurs

        /// <summary>
        /// <para><b>[FR]</b> Initialise une matrice vide (0 × 0).</para>
        /// <para><b>[EN]</b> Initializes an empty matrix (0 × 0).</para>
        /// </summary>
        public Matrix()
        {
            Values = new ArrayList();
            rowCount = 0;
            columnCount = 0;
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice m × n remplie de zéros. Équivalent à <see cref="Zeros(int, int)"/>.</para>
        /// <para><b>[EN]</b> Creates an m × n matrix filled with zeros. Equivalent to <see cref="Zeros(int, int)"/>.</para>
        /// </summary>
        /// <param name="m">
        /// <b>[FR]</b> Nombre de lignes. /
        /// <b>[EN]</b> Number of rows.
        /// </param>
        /// <param name="n">
        /// <b>[FR]</b> Nombre de colonnes. /
        /// <b>[EN]</b> Number of columns.
        /// </param>
        public Matrix(int m, int n)
        {
            rowCount = m;
            columnCount = n;
            Values = new ArrayList(m);
            for (int i = 0; i < m; i++)
            {
                Values.Add(new ArrayList(n));
                for (int j = 0; j < n; j++)
                    ((ArrayList)Values[i]).Add(Complex.Zero);
            }
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice carrée n × n remplie de zéros.</para>
        /// <para><b>[EN]</b> Creates an n × n square matrix filled with zeros.</para>
        /// </summary>
        /// <param name="n">
        /// <b>[FR]</b> Dimension (nombre de lignes = nombre de colonnes). /
        /// <b>[EN]</b> Dimension (number of rows = number of columns).
        /// </param>
        public Matrix(int n)
        {
            rowCount = n;
            columnCount = n;
            Values = new ArrayList(n);
            for (int i = 0; i < n; i++)
            {
                Values.Add(new ArrayList(n));
                for (int j = 0; j < n; j++)
                    ((ArrayList)Values[i]).Add(Complex.Zero);
            }
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice 1 × 1 contenant le complexe <paramref name="x"/>.</para>
        /// <para><b>[EN]</b> Creates a 1 × 1 matrix containing the complex number <paramref name="x"/>.</para>
        /// </summary>
        /// <param name="x">
        /// <b>[FR]</b> Valeur complexe. /
        /// <b>[EN]</b> Complex value.
        /// </param>
        public Matrix(Complex x)
        {
            rowCount = 1;
            columnCount = 1;
            Values = new ArrayList(1);
            Values.Add(new ArrayList(1));
            ((ArrayList)Values[0]).Add(x);
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice à partir d'un tableau 2D de complexes.</para>
        /// <para><b>[EN]</b> Creates a matrix from a 2D complex array.</para>
        /// </summary>
        /// <param name="values">
        /// <b>[FR]</b> Tableau 2D [m, n] de nombres complexes. /
        /// <b>[EN]</b> 2D [m, n] array of complex numbers.
        /// </param>
        public Matrix(Complex[,] values)
        {
            if (values == null) { Values = new ArrayList(); columnCount = 0; rowCount = 0; }
            rowCount = (int)values.GetLongLength(0);
            columnCount = (int)values.GetLongLength(1);
            Values = new ArrayList(rowCount);
            for (int i = 0; i < rowCount; i++)
            {
                Values.Add(new ArrayList(columnCount));
                for (int j = 0; j < columnCount; j++)
                    ((ArrayList)Values[i]).Add(values[i, j]);
            }
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée un vecteur colonne à partir d'un tableau de complexes.</para>
        /// <para><b>[EN]</b> Creates a column vector from a complex array.</para>
        /// </summary>
        /// <param name="values">
        /// <b>[FR]</b> Tableau de nombres complexes. /
        /// <b>[EN]</b> Array of complex numbers.
        /// </param>
        public Matrix(Complex[] values)
        {
            if (values == null) { Values = new ArrayList(); columnCount = 0; rowCount = 0; }
            rowCount = values.Length;
            columnCount = 1;
            Values = new ArrayList(rowCount);
            for (int i = 0; i < rowCount; i++)
            {
                Values.Add(new ArrayList(1));
                ((ArrayList)Values[i]).Add(values[i]);
            }
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice 1 × 1 contenant le réel <paramref name="x"/>.</para>
        /// <para><b>[EN]</b> Creates a 1 × 1 matrix containing the real number <paramref name="x"/>.</para>
        /// </summary>
        /// <param name="x">
        /// <b>[FR]</b> Valeur réelle. /
        /// <b>[EN]</b> Real value.
        /// </param>
        public Matrix(double x)
        {
            rowCount = 1;
            columnCount = 1;
            Values = new ArrayList(1);
            Values.Add(new ArrayList(1));
            ((ArrayList)Values[0]).Add(new Complex(x));
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice à partir d'un tableau 2D de réels.</para>
        /// <para><b>[EN]</b> Creates a matrix from a 2D double array.</para>
        /// </summary>
        /// <param name="values">
        /// <b>[FR]</b> Tableau 2D [m, n] de réels. /
        /// <b>[EN]</b> 2D [m, n] array of doubles.
        /// </param>
        public Matrix(double[,] values)
        {
            if (values == null) { Values = new ArrayList(); columnCount = 0; rowCount = 0; }
            rowCount = (int)values.GetLongLength(0);
            columnCount = (int)values.GetLongLength(1);
            Values = new ArrayList(rowCount);
            for (int i = 0; i < rowCount; i++)
            {
                Values.Add(new ArrayList(columnCount));
                for (int j = 0; j < columnCount; j++)
                    ((ArrayList)Values[i]).Add(new Complex(values[i, j]));
            }
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée un vecteur colonne à partir d'un tableau de réels.</para>
        /// <para><b>[EN]</b> Creates a column vector from a double array.</para>
        /// </summary>
        /// <param name="values">
        /// <b>[FR]</b> Tableau de réels. /
        /// <b>[EN]</b> Array of doubles.
        /// </param>
        public Matrix(double[] values)
        {
            if (values == null) { Values = new ArrayList(); columnCount = 0; rowCount = 0; }
            rowCount = values.Length;
            columnCount = 1;
            Values = new ArrayList(rowCount);
            for (int i = 0; i < rowCount; i++)
            {
                Values.Add(new ArrayList(1));
                ((ArrayList)Values[i]).Add(new Complex(values[i]));
            }
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice réelle à partir d'une chaîne encodée.
        /// Exemple : "1,0;0,1" produit la matrice identité 2 × 2.
        /// Les lignes sont séparées par ';', les colonnes par ','.</para>
        /// <para><b>[EN]</b> Creates a real matrix from an encoded string.
        /// Example: "1,0;0,1" produces the 2 × 2 identity matrix.
        /// Rows are separated by ';', columns by ','.</para>
        /// </summary>
        /// <param name="matrixString">
        /// <b>[FR]</b> Chaîne encodant la matrice. /
        /// <b>[EN]</b> String encoding the matrix.
        /// </param>
        public Matrix(string matrixString)
        {
            matrixString = matrixString.Replace(" ", "");
            string[] rows = matrixString.Split(new char[] { ';' });
            rowCount = rows.Length;
            Values = new ArrayList(rowCount);
            columnCount = 0;
            for (int i = 0; i < rowCount; i++)
                Values.Add(new ArrayList());

            string[] curcol;
            for (int i = 1; i <= rowCount; i++)
            {
                curcol = rows[i - 1].Split(new char[] { ',' });
                for (int j = 1; j <= curcol.Length; j++)
                    this[i, j] = new Complex(Convert.ToDouble(curcol[j - 1], CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// <para><b>[FR]</b> Matrice identité 4 × 4 préconstruite.</para>
        /// <para><b>[EN]</b> Pre-built 4 × 4 identity matrix.</para>
        /// </summary>
        public static Matrix Identity4x4
        {
            get
            {
                return new Matrix(new double[,]
                {
                    { 1, 0, 0, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 0, 0, 1 }
                });
            }
        }

        /// <summary>
        /// <para><b>[FR]</b> Matrice identité 3 × 3 préconstruite.</para>
        /// <para><b>[EN]</b> Pre-built 3 × 3 identity matrix.</para>
        /// </summary>
        public static Matrix Identity3x3
        {
            get
            {
                return new Matrix(new double[,]
                {
                    { 1, 0, 0 },
                    { 0, 1, 0 },
                    { 0, 0, 1 }
                });
            }
        }

        #endregion

        #region Static Methods / Méthodes statiques

        /// <summary>
        /// <para><b>[FR]</b> Retourne le j-ième vecteur de la base canonique de ℝⁿ
        /// (vecteur colonne avec 1 en position j, 0 ailleurs).</para>
        /// <para><b>[EN]</b> Returns the j-th canonical basis vector of ℝⁿ
        /// (column vector with 1 at position j, 0 elsewhere).</para>
        /// </summary>
        /// <param name="n">
        /// <b>[FR]</b> Dimension de l'espace. /
        /// <b>[EN]</b> Space dimension.
        /// </param>
        /// <param name="j">
        /// <b>[FR]</b> Indice (base 1) du vecteur de base. /
        /// <b>[EN]</b> One-based index of the basis vector.
        /// </param>
        public static Matrix CanonicalBasisVector(int n, int j)
        {
            Matrix e = Zeros(n, 1);
            e[j] = Complex.One;
            return e;
        }

        /// <summary>
        /// <para><b>[FR]</b> Delta de Kronecker : retourne 1 si i = j, 0 sinon.</para>
        /// <para><b>[EN]</b> Kronecker delta: returns 1 if i = j, 0 otherwise.</para>
        /// </summary>
        /// <param name="i"><b>[FR]</b> Premier indice. / <b>[EN]</b> First index.</param>
        /// <param name="j"><b>[FR]</b> Second indice. / <b>[EN]</b> Second index.</param>
        public static Complex KroneckerDelta(int i, int j)
        {
            return new Complex(System.Math.Min(System.Math.Abs(i - j), 1));
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice m × n en damier (alternance de 0 et de 1).</para>
        /// <para><b>[EN]</b> Creates an m × n chessboard matrix (alternating 0s and 1s).</para>
        /// </summary>
        /// <param name="m"><b>[FR]</b> Nombre de lignes. / <b>[EN]</b> Number of rows.</param>
        /// <param name="n"><b>[FR]</b> Nombre de colonnes. / <b>[EN]</b> Number of columns.</param>
        /// <param name="even">
        /// <para><b>[FR]</b> Si <c>true</c>, l'entrée (1,1) vaut 1 ; si <c>false</c>, elle vaut 0.</para>
        /// <para><b>[EN]</b> If <c>true</c>, entry (1,1) equals 1; if <c>false</c>, it equals 0.</para>
        /// </param>
        public static Matrix CreateChessboard(int m, int n, bool even)
        {
            Matrix M = new Matrix(m, n);
            if (even)
                for (int i = 1; i <= m; i++)
                    for (int j = 1; j <= n; j++)
                        M[i, j] = KroneckerDelta((i + j) % 2, 0);
            else
                for (int i = 1; i <= m; i++)
                    for (int j = 1; j <= n; j++)
                        M[i, j] = KroneckerDelta((i + j) % 2, 1);
            return M;
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice carrée n × n en damier (alternance de 0 et de 1).</para>
        /// <para><b>[EN]</b> Creates an n × n square chessboard matrix (alternating 0s and 1s).</para>
        /// </summary>
        /// <param name="n"><b>[FR]</b> Dimension. / <b>[EN]</b> Dimension.</param>
        /// <param name="even">
        /// <para><b>[FR]</b> Si <c>true</c>, l'entrée (1,1) vaut 1.</para>
        /// <para><b>[EN]</b> If <c>true</c>, entry (1,1) equals 1.</para>
        /// </param>
        public static Matrix CreateChessboard(int n, bool even)
        {
            Matrix M = new Matrix(n);
            if (even)
                for (int i = 1; i <= n; i++)
                    for (int j = 1; j <= n; j++)
                        M[i, j] = KroneckerDelta((i + j) % 2, 0);
            else
                for (int i = 1; i <= n; i++)
                    for (int j = 1; j <= n; j++)
                        M[i, j] = KroneckerDelta((i + j) % 2, 1);
            return M;
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice m × n remplie de zéros.</para>
        /// <para><b>[EN]</b> Creates an m × n matrix filled with zeros.</para>
        /// </summary>
        public static Matrix Zeros(int m, int n) { return new Matrix(m, n); }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice carrée n × n remplie de zéros.</para>
        /// <para><b>[EN]</b> Creates an n × n square matrix filled with zeros.</para>
        /// </summary>
        public static Matrix Zeros(int n) { return new Matrix(n); }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice m × n remplie de 1.</para>
        /// <para><b>[EN]</b> Creates an m × n matrix filled with ones.</para>
        /// </summary>
        public static Matrix Ones(int m, int n)
        {
            Matrix M = new Matrix(m, n);
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                    ((ArrayList)M.Values[i])[j] = Complex.One;
            return M;
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice carrée n × n remplie de 1.</para>
        /// <para><b>[EN]</b> Creates an n × n square matrix filled with ones.</para>
        /// </summary>
        public static Matrix Ones(int n)
        {
            Matrix M = new Matrix(n);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    ((ArrayList)M.Values[i])[j] = Complex.One;
            return M;
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée la matrice identité n × n.</para>
        /// <para><b>[EN]</b> Creates the n × n identity matrix.</para>
        /// </summary>
        public static Matrix Identity(int n) { return Diag(Ones(n, 1)); }

        /// <summary>
        /// <para><b>[FR]</b> Alias de <see cref="Identity(int)"/> (convention MATLAB).</para>
        /// <para><b>[EN]</b> Alias for <see cref="Identity(int)"/> (MATLAB convention).</para>
        /// </summary>
        public static Matrix Eye(int n) { return Identity(n); }

        /// <summary>
        /// <para><b>[FR]</b> Concatène deux matrices horizontalement (côte à côte, ajout de colonnes) :
        /// résultat = [A | B].</para>
        /// <para><b>[EN]</b> Concatenates two matrices horizontally (side by side, appending columns):
        /// result = [A | B].</para>
        /// </summary>
        /// <param name="A"><b>[FR]</b> Matrice gauche. / <b>[EN]</b> Left matrix.</param>
        /// <param name="B"><b>[FR]</b> Matrice droite. / <b>[EN]</b> Right matrix.</param>
        /// <returns>
        /// <b>[FR]</b> Matrice [A | B]. /
        /// <b>[EN]</b> Matrix [A | B].
        /// </returns>
        public static Matrix HorizontalStack(Matrix A, Matrix B)
        {
            Matrix C = A.Column(1);
            for (int j = 2; j <= A.ColumnCount; j++)
                C.InsertColumn(A.Column(j), j);
            for (int j = 1; j <= B.ColumnCount; j++)
                C.InsertColumn(B.Column(j), C.ColumnCount + 1);
            return C;
        }

        /// <summary>
        /// <para><b>[FR]</b> Concatène un tableau de matrices horizontalement.</para>
        /// <para><b>[EN]</b> Horizontally concatenates an array of matrices.</para>
        /// </summary>
        public static Matrix HorizontalStack(Matrix[] A)
        {
            if (A == null) throw new ArgumentNullException();
            if (A.Length == 1) return A[0];
            Matrix C = HorizontalStack(A[0], A[1]);
            for (int i = 2; i < A.Length; i++)
                C = HorizontalStack(C, A[i]);
            return C;
        }

        /// <summary>
        /// <para><b>[FR]</b> Concatène deux matrices verticalement (l'une sous l'autre, ajout de lignes) :
        /// résultat = [A ; B].</para>
        /// <para><b>[EN]</b> Concatenates two matrices vertically (one below the other, appending rows):
        /// result = [A ; B].</para>
        /// </summary>
        /// <param name="A"><b>[FR]</b> Matrice du dessus. / <b>[EN]</b> Top matrix.</param>
        /// <param name="B"><b>[FR]</b> Matrice du dessous. / <b>[EN]</b> Bottom matrix.</param>
        public static Matrix VerticalStack(Matrix A, Matrix B)
        {
            Matrix C = A.Row(1);
            for (int i = 2; i <= A.RowCount; i++)
                C.InsertRow(A.Row(i), i);
            for (int i = 1; i <= B.RowCount; i++)
                C.InsertRow(B.Row(i), C.RowCount + 1);
            return C;
        }

        /// <summary>
        /// <para><b>[FR]</b> Concatène un tableau de matrices verticalement.</para>
        /// <para><b>[EN]</b> Vertically concatenates an array of matrices.</para>
        /// </summary>
        public static Matrix VerticalStack(Matrix[] A)
        {
            if (A == null) throw new ArgumentNullException();
            if (A.Length == 1) return A[0];
            Matrix C = VerticalStack(A[0], A[1]);
            for (int i = 2; i < A.Length; i++)
                C = VerticalStack(C, A[i]);
            return C;
        }

        /// <summary>
        /// <para><b>[FR]</b> Génère une matrice diagonale à partir d'un vecteur colonne ou ligne.</para>
        /// <para><b>[EN]</b> Generates a diagonal matrix from a column or row vector.</para>
        /// </summary>
        /// <param name="diagVector">
        /// <b>[FR]</b> Vecteur contenant les éléments diagonaux. /
        /// <b>[EN]</b> Vector containing the diagonal elements.
        /// </param>
        public static Matrix Diag(Matrix diagVector)
        {
            int dim = diagVector.VectorDimension();
            if (dim == 0) throw new ArgumentException("diagVector must be 1×N or N×1.");
            Matrix M = new Matrix(dim, dim);
            for (int i = 1; i <= dim; i++)
                M[i, i] = diagVector[i];
            return M;
        }

        /// <summary>
        /// <para><b>[FR]</b> Génère une matrice diagonale avec décalage à partir d'un vecteur.
        /// Un offset positif place la diagonale au-dessus de la principale,
        /// un offset négatif en dessous.</para>
        /// <para><b>[EN]</b> Generates an offset diagonal matrix from a vector.
        /// A positive offset places the diagonal above the main diagonal,
        /// a negative offset below it.</para>
        /// </summary>
        /// <param name="diagVector">
        /// <b>[FR]</b> Vecteur des éléments diagonaux. /
        /// <b>[EN]</b> Diagonal element vector.
        /// </param>
        /// <param name="offset">
        /// <b>[FR]</b> Décalage de la diagonale (positif = au-dessus, négatif = en dessous). /
        /// <b>[EN]</b> Diagonal offset (positive = above main, negative = below main).
        /// </param>
        public static Matrix Diag(Matrix diagVector, int offset)
        {
            int dim = diagVector.VectorDimension();
            if (dim == 0) throw new ArgumentException("diagVector must be 1×N or N×1.");
            Matrix M = new Matrix(dim + System.Math.Abs(offset), dim + System.Math.Abs(offset));
            dim = M.RowCount;
            if (offset >= 0)
                for (int i = 1; i <= dim - offset; i++)
                    M[i, i + offset] = diagVector[i];
            else
                for (int i = 1; i <= dim + offset; i++)
                    M[i - offset, i] = diagVector[i];
            return M;
        }

        /// <summary>
        /// <para><b>[FR]</b> Génère une matrice tridiagonale carrée n × n à valeurs constantes
        /// sur la diagonale principale et les deux diagonales secondaires.</para>
        /// <para><b>[EN]</b> Generates an n × n square tridiagonal matrix with constant values
        /// on the main diagonal and both secondary diagonals.</para>
        /// </summary>
        /// <param name="lower"><b>[FR]</b> Valeur de la sous-diagonale. / <b>[EN]</b> Lower secondary diagonal value.</param>
        /// <param name="main"><b>[FR]</b> Valeur de la diagonale principale. / <b>[EN]</b> Main diagonal value.</param>
        /// <param name="upper"><b>[FR]</b> Valeur de la sur-diagonale. / <b>[EN]</b> Upper secondary diagonal value.</param>
        /// <param name="n"><b>[FR]</b> Dimension de la matrice. / <b>[EN]</b> Matrix dimension.</param>
        public static Matrix CreateTridiagonal(Complex lower, Complex main, Complex upper, int n)
        {
            if (n <= 1) throw new ArgumentException("Matrix dimension must be greater than one.");
            return Diag(lower * Ones(n - 1, 1), -1) + Diag(main * Ones(n, 1)) + Diag(upper * Ones(n - 1, 1), 1);
        }

        /// <summary>
        /// <para><b>[FR]</b> Génère une matrice tridiagonale à partir de vecteurs pour chaque diagonale.
        /// La dimension est déterminée par la longueur de <paramref name="mainDiag"/>.</para>
        /// <para><b>[EN]</b> Generates a tridiagonal matrix from vectors for each diagonal.
        /// The dimension is determined by the length of <paramref name="mainDiag"/>.</para>
        /// </summary>
        /// <param name="lowerDiag"><b>[FR]</b> Vecteur de la sous-diagonale (longueur n−1). / <b>[EN]</b> Lower secondary diagonal vector (length n−1).</param>
        /// <param name="mainDiag"><b>[FR]</b> Vecteur de la diagonale principale (longueur n). / <b>[EN]</b> Main diagonal vector (length n).</param>
        /// <param name="upperDiag"><b>[FR]</b> Vecteur de la sur-diagonale (longueur n−1). / <b>[EN]</b> Upper secondary diagonal vector (length n−1).</param>
        public static Matrix CreateTridiagonal(Matrix lowerDiag, Matrix mainDiag, Matrix upperDiag)
        {
            int sizeL = lowerDiag.VectorDimension();
            int sizeD = mainDiag.VectorDimension();
            int sizeU = upperDiag.VectorDimension();
            if (sizeL * sizeD * sizeU == 0) throw new ArgumentException("At least one parameter is not a vector.");
            if (sizeL != sizeU) throw new ArgumentException("Lower and upper secondary diagonals must have the same length.");
            if (sizeL + 1 != sizeD) throw new ArgumentException("Main diagonal must have exactly one more element than secondary diagonals.");
            return Diag(lowerDiag, -1) + Diag(mainDiag) + Diag(upperDiag, 1);
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le produit scalaire de deux vecteurs (ligne ou colonne).</para>
        /// <para><b>[EN]</b> Computes the dot product of two vectors (row or column).</para>
        /// </summary>
        /// <param name="v"><b>[FR]</b> Premier vecteur. / <b>[EN]</b> First vector.</param>
        /// <param name="w"><b>[FR]</b> Second vecteur. / <b>[EN]</b> Second vector.</param>
        public static Complex Dot(Matrix v, Matrix w)
        {
            int m = v.VectorDimension();
            int n = w.VectorDimension();
            if (m == 0 || n == 0) throw new ArgumentException("Arguments must be vectors.");
            if (m != n) throw new ArgumentException("Vectors must have the same length.");
            Complex buf = Complex.Zero;
            for (int i = 1; i <= m; i++)
                buf += v[i] * w[i];
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le n-ième nombre de Fibonacci en O(n) via exponentiation matricielle.</para>
        /// <para><b>[EN]</b> Computes the n-th Fibonacci number in O(n) via matrix exponentiation.</para>
        /// </summary>
        /// <param name="n">
        /// <b>[FR]</b> Indice (base 1) du nombre de Fibonacci. /
        /// <b>[EN]</b> One-based index of the Fibonacci number.
        /// </param>
        public static Complex Fibonacci(int n)
        {
            Matrix M = Ones(2, 2);
            M[2, 2] = Complex.Zero;
            return (M ^ (n - 1))[1, 1];
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice de graphe aléatoire n × n avec des valeurs dans [0,1]
        /// et des zéros sur la diagonale principale.</para>
        /// <para><b>[EN]</b> Creates an n × n random graph matrix with values in [0,1]
        /// and zeros on the main diagonal.</para>
        /// </summary>
        public static Matrix CreateRandomGraph(int n)
        {
            Matrix buf = Random(n, n);
            buf -= Diag(buf.DiagonalVector());
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice de graphe aléatoire n × n où une proportion <paramref name="edgeProbability"/>
        /// des arêtes a un poids fini dans [0,1] et les autres valent +∞ (arête absente).
        /// La diagonale vaut 0.</para>
        /// <para><b>[EN]</b> Creates an n × n random graph matrix where a fraction <paramref name="edgeProbability"/>
        /// of edges have a finite weight in [0,1] and the rest are +∞ (missing edge).
        /// The diagonal is 0.</para>
        /// </summary>
        /// <param name="n"><b>[FR]</b> Nombre de sommets. / <b>[EN]</b> Number of vertices.</param>
        /// <param name="edgeProbability">
        /// <b>[FR]</b> Probabilité qu'une arête soit présente (∈ [0,1]). /
        /// <b>[EN]</b> Probability that an edge exists (∈ [0,1]).
        /// </param>
        public static Matrix CreateRandomGraph(int n, double edgeProbability)
        {
            Matrix buf = new Matrix(n);
            Random r = new Random();
            for (int i = 1; i <= n; i++)
                for (int j = 1; j <= n; j++)
                    if (i == j) buf[i, j] = Complex.Zero;
                    else if (r.NextDouble() < edgeProbability) buf[i, j] = new Complex(r.NextDouble());
                    else buf[i, j] = new Complex(double.PositiveInfinity);
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice m × n remplie de valeurs aléatoires dans [0,1].</para>
        /// <para><b>[EN]</b> Creates an m × n matrix filled with random values in [0,1].</para>
        /// </summary>
        public static Matrix Random(int m, int n)
        {
            Matrix M = new Matrix(m, n);
            Random r = new Random();
            for (int i = 1; i <= m; i++)
                for (int j = 1; j <= n; j++)
                    M[i, j] = new Complex(r.NextDouble());
            return M;
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice carrée n × n remplie de valeurs aléatoires dans [0,1].</para>
        /// <para><b>[EN]</b> Creates an n × n square matrix filled with random values in [0,1].</para>
        /// </summary>
        public static Matrix Random(int n)
        {
            Matrix M = new Matrix(n);
            Random r = new Random();
            for (int i = 1; i <= n; i++)
                for (int j = 1; j <= n; j++)
                    M[i, j] = new Complex(r.NextDouble());
            return M;
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice carrée n × n remplie d'entiers aléatoires dans {lo, …, hi−1}.</para>
        /// <para><b>[EN]</b> Creates an n × n square matrix filled with random integers in {lo, …, hi−1}.</para>
        /// </summary>
        /// <param name="n"><b>[FR]</b> Dimension. / <b>[EN]</b> Dimension.</param>
        /// <param name="lo"><b>[FR]</b> Borne inférieure inclusive. / <b>[EN]</b> Inclusive lower bound.</param>
        /// <param name="hi"><b>[FR]</b> Borne supérieure exclusive. / <b>[EN]</b> Exclusive upper bound.</param>
        public static Matrix Random(int n, int lo, int hi)
        {
            Matrix M = new Matrix(n);
            Random r = new Random();
            for (int i = 1; i <= n; i++)
                for (int j = 1; j <= n; j++)
                    M[i, j] = new Complex((double)r.Next(lo, hi));
            return M;
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice m × n remplie d'entiers aléatoires dans {lo, …, hi−1}.</para>
        /// <para><b>[EN]</b> Creates an m × n matrix filled with random integers in {lo, …, hi−1}.</para>
        /// </summary>
        public static Matrix Random(int m, int n, int lo, int hi)
        {
            Matrix M = new Matrix(m, n);
            Random r = new Random();
            for (int i = 1; i <= m; i++)
                for (int j = 1; j <= n; j++)
                    M[i, j] = new Complex((double)r.Next(lo, hi));
            return M;
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice m × n aléatoire binaire (0 ou 1), où chaque entrée
        /// vaut 1 avec la probabilité <paramref name="probability"/>.</para>
        /// <para><b>[EN]</b> Creates an m × n random binary matrix (0 or 1), where each entry
        /// equals 1 with probability <paramref name="probability"/>.</para>
        /// </summary>
        /// <param name="m"><b>[FR]</b> Nombre de lignes. / <b>[EN]</b> Number of rows.</param>
        /// <param name="n"><b>[FR]</b> Nombre de colonnes. / <b>[EN]</b> Number of columns.</param>
        /// <param name="probability">
        /// <b>[FR]</b> Probabilité qu'une entrée vaille 1 (∈ [0,1]). /
        /// <b>[EN]</b> Probability for each entry to be 1 (∈ [0,1]).
        /// </param>
        public static Matrix RandomBinary(int m, int n, double probability)
        {
            Matrix M = new Matrix(m, n);
            Random r = new Random();
            for (int i = 1; i <= m; i++)
                for (int j = 1; j <= n; j++)
                    if (r.NextDouble() <= probability) M[i, j] = Complex.One;
            return M;
        }

        /// <summary>
        /// <para><b>[FR]</b> Crée une matrice carrée n × n aléatoire binaire (0 ou 1), où chaque entrée
        /// vaut 1 avec la probabilité <paramref name="probability"/>.</para>
        /// <para><b>[EN]</b> Creates an n × n random binary square matrix (0 or 1), where each entry
        /// equals 1 with probability <paramref name="probability"/>.</para>
        /// </summary>
        public static Matrix RandomBinary(int n, double probability)
        {
            Matrix M = new Matrix(n, n);
            Random r = new Random();
            for (int i = 1; i <= n; i++)
                for (int j = 1; j <= n; j++)
                    if (r.NextDouble() <= probability) M[i, j] = Complex.One;
            return M;
        }

        /// <summary>
        /// <para><b>[FR]</b> Construit la matrice de Vandermonde à partir d'un tableau de nœuds.</para>
        /// <para><b>[EN]</b> Builds the Vandermonde matrix from an array of nodes.</para>
        /// </summary>
        /// <param name="nodes">
        /// <b>[FR]</b> Tableau de nœuds complexes x₀, …, xₙ. /
        /// <b>[EN]</b> Array of complex nodes x₀, …, xₙ.
        /// </param>
        public static Matrix CreateVandermonde(Complex[] nodes)
        {
            if (nodes == null || nodes.Length < 1) throw new ArgumentNullException();
            int n = nodes.Length - 1;
            Matrix V = new Matrix(n + 1);
            for (int i = 0; i <= n; i++)
                for (int p = 0; p <= n; p++)
                    V[i + 1, p + 1] = Complex.Pow(nodes[i], p);
            return V;
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule toutes les plus courtes distances entre sommets d'un graphe
        /// via l'algorithme de Floyd-Warshall.</para>
        /// <para><b>[EN]</b> Computes all shortest distances between vertices in a graph
        /// using the Floyd-Warshall algorithm.</para>
        /// </summary>
        /// <param name="adjacenceMatrix">
        /// <para><b>[FR]</b> Matrice carrée d'adjacence. La diagonale doit être nulle ;
        /// les arêtes absentes sont marquées +∞.</para>
        /// <para><b>[EN]</b> Square adjacency matrix. The diagonal must be zero;
        /// missing edges must be marked +∞.</para>
        /// </param>
        /// <returns>
        /// <para><b>[FR]</b> Tableau de deux matrices [D, P] : D[u,v] = distance minimale entre u et v ;
        /// P[u,v] = sommet intermédiaire sur le chemin optimal.</para>
        /// <para><b>[EN]</b> Array of two matrices [D, P]: D[u,v] = shortest distance between u and v;
        /// P[u,v] = intermediate vertex on the optimal path.</para>
        /// </returns>
        public static Matrix[] Floyd(Matrix adjacenceMatrix)
        {
            if (!adjacenceMatrix.IsSquare()) throw new ArgumentException("Expected square matrix.");
            if (!adjacenceMatrix.IsReal()) throw new ArgumentException("Adjacency matrices are expected to be real.");
            int n = adjacenceMatrix.RowCount;
            Matrix D = adjacenceMatrix.Clone();
            Matrix P = new Matrix(n);
            double buf;
            for (int k = 1; k <= n; k++)
                for (int i = 1; i <= n; i++)
                    for (int j = 1; j <= n; j++)
                    {
                        buf = D[i, k].Re + D[k, j].Re;
                        if (buf < D[i, j].Re) { D[i, j].Re = buf; P[i, j].Re = k; }
                    }
            return new Matrix[] { D, P };
        }

        /// <summary>
        /// <para><b>[FR]</b> Retrace le chemin le plus court entre deux sommets à partir de la
        /// matrice de prédécesseurs produite par <see cref="Floyd"/>.</para>
        /// <para><b>[EN]</b> Retrieves the shortest path between two vertices from the
        /// predecessor matrix produced by <see cref="Floyd"/>.</para>
        /// </summary>
        /// <param name="predecessorMatrix">
        /// <b>[FR]</b> Matrice de prédécesseurs retournée par Floyd. /
        /// <b>[EN]</b> Predecessor matrix returned by Floyd.
        /// </param>
        /// <param name="source">
        /// <b>[FR]</b> Indice (base 1) du sommet source. /
        /// <b>[EN]</b> One-based index of the source vertex.
        /// </param>
        /// <param name="target">
        /// <b>[FR]</b> Indice (base 1) du sommet cible. /
        /// <b>[EN]</b> One-based index of the target vertex.
        /// </param>
        /// <returns>
        /// <b>[FR]</b> Liste ordonnée des indices de sommets sur le chemin. /
        /// <b>[EN]</b> Ordered list of vertex indices along the path.
        /// </returns>
        public static ArrayList GetFloydPath(Matrix predecessorMatrix, int source, int target)
        {
            if (!predecessorMatrix.IsSquare()) throw new ArgumentException("Path matrix must be square.");
            if (!predecessorMatrix.IsReal()) throw new ArgumentException("Path matrix must be real.");
            ArrayList path = new ArrayList();
            path.Add(source);
            while (predecessorMatrix[source, target] != 0)
            {
                source = Convert.ToInt32(predecessorMatrix[source, target]);
                path.Add(source);
            }
            path.Add(target);
            return path;
        }

        /// <summary>
        /// <para><b>[FR]</b> Effectue un parcours en profondeur (DFS) sur un graphe donné par
        /// sa matrice d'adjacence et retourne l'arbre couvrant.</para>
        /// <para><b>[EN]</b> Performs a depth-first search (DFS) on a graph given by its
        /// adjacency matrix and returns the spanning tree.</para>
        /// </summary>
        /// <param name="adjacenceMatrix">
        /// <para><b>[FR]</b> Matrice d'adjacence carrée : A[i,j] = 0 ou +∞ si pas d'arête, sinon non nul.</para>
        /// <para><b>[EN]</b> Square adjacency matrix: A[i,j] = 0 or +∞ if no edge, non-zero otherwise.</para>
        /// </param>
        /// <param name="root">
        /// <b>[FR]</b> Sommet de départ (base 1). /
        /// <b>[EN]</b> Starting vertex (one-based).
        /// </param>
        /// <returns>
        /// <b>[FR]</b> Matrice d'adjacence de l'arbre couvrant calculé. /
        /// <b>[EN]</b> Adjacency matrix of the computed spanning tree.
        /// </returns>
        public static Matrix DepthFirstSearch(Matrix adjacenceMatrix, int root)
        {
            if (!adjacenceMatrix.IsSquare()) throw new ArgumentException("Adjacency matrices must be square.");
            if (!adjacenceMatrix.IsReal()) throw new ArgumentException("Adjacency matrices must be real.");
            int n = adjacenceMatrix.RowCount;
            if (root < 1 || root > n) throw new ArgumentException("Root must be a vertex in {1, ..., n}.");
            Matrix spanTree = new Matrix(n);
            bool[] marked = new bool[n + 1];
            Stack todo = new Stack();
            todo.Push(root);
            marked[root] = true;
            ArrayList[] A = new ArrayList[n + 1];
            for (int i = 1; i <= n; i++)
            {
                A[i] = new ArrayList();
                for (int j = 1; j <= n; j++)
                    if (adjacenceMatrix[i, j].Re != 0 && adjacenceMatrix[i, j].Im != double.PositiveInfinity)
                        A[i].Add(j);
            }
            int v, w;
            while (todo.Count > 0)
            {
                v = (int)todo.Peek();
                if (A[v].Count > 0)
                {
                    w = (int)A[v][0];
                    if (!marked[w]) { marked[w] = true; spanTree[v, w].Re = 1; todo.Push(w); }
                    A[v].RemoveAt(0);
                }
                else todo.Pop();
            }
            return spanTree;
        }

        /// <summary>
        /// <para><b>[FR]</b> Effectue un parcours en largeur (BFS) sur un graphe donné par
        /// sa matrice d'adjacence et retourne l'arbre couvrant.</para>
        /// <para><b>[EN]</b> Performs a breadth-first search (BFS) on a graph given by its
        /// adjacency matrix and returns the spanning tree.</para>
        /// </summary>
        /// <param name="adjacenceMatrix">
        /// <para><b>[FR]</b> Matrice d'adjacence carrée : A[i,j] = 0 ou +∞ si pas d'arête, sinon non nul.</para>
        /// <para><b>[EN]</b> Square adjacency matrix: A[i,j] = 0 or +∞ if no edge, non-zero otherwise.</para>
        /// </param>
        /// <param name="root">
        /// <b>[FR]</b> Sommet de départ (base 1). /
        /// <b>[EN]</b> Starting vertex (one-based).
        /// </param>
        /// <returns>
        /// <b>[FR]</b> Matrice d'adjacence de l'arbre couvrant calculé. /
        /// <b>[EN]</b> Adjacency matrix of the computed spanning tree.
        /// </returns>
        public static Matrix BreadthFirstSearch(Matrix adjacenceMatrix, int root)
        {
            if (!adjacenceMatrix.IsSquare()) throw new ArgumentException("Adjacency matrices must be square.");
            if (!adjacenceMatrix.IsReal()) throw new ArgumentException("Adjacency matrices must be real.");
            int n = adjacenceMatrix.RowCount;
            if (root < 1 || root > n) throw new ArgumentException("Root must be a vertex in {1, ..., n}.");
            Matrix spanTree = new Matrix(n);
            bool[] marked = new bool[n + 1];
            Queue todo = new Queue();
            todo.Enqueue(root);
            marked[root] = true;
            ArrayList[] A = new ArrayList[n + 1];
            for (int i = 1; i <= n; i++)
            {
                A[i] = new ArrayList();
                for (int j = 1; j <= n; j++)
                    if (adjacenceMatrix[i, j].Re != 0 && adjacenceMatrix[i, j].Re != double.PositiveInfinity)
                        A[i].Add(j);
            }
            int v, w;
            while (todo.Count > 0)
            {
                v = (int)todo.Peek();
                if (A[v].Count > 0)
                {
                    w = (int)A[v][0];
                    if (!marked[w]) { marked[w] = true; spanTree[v, w].Re = 1; todo.Enqueue(w); }
                    A[v].RemoveAt(0);
                }
                else todo.Dequeue();
            }
            return spanTree;
        }

        /// <summary>
        /// <para><b>[FR]</b> Construit une matrice bloc [A, B ; C, D] à partir de quatre sous-matrices.</para>
        /// <para><b>[EN]</b> Builds a block matrix [A, B ; C, D] from four sub-matrices.</para>
        /// </summary>
        /// <param name="A"><b>[FR]</b> Sous-matrice supérieure gauche. / <b>[EN]</b> Upper-left sub-matrix.</param>
        /// <param name="B"><b>[FR]</b> Sous-matrice supérieure droite. / <b>[EN]</b> Upper-right sub-matrix.</param>
        /// <param name="C"><b>[FR]</b> Sous-matrice inférieure gauche. / <b>[EN]</b> Lower-left sub-matrix.</param>
        /// <param name="D"><b>[FR]</b> Sous-matrice inférieure droite. / <b>[EN]</b> Lower-right sub-matrix.</param>
        public static Matrix CreateBlockMatrix(Matrix A, Matrix B, Matrix C, Matrix D)
        {
            if (A.RowCount != B.RowCount || C.RowCount != D.RowCount
                || A.ColumnCount != C.ColumnCount || B.ColumnCount != D.ColumnCount)
                throw new ArgumentException("Matrix dimensions must agree.");
            Matrix R = new Matrix(A.RowCount + C.RowCount, A.ColumnCount + B.ColumnCount);
            for (int i = 1; i <= R.rowCount; i++)
                for (int j = 1; j <= R.columnCount; j++)
                    if (i <= A.RowCount)
                        R[i, j] = j <= A.ColumnCount ? A[i, j] : B[i, j - A.ColumnCount];
                    else
                        R[i, j] = j <= C.ColumnCount ? C[i - A.RowCount, j] : D[i - A.RowCount, j - C.ColumnCount];
            return R;
        }

        /// <summary>
        /// <para><b>[FR]</b> Résout le système linéaire Ax = b par décomposition LU avec pivotage partiel.</para>
        /// <para><b>[EN]</b> Solves the linear system Ax = b via LU decomposition with partial pivoting.</para>
        /// </summary>
        /// <param name="A">
        /// <b>[FR]</b> Matrice carrée du système. /
        /// <b>[EN]</b> Square system matrix.
        /// </param>
        /// <param name="b">
        /// <b>[FR]</b> Vecteur du membre droit. /
        /// <b>[EN]</b> Right-hand side vector.
        /// </param>
        /// <returns>
        /// <b>[FR]</b> Vecteur solution x tel que Ax = b. /
        /// <b>[EN]</b> Solution vector x such that Ax = b.
        /// </returns>
        /// <remarks>
        /// <b>[FR]</b> Complexité approximative : O(n³). /
        /// <b>[EN]</b> Approximate complexity: O(n³).
        /// </remarks>
        public static Matrix Solve(Matrix A, Matrix b)
        {
            Matrix A2 = A.Clone();
            Matrix b2 = b.Clone();
            if (!A2.IsSquare()) throw new InvalidOperationException("Cannot uniquely solve a non-square system.");
            int n = A2.RowCount;
            Matrix P = A2.LUSafe();
            b2 = P * b2;
            (A2.ExtractLowerTrapezoid() - Diag(A2.DiagonalVector()) + Identity(n)).ForwardSubstitution(b2);
            (A2.ExtractUpperTrapezoid()).BackwardSubstitution(b2);
            return b2;
        }

        #endregion

        #region Matrix Manipulations / Manipulations matricielles

        /// <summary>
        /// <para><b>[FR]</b> Retourne la matrice des parties réelles des entrées.</para>
        /// <para><b>[EN]</b> Returns the matrix of real parts of the entries.</para>
        /// </summary>
        public Matrix RealPart()
        {
            Matrix M = new Matrix(rowCount, columnCount);
            for (int i = 1; i <= rowCount; i++)
                for (int j = 1; j <= columnCount; j++)
                    M[i, j] = new Complex(this[i, j].Re);
            return M;
        }

        /// <summary>
        /// <para><b>[FR]</b> Retourne la matrice des parties imaginaires des entrées.</para>
        /// <para><b>[EN]</b> Returns the matrix of imaginary parts of the entries.</para>
        /// </summary>
        public Matrix ImaginaryPart()
        {
            Matrix M = new Matrix(rowCount, columnCount);
            for (int i = 1; i <= rowCount; i++)
                for (int j = 1; j <= columnCount; j++)
                    M[i, j] = new Complex(this[i, j].Im);
            return M;
        }

        /// <summary>
        /// <para><b>[FR]</b> Effectue la réduction de Hessenberg par la méthode de Householder.
        /// Retourne {H, Q} avec H de forme Hessenberg, Q orthogonale et H = Q'AQ.</para>
        /// <para><b>[EN]</b> Performs Hessenberg reduction using the Householder method.
        /// Returns {H, Q} with H in Hessenberg form, Q orthogonal and H = Q'AQ.</para>
        /// </summary>
        public Matrix[] HessenbergHouseholder()
        {
            if (!this.IsSquare()) throw new InvalidOperationException("Cannot perform Hessenberg-Householder decomposition of a non-square matrix.");
            int n = rowCount;
            Matrix Q = Identity(n);
            Matrix H = this.Clone();
            Matrix I, N, R, P;
            Matrix[] vbeta;
            int m;
            for (int k = 1; k <= n - 2; k++)
            {
                vbeta = ComputeHouseholderVector(H.Extract(k + 1, n, k, k));
                I = Identity(k);
                N = Zeros(k, n - k);
                m = vbeta[0].VectorDimension();
                R = Identity(m) - vbeta[1][1, 1] * vbeta[0] * vbeta[0].Transpose();
                H.Insert(k + 1, k, R * H.Extract(k + 1, n, k, n));
                H.Insert(1, k + 1, H.Extract(1, n, k + 1, n) * R);
                P = CreateBlockMatrix(I, N, N.Transpose(), R);
                Q = Q * P;
            }
            return new Matrix[] { H, Q };
        }

        /// <summary>
        /// <para><b>[FR]</b> Extrait une sous-matrice de la ligne i1 à i2 et de la colonne j1 à j2 (base 1).</para>
        /// <para><b>[EN]</b> Extracts a sub-matrix from row i1 to i2 and column j1 to j2 (one-based).</para>
        /// </summary>
        /// <param name="i1"><b>[FR]</b> Ligne de début. / <b>[EN]</b> Start row.</param>
        /// <param name="i2"><b>[FR]</b> Ligne de fin. / <b>[EN]</b> End row.</param>
        /// <param name="j1"><b>[FR]</b> Colonne de début. / <b>[EN]</b> Start column.</param>
        /// <param name="j2"><b>[FR]</b> Colonne de fin. / <b>[EN]</b> End column.</param>
        public Matrix Extract(int i1, int i2, int j1, int j2)
        {
            if (i2 < i1 || j2 < j1 || i1 <= 0 || j2 <= 0 || i2 > rowCount || j2 > columnCount)
                throw new ArgumentException("Index exceeds matrix dimension.");
            Matrix B = new Matrix(i2 - i1 + 1, j2 - j1 + 1);
            for (int i = i1; i <= i2; i++)
                for (int j = j1; j <= j2; j++)
                    B[i - i1 + 1, j - j1 + 1] = this[i, j];
            return B;
        }

        /// <summary>
        /// <para><b>[FR]</b> Extrait la partie trapézoïdale inférieure (triangle inférieur inclus) de la matrice.</para>
        /// <para><b>[EN]</b> Extracts the lower trapezoidal part (lower triangle inclusive) of the matrix.</para>
        /// </summary>
        public Matrix ExtractLowerTrapezoid()
        {
            Matrix buf = new Matrix(rowCount, columnCount);
            for (int i = 1; i <= rowCount; i++)
                for (int j = 1; j <= i; j++)
                    buf[i, j] = this[i, j];
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Extrait la partie trapézoïdale supérieure (triangle supérieur inclus) de la matrice.</para>
        /// <para><b>[EN]</b> Extracts the upper trapezoidal part (upper triangle inclusive) of the matrix.</para>
        /// </summary>
        public Matrix ExtractUpperTrapezoid()
        {
            Matrix buf = new Matrix(rowCount, columnCount);
            for (int i = 1; i <= rowCount; i++)
                for (int j = i; j <= columnCount; j++)
                    buf[i, j] = this[i, j];
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Décompose la matrice en ses vecteurs colonnes.</para>
        /// <para><b>[EN]</b> Splits the matrix into its column vectors.</para>
        /// </summary>
        /// <returns>
        /// <b>[FR]</b> Tableau de vecteurs colonnes. /
        /// <b>[EN]</b> Array of column vectors.
        /// </returns>
        public Matrix[] ToColumnVectors()
        {
            Matrix[] buf = new Matrix[columnCount];
            for (int j = 1; j <= buf.Length; j++)
                buf[j] = this.Column(j);
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Décompose la matrice en ses vecteurs lignes.</para>
        /// <para><b>[EN]</b> Splits the matrix into its row vectors.</para>
        /// </summary>
        /// <returns>
        /// <b>[FR]</b> Tableau de vecteurs lignes. /
        /// <b>[EN]</b> Array of row vectors.
        /// </returns>
        public Matrix[] ToRowVectors()
        {
            Matrix[] buf = new Matrix[rowCount];
            for (int i = 1; i <= buf.Length; i++)
                buf[i] = this.Row(i);
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Retourne la matrice à l'envers verticalement (inverse l'ordre des lignes).</para>
        /// <para><b>[EN]</b> Flips the matrix vertically (reverses the row order).</para>
        /// </summary>
        public void VerticalFlip() { Values.Reverse(); }

        /// <summary>
        /// <para><b>[FR]</b> Retourne la matrice à l'envers horizontalement (inverse l'ordre des colonnes).</para>
        /// <para><b>[EN]</b> Flips the matrix horizontally (reverses the column order).</para>
        /// </summary>
        public void HorizontalFlip()
        {
            for (int i = 0; i < rowCount; i++)
                ((ArrayList)Values[i]).Reverse();
        }

        /// <summary>
        /// <para><b>[FR]</b> Échange deux colonnes aux indices spécifiés (base 1). Sans effet si j1 = j2.</para>
        /// <para><b>[EN]</b> Swaps two columns at the specified indices (one-based). No-op if j1 = j2.</para>
        /// </summary>
        public void SwapColumns(int j1, int j2)
        {
            if (j1 <= 0 || j1 > columnCount || j2 <= 0 || j2 > columnCount)
                throw new ArgumentException("Indices must be positive and ≤ number of columns.");
            if (j1 == j2) return;
            j1--; j2--;
            object buf;
            for (int i = 0; i < rowCount; i++)
            {
                buf = ((ArrayList)Values[i])[j1];
                ((ArrayList)Values[i])[j1] = ((ArrayList)Values[i])[j2];
                ((ArrayList)Values[i])[j2] = buf;
            }
        }

        /// <summary>
        /// <para><b>[FR]</b> Échange deux lignes aux indices spécifiés (base 1). Sans effet si i1 = i2.</para>
        /// <para><b>[EN]</b> Swaps two rows at the specified indices (one-based). No-op if i1 = i2.</para>
        /// </summary>
        public void SwapRows(int i1, int i2)
        {
            if (i1 <= 0 || i1 > rowCount || i2 <= 0 || i2 > rowCount)
                throw new ArgumentException("Indices must be positive and ≤ number of rows.");
            if (i1 == i2) return;
            ArrayList buf = (ArrayList)Values[--i1];
            Values[i1] = Values[--i2];
            Values[i2] = buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Supprime la ligne à l'indice spécifié (base 1).</para>
        /// <para><b>[EN]</b> Deletes the row at the specified index (one-based).</para>
        /// </summary>
        public void DeleteRow(int i)
        {
            if (i <= 0 || i > rowCount) throw new ArgumentException("Index must be positive and ≤ number of rows.");
            Values.RemoveAt(i - 1);
            rowCount--;
        }

        /// <summary>
        /// <para><b>[FR]</b> Supprime la colonne à l'indice spécifié (base 1).</para>
        /// <para><b>[EN]</b> Deletes the column at the specified index (one-based).</para>
        /// </summary>
        public void DeleteColumn(int j)
        {
            if (j <= 0 || j > columnCount) throw new ArgumentException("Index must be positive and ≤ number of columns.");
            for (int i = 0; i < rowCount; i++)
                ((ArrayList)Values[i]).RemoveAt(j - 1);
            columnCount--;
        }

        /// <summary>
        /// <para><b>[FR]</b> Extrait et supprime le vecteur ligne à l'indice spécifié (base 1).</para>
        /// <para><b>[EN]</b> Extracts and removes the row vector at the specified index (one-based).</para>
        /// </summary>
        public Matrix ExtractRow(int i) { Matrix buf = this.Row(i); this.DeleteRow(i); return buf; }

        /// <summary>
        /// <para><b>[FR]</b> Extrait et supprime le vecteur colonne à l'indice spécifié (base 1).</para>
        /// <para><b>[EN]</b> Extracts and removes the column vector at the specified index (one-based).</para>
        /// </summary>
        public Matrix ExtractColumn(int j)
        {
            if (j <= 0 || j > columnCount) throw new ArgumentException("Index must be positive and ≤ number of columns.");
            Matrix buf = this.Column(j);
            this.DeleteColumn(j);
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Insère un vecteur ligne à l'indice spécifié (base 1). La matrice est agrandie si nécessaire.</para>
        /// <para><b>[EN]</b> Inserts a row vector at the specified index (one-based). The matrix is expanded if needed.</para>
        /// </summary>
        public void InsertRow(Matrix row, int i)
        {
            int size = row.VectorDimension();
            if (size == 0) throw new InvalidOperationException("Row must be a vector of length > 0.");
            if (i <= 0) throw new ArgumentException("Row index must be positive.");
            if (i > rowCount) this[i, size] = Complex.Zero;
            else if (size > columnCount) { this[i, size] = Complex.Zero; rowCount++; }
            else rowCount++;
            Values.Insert(--i, new ArrayList(size));
            for (int k = 1; k <= size; k++)
                ((ArrayList)Values[i]).Add(row[k]);
            for (int k = size; k < columnCount; k++)
                ((ArrayList)Values[i]).Add(Complex.Zero);
        }

        /// <summary>
        /// <para><b>[FR]</b> Insère une sous-matrice M à partir de la ligne i et de la colonne j (base 1).</para>
        /// <para><b>[EN]</b> Inserts a sub-matrix M starting at row i and column j (one-based).</para>
        /// </summary>
        public void Insert(int i, int j, Matrix M)
        {
            for (int m = 1; m <= M.rowCount; m++)
                for (int n = 1; n <= M.columnCount; n++)
                    this[i + m - 1, j + n - 1] = M[m, n];
        }

        /// <summary>
        /// <para><b>[FR]</b> Insère un vecteur colonne à l'indice spécifié (base 1). La matrice est agrandie si nécessaire.</para>
        /// <para><b>[EN]</b> Inserts a column vector at the specified index (one-based). The matrix is expanded if needed.</para>
        /// </summary>
        public void InsertColumn(Matrix col, int j)
        {
            int size = col.VectorDimension();
            if (size == 0) throw new InvalidOperationException("Column must be a vector of length > 0.");
            if (j <= 0) throw new ArgumentException("Column index must be positive.");
            if (j > columnCount) this[size, j] = Complex.Zero;
            else columnCount++;
            if (size > rowCount) this[size, j] = Complex.Zero;
            j--;
            for (int k = 0; k < size; k++)
                ((ArrayList)Values[k]).Insert(j, col[k + 1]);
            for (int k = size; k < rowCount; k++)
                ((ArrayList)Values[k]).Insert(j, 0);
        }

        /// <summary>
        /// <para><b>[FR]</b> Inverse la matrice carrée (det ≠ 0). Utilise des optimisations pour les matrices
        /// orthogonales, unitaires et diagonales.</para>
        /// <para><b>[EN]</b> Inverts the square matrix (det ≠ 0). Uses optimizations for orthogonal,
        /// unitary and diagonal matrices.</para>
        /// </summary>
        public Matrix Inverse()
        {
            if (!this.IsSquare()) throw new InvalidOperationException("Cannot invert a non-square matrix.");
            Complex det = this.Determinant();
            if (det == Complex.Zero) throw new InvalidOperationException("Cannot invert a (nearly) singular matrix.");
            int n = this.columnCount;
            if (n == 1) return new Matrix(1 / det);
            if (this.IsReal() && this.IsOrthogonal()) return this.Transpose();
            else if (this.IsUnitary()) return this.ConjugateTranspose();
            if (this.IsDiagonal())
            {
                Matrix d = this.DiagonalVector();
                for (int i = 1; i <= n; i++) d[i] = 1 / d[i];
                return Diag(d);
            }
            Complex[,] buf = new Complex[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    try { buf[i, j] = System.Math.Pow(-1, i + j) * this.Minor(j + 1, i + 1).Determinant(); }
                    catch (DivideByZeroException) { buf[i, j] = new Complex(0.0); }
            return (new Matrix(buf) / det);
        }

        /// <summary>
        /// <para><b>[FR]</b> Inverse la matrice via la formule de Leverrier.</para>
        /// <para><b>[EN]</b> Inverts the matrix using Leverrier's formula.</para>
        /// </summary>
        public Matrix InverseLeverrier()
        {
            if (!this.IsSquare()) throw new InvalidOperationException("Cannot invert a non-square matrix.");
            int n = this.rowCount;
            Matrix Id = Identity(n);
            Matrix B = Id;
            Complex alpha;
            for (int k = 1; k < n; k++)
            {
                Matrix buf = (this * B);
                alpha = ((double)1 / k) * buf.Trace();
                B = alpha * Id - buf;
            }
            alpha = (this * B).Trace() / n;
            if (alpha != Complex.Zero) return B / alpha;
            else throw new InvalidOperationException("WARNING: Matrix nearly singular or badly scaled.");
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le mineur (i,j) : matrice obtenue en supprimant la ligne i et la colonne j.</para>
        /// <para><b>[EN]</b> Computes the (i,j) minor: matrix obtained by deleting row i and column j.</para>
        /// </summary>
        public Matrix Minor(int i, int j)
        {
            Matrix A = this.Clone();
            A.DeleteRow(i);
            A.DeleteColumn(j);
            return A;
        }

        /// <summary>
        /// <para><b>[FR]</b> Produit une copie superficielle de la matrice en O(m).</para>
        /// <para><b>[EN]</b> Produces a shallow copy of the matrix in O(m).</para>
        /// </summary>
        public Matrix Clone()
        {
            Matrix A = new Matrix();
            A.rowCount = rowCount;
            A.columnCount = columnCount;
            for (int i = 0; i < rowCount; i++)
                A.Values.Add(((ArrayList)this.Values[i]).Clone());
            return A;
        }

        /// <summary>
        /// <para><b>[FR]</b> Extrait le vecteur colonne de la diagonale principale de la matrice carrée.</para>
        /// <para><b>[EN]</b> Extracts the main diagonal as a column vector from the square matrix.</para>
        /// </summary>
        public Matrix DiagonalVector()
        {
            if (!this.IsSquare()) throw new InvalidOperationException("Cannot get diagonal of a non-square matrix.");
            Matrix v = new Matrix(this.columnCount, 1);
            for (int i = 1; i <= this.columnCount; i++)
                v[i] = this[i, i];
            return v;
        }

        /// <summary>
        /// <para><b>[FR]</b> Retourne la j-ième colonne de la matrice (base 1).</para>
        /// <para><b>[EN]</b> Returns the j-th column of the matrix (one-based).</para>
        /// </summary>
        public Matrix Column(int j)
        {
            Matrix buf = new Matrix(this.rowCount, 1);
            for (int i = 1; i <= this.rowCount; i++)
                buf[i] = this[i, j];
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Retourne la i-ième ligne de la matrice comme vecteur colonne (base 1).</para>
        /// <para><b>[EN]</b> Returns the i-th row of the matrix as a column vector (one-based).</para>
        /// </summary>
        public Matrix Row(int i)
        {
            if (i <= 0 || i > rowCount) throw new ArgumentException("Index exceeds matrix dimension.");
            Matrix buf = new Matrix(columnCount, 1);
            for (int j = 1; j <= this.columnCount; j++)
                buf[j] = this[i, j];
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Retourne la matrice transposée (échange A[i,j] et A[j,i]).</para>
        /// <para><b>[EN]</b> Returns the transposed matrix (swaps A[i,j] and A[j,i]).</para>
        /// </summary>
        public Matrix Transpose()
        {
            Matrix M = new Matrix(columnCount, rowCount);
            for (int i = 1; i <= columnCount; i++)
                for (int j = 1; j <= rowCount; j++)
                    M[i, j] = this[j, i];
            return M;
        }

        /// <summary>
        /// <para><b>[FR]</b> Retourne la matrice conjuguée (remplace chaque z = x+iy par x−iy).</para>
        /// <para><b>[EN]</b> Returns the conjugated matrix (replaces each z = x+iy by x−iy).</para>
        /// </summary>
        public Matrix Conjugate()
        {
            Matrix M = new Matrix(rowCount, columnCount);
            for (int i = 1; i <= rowCount; i++)
                for (int j = 1; j <= columnCount; j++)
                    M[i, j] = Complex.Conj(this[i, j]);
            return M;
        }

        /// <summary>
        /// <para><b>[FR]</b> Retourne la transposée conjuguée (hermitienne) de la matrice : (A*)ᵀ.</para>
        /// <para><b>[EN]</b> Returns the conjugate transpose (Hermitian adjoint) of the matrix: (A*)ᵀ.</para>
        /// </summary>
        public Matrix ConjugateTranspose() { return this.Transpose().Conjugate(); }

        /// <summary>
        /// <para><b>[FR]</b> Effectue la décomposition LU en place (sans pivotage). Les éléments de L
        /// (sauf la diagonale de 1) et de U sont stockés dans la matrice courante.</para>
        /// <para><b>[EN]</b> Performs in-place LU decomposition (without pivoting). L elements
        /// (excluding the unit diagonal) and U elements are stored in the current matrix.</para>
        /// </summary>
        /// <exception cref="DivideByZeroException">
        /// <b>[FR]</b> Levée si un pivot nul est rencontré. Utilisez <see cref="LUSafe"/> à la place. /
        /// <b>[EN]</b> Thrown if a zero pivot is encountered. Use <see cref="LUSafe"/> instead.
        /// </exception>
        public void LU()
        {
            if (!this.IsSquare()) throw new InvalidOperationException("Cannot perform LU decomposition of a non-square matrix.");
            int n = this.columnCount;
            for (int j = 1; j <= n; j++)
            {
                if (this[j, j] == 0) throw new DivideByZeroException("Warning: Matrix badly scaled or close to singular. Try LUSafe() instead.");
                for (int k = 1; k < j; k++)
                    for (int i = k + 1; i <= n; i++)
                        this[i, j] = this[i, j] - this[i, k] * this[k, j];
                for (int i = j + 1; i <= n; i++)
                    this[i, j] = this[i, j] / this[j, j];
            }
        }

        /// <summary>
        /// <para><b>[FR]</b> Effectue la décomposition LU avec pivotage partiel (colonne) en place.
        /// Les éléments de L et U sont stockés dans la matrice courante.</para>
        /// <para><b>[EN]</b> Performs in-place LU decomposition with partial (column) pivoting.
        /// L and U elements are stored in the current matrix.</para>
        /// </summary>
        /// <returns>
        /// <b>[FR]</b> Matrice de permutation P telle que P·this = L·U. /
        /// <b>[EN]</b> Permutation matrix P such that P·this = L·U.
        /// </returns>
        public Matrix LUSafe()
        {
            if (!this.IsSquare()) throw new InvalidOperationException("Cannot perform LU decomposition of a non-square matrix.");
            int n = this.columnCount;
            Matrix P = Identity(n);
            int m;
            for (int j = 1; j <= n; j++)
            {
                if (j < n)
                {
                    m = j;
                    for (int i = j + 1; i <= n; i++)
                        if (Complex.Abs(this[i, j]) > Complex.Abs(this[m, j])) m = i;
                    if (m > j) { P.SwapRows(j, m); this.SwapRows(j, m); }
                    if (this[j, j] == 0) throw new DivideByZeroException("Warning: Matrix close to singular.");
                }
                for (int k = 1; k < j; k++)
                    for (int i = k + 1; i <= n; i++)
                        this[i, j] = this[i, j] - this[i, k] * this[k, j];
                for (int i = j + 1; i <= n; i++)
                    this[i, j] = this[i, j] / this[j, j];
            }
            return P;
        }

        /// <summary>
        /// <para><b>[FR]</b> Effectue la décomposition de Cholesky A = LL' en place pour une matrice
        /// symétrique définie positive. Le résultat est stocké dans la partie triangulaire inférieure.</para>
        /// <para><b>[EN]</b> Performs Cholesky decomposition A = LL' in-place for a symmetric positive
        /// definite matrix. The result is stored in the lower triangular part.</para>
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <b>[FR]</b> Levée si la matrice n'est pas symétrique définie positive. /
        /// <b>[EN]</b> Thrown if the matrix is not symmetric positive definite.
        /// </exception>
        public void Cholesky()
        {
            if (!this.IsSquare()) throw new InvalidOperationException("Cannot perform Cholesky decomposition of a non-square matrix.");
            if (!this.IsSPD()) throw new InvalidOperationException("Cannot perform Cholesky decomposition of a non-SPD matrix.");
            int n = rowCount;
            for (int k = 1; k < n; k++)
            {
                this[k, k] = Complex.Sqrt(this[k, k]);
                for (int i = 1; i <= n - k; i++)
                    this[k + i, k] = this[k + i, k] / this[k, k];
                for (int j = k + 1; j <= n; j++)
                    for (int i = 0; i <= n - j; i++)
                        this[j + i, j] = this[j + i, j] - this[j + i, k] * this[j, k];
            }
            this[n, n] = Complex.Sqrt(this[n, n]);
        }

        /// <summary>
        /// <para><b>[FR]</b> Annule la décomposition de Cholesky pour restaurer la matrice symétrique d'origine.</para>
        /// <para><b>[EN]</b> Undoes the Cholesky decomposition to restore the original symmetric matrix.</para>
        /// </summary>
        public void CholeskyRestore()
        {
            if (!this.IsSquare()) throw new InvalidOperationException("Cannot undo Cholesky decomposition on a non-square matrix.");
            this[1, 1] = Square(this[1, 1]);
            Complex buf;
            for (int i = 2; i <= rowCount; i++)
            {
                buf = Complex.Zero;
                for (int k = 1; k <= i - 1; k++)
                    buf += Square(this[i, k]);
                this[i, i] = Square(this[i, i]) + buf;
            }
            this.SymmetrizeFromUpper();
        }

        /// <summary>
        /// <para><b>[FR]</b> Résout Lx = b par substitution avant pour une matrice triangulaire inférieure régulière.
        /// La solution est écrasée dans <paramref name="b"/>.</para>
        /// <para><b>[EN]</b> Solves Lx = b by forward substitution for a regular lower triangular matrix.
        /// The solution is written back into <paramref name="b"/>.</para>
        /// </summary>
        /// <param name="b">
        /// <b>[FR]</b> Vecteur du membre droit (modifié en place). /
        /// <b>[EN]</b> Right-hand side vector (modified in-place).
        /// </param>
        public void ForwardSubstitution(Matrix b)
        {
            if (!this.IsLowerTriangular()) throw new InvalidOperationException("Cannot perform forward substitution for a non-lower-triangular matrix.");
            if (this.DiagonalProduct() == 0) throw new InvalidOperationException("Warning: Matrix is nearly singular.");
            int n = rowCount;
            if (b.VectorDimension() != n) throw new ArgumentException("Parameter must be a vector of the same height as the matrix.");
            for (int j = 1; j <= n - 1; j++)
            {
                b[j] /= this[j, j];
                for (int i = 1; i <= n - j; i++)
                    b[j + i] -= b[j] * this[j + i, j];
            }
            b[n] /= this[n, n];
        }

        /// <summary>
        /// <para><b>[FR]</b> Résout Ux = b par substitution arrière pour une matrice triangulaire supérieure régulière.
        /// La solution est écrasée dans <paramref name="b"/>.</para>
        /// <para><b>[EN]</b> Solves Ux = b by backward substitution for a regular upper triangular matrix.
        /// The solution is written back into <paramref name="b"/>.</para>
        /// </summary>
        /// <param name="b">
        /// <b>[FR]</b> Vecteur du membre droit (modifié en place). /
        /// <b>[EN]</b> Right-hand side vector (modified in-place).
        /// </param>
        public void BackwardSubstitution(Matrix b)
        {
            if (!this.IsUpperTriangular()) throw new InvalidOperationException("Cannot perform backward substitution for a non-upper-triangular matrix.");
            if (this.DiagonalProduct() == 0) throw new InvalidOperationException("Warning: Matrix is nearly singular.");
            int n = rowCount;
            if (b.VectorDimension() != n) throw new ArgumentException("Parameter must be a vector of the same height as the matrix.");
            for (int j = n; j >= 2; j--)
            {
                b[j] /= this[j, j];
                for (int i = 1; i <= j - 1; i++)
                    b[i] -= b[j] * this[i, j];
            }
            b[1] /= this[1, 1];
        }

        /// <summary>
        /// <para><b>[FR]</b> Symétrise la matrice carrée en copiant la partie triangulaire supérieure
        /// vers la partie inférieure : A[i,j] ← A[j,i] pour i > j.</para>
        /// <para><b>[EN]</b> Symmetrizes the square matrix by copying the upper triangular part
        /// to the lower part: A[i,j] ← A[j,i] for i > j.</para>
        /// </summary>
        public void SymmetrizeFromUpper()
        {
            if (!this.IsSquare()) throw new InvalidOperationException("Cannot symmetrize a non-square matrix.");
            for (int j = 1; j <= columnCount; j++)
                for (int i = j + 1; i <= columnCount; i++)
                    this[i, j] = this[j, i];
        }

        /// <summary>
        /// <para><b>[FR]</b> Symétrise la matrice carrée en copiant la partie triangulaire inférieure
        /// vers la partie supérieure : A[i,j] ← A[j,i] pour i &lt; j.</para>
        /// <para><b>[EN]</b> Symmetrizes the square matrix by copying the lower triangular part
        /// to the upper part: A[i,j] ← A[j,i] for i &lt; j.</para>
        /// </summary>
        public void SymmetrizeFromLower()
        {
            if (!this.IsSquare()) throw new InvalidOperationException("Cannot symmetrize a non-square matrix.");
            for (int i = 1; i <= rowCount; i++)
                for (int j = i + 1; j <= columnCount; j++)
                    this[i, j] = this[j, i];
        }

        /// <summary>
        /// <para><b>[FR]</b> Orthogonalisation de Gram-Schmidt : A = QR, avec Q orthogonale m × n
        /// et R triangulaire supérieure n × n.</para>
        /// <para><b>[EN]</b> Gram-Schmidt QR decomposition: A = QR, with Q orthogonal m × n
        /// and R upper triangular n × n.</para>
        /// </summary>
        public Matrix[] QRGramSchmidt()
        {
            int m = rowCount, n = columnCount;
            Matrix A = this.Clone();
            Matrix Q = new Matrix(m, n);
            Matrix R = new Matrix(n, n);
            for (int i = 1; i <= m; i++) Q[i, 1] = A[i, 1];
            R[1, 1] = Complex.One;
            for (int k = 1; k <= n; k++)
            {
                R[k, k] = new Complex(A.Column(k).Norm());
                for (int i = 1; i <= m; i++) Q[i, k] = A[i, k] / R[k, k];
                for (int j = k + 1; j <= n; j++)
                {
                    R[k, j] = Dot(Q.Column(k), A.Column(j));
                    for (int i = 1; i <= m; i++) A[i, j] = A[i, j] - Q[i, k] * R[k, j];
                }
            }
            return new Matrix[] { Q, R };
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule des approximations des valeurs propres via l'itération QR basique
        /// avec orthogonalisation de Gram-Schmidt (40 itérations).
        /// <b>Attention :</b> ne fonctionne correctement que si |λ₁| > |λ₂| > … > |λₙ| et si les
        /// valeurs propres sont toutes réelles.</para>
        /// <para><b>[EN]</b> Computes eigenvalue approximations via basic QR iteration with Gram-Schmidt
        /// orthogonalization (40 iterations).
        /// <b>Warning:</b> only works correctly when |λ₁| > |λ₂| > … > |λₙ| and all eigenvalues are real.</para>
        /// </summary>
        public Matrix Eigenvalues() { return this.QRIterationBasic(40).DiagonalVector(); }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le vecteur propre d'une matrice 2 × 2 pour une valeur propre donnée.</para>
        /// <para><b>[EN]</b> Computes the eigenvector of a 2 × 2 matrix for a given eigenvalue.</para>
        /// </summary>
        /// <param name="eigenvalue">
        /// <b>[FR]</b> Valeur propre associée au vecteur recherché. /
        /// <b>[EN]</b> Eigenvalue associated with the sought eigenvector.
        /// </param>
        public Matrix ComputeEigenvector2x2(Complex eigenvalue)
        {
            Matrix Dup = this.Clone();
            ((ArrayList)Dup.Values[0])[0] = (((ArrayList)Dup.Values[0])[0] as Complex) - eigenvalue;
            ((ArrayList)Dup.Values[1])[1] = (((ArrayList)Dup.Values[1])[1] as Complex) - eigenvalue;
            double v0 = -1.0 * (((ArrayList)Dup.Values[0])[1] as Complex).Re / (((ArrayList)Dup.Values[0])[0] as Complex).Re;
            double v1 = 1.0;
            double norme = System.Math.Sqrt(v0 * v0 + v1 * v1);
            return new Matrix(new double[] { v0 / norme, v1 / norme });
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le vecteur propre d'une matrice 3 × 3 pour une valeur propre donnée.</para>
        /// <para><b>[EN]</b> Computes the eigenvector of a 3 × 3 matrix for a given eigenvalue.</para>
        /// </summary>
        /// <param name="eigenvalue">
        /// <b>[FR]</b> Valeur propre associée au vecteur recherché. /
        /// <b>[EN]</b> Eigenvalue associated with the sought eigenvector.
        /// </param>
        public Matrix ComputeEigenvector3x3(Complex eigenvalue)
        {
            Matrix Dup = this.Clone();
            ((ArrayList)Dup.Values[0])[0] = (((ArrayList)Dup.Values[0])[0] as Complex) - eigenvalue;
            ((ArrayList)Dup.Values[1])[1] = (((ArrayList)Dup.Values[1])[1] as Complex) - eigenvalue;
            ((ArrayList)Dup.Values[2])[2] = (((ArrayList)Dup.Values[2])[2] as Complex) - eigenvalue;
            double a = (((ArrayList)Dup.Values[0])[0] as Complex).Re, b = (((ArrayList)Dup.Values[0])[1] as Complex).Re, c = (((ArrayList)Dup.Values[0])[2] as Complex).Re;
            double d = (((ArrayList)Dup.Values[1])[0] as Complex).Re, e = (((ArrayList)Dup.Values[1])[1] as Complex).Re, f = (((ArrayList)Dup.Values[1])[2] as Complex).Re;
            if (a == 0 && b == 0 && c == 0) { a = (((ArrayList)Dup.Values[2])[0] as Complex).Re; b = (((ArrayList)Dup.Values[2])[1] as Complex).Re; c = (((ArrayList)Dup.Values[2])[2] as Complex).Re; }
            else if (d == 0 && e == 0 && f == 0) { d = (((ArrayList)Dup.Values[2])[0] as Complex).Re; e = (((ArrayList)Dup.Values[2])[1] as Complex).Re; f = (((ArrayList)Dup.Values[2])[2] as Complex).Re; }
            double v0 = 0, v1 = 0, v2;
            if (a == 0 && d == 0) { v0 = 1; if (b == 0) { v2 = -a / c; v1 = (-d - f * v2) / e; } else if (c == 0) { v1 = -a / b; v2 = (-d - e * v1) / f; } else { v2 = (e * a - d * b) / (f * b - e * c); v1 = (-a - c * v2) / b; } }
            else if (b == 0 && e == 0) { v1 = 1; if (a == 0) { v2 = -b / c; v0 = (-e - f * v2) / d; } else if (c == 0) { v0 = -b / a; v2 = (-e - d * v0) / f; } else { v2 = (-a * e + d * b) / (a * f - d * c); v0 = (-b - c * v2) / a; } }
            else { v2 = 1.0; if (a == 0) { v1 = -c / b; v0 = (-f - e * v1) / d; } else if (b == 0) { v0 = -c / a; v1 = (-f - d * v0) / e; } else { v1 = (-a * f + d * c) / (-d * b + a * e); v0 = -1 * (c + b * v1) / a; } }
            double norm = System.Math.Sqrt(v0 * v0 + v1 * v1 + v2 * v2);
            return new Matrix(new double[] { v0 / norm, v1 / norm, v2 / norm });
        }

        /// <summary>
        /// <para><b>[FR]</b> Résout le système this·x = b par la méthode du gradient conjugué.
        /// Requiert une matrice symétrique définie positive et réelle.</para>
        /// <para><b>[EN]</b> Solves the system this·x = b using the conjugate gradient method.
        /// Requires a symmetric positive definite real matrix.</para>
        /// </summary>
        public Matrix SolveConjugateGradient(Matrix b)
        {
            if (!this.IsSPD()) throw new InvalidOperationException("CG method only works for SPD matrices.");
            if (!this.IsReal()) throw new InvalidOperationException("CG method only works for real matrices.");
            int n = rowCount, maxIter = 150;
            double tolerance = 1e-6;
            Matrix x = Ones(n, 1);
            Matrix r = b - this * x;
            Matrix dir = r;
            double delta = r.Norm(); delta *= delta;
            tolerance *= tolerance;
            Matrix h = Zeros(n, 1);
            double alpha, gamma, oldDelta;
            if (delta <= tolerance) return x;
            for (int i = 0; i < maxIter; i++)
            {
                h = this * dir;
                gamma = Dot(h, dir).Re;
                if (System.Math.Abs(gamma) <= tolerance) return x;
                alpha = delta / gamma;
                x += alpha * dir;
                r -= alpha * h;
                oldDelta = delta;
                delta = r.Norm(); delta *= delta;
                if (delta <= tolerance) return x;
                dir = r + delta / oldDelta * dir;
            }
            return x;
        }

        /// <summary>
        /// <para><b>[FR]</b> Exécute l'itération QR basique (sans décalage) sur la matrice réelle courante.</para>
        /// <para><b>[EN]</b> Executes the basic QR iteration (without shift) on the current real matrix.</para>
        /// </summary>
        /// <param name="maxIterations">
        /// <b>[FR]</b> Nombre maximal d'itérations. /
        /// <b>[EN]</b> Maximum number of iterations.
        /// </param>
        public Matrix QRIterationBasic(int maxIterations)
        {
            if (!this.IsReal()) throw new InvalidOperationException("Basic QR iteration is only possible for real matrices.");
            Matrix T = this.Clone();
            Matrix[] QR;
            for (int i = 0; i < maxIterations; i++) { QR = T.QRGramSchmidt(); T = QR[1] * QR[0]; }
            return T;
        }

        /// <summary>
        /// <para><b>[FR]</b> Itération QR avec réduction de Hessenberg préalable.</para>
        /// <para><b>[EN]</b> QR iteration using prior Hessenberg reduction.</para>
        /// </summary>
        /// <param name="maxIterations">
        /// <b>[FR]</b> Nombre maximal d'itérations. /
        /// <b>[EN]</b> Maximum number of iterations.
        /// </param>
        public Matrix QRIterationHessenberg(int maxIterations)
        {
            if (!this.IsSquare()) throw new InvalidOperationException("Cannot perform QR iteration of a non-square matrix.");
            int n = this.RowCount;
            Matrix[] TQ = this.HessenbergHouseholder();
            Matrix T = TQ[0];
            for (int j = 1; j <= maxIterations; j++)
            {
                Matrix[] QRcs = T.QRGivens();
                T = QRcs[1];
                for (int k = 1; k <= n - 1; k++)
                    T.ApplyGivensColumn(QRcs[2][k], QRcs[3][k], 1, k + 1, k, k + 1);
            }
            return T;
        }

        /// <summary>
        /// <para><b>[FR]</b> Décomposition QR par rotations de Givens.</para>
        /// <para><b>[EN]</b> QR factorization using Givens rotations.</para>
        /// </summary>
        public Matrix[] QRGivens()
        {
            Matrix H = this.Clone();
            int n = H.ColumnCount;
            Matrix c = Zeros(n - 1, 1), s = Zeros(n - 1, 1);
            Complex[] cs;
            for (int k = 1; k <= n - 1; k++)
            {
                cs = ComputeGivensRotation(H[k, k], H[k + 1, k]);
                c[k] = cs[0]; s[k] = cs[1];
                this.ApplyGivensRow(c[k], s[k], 1, k + 1, k, k + 1);
            }
            return new Matrix[] { ComputeGivensProduct(c, s, n), H, c, s };
        }

        /// <summary>
        /// <para><b>[FR]</b> Applique la rotation de Givens G(i,k,θ)ᵀ · this sur les lignes i et k,
        /// pour les colonnes j1 à j2.</para>
        /// <para><b>[EN]</b> Applies the Givens rotation G(i,k,θ)ᵀ · this on rows i and k,
        /// for columns j1 to j2.</para>
        /// </summary>
        public void ApplyGivensColumn(Complex c, Complex s, int j1, int j2, int i, int k)
        {
            for (int j = j1; j <= j2; j++)
            {
                Complex t1 = this[j, i], t2 = this[j, k];
                this[j, i] = c * t1 - s * t2;
                this[j, k] = s * t1 + c * t2;
            }
        }

        #endregion

        #region Scalar Methods / Méthodes scalaires

        /// <summary>
        /// <para><b>[FR]</b> Calcule le déterminant de la matrice carrée.</para>
        /// <para><b>[EN]</b> Computes the determinant of the square matrix.</para>
        /// </summary>
        public Complex Determinant()
        {
            if (!this.IsSquare()) throw new InvalidOperationException("Cannot compute determinant of a non-square matrix.");
            if (this.columnCount == 1) return this[1, 1];
            if (this.IsTrapezoid()) return this.DiagonalProduct();
            Matrix X = this.Clone();
            Matrix P = X.LUSafe();
            return (double)P.Signum() * X.DiagonalProduct();
        }

        /// <summary>
        /// <para><b>[FR]</b> Norme-1 (somme des colonnes) de la matrice. Alias de <see cref="L1Norm"/>.</para>
        /// <para><b>[EN]</b> Matrix 1-norm (column-sum norm). Alias for <see cref="L1Norm"/>.</para>
        /// </summary>
        public double ColumnSumNorm() { return this.L1Norm(); }

        /// <summary>
        /// <para><b>[FR]</b> Norme infinie (somme des lignes) de la matrice. Alias de <see cref="InfinityNorm"/>.</para>
        /// <para><b>[EN]</b> Matrix infinity norm (row-sum norm). Alias for <see cref="InfinityNorm"/>.</para>
        /// </summary>
        public double RowSumNorm() { return this.InfinityNorm(); }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le permanent de la matrice carrée.
        /// <b>Attention :</b> complexité exponentielle — à n'utiliser que pour de très petites matrices.</para>
        /// <para><b>[EN]</b> Computes the permanent of the square matrix.
        /// <b>Warning:</b> exponential runtime — only use for very small matrices.</para>
        /// </summary>
        public Complex Permanent()
        {
            if (!this.IsSquare()) throw new InvalidOperationException("Cannot compute permanent of a non-square matrix.");
            if (this.HasZeroRowOrColumn()) return Complex.Zero;
            if (this == Ones(rowCount)) return new Complex(ComputeFactorial(rowCount));
            if (this.IsPermutation()) return Complex.One;
            Complex buf = Complex.Zero;
            int minRow = this.GetMinRow(), minColumn = this.GetMinColumn();
            if (this.AbsoluteRowSum(minRow) < this.AbsoluteColumnSum(minColumn))
                for (int j = 1; j <= columnCount; j++)
                    if (this[minRow, j] != 0) buf += this[minRow, j] * this.Minor(minRow, j).Permanent();
                    else
                        for (int i = 1; i <= rowCount; i++)
                            if (this[i, minColumn] != 0) buf += this[i, minColumn] * this.Minor(i, minColumn).Permanent();
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Retourne l'indice (base 1) de la ligne ayant la plus petite somme absolue.</para>
        /// <para><b>[EN]</b> Returns the one-based index of the row with the smallest absolute row sum.</para>
        /// </summary>
        public int GetMinRow()
        {
            double buf = this.AbsoluteRowSum(1); int index = 1;
            for (int i = 2; i <= rowCount; i++) { double b2 = this.AbsoluteRowSum(i); if (b2 < buf) { buf = b2; index = i; } }
            return index;
        }

        /// <summary>
        /// <para><b>[FR]</b> Retourne l'indice (base 1) de la colonne ayant la plus petite somme absolue.</para>
        /// <para><b>[EN]</b> Returns the one-based index of the column with the smallest absolute column sum.</para>
        /// </summary>
        public int GetMinColumn()
        {
            double buf = this.AbsoluteColumnSum(1); int index = 1;
            for (int j = 2; j <= columnCount; j++) { double b2 = this.AbsoluteColumnSum(j); if (b2 < buf) { buf = b2; index = j; } }
            return index;
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le signe (signum) d'une matrice de permutation : +1 si le nombre
        /// d'échanges est pair, −1 sinon.</para>
        /// <para><b>[EN]</b> Computes the sign (signum) of a permutation matrix: +1 if the number
        /// of swaps is even, −1 otherwise.</para>
        /// </summary>
        public double Signum()
        {
            double buf = 1; int n = rowCount; double fi, fj;
            for (int i = 1; i < n; i++)
            {
                for (fi = 1; fi < n && this[i, (int)fi] != Complex.One; fi++) ;
                for (int j = i + 1; j <= n; j++)
                {
                    for (fj = 1; fj <= n && this[j, (int)fj] != Complex.One; fj++) ;
                    buf *= (fi - fj) / (i - j);
                }
            }
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le nombre de conditionnement par rapport à l'inversion via la norme-1 :
        /// cond(A) = ||A||₁ · ||A⁻¹||₁.</para>
        /// <para><b>[EN]</b> Computes the condition number with respect to inversion using the 1-norm:
        /// cond(A) = ||A||₁ · ||A⁻¹||₁.</para>
        /// </summary>
        public double Condition() { return this.L1Norm() * this.Inverse().L1Norm(); }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le nombre de conditionnement en utilisant la p-norme spécifiée.</para>
        /// <para><b>[EN]</b> Computes the condition number using the specified p-norm.</para>
        /// </summary>
        /// <param name="p">
        /// <b>[FR]</b> Indice de la norme (1 ou +∞). /
        /// <b>[EN]</b> Norm index (1 or +∞).
        /// </param>
        public double Condition(int p) { return this.PNorm(p) * this.Inverse().PNorm(p); }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le nombre de conditionnement via la norme de Frobenius :
        /// cond_F(A) = ||A||_F · ||A⁻¹||_F.</para>
        /// <para><b>[EN]</b> Computes the condition number using the Frobenius norm:
        /// cond_F(A) = ||A||_F · ||A⁻¹||_F.</para>
        /// </summary>
        public double ConditionFrobenius() { return this.FrobeniusNorm() * this.Inverse().FrobeniusNorm(); }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la p-norme de la matrice ou du vecteur :
        /// (∑|aᵢⱼ|ᵖ)^(1/p). Pour p = 1 ou p = +∞, des formules spécifiques s'appliquent.</para>
        /// <para><b>[EN]</b> Computes the p-norm of the matrix or vector:
        /// (∑|aᵢⱼ|ᵖ)^(1/p). For p = 1 or p = +∞, specific formulas apply.</para>
        /// </summary>
        public double PNorm(double p)
        {
            if (p <= 0) throw new ArgumentException("Argument must be greater than zero.");
            if (p == 1) return L1Norm();
            else if (p == double.PositiveInfinity) return InfinityNorm();
            int dim = this.VectorDimension();
            if (dim == 0) throw new InvalidOperationException("Cannot compute p-norm of a general matrix with this overload.");
            double buf = 0;
            for (int i = 1; i <= dim; i++) buf += System.Math.Pow(Complex.Abs(this[i]), p);
            return System.Math.Pow(buf, 1 / p);
        }

        /// <summary>
        /// <para><b>[FR]</b> Norme euclidienne (2-norme) d'un vecteur. Pour une matrice générale, utiliser <see cref="FrobeniusNorm"/>.</para>
        /// <para><b>[EN]</b> Euclidean (2-norm) of a vector. For a general matrix, use <see cref="FrobeniusNorm"/>.</para>
        /// </summary>
        public double Norm() { return PNorm(2); }

        /// <summary>
        /// <para><b>[FR]</b> Norme de Frobenius d'une matrice carrée : sqrt(∑ᵢⱼ |aᵢⱼ|²).</para>
        /// <para><b>[EN]</b> Frobenius norm of a square matrix: sqrt(∑ᵢⱼ |aᵢⱼ|²).</para>
        /// </summary>
        public double FrobeniusNorm()
        {
            if (!this.IsSquare()) throw new InvalidOperationException("Cannot compute Frobenius norm of a non-square matrix.");
            int n = this.columnCount; double buf = 0;
            for (int i = 1; i <= n; i++)
                for (int j = 1; j <= n; j++)
                    buf += (this[i, j] * Complex.Conj(this[i, j])).Re;
            return System.Math.Sqrt(buf);
        }

        /// <summary>
        /// <para><b>[FR]</b> Norme-1 matricielle : maximum des sommes absolues de colonnes.</para>
        /// <para><b>[EN]</b> Matrix 1-norm: maximum of absolute column sums.</para>
        /// </summary>
        public double L1Norm()
        {
            double buf = 0;
            int dim = this.VectorDimension();
            if (dim != 0) { for (int i = 1; i <= dim; i++) buf += Complex.Abs(this[i]); }
            else { double buf2; for (int j = 1; j <= columnCount; j++) { buf2 = AbsoluteColumnSum(j); if (buf2 > buf) buf = buf2; } }
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Norme infinie matricielle : maximum des sommes absolues de lignes.</para>
        /// <para><b>[EN]</b> Matrix infinity norm: maximum of absolute row sums.</para>
        /// </summary>
        public double InfinityNorm()
        {
            double buf = 0, buf2;
            int dim = this.VectorDimension();
            if (dim != 0) { for (int i = 1; i <= dim; i++) { buf2 = Complex.Abs(this[i]); if (buf2 > buf) buf = buf2; } }
            else { for (int i = 1; i <= rowCount; i++) { buf2 = AbsoluteRowSum(i); if (buf2 > buf) buf = buf2; } }
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la somme des éléments de la j-ième colonne (base 1).</para>
        /// <para><b>[EN]</b> Computes the sum of elements in the j-th column (one-based).</para>
        /// </summary>
        public Complex ColumnSum(int j)
        {
            if (j <= 0 || j > columnCount) throw new ArgumentException("Index out of range.");
            Complex buf = Complex.Zero; j--;
            for (int i = 0; i < rowCount; i++) buf += (Complex)(((ArrayList)Values[i])[j]);
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la somme des valeurs absolues des éléments de la j-ième colonne (base 1).</para>
        /// <para><b>[EN]</b> Computes the sum of absolute values of the j-th column elements (one-based).</para>
        /// </summary>
        public double AbsoluteColumnSum(int j)
        {
            if (j <= 0 || j > columnCount) throw new ArgumentException("Index out of range.");
            double buf = 0;
            for (int i = 1; i <= rowCount; i++) buf += Complex.Abs(this[i, j]);
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la somme des éléments de la i-ième ligne (base 1).</para>
        /// <para><b>[EN]</b> Computes the sum of elements in the i-th row (one-based).</para>
        /// </summary>
        public Complex RowSum(int i)
        {
            if (i <= 0 || i > rowCount) throw new ArgumentException("Index out of range.");
            Complex buf = Complex.Zero;
            ArrayList row = (ArrayList)Values[i - 1];
            for (int j = 0; j < columnCount; j++) buf += (Complex)(row[j]);
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la somme des valeurs absolues des éléments de la i-ième ligne (base 1).</para>
        /// <para><b>[EN]</b> Computes the sum of absolute values of the i-th row elements (one-based).</para>
        /// </summary>
        public double AbsoluteRowSum(int i)
        {
            if (i <= 0 || i > rowCount) throw new ArgumentException("Index out of range.");
            double buf = 0;
            for (int j = 1; j <= columnCount; j++) buf += Complex.Abs(this[i, j]);
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le produit des éléments de la diagonale principale.</para>
        /// <para><b>[EN]</b> Computes the product of the main diagonal elements.</para>
        /// </summary>
        public Complex DiagonalProduct()
        {
            Complex buf = Complex.One;
            int dim = System.Math.Min(this.rowCount, this.columnCount);
            for (int i = 1; i <= dim; i++) buf *= this[i, i];
            return buf;
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la trace de la matrice carrée : somme des éléments diagonaux.</para>
        /// <para><b>[EN]</b> Computes the trace of the square matrix: sum of diagonal elements.</para>
        /// </summary>
        public Complex Trace()
        {
            if (!this.IsSquare()) throw new InvalidOperationException("Cannot compute trace of a non-square matrix.");
            Complex buf = Complex.Zero;
            for (int i = 1; i <= this.rowCount; i++) buf += this[i, i];
            return buf;
        }

        #endregion

        #region Checks / Vérifications

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si la matrice est normale : A·A* = A*·A.</para>
        /// <para><b>[EN]</b> Checks whether the matrix is normal: A·A* = A*·A.</para>
        /// </summary>
        public bool IsNormal() { return (this * this.ConjugateTranspose() == this.ConjugateTranspose() * this); }

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si la matrice est unitaire : A*·A = I.</para>
        /// <para><b>[EN]</b> Checks whether the matrix is unitary: A*·A = I.</para>
        /// </summary>
        public bool IsUnitary() { return this.IsSquare() && (this.ConjugateTranspose() * this == Identity(rowCount)); }

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si la matrice est hermitienne : A* = A.</para>
        /// <para><b>[EN]</b> Checks whether the matrix is Hermitian: A* = A.</para>
        /// </summary>
        public bool IsHermitian() { return this.IsSquare() && this.ConjugateTranspose() == this; }

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si toutes les entrées de la matrice sont réelles (Im = 0).</para>
        /// <para><b>[EN]</b> Checks whether all matrix entries are real (Im = 0).</para>
        /// </summary>
        public bool IsReal()
        {
            for (int i = 1; i <= rowCount; i++)
                for (int j = 1; j <= columnCount; j++)
                    if (!this[i, j].IsReal()) return false;
            return true;
        }

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si la matrice est symétrique définie positive (SPD).</para>
        /// <para><b>[EN]</b> Checks whether the matrix is symmetric positive definite (SPD).</para>
        /// </summary>
        public bool IsSymmetricPositiveDefinite() { return this.IsSymmetric() && this.Definiteness() == DefinitenessType.PositiveDefinite; }

        /// <summary>
        /// <para><b>[FR]</b> Alias de <see cref="IsSymmetricPositiveDefinite"/> (SPD).</para>
        /// <para><b>[EN]</b> Alias for <see cref="IsSymmetricPositiveDefinite"/> (SPD).</para>
        /// </summary>
        public bool IsSPD() { return this.IsSymmetricPositiveDefinite(); }

        /// <summary>
        /// <para><b>[FR]</b> Détermine le type de définie-positivité de la matrice symétrique carrée.</para>
        /// <para><b>[EN]</b> Determines the definiteness type of the square symmetric matrix.</para>
        /// </summary>
        public DefinitenessType Definiteness()
        {
            if (!this.IsSquare()) throw new InvalidOperationException("Definiteness is undefined for non-square matrices.");
            if (this == Zeros(this.rowCount, this.columnCount)) return DefinitenessType.Indefinite;
            if (!this.IsSymmetric()) throw new InvalidOperationException("This test works only for symmetric matrices.");
            if (!this.IsReal()) throw new InvalidOperationException("This test works only for real matrices.");
            int n = this.rowCount;
            Matrix[] y = new Matrix[n + 1];
            for (int i = 0; i <= n; i++) y[i] = Matrix.Zeros(n, 1);
            y[1] = this.Column(1);
            Matrix xk, buf;
            for (int k = 2; k <= n; k++)
            {
                xk = this.Column(k); buf = Zeros(n, 1);
                for (int i = 1; i < k; i++) buf += y[i] * Dot(xk, this * y[i]) / Dot(y[i], this * y[i]);
                y[k] = xk - buf;
            }
            bool strict = true; Complex res;
            for (int i = 1; i < n; i++)
            {
                res = Dot(y[i], this * y[i]) * Dot(y[i + 1], this * y[i + 1]);
                if (res == 0) strict = false;
                else if (res.Re < 0) return DefinitenessType.Indefinite;
            }
            if (Dot(y[1], this * y[1]).Re >= 0)
                return strict ? DefinitenessType.PositiveDefinite : DefinitenessType.PositiveSemidefinite;
            else
                return strict ? DefinitenessType.NegativeDefinite : DefinitenessType.NegativeSemidefinite;
        }

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si la matrice contient une ligne ou une colonne entièrement nulle.</para>
        /// <para><b>[EN]</b> Checks whether the matrix has a row or column consisting entirely of zeros.</para>
        /// </summary>
        public bool HasZeroRowOrColumn()
        {
            for (int i = 1; i <= rowCount; i++) if (this.AbsoluteRowSum(i) == 0) return true;
            for (int i = 1; i <= columnCount; i++) if (this.AbsoluteColumnSum(i) == 0) return true;
            return false;
        }

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si la matrice est une matrice binaire (uniquement 0 et 1).</para>
        /// <para><b>[EN]</b> Checks whether the matrix is a binary matrix (only 0s and 1s).</para>
        /// </summary>
        public bool IsBinaryMatrix()
        {
            for (int i = 1; i <= rowCount; i++)
                for (int j = 1; j <= columnCount; j++)
                    if (this[i, j] != Complex.Zero && this[i, j] != Complex.One) return false;
            return true;
        }

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si la matrice est une matrice de permutation (permutation de l'identité).</para>
        /// <para><b>[EN]</b> Checks whether the matrix is a permutation matrix (permutation of identity).</para>
        /// </summary>
        public bool IsPermutation() { return !this.IsSquare() && this.IsBinaryMatrix() && this.IsInvolutory(); }

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si la matrice est diagonale.</para>
        /// <para><b>[EN]</b> Checks whether the matrix is diagonal.</para>
        /// </summary>
        public bool IsDiagonal() { return (this.Clone() - Diag(this.DiagonalVector()) == Zeros(rowCount, columnCount)); }

        /// <summary>
        /// <para><b>[FR]</b> Retourne la dimension du vecteur si la matrice est un vecteur (1×n ou n×1),
        /// sinon 0.</para>
        /// <para><b>[EN]</b> Returns the vector dimension if the matrix is a vector (1×n or n×1),
        /// otherwise 0.</para>
        /// </summary>
        public int VectorDimension()
        {
            if (columnCount > 1 && rowCount > 1) return 0;
            return System.Math.Max(columnCount, rowCount);
        }

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si la matrice est carrée (m = n).</para>
        /// <para><b>[EN]</b> Checks whether the matrix is square (m = n).</para>
        /// </summary>
        public bool IsSquare() { return (this.columnCount == this.rowCount); }

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si la matrice est involutoire : A·A = I.</para>
        /// <para><b>[EN]</b> Checks whether the matrix is involutory: A·A = I.</para>
        /// </summary>
        public bool IsInvolutory() { return (this * this == Identity(rowCount)); }

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si la matrice est symétrique : A[i,j] = A[j,i].</para>
        /// <para><b>[EN]</b> Checks whether the matrix is symmetric: A[i,j] = A[j,i].</para>
        /// </summary>
        public bool IsSymmetric()
        {
            for (int i = 1; i <= this.rowCount; i++)
                for (int j = 1; j <= this.columnCount; j++)
                    if (this[i, j] != this[j, i]) return false;
            return true;
        }

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si la matrice est orthogonale : A·Aᵀ = I.</para>
        /// <para><b>[EN]</b> Checks whether the matrix is orthogonal: A·Aᵀ = I.</para>
        /// </summary>
        public bool IsOrthogonal() { return this.IsSquare() && this * this.Transpose() == Identity(this.rowCount); }

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si la matrice est trapézoïdale (triangulaire supérieure ou inférieure).</para>
        /// <para><b>[EN]</b> Checks whether the matrix is trapezoidal (upper or lower triangular).</para>
        /// </summary>
        public bool IsTrapezoid() { return this.IsUpperTrapezoid() || this.IsLowerTrapezoid(); }

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si la matrice est triangulaire (carrée et trapézoïdale).</para>
        /// <para><b>[EN]</b> Checks whether the matrix is triangular (square and trapezoidal).</para>
        /// </summary>
        public bool IsTriangular() { return this.IsLowerTriangular() || this.IsUpperTriangular(); }

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si la matrice est triangulaire supérieure (carrée et trapézoïdale supérieure).</para>
        /// <para><b>[EN]</b> Checks whether the matrix is upper triangular (square and upper trapezoidal).</para>
        /// </summary>
        public bool IsUpperTriangular() { return this.IsSquare() && this.IsUpperTrapezoid(); }

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si la matrice est triangulaire inférieure (carrée et trapézoïdale inférieure).</para>
        /// <para><b>[EN]</b> Checks whether the matrix is lower triangular (square and lower trapezoidal).</para>
        /// </summary>
        public bool IsLowerTriangular() { return this.IsSquare() && this.IsLowerTrapezoid(); }

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si A[i,j] = 0 pour tous i > j (partie strictement inférieure nulle).</para>
        /// <para><b>[EN]</b> Checks whether A[i,j] = 0 for all i > j (strictly lower part is zero).</para>
        /// </summary>
        public bool IsUpperTrapezoid()
        {
            for (int j = 1; j <= columnCount; j++)
                for (int i = j + 1; i <= rowCount; i++)
                    if (this[i, j] != 0) return false;
            return true;
        }

        /// <summary>
        /// <para><b>[FR]</b> Vérifie si A[i,j] = 0 pour tous i &lt; j (partie strictement supérieure nulle).</para>
        /// <para><b>[EN]</b> Checks whether A[i,j] = 0 for all i &lt; j (strictly upper part is zero).</para>
        /// </summary>
        public bool IsLowerTrapezoid()
        {
            for (int i = 1; i <= rowCount; i++)
                for (int j = i + 1; j <= columnCount; j++)
                    if (this[i, j] != 0) return false;
            return true;
        }

        #endregion

        #region Operators & Overrides / Opérateurs et substitutions

        /// <inheritdoc/>
        public override string ToString()
        {
            string s = "";
            Complex buf;
            for (int i = 1; i <= rowCount; i++)
            {
                for (int j = 1; j <= columnCount; j++)
                {
                    buf = this[i, j];
                    if (buf.Re == double.PositiveInfinity || buf.Re == double.NegativeInfinity || buf.Im == double.PositiveInfinity || buf.Im == double.NegativeInfinity) s += "oo";
                    else if (double.IsNaN(buf.Re) || double.IsNaN(buf.Im)) s += "?";
                    else s += buf.ToString();
                    s += ";" + "\t";
                }
                s += "\\" + System.Environment.NewLine;
            }
            return s;
        }

        /// <summary>
        /// <para><b>[FR]</b> Retourne la représentation textuelle de la matrice avec un format numérique.</para>
        /// <para><b>[EN]</b> Returns the string representation of the matrix with a numeric format.</para>
        /// </summary>
        public string ToString(string format)
        {
            string s = "";
            Complex buf;
            for (int i = 1; i <= rowCount; i++)
            {
                for (int j = 1; j <= columnCount; j++)
                {
                    buf = this[i, j];
                    if (buf.Re == double.PositiveInfinity || buf.Re == double.NegativeInfinity || buf.Im == double.PositiveInfinity || buf.Im == double.NegativeInfinity) s += "oo";
                    else if (double.IsNaN(buf.Re) || double.IsNaN(buf.Im)) s += "?";
                    else s += buf.ToString(format);
                    s += ";" + "\t";
                }
                s += "\\" + System.Environment.NewLine;
            }
            return s;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) { return obj?.ToString() == this.ToString(); }

        /// <inheritdoc/>
        public override int GetHashCode() { return -1; }

        /// <summary>
        /// <para><b>[FR]</b> Égalité composante par composante entre deux matrices de même dimension.</para>
        /// <para><b>[EN]</b> Component-wise equality between two matrices of the same dimension.</para>
        /// </summary>
        public static bool operator ==(Matrix A, Matrix B)
        {
            if (A.RowCount != B.RowCount || A.ColumnCount != B.ColumnCount) return false;
            for (int i = 1; i <= A.RowCount; i++)
                for (int j = 1; j <= A.ColumnCount; j++)
                    if (A[i, j] != B[i, j]) return false;
            return true;
        }

        /// <summary>
        /// <para><b>[FR]</b> Inégalité composante par composante.</para>
        /// <para><b>[EN]</b> Component-wise inequality.</para>
        /// </summary>
        public static bool operator !=(Matrix A, Matrix B) { return !(A == B); }

        /// <summary>
        /// <para><b>[FR]</b> Addition composante par composante (matrices de même dimension).</para>
        /// <para><b>[EN]</b> Component-wise addition (matrices of the same dimension).</para>
        /// </summary>
        public static Matrix operator +(Matrix A, Matrix B)
        {
            if (A.RowCount != B.RowCount || A.ColumnCount != B.ColumnCount) throw new ArgumentException("Matrices must have the same dimension.");
            for (int i = 1; i <= A.RowCount; i++)
                for (int j = 1; j <= A.ColumnCount; j++)
                    A[i, j] += B[i, j];
            return A;
        }

        /// <summary>
        /// <para><b>[FR]</b> Soustraction composante par composante (matrices de même dimension).</para>
        /// <para><b>[EN]</b> Component-wise subtraction (matrices of the same dimension).</para>
        /// </summary>
        public static Matrix operator -(Matrix A, Matrix B)
        {
            if (A.RowCount != B.RowCount || A.ColumnCount != B.ColumnCount) throw new ArgumentException("Matrices must have the same dimension.");
            for (int i = 1; i <= A.RowCount; i++)
                for (int j = 1; j <= A.ColumnCount; j++)
                    A[i, j] -= B[i, j];
            return A;
        }

        /// <summary>
        /// <para><b>[FR]</b> Oppose de la matrice (changement de signe de toutes les entrées).</para>
        /// <para><b>[EN]</b> Negation of the matrix (sign change of all entries).</para>
        /// </summary>
        public static Matrix operator -(Matrix A)
        {
            for (int i = 1; i <= A.RowCount; i++)
                for (int j = 1; j <= A.ColumnCount; j++)
                    A[i, j] = -A[i, j];
            return A;
        }

        /// <summary>
        /// <para><b>[FR]</b> Multiplication matricielle standard : C = A·B (dimensions internes compatibles).</para>
        /// <para><b>[EN]</b> Standard matrix multiplication: C = A·B (compatible inner dimensions).</para>
        /// </summary>
        public static Matrix operator *(Matrix A, Matrix B)
        {
            if (A.ColumnCount != B.RowCount) throw new ArgumentException("Inner matrix dimensions must agree.");
            Matrix C = new Matrix(A.RowCount, B.ColumnCount);
            for (int i = 1; i <= A.RowCount; i++)
                for (int j = 1; j <= B.ColumnCount; j++)
                    C[i, j] = Dot(A.Row(i), B.Column(j));
            return C;
        }

        /// <summary>
        /// <para><b>[FR]</b> Multiplication d'une matrice par un scalaire complexe.</para>
        /// <para><b>[EN]</b> Multiplication of a matrix by a complex scalar.</para>
        /// </summary>
        public static Matrix operator *(Matrix A, Complex x)
        {
            Matrix B = new Matrix(A.rowCount, A.columnCount);
            for (int i = 1; i <= A.RowCount; i++)
                for (int j = 1; j <= A.ColumnCount; j++)
                    B[i, j] = A[i, j] * x;
            return B;
        }

        /// <summary>
        /// <para><b>[FR]</b> Multiplication d'un scalaire complexe par une matrice.</para>
        /// <para><b>[EN]</b> Multiplication of a complex scalar by a matrix.</para>
        /// </summary>
        public static Matrix operator *(Complex x, Matrix A)
        {
            Matrix B = new Matrix(A.RowCount, A.ColumnCount);
            for (int i = 1; i <= A.RowCount; i++)
                for (int j = 1; j <= A.ColumnCount; j++)
                    B[i, j] = A[i, j] * x;
            return B;
        }

        /// <summary>
        /// <para><b>[FR]</b> Multiplication d'une matrice par un scalaire réel.</para>
        /// <para><b>[EN]</b> Multiplication of a matrix by a real scalar.</para>
        /// </summary>
        public static Matrix operator *(Matrix A, double x) { return (new Complex(x)) * A; }

        /// <summary>
        /// <para><b>[FR]</b> Multiplication d'un scalaire réel par une matrice.</para>
        /// <para><b>[EN]</b> Multiplication of a real scalar by a matrix.</para>
        /// </summary>
        public static Matrix operator *(double x, Matrix A) { return (new Complex(x)) * A; }

        /// <summary>
        /// <para><b>[FR]</b> Division d'une matrice par un scalaire complexe.</para>
        /// <para><b>[EN]</b> Division of a matrix by a complex scalar.</para>
        /// </summary>
        public static Matrix operator /(Matrix A, Complex x) { return (1 / x) * A; }

        /// <summary>
        /// <para><b>[FR]</b> Division d'une matrice par un scalaire réel.</para>
        /// <para><b>[EN]</b> Division of a matrix by a real scalar.</para>
        /// </summary>
        public static Matrix operator /(Matrix A, double x) { return (new Complex(1 / x)) * A; }

        /// <summary>
        /// <para><b>[FR]</b> Puissance entière de la matrice carrée.
        /// k = 0 → identité, k &lt; 0 → puissance de l'inverse, k ≥ 1 → produit k fois.</para>
        /// <para><b>[EN]</b> Integer power of the square matrix.
        /// k = 0 → identity, k &lt; 0 → power of the inverse, k ≥ 1 → k-fold product.</para>
        /// <remarks>
        /// <b>[FR]</b> L'opérateur ^ en C# est habituellement réservé au XOR ; ici il est réaffecté à la puissance matricielle. /
        /// <b>[EN]</b> The ^ operator in C# is usually reserved for XOR; here it is reassigned to matrix power.
        /// </remarks>
        /// </summary>
        public static Matrix operator ^(Matrix A, int k)
        {
            if (k < 0)
                return A.IsSquare() ? A.InverseLeverrier() ^ (-k) : throw new InvalidOperationException("Cannot take a non-square matrix to a negative power.");
            if (k == 0)
                return A.IsSquare() ? Matrix.Identity(A.RowCount) : throw new InvalidOperationException("Cannot take a non-square matrix to the power of zero.");
            if (k == 1)
                return A.IsSquare() ? A : throw new InvalidOperationException("Cannot take a non-square matrix to the power of one.");
            Matrix M = A;
            for (int i = 1; i < k; i++) M *= A;
            return M;
        }

        #endregion

        #region Indexers / Indexeurs

        /// <summary>
        /// <para><b>[FR]</b> Accès à l'élément (i, j) de la matrice (indices en base 1).
        /// La matrice est agrandie dynamiquement si les indices dépassent les dimensions courantes.</para>
        /// <para><b>[EN]</b> Access to element (i, j) of the matrix (one-based indices).
        /// The matrix is dynamically expanded if indices exceed the current dimensions.</para>
        /// </summary>
        public virtual Complex this[int i, int j]
        {
            set
            {
                if (i <= 0 || j <= 0) throw new ArgumentOutOfRangeException("Indices must be strictly positive.");
                if (i > rowCount)
                {
                    for (int k = 0; k < i - rowCount; k++)
                    {
                        this.Values.Add(new ArrayList(columnCount));
                        for (int t = 0; t < columnCount; t++)
                            ((ArrayList)Values[rowCount + k]).Add(Complex.Zero);
                    }
                    rowCount = i;
                }
                if (j > columnCount)
                {
                    for (int k = 0; k < rowCount; k++)
                        for (int t = 0; t < j - columnCount; t++)
                            ((ArrayList)Values[k]).Add(Complex.Zero);
                    columnCount = j;
                }
                ((ArrayList)Values[i - 1])[j - 1] = value;
            }
            get
            {
                if (i > 0 && i <= rowCount && j > 0 && j <= columnCount)
                    return (Complex)(((ArrayList)Values[i - 1])[j - 1]);
                else
                    throw new ArgumentOutOfRangeException("Indices must not exceed matrix dimensions.");
            }
        }

        /// <summary>
        /// <para><b>[FR]</b> Accès à la i-ième composante d'un vecteur ligne (1×n) ou colonne (n×1) (base 1).</para>
        /// <para><b>[EN]</b> Access to the i-th component of a row (1×n) or column (n×1) vector (one-based).</para>
        /// </summary>
        public virtual Complex this[int i]
        {
            set
            {
                if (rowCount == 1)
                {
                    if (i > columnCount)
                    {
                        for (int t = 0; t < i - columnCount; t++)
                            ((ArrayList)Values[0]).Add(Complex.Zero);
                        columnCount = i;
                    }
                    ((ArrayList)Values[0])[i - 1] = value;
                }
                else if (columnCount == 1)
                {
                    if (i > rowCount)
                    {
                        for (int k = 0; k < i - rowCount; k++)
                        {
                            this.Values.Add(new ArrayList(columnCount));
                            ((ArrayList)Values[rowCount + k]).Add(Complex.Zero);
                        }
                        rowCount = i;
                    }
                    ((ArrayList)Values[i - 1])[0] = value;
                }
                else throw new InvalidOperationException("Cannot access a multi-dimensional matrix via a single index.");
            }
            get
            {
                if (this.RowCount == 1 && this.ColumnCount >= i) return (Complex)(((ArrayList)Values[0])[i - 1]);
                else if (this.RowCount == 1) return (Complex)(((ArrayList)Values[0])[this.ColumnCount - 1]);
                else if (this.ColumnCount == 1 && this.RowCount >= i) return (Complex)(((ArrayList)Values[i - 1])[0]);
                else if (this.ColumnCount == 1) return (Complex)(((ArrayList)Values[this.RowCount - 1])[0]);
                else throw new InvalidOperationException("General matrix access requires double indexing.");
            }
        }

        #endregion

        #region Private Methods / Méthodes privées

        /// <summary>
        /// <para><b>[FR]</b> Calcule le vecteur de Householder et le coefficient β associé.</para>
        /// <para><b>[EN]</b> Computes the Householder vector and associated coefficient β.</para>
        /// </summary>
        private static Matrix[] ComputeHouseholderVector(Matrix x)
        {
            int n = x.VectorDimension();
            if (n == 0) throw new InvalidOperationException("Expected a vector as argument.");
            Matrix y = x / x.Norm();
            Matrix buf = y.Extract(2, n, 1, 1);
            Complex s = Dot(buf, buf);
            Matrix v = Zeros(n, 1);
            v[1] = Complex.One;
            v.Insert(2, 1, buf);
            double beta = 0;
            if (s != 0)
            {
                Complex mu = Complex.Sqrt(y[1] * y[1] + s);
                v[1] = y[1].Re <= 0 ? y[1] - mu : -s / (y[1] + mu);
                beta = 2 * v[1].Re * v[1].Re / (s.Re + v[1].Re * v[1].Re);
                v = v / v[1];
            }
            return new Matrix[] { v, new Matrix(beta) };
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le produit de rotations de Givens Q à partir des vecteurs c et s.</para>
        /// <para><b>[EN]</b> Computes the Givens rotation product matrix Q from vectors c and s.</para>
        /// </summary>
        private Matrix ComputeGivensProduct(Matrix c, Matrix s, int n)
        {
            int n1 = n - 1, n2 = n - 2;
            Matrix Q = Eye(n);
            Q[n1, n1] = c[n1]; Q[n, n] = c[n1]; Q[n1, n] = s[n1]; Q[n, n1] = -s[n1];
            for (int k = n2; k >= 1; k--)
            {
                int k1 = k + 1;
                Q[k, k] = c[k]; Q[k1, k] = -s[k];
                Matrix q = Q.Extract(k1, k1, k1, n);
                Q.Insert(k, k1, s[k] * q);
                Q.Insert(k1, k1, c[k] * q);
            }
            return Q;
        }

        /// <summary>
        /// <para><b>[FR]</b> Applique la rotation de Givens G(i,k,θ)ᵀ · this sur les lignes i et k,
        /// pour les colonnes j1 à j2 (usage interne QR).</para>
        /// <para><b>[EN]</b> Applies the Givens rotation G(i,k,θ)ᵀ · this on rows i and k,
        /// for columns j1 to j2 (internal QR use).</para>
        /// </summary>
        private void ApplyGivensRow(Complex c, Complex s, int i, int k, int j1, int j2)
        {
            for (int j = j1; j <= j2; j++)
            {
                Complex t1 = this[i, j], t2 = this[k, j];
                this[i, j] = c * t1 - s * t2;
                this[k, j] = s * t1 + c * t2;
            }
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le cosinus et le sinus de la rotation de Givens pour annuler xk.</para>
        /// <para><b>[EN]</b> Computes the cosine and sine of the Givens rotation to zero out xk.</para>
        /// </summary>
        private Complex[] ComputeGivensRotation(Complex xi, Complex xk)
        {
            Complex c = Complex.Zero, s = Complex.Zero;
            if (xk == 0) { c = Complex.One; }
            else if (Complex.Abs(xk) > Complex.Abs(xi))
            { Complex t = -xi / xk; s = 1 / (Complex.Sqrt(1 + t * t)); c = s * t; }
            else
            { Complex t = -xk / xi; c = 1 / (Complex.Sqrt(1 + t * t)); s = c * t; }
            return new Complex[] { c, s };
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le carré d'un nombre complexe : x².</para>
        /// <para><b>[EN]</b> Computes the square of a complex number: x².</para>
        /// </summary>
        private Complex Square(Complex x) { return x * x; }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la factorielle de x : x!</para>
        /// <para><b>[EN]</b> Computes the factorial of x: x!</para>
        /// </summary>
        private double ComputeFactorial(int x)
        {
            double buf = 1;
            for (int i = 2; i <= x; i++) buf *= i;
            return buf;
        }

        #endregion

        #region Enums / Énumérations

        /// <summary>
        /// <para><b>[FR]</b> Type de définie-positivité d'une matrice symétrique.</para>
        /// <para><b>[EN]</b> Definiteness type of a symmetric matrix.</para>
        /// </summary>
        public enum DefinitenessType
        {
            /// <summary><b>[FR]</b> Définie positive. / <b>[EN]</b> Positive definite.</summary>
            PositiveDefinite,
            /// <summary><b>[FR]</b> Semi-définie positive. / <b>[EN]</b> Positive semi-definite.</summary>
            PositiveSemidefinite,
            /// <summary><b>[FR]</b> Définie négative. / <b>[EN]</b> Negative definite.</summary>
            NegativeDefinite,
            /// <summary><b>[FR]</b> Semi-définie négative. / <b>[EN]</b> Negative semi-definite.</summary>
            NegativeSemidefinite,
            /// <summary><b>[FR]</b> Indéfinie. / <b>[EN]</b> Indefinite.</summary>
            Indefinite
        }

        #endregion
    }
}