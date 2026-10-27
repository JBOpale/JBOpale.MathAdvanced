// SPDX-License-Identifier: MIT
// Copyright (C) 2026 Jérôme FRANCOIS / JB-Opale — voir LICENSE / see LICENSE
using System;
using System.Collections.Generic;

namespace JBOpale.MathAdvanced
{
    /// <summary>
    /// <para><b>[FR]</b> Fonctions d'arithmétique et de théorie des nombres sur entiers
    /// (PGCD, PPCM, primalité, factorisation, factorielle, combinaisons, Fibonacci, puissance modulaire).</para>
    /// <para><b>[EN]</b> Integer arithmetic and number theory helpers
    /// (GCD, LCM, primality, factorisation, factorial, binomial, Fibonacci, modular exponentiation).</para>
    /// </summary>
    public static class NumberTheory
    {
        /// <summary>
        /// <para><b>[FR]</b> Plus grand commun diviseur (algorithme d'Euclide). Toujours positif ; <c>Gcd(0, 0) = 0</c>.</para>
        /// <para><b>[EN]</b> Greatest common divisor (Euclid's algorithm). Always non-negative; <c>Gcd(0, 0) = 0</c>.</para>
        /// </summary>
        /// <exception cref="OverflowException"><b>[FR]</b> Si une valeur vaut <c>long.MinValue</c>. / <b>[EN]</b> If a value is <c>long.MinValue</c>.</exception>
        public static long Gcd(long a, long b)
        {
            a = checked(System.Math.Abs(a));
            b = checked(System.Math.Abs(b));
            while (b != 0)
            {
                long t = a % b;
                a = b;
                b = t;
            }
            return a;
        }

        /// <summary>
        /// <para><b>[FR]</b> Plus petit commun multiple. Retourne 0 si l'une des valeurs est nulle.</para>
        /// <para><b>[EN]</b> Least common multiple. Returns 0 if either value is zero.</para>
        /// </summary>
        /// <exception cref="OverflowException"><b>[FR]</b> Si le résultat dépasse <c>long</c>. / <b>[EN]</b> If the result exceeds <c>long</c>.</exception>
        public static long Lcm(long a, long b)
        {
            if (a == 0 || b == 0) return 0;
            return checked(System.Math.Abs(a / Gcd(a, b) * b));
        }

        /// <summary>
        /// <para><b>[FR]</b> Indique si <paramref name="n"/> est premier (division d'essai en 6k ± 1).</para>
        /// <para><b>[EN]</b> Tells whether <paramref name="n"/> is prime (6k ± 1 trial division).</para>
        /// </summary>
        public static bool IsPrime(long n)
        {
            if (n < 2) return false;
            if (n < 4) return true;
            if (n % 2 == 0 || n % 3 == 0) return false;
            for (long i = 5; i <= n / i; i += 6)
            {
                if (n % i == 0 || n % (i + 2) == 0) return false;
            }
            return true;
        }

        /// <summary>
        /// <para><b>[FR]</b> Décomposition en facteurs premiers, par ordre croissant avec répétitions
        /// (ex. 360 → 2, 2, 2, 3, 3, 5). Liste vide pour <paramref name="n"/> &lt; 2.</para>
        /// <para><b>[EN]</b> Prime factorisation in ascending order with repetitions
        /// (e.g. 360 → 2, 2, 2, 3, 3, 5). Empty list for <paramref name="n"/> &lt; 2.</para>
        /// </summary>
        public static List<long> PrimeFactors(long n)
        {
            List<long> factors = new List<long>();
            if (n < 2) return factors;
            for (long d = 2; d <= n / d; d++)
            {
                while (n % d == 0)
                {
                    factors.Add(d);
                    n /= d;
                }
            }
            if (n > 1) factors.Add(n);
            return factors;
        }

        /// <summary>
        /// <para><b>[FR]</b> Factorielle n! en <c>double</c>. Retourne <c>+∞</c> au-delà de 170.</para>
        /// <para><b>[EN]</b> Factorial n! as a <c>double</c>. Returns <c>+∞</c> beyond 170.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><b>[FR]</b> Si n &lt; 0. / <b>[EN]</b> If n &lt; 0.</exception>
        public static double Factorial(int n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n));
            double result = 1;
            for (int i = 2; i <= n; i++) result *= i;
            return result;
        }

        /// <summary>
        /// <para><b>[FR]</b> Coefficient binomial C(n, k) (nombre de combinaisons), arrondi à l'entier le plus proche.
        /// Retourne 0 si k &gt; n.</para>
        /// <para><b>[EN]</b> Binomial coefficient C(n, k) (number of combinations), rounded to the nearest integer.
        /// Returns 0 if k &gt; n.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><b>[FR]</b> Si n ou k &lt; 0. / <b>[EN]</b> If n or k &lt; 0.</exception>
        public static double Binomial(int n, int k)
        {
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n));
            if (k < 0) throw new ArgumentOutOfRangeException(nameof(k));
            if (k > n) return 0;
            k = System.Math.Min(k, n - k);
            double result = 1;
            for (int i = 1; i <= k; i++) result = result * (n - k + i) / i;
            return System.Math.Round(result);
        }

        /// <summary>
        /// <para><b>[FR]</b> n-ième terme de la suite de Fibonacci (F0 = 0, F1 = 1), exact jusqu'à n = 92.</para>
        /// <para><b>[EN]</b> n-th Fibonacci number (F0 = 0, F1 = 1), exact up to n = 92.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><b>[FR]</b> Si n &lt; 0. / <b>[EN]</b> If n &lt; 0.</exception>
        /// <exception cref="OverflowException"><b>[FR]</b> Si n &gt; 92. / <b>[EN]</b> If n &gt; 92.</exception>
        public static long Fibonacci(int n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n));
            if (n < 2) return n;
            long a = 0, b = 1;
            for (int i = 2; i <= n; i++)
            {
                long t = checked(a + b);
                a = b;
                b = t;
            }
            return b;
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule (<paramref name="baseValue"/> ^ <paramref name="exponent"/>) mod <paramref name="modulus"/>
        /// par exponentiation rapide, sans dépassement de capacité.</para>
        /// <para><b>[EN]</b> Computes (<paramref name="baseValue"/> ^ <paramref name="exponent"/>) mod <paramref name="modulus"/>
        /// using fast exponentiation, without overflow.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><b>[FR]</b> Si exposant &lt; 0 ou module &lt;= 0. / <b>[EN]</b> If exponent &lt; 0 or modulus &lt;= 0.</exception>
        public static long PowMod(long baseValue, long exponent, long modulus)
        {
            if (exponent < 0) throw new ArgumentOutOfRangeException(nameof(exponent));
            if (modulus <= 0) throw new ArgumentOutOfRangeException(nameof(modulus));
            ulong m = (ulong)modulus;
            long r0 = baseValue % modulus;
            if (r0 < 0) r0 += modulus;
            ulong b = (ulong)r0;
            ulong result = 1 % m;
            while (exponent > 0)
            {
                if ((exponent & 1) == 1) result = MulMod(result, b, m);
                b = MulMod(b, b, m);
                exponent >>= 1;
            }
            return (long)result;
        }

        // Multiplication modulaire par doublement-addition : a, b < m, aucun dépassement.
        private static ulong MulMod(ulong a, ulong b, ulong m)
        {
            ulong result = 0;
            while (b > 0)
            {
                if ((b & 1) == 1) result = AddMod(result, a, m);
                a = AddMod(a, a, m);
                b >>= 1;
            }
            return result;
        }

        private static ulong AddMod(ulong a, ulong b, ulong m)
        {
            return a >= m - b ? a - (m - b) : a + b;
        }
    }
}
