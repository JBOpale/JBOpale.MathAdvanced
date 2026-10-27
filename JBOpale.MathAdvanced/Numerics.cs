// SPDX-License-Identifier: MIT
// Copyright (C) 2026 Jérôme FRANCOIS / JB-Opale — voir LICENSE / see LICENSE
using System;

namespace JBOPaleAPI.MathAdvanced
{
    /// <summary>
    /// <para><b>[FR]</b> Outils numériques : bornage, interpolation, comparaison de réels,
    /// dérivation, intégration (Simpson) et recherche de racines (dichotomie, Newton).</para>
    /// <para><b>[EN]</b> Numerical tools: clamping, interpolation, floating-point comparison,
    /// differentiation, integration (Simpson) and root finding (bisection, Newton).</para>
    /// </summary>
    public static class Numerics
    {
        /// <summary>
        /// <para><b>[FR]</b> Borne <paramref name="value"/> dans [<paramref name="min"/>, <paramref name="max"/>].</para>
        /// <para><b>[EN]</b> Clamps <paramref name="value"/> into [<paramref name="min"/>, <paramref name="max"/>].</para>
        /// </summary>
        /// <exception cref="ArgumentException"><b>[FR]</b> Si min &gt; max. / <b>[EN]</b> If min &gt; max.</exception>
        public static double Clamp(double value, double min, double max)
        {
            if (min > max) throw new ArgumentException("min doit être <= max. / min must be <= max.");
            return value < min ? min : (value > max ? max : value);
        }

        /// <summary>
        /// <para><b>[FR]</b> Interpolation linéaire : <c>a + (b − a)·t</c> (t non borné).</para>
        /// <para><b>[EN]</b> Linear interpolation: <c>a + (b − a)·t</c> (t unclamped).</para>
        /// </summary>
        public static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }

        /// <summary>
        /// <para><b>[FR]</b> Compare deux réels avec une tolérance relative et absolue (<paramref name="epsilon"/>).</para>
        /// <para><b>[EN]</b> Compares two doubles with a relative and absolute tolerance (<paramref name="epsilon"/>).</para>
        /// </summary>
        public static bool AlmostEqual(double a, double b, double epsilon = 1e-9)
        {
            if (a == b) return true;
            double diff = System.Math.Abs(a - b);
            double scale = System.Math.Max(1.0, System.Math.Max(System.Math.Abs(a), System.Math.Abs(b)));
            return diff <= epsilon * scale;
        }

        /// <summary>
        /// <para><b>[FR]</b> Dérivée numérique en <paramref name="x"/> par différences centrées.</para>
        /// <para><b>[EN]</b> Numerical derivative at <paramref name="x"/> using central differences.</para>
        /// </summary>
        public static double Derivative(Func<double, double> f, double x, double h = 1e-5)
        {
            if (f == null) throw new ArgumentNullException(nameof(f));
            if (h <= 0) throw new ArgumentOutOfRangeException(nameof(h));
            return (f(x + h) - f(x - h)) / (2 * h);
        }

        /// <summary>
        /// <para><b>[FR]</b> Intégrale de <paramref name="f"/> sur [a, b] par la méthode de Simpson composée
        /// (<paramref name="intervals"/> pair et positif).</para>
        /// <para><b>[EN]</b> Integral of <paramref name="f"/> over [a, b] using composite Simpson's rule
        /// (<paramref name="intervals"/> must be positive and even).</para>
        /// </summary>
        public static double Integrate(Func<double, double> f, double a, double b, int intervals = 1000)
        {
            if (f == null) throw new ArgumentNullException(nameof(f));
            if (intervals <= 0 || intervals % 2 != 0) throw new ArgumentOutOfRangeException(nameof(intervals));
            double h = (b - a) / intervals;
            double sum = f(a) + f(b);
            for (int i = 1; i < intervals; i++)
                sum += f(a + i * h) * (i % 2 == 0 ? 2 : 4);
            return sum * h / 3.0;
        }

        /// <summary>
        /// <para><b>[FR]</b> Racine de <paramref name="f"/> dans [a, b] par dichotomie. f(a) et f(b) doivent être de signes opposés.</para>
        /// <para><b>[EN]</b> Root of <paramref name="f"/> in [a, b] by bisection. f(a) and f(b) must have opposite signs.</para>
        /// </summary>
        /// <exception cref="ArgumentException"><b>[FR]</b> Si f(a) et f(b) ont le même signe. / <b>[EN]</b> If f(a) and f(b) have the same sign.</exception>
        public static double Bisection(Func<double, double> f, double a, double b, double tolerance = 1e-12, int maxIterations = 200)
        {
            if (f == null) throw new ArgumentNullException(nameof(f));
            double fa = f(a), fb = f(b);
            if (fa == 0) return a;
            if (fb == 0) return b;
            if (System.Math.Sign(fa) == System.Math.Sign(fb))
                throw new ArgumentException("f(a) et f(b) doivent être de signes opposés. / f(a) and f(b) must have opposite signs.");
            for (int i = 0; i < maxIterations && System.Math.Abs(b - a) > tolerance; i++)
            {
                double mid = (a + b) / 2;
                double fm = f(mid);
                if (fm == 0) return mid;
                if (System.Math.Sign(fm) == System.Math.Sign(fa)) { a = mid; fa = fm; }
                else { b = mid; }
            }
            return (a + b) / 2;
        }

        /// <summary>
        /// <para><b>[FR]</b> Racine de <paramref name="f"/> par la méthode de Newton à partir de <paramref name="x0"/>
        /// (dérivée numérique si <paramref name="df"/> est null).</para>
        /// <para><b>[EN]</b> Root of <paramref name="f"/> using Newton's method from <paramref name="x0"/>
        /// (numerical derivative when <paramref name="df"/> is null).</para>
        /// </summary>
        /// <exception cref="InvalidOperationException"><b>[FR]</b> Si la méthode ne converge pas. / <b>[EN]</b> If the method does not converge.</exception>
        public static double Newton(Func<double, double> f, double x0, Func<double, double>? df = null, double tolerance = 1e-12, int maxIterations = 100)
        {
            if (f == null) throw new ArgumentNullException(nameof(f));
            double x = x0;
            for (int i = 0; i < maxIterations; i++)
            {
                double fx = f(x);
                if (System.Math.Abs(fx) <= tolerance) return x;
                double d = df != null ? df(x) : Derivative(f, x);
                if (d == 0 || double.IsNaN(d)) break;
                double next = x - fx / d;
                if (double.IsNaN(next) || double.IsInfinity(next)) break;
                if (System.Math.Abs(next - x) <= tolerance) return next;
                x = next;
            }
            throw new InvalidOperationException("Newton n'a pas convergé. / Newton did not converge.");
        }
    }
}
