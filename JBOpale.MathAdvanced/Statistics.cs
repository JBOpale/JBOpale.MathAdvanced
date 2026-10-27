// SPDX-License-Identifier: MIT
// Copyright (C) 2026 Jérôme FRANCOIS / JB-Opale — voir LICENSE / see LICENSE
using System;
using System.Collections.Generic;
using System.Linq;

namespace JBOpale.MathAdvanced
{
    /// <summary>
    /// <para><b>[FR]</b> Statistiques descriptives et régression linéaire simple.</para>
    /// <para><b>[EN]</b> Descriptive statistics and simple linear regression.</para>
    /// </summary>
    public static class Statistics
    {
        private static double[] ToArray(IEnumerable<double> values, string name)
        {
            if (values == null) throw new ArgumentNullException(name);
            double[] array = values.ToArray();
            if (array.Length == 0) throw new ArgumentException("La séquence est vide. / The sequence is empty.", name);
            return array;
        }

        /// <summary>
        /// <para><b>[FR]</b> Moyenne arithmétique.</para>
        /// <para><b>[EN]</b> Arithmetic mean.</para>
        /// </summary>
        public static double Mean(IEnumerable<double> values)
        {
            double[] v = ToArray(values, nameof(values));
            return v.Sum() / v.Length;
        }

        /// <summary>
        /// <para><b>[FR]</b> Médiane (moyenne des deux valeurs centrales si l'effectif est pair).</para>
        /// <para><b>[EN]</b> Median (mean of the two middle values when the count is even).</para>
        /// </summary>
        public static double Median(IEnumerable<double> values)
        {
            return Percentile(values, 50);
        }

        /// <summary>
        /// <para><b>[FR]</b> Percentile avec interpolation linéaire (méthode « inclusive » du tableur), p entre 0 et 100.</para>
        /// <para><b>[EN]</b> Percentile with linear interpolation (spreadsheet "inclusive" method), p between 0 and 100.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><b>[FR]</b> Si p hors de [0, 100]. / <b>[EN]</b> If p is outside [0, 100].</exception>
        public static double Percentile(IEnumerable<double> values, double p)
        {
            if (double.IsNaN(p) || p < 0 || p > 100) throw new ArgumentOutOfRangeException(nameof(p));
            double[] v = ToArray(values, nameof(values));
            Array.Sort(v);
            double rank = p / 100.0 * (v.Length - 1);
            int low = (int)System.Math.Floor(rank);
            int high = (int)System.Math.Ceiling(rank);
            return v[low] + (rank - low) * (v[high] - v[low]);
        }

        /// <summary>
        /// <para><b>[FR]</b> Variance. <paramref name="sample"/> = true : variance d'échantillon (n − 1), sinon de population (n).</para>
        /// <para><b>[EN]</b> Variance. <paramref name="sample"/> = true: sample variance (n − 1), otherwise population (n).</para>
        /// </summary>
        /// <exception cref="ArgumentException"><b>[FR]</b> Si échantillon de moins de 2 valeurs. / <b>[EN]</b> If sample has fewer than 2 values.</exception>
        public static double Variance(IEnumerable<double> values, bool sample = true)
        {
            double[] v = ToArray(values, nameof(values));
            if (sample && v.Length < 2) throw new ArgumentException("Au moins 2 valeurs requises. / At least 2 values required.", nameof(values));
            double mean = v.Sum() / v.Length;
            double sum = 0;
            foreach (double x in v) sum += (x - mean) * (x - mean);
            return sum / (sample ? v.Length - 1 : v.Length);
        }

        /// <summary>
        /// <para><b>[FR]</b> Écart-type (racine de la variance).</para>
        /// <para><b>[EN]</b> Standard deviation (square root of the variance).</para>
        /// </summary>
        public static double StandardDeviation(IEnumerable<double> values, bool sample = true)
        {
            return System.Math.Sqrt(Variance(values, sample));
        }

        /// <summary>
        /// <para><b>[FR]</b> Covariance de deux séries de même longueur (population si <paramref name="sample"/> = false).</para>
        /// <para><b>[EN]</b> Covariance of two equal-length series (population when <paramref name="sample"/> = false).</para>
        /// </summary>
        public static double Covariance(IEnumerable<double> x, IEnumerable<double> y, bool sample = true)
        {
            double[] a = ToArray(x, nameof(x));
            double[] b = ToArray(y, nameof(y));
            if (a.Length != b.Length) throw new ArgumentException("Les séries doivent avoir la même longueur. / Series must have the same length.");
            if (sample && a.Length < 2) throw new ArgumentException("Au moins 2 valeurs requises. / At least 2 values required.");
            double ma = a.Sum() / a.Length, mb = b.Sum() / b.Length;
            double sum = 0;
            for (int i = 0; i < a.Length; i++) sum += (a[i] - ma) * (b[i] - mb);
            return sum / (sample ? a.Length - 1 : a.Length);
        }

        /// <summary>
        /// <para><b>[FR]</b> Coefficient de corrélation de Pearson dans [−1, 1]. <c>NaN</c> si une série est constante.</para>
        /// <para><b>[EN]</b> Pearson correlation coefficient in [−1, 1]. <c>NaN</c> if a series is constant.</para>
        /// </summary>
        public static double Correlation(IEnumerable<double> x, IEnumerable<double> y)
        {
            double[] a = ToArray(x, nameof(x));
            double[] b = ToArray(y, nameof(y));
            double denom = System.Math.Sqrt(Variance(a, false) * Variance(b, false));
            if (denom == 0) return double.NaN;
            return Covariance(a, b, false) / denom;
        }

        /// <summary>
        /// <para><b>[FR]</b> Régression linéaire par moindres carrés y = pente·x + ordonnée. Retourne le coefficient de détermination R².</para>
        /// <para><b>[EN]</b> Least-squares linear regression y = slope·x + intercept. Returns the coefficient of determination R².</para>
        /// </summary>
        /// <exception cref="ArgumentException"><b>[FR]</b> Si x est constant (pente indéfinie). / <b>[EN]</b> If x is constant (undefined slope).</exception>
        public static double LinearRegression(IEnumerable<double> x, IEnumerable<double> y, out double slope, out double intercept)
        {
            double[] a = ToArray(x, nameof(x));
            double[] b = ToArray(y, nameof(y));
            double varX = Variance(a, false);
            if (varX == 0) throw new ArgumentException("x est constant. / x is constant.", nameof(x));
            slope = Covariance(a, b, false) / varX;
            intercept = b.Sum() / b.Length - slope * (a.Sum() / a.Length);
            double r = Correlation(a, b);
            return double.IsNaN(r) ? 1.0 : r * r;
        }
    }
}
