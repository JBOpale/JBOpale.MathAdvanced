// SPDX-License-Identifier: LGPL-3.0-or-later
// Copyright (C) 2026 Jérôme FRANCOIS / JB-Opale — voir LICENSE.md / see LICENSE.md
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
#pragma warning disable CS8765

namespace JBOPaleAPI.MathAdvanced
{
    /// <summary>
    /// <para><b>[FR]</b> Représente un nombre complexe sous la forme z = Re + i·Im,
    /// où Re est la partie réelle et Im la partie imaginaire.
    /// Fournit les opérations arithmétiques, les fonctions transcendantes et les conversions courantes.</para>
    /// <para><b>[EN]</b> Represents a complex number in the form z = Re + i·Im,
    /// where Re is the real part and Im the imaginary part.
    /// Provides arithmetic operations, transcendental functions and common conversions.</para>
    /// </summary>
    public class Complex
    {
        double re;
        double im;

        /// <summary>
        /// <para><b>[FR]</b> Partie réelle du nombre complexe.</para>
        /// <para><b>[EN]</b> Real part of the complex number.</para>
        /// </summary>
        public double Re
        {
            get { return re; }
            set { re = value; }
        }

        /// <summary>
        /// <para><b>[FR]</b> Partie imaginaire du nombre complexe.</para>
        /// <para><b>[EN]</b> Imaginary part of the complex number.</para>
        /// </summary>
        public double Im
        {
            get { return im; }
            set { im = value; }
        }

        /// <summary>
        /// <para><b>[FR]</b> Unité imaginaire i, telle que i² = −1.</para>
        /// <para><b>[EN]</b> Imaginary unit i, such that i² = −1.</para>
        /// </summary>
        public static Complex I
        {
            get { return new Complex(0, 1); }
        }

        /// <summary>
        /// <para><b>[FR]</b> Nombre complexe zéro : 0 + 0i.</para>
        /// <para><b>[EN]</b> Complex number zero: 0 + 0i.</para>
        /// </summary>
        public static Complex Zero
        {
            get { return new Complex(0, 0); }
        }

        /// <summary>
        /// <para><b>[FR]</b> Nombre complexe un : 1 + 0i.</para>
        /// <para><b>[EN]</b> Complex number one: 1 + 0i.</para>
        /// </summary>
        public static Complex One
        {
            get { return new Complex(1, 0); }
        }

        #region Constructors / Constructeurs

        /// <summary>
        /// <para><b>[FR]</b> Initialise un nombre complexe à zéro : 0 + 0i.</para>
        /// <para><b>[EN]</b> Initializes a complex number to zero: 0 + 0i.</para>
        /// </summary>
        public Complex()
        {
            Re = 0;
            Im = 0;
        }

        /// <summary>
        /// <para><b>[FR]</b> Initialise un nombre complexe purement réel : <paramref name="realPart"/> + 0i.</para>
        /// <para><b>[EN]</b> Initializes a purely real complex number: <paramref name="realPart"/> + 0i.</para>
        /// </summary>
        /// <param name="realPart">
        /// <b>[FR]</b> Partie réelle du nombre complexe. /
        /// <b>[EN]</b> Real part of the complex number.
        /// </param>
        public Complex(double realPart)
        {
            Re = realPart;
            Im = 0;
        }

        /// <summary>
        /// <para><b>[FR]</b> Initialise un nombre complexe à partir de ses parties réelle et imaginaire.</para>
        /// <para><b>[EN]</b> Initializes a complex number from its real and imaginary parts.</para>
        /// </summary>
        /// <param name="realPart">
        /// <b>[FR]</b> Partie réelle. /
        /// <b>[EN]</b> Real part.
        /// </param>
        /// <param name="imaginaryPart">
        /// <b>[FR]</b> Partie imaginaire. /
        /// <b>[EN]</b> Imaginary part.
        /// </param>
        public Complex(double realPart, double imaginaryPart)
        {
            Re = realPart;
            Im = imaginaryPart;
        }

        /// <summary>
        /// <para><b>[FR]</b> Initialise un nombre complexe à partir d'une chaîne de caractères
        /// de la forme "a+bi".</para>
        /// <para><b>[EN]</b> Initializes a complex number from a string of the form "a+bi".</para>
        /// </summary>
        /// <param name="s">
        /// <b>[FR]</b> Représentation textuelle du nombre complexe. /
        /// <b>[EN]</b> String representation of the complex number.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// <b>[FR]</b> Non implémenté. /
        /// <b>[EN]</b> Not yet implemented.
        /// </exception>
        public Complex(string s)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <para><b>[FR]</b> Teste la correspondance d'une chaîne avec le format complexe "a+bi"
        /// via une expression régulière.</para>
        /// <para><b>[EN]</b> Tests whether a string matches the complex format "a+bi"
        /// using a regular expression.</para>
        /// </summary>
        /// <param name="s">
        /// <b>[FR]</b> Chaîne à tester. /
        /// <b>[EN]</b> String to test.
        /// </param>
        /// <returns>
        /// <b>[FR]</b> Résultat de la correspondance regex. /
        /// <b>[EN]</b> Regex match result.
        /// </returns>
        public static Match Test(string s)
        {
            string dp = "([0-9]+[.]?[0-9]*|[.][0-9]+)";
            string dm = "[-]?" + dp;
            Regex r = new Regex("^(?<RePart>(" + dm + ")[-+](?<ImPart>(" + dp + "))[i])$");
            return r.Match(s);
        }

        #endregion

        #region Operators / Opérateurs

        /// <summary>
        /// <para><b>[FR]</b> Additionne deux nombres complexes.</para>
        /// <para><b>[EN]</b> Adds two complex numbers.</para>
        /// </summary>
        /// <param name="a"><b>[FR]</b> Premier opérande. / <b>[EN]</b> First operand.</param>
        /// <param name="b"><b>[FR]</b> Second opérande. / <b>[EN]</b> Second operand.</param>
        /// <returns>
        /// <b>[FR]</b> Somme : (a.Re+b.Re) + i(a.Im+b.Im). /
        /// <b>[EN]</b> Sum: (a.Re+b.Re) + i(a.Im+b.Im).
        /// </returns>
        public static Complex operator +(Complex a, Complex b)
        {
            return new Complex(a.Re + b.Re, a.Im + b.Im);
        }

        /// <summary>
        /// <para><b>[FR]</b> Additionne un nombre complexe et un réel.</para>
        /// <para><b>[EN]</b> Adds a complex number and a real number.</para>
        /// </summary>
        public static Complex operator +(Complex a, double b)
        {
            return new Complex(a.Re + b, a.Im);
        }

        /// <summary>
        /// <para><b>[FR]</b> Additionne un réel et un nombre complexe.</para>
        /// <para><b>[EN]</b> Adds a real number and a complex number.</para>
        /// </summary>
        public static Complex operator +(double a, Complex b)
        {
            return new Complex(a + b.Re, b.Im);
        }

        /// <summary>
        /// <para><b>[FR]</b> Soustrait deux nombres complexes.</para>
        /// <para><b>[EN]</b> Subtracts two complex numbers.</para>
        /// </summary>
        /// <param name="a"><b>[FR]</b> Minuende. / <b>[EN]</b> Minuend.</param>
        /// <param name="b"><b>[FR]</b> Diminuende. / <b>[EN]</b> Subtrahend.</param>
        /// <returns>
        /// <b>[FR]</b> Différence : (a.Re−b.Re) + i(a.Im−b.Im). /
        /// <b>[EN]</b> Difference: (a.Re−b.Re) + i(a.Im−b.Im).
        /// </returns>
        public static Complex operator -(Complex a, Complex b)
        {
            return new Complex(a.Re - b.Re, a.Im - b.Im);
        }

        /// <summary>
        /// <para><b>[FR]</b> Soustrait un réel d'un nombre complexe.</para>
        /// <para><b>[EN]</b> Subtracts a real number from a complex number.</para>
        /// </summary>
        public static Complex operator -(Complex a, double b)
        {
            return new Complex(a.Re - b, a.Im);
        }

        /// <summary>
        /// <para><b>[FR]</b> Soustrait un nombre complexe d'un réel.</para>
        /// <para><b>[EN]</b> Subtracts a complex number from a real number.</para>
        /// </summary>
        public static Complex operator -(double a, Complex b)
        {
            return new Complex(a - b.Re, -b.Im);
        }

        /// <summary>
        /// <para><b>[FR]</b> Retourne l'opposé d'un nombre complexe.</para>
        /// <para><b>[EN]</b> Returns the negation of a complex number.</para>
        /// </summary>
        /// <param name="a"><b>[FR]</b> Nombre complexe. / <b>[EN]</b> Complex number.</param>
        /// <returns>
        /// <b>[FR]</b> Opposé −a : (−a.Re) + i(−a.Im). /
        /// <b>[EN]</b> Negation −a: (−a.Re) + i(−a.Im).
        /// </returns>
        public static Complex operator -(Complex a)
        {
            return new Complex(-a.Re, -a.Im);
        }

        /// <summary>
        /// <para><b>[FR]</b> Multiplie deux nombres complexes : (a.Re·b.Re − a.Im·b.Im) + i(a.Im·b.Re + a.Re·b.Im).</para>
        /// <para><b>[EN]</b> Multiplies two complex numbers: (a.Re·b.Re − a.Im·b.Im) + i(a.Im·b.Re + a.Re·b.Im).</para>
        /// </summary>
        public static Complex operator *(Complex a, Complex b)
        {
            return new Complex(
                a.Re * b.Re - a.Im * b.Im,
                a.Im * b.Re + a.Re * b.Im);
        }

        /// <summary>
        /// <para><b>[FR]</b> Multiplie un nombre complexe par un scalaire réel.</para>
        /// <para><b>[EN]</b> Multiplies a complex number by a real scalar.</para>
        /// </summary>
        public static Complex operator *(Complex a, double d)
        {
            return new Complex(d) * a;
        }

        /// <summary>
        /// <para><b>[FR]</b> Multiplie un scalaire réel par un nombre complexe.</para>
        /// <para><b>[EN]</b> Multiplies a real scalar by a complex number.</para>
        /// </summary>
        public static Complex operator *(double d, Complex a)
        {
            return new Complex(d) * a;
        }

        /// <summary>
        /// <para><b>[FR]</b> Divise deux nombres complexes : a · conj(b) / |b|².</para>
        /// <para><b>[EN]</b> Divides two complex numbers: a · conj(b) / |b|².</para>
        /// </summary>
        public static Complex operator /(Complex a, Complex b)
        {
            return a * Conj(b) * (1 / (Abs(b) * Abs(b)));
        }

        /// <summary>
        /// <para><b>[FR]</b> Divise un nombre complexe par un scalaire réel.</para>
        /// <para><b>[EN]</b> Divides a complex number by a real scalar.</para>
        /// </summary>
        public static Complex operator /(Complex a, double b)
        {
            return a * (1 / b);
        }

        /// <summary>
        /// <para><b>[FR]</b> Divise un scalaire réel par un nombre complexe.</para>
        /// <para><b>[EN]</b> Divides a real scalar by a complex number.</para>
        /// </summary>
        public static Complex operator /(double a, Complex b)
        {
            return a * Conj(b) * (1 / (Abs(b) * Abs(b)));
        }

        /// <summary>
        /// <para><b>[FR]</b> Détermine si deux nombres complexes sont égaux (Re et Im identiques).</para>
        /// <para><b>[EN]</b> Determines whether two complex numbers are equal (identical Re and Im).</para>
        /// </summary>
        public static bool operator ==(Complex a, Complex b)
        {
            return (a.Re == b.Re && a.Im == b.Im);
        }

        /// <summary>
        /// <para><b>[FR]</b> Détermine si un nombre complexe est égal à un réel.</para>
        /// <para><b>[EN]</b> Determines whether a complex number equals a real number.</para>
        /// </summary>
        public static bool operator ==(Complex a, double b)
        {
            return a == new Complex(b);
        }

        /// <summary>
        /// <para><b>[FR]</b> Détermine si un réel est égal à un nombre complexe.</para>
        /// <para><b>[EN]</b> Determines whether a real number equals a complex number.</para>
        /// </summary>
        public static bool operator ==(double a, Complex b)
        {
            return new Complex(a) == b;
        }

        /// <summary>
        /// <para><b>[FR]</b> Détermine si deux nombres complexes sont différents.</para>
        /// <para><b>[EN]</b> Determines whether two complex numbers are different.</para>
        /// </summary>
        public static bool operator !=(Complex a, Complex b)
        {
            return !(a == b);
        }

        /// <summary>
        /// <para><b>[FR]</b> Détermine si un nombre complexe est différent d'un réel.</para>
        /// <para><b>[EN]</b> Determines whether a complex number is different from a real number.</para>
        /// </summary>
        public static bool operator !=(Complex a, double b)
        {
            return !(a == b);
        }

        /// <summary>
        /// <para><b>[FR]</b> Détermine si un réel est différent d'un nombre complexe.</para>
        /// <para><b>[EN]</b> Determines whether a real number is different from a complex number.</para>
        /// </summary>
        public static bool operator !=(double a, Complex b)
        {
            return !(a == b);
        }

        #endregion

        #region Static Functions / Fonctions statiques

        /// <summary>
        /// <para><b>[FR]</b> Calcule le module (valeur absolue) d'un nombre complexe : sqrt(Re² + Im²).</para>
        /// <para><b>[EN]</b> Computes the modulus (absolute value) of a complex number: sqrt(Re² + Im²).</para>
        /// </summary>
        /// <param name="a">
        /// <b>[FR]</b> Nombre complexe. /
        /// <b>[EN]</b> Complex number.
        /// </param>
        /// <returns>
        /// <b>[FR]</b> Module de a. /
        /// <b>[EN]</b> Modulus of a.
        /// </returns>
        public static double Abs(Complex a)
        {
            return System.Math.Sqrt(a.Im * a.Im + a.Re * a.Re);
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule l'inverse d'un nombre complexe : conj(a) / |a|².</para>
        /// <para><b>[EN]</b> Computes the inverse of a complex number: conj(a) / |a|².</para>
        /// </summary>
        /// <param name="a">
        /// <b>[FR]</b> Nombre complexe à inverser (non nul). /
        /// <b>[EN]</b> Complex number to invert (non-zero).
        /// </param>
        /// <returns>
        /// <b>[FR]</b> Inverse 1/a. /
        /// <b>[EN]</b> Inverse 1/a.
        /// </returns>
        public static Complex Inv(Complex a)
        {
            return new Complex(
                 a.Re / (a.Re * a.Re + a.Im * a.Im),
                -a.Im / (a.Re * a.Re + a.Im * a.Im));
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la tangente complexe de a : sin(a) / cos(a).</para>
        /// <para><b>[EN]</b> Computes the complex tangent of a: sin(a) / cos(a).</para>
        /// </summary>
        public static Complex Tan(Complex a)
        {
            return Sin(a) / Cos(a);
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le cosinus hyperbolique complexe de a : (eᵃ + e⁻ᵃ) / 2.</para>
        /// <para><b>[EN]</b> Computes the complex hyperbolic cosine of a: (eᵃ + e⁻ᵃ) / 2.</para>
        /// </summary>
        public static Complex Cosh(Complex a)
        {
            return (Exp(a) + Exp(-a)) / 2;
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le sinus hyperbolique complexe de a : (eᵃ − e⁻ᵃ) / 2.</para>
        /// <para><b>[EN]</b> Computes the complex hyperbolic sine of a: (eᵃ − e⁻ᵃ) / 2.</para>
        /// </summary>
        public static Complex Sinh(Complex a)
        {
            return (Exp(a) - Exp(-a)) / 2;
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la tangente hyperbolique complexe de a : (e²ᵃ − 1) / (e²ᵃ + 1).</para>
        /// <para><b>[EN]</b> Computes the complex hyperbolic tangent of a: (e²ᵃ − 1) / (e²ᵃ + 1).</para>
        /// </summary>
        public static Complex Tanh(Complex a)
        {
            return (Exp(2 * a) - 1) / (Exp(2 * a) + 1);
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la cotangente hyperbolique complexe de a : (e²ᵃ + 1) / (e²ᵃ − 1).</para>
        /// <para><b>[EN]</b> Computes the complex hyperbolic cotangent of a: (e²ᵃ + 1) / (e²ᵃ − 1).</para>
        /// </summary>
        public static Complex Coth(Complex a)
        {
            return (Exp(2 * a) + 1) / (Exp(2 * a) - 1);
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la sécante hyperbolique complexe de a : 1 / cosh(a).</para>
        /// <para><b>[EN]</b> Computes the complex hyperbolic secant of a: 1 / cosh(a).</para>
        /// </summary>
        public static Complex Sech(Complex a)
        {
            return Inv(Cosh(a));
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la cosécante hyperbolique complexe de a : 1 / sinh(a).</para>
        /// <para><b>[EN]</b> Computes the complex hyperbolic cosecant of a: 1 / sinh(a).</para>
        /// </summary>
        public static Complex Csch(Complex a)
        {
            return Inv(Sinh(a));
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la cotangente complexe de a : cos(a) / sin(a).</para>
        /// <para><b>[EN]</b> Computes the complex cotangent of a: cos(a) / sin(a).</para>
        /// </summary>
        public static Complex Cot(Complex a)
        {
            return Cos(a) / Sin(a);
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le conjugué d'un nombre complexe : Re − i·Im.</para>
        /// <para><b>[EN]</b> Computes the conjugate of a complex number: Re − i·Im.</para>
        /// </summary>
        /// <param name="a">
        /// <b>[FR]</b> Nombre complexe. /
        /// <b>[EN]</b> Complex number.
        /// </param>
        /// <returns>
        /// <b>[FR]</b> Conjugué de a. /
        /// <b>[EN]</b> Conjugate of a.
        /// </returns>
        public static Complex Conj(Complex a)
        {
            return new Complex(a.Re, -a.Im);
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la racine carrée complexe d'un réel.
        /// Retourne un imaginaire pur si l'argument est négatif.</para>
        /// <para><b>[EN]</b> Computes the complex square root of a real number.
        /// Returns a pure imaginary number if the argument is negative.</para>
        /// </summary>
        /// <param name="d">
        /// <b>[FR]</b> Valeur réelle (peut être négative). /
        /// <b>[EN]</b> Real value (may be negative).
        /// </param>
        /// <returns>
        /// <b>[FR]</b> sqrt(d) si d ≥ 0, sinon i·sqrt(|d|). /
        /// <b>[EN]</b> sqrt(d) if d ≥ 0, otherwise i·sqrt(|d|).
        /// </returns>
        public static Complex Sqrt(double d)
        {
            if (d >= 0)
                return new Complex(System.Math.Sqrt(d));
            else
                return new Complex(0, System.Math.Sqrt(-d));
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la racine carrée complexe de a via Pow(a, 0.5).</para>
        /// <para><b>[EN]</b> Computes the complex square root of a via Pow(a, 0.5).</para>
        /// </summary>
        /// <param name="a">
        /// <b>[FR]</b> Nombre complexe. /
        /// <b>[EN]</b> Complex number.
        /// </param>
        public static Complex Sqrt(Complex a)
        {
            return Pow(a, .5);
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule l'exponentielle complexe de a : e^Re · (cos(Im) + i·sin(Im)).</para>
        /// <para><b>[EN]</b> Computes the complex exponential of a: e^Re · (cos(Im) + i·sin(Im)).</para>
        /// </summary>
        /// <param name="a">
        /// <b>[FR]</b> Exposant complexe. /
        /// <b>[EN]</b> Complex exponent.
        /// </param>
        public static Complex Exp(Complex a)
        {
            return new Complex(
                System.Math.Exp(a.Re) * System.Math.Cos(a.Im),
                System.Math.Exp(a.Re) * System.Math.Sin(a.Im));
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la valeur principale du logarithme complexe de a :
        /// ln|a| + i·Arg(a).</para>
        /// <para><b>[EN]</b> Computes the principal value of the complex logarithm of a:
        /// ln|a| + i·Arg(a).</para>
        /// </summary>
        /// <param name="a">
        /// <b>[FR]</b> Nombre complexe non nul. /
        /// <b>[EN]</b> Non-zero complex number.
        /// </param>
        public static Complex Log(Complex a)
        {
            return new Complex(System.Math.Log(Abs(a)), Arg(a));
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule l'argument (phase) d'un nombre complexe en radians,
        /// dans l'intervalle (−π, π].</para>
        /// <para><b>[EN]</b> Computes the argument (phase angle) of a complex number in radians,
        /// in the range (−π, π].</para>
        /// </summary>
        /// <param name="a">
        /// <b>[FR]</b> Nombre complexe. /
        /// <b>[EN]</b> Complex number.
        /// </param>
        /// <returns>
        /// <b>[FR]</b> Argument de a en radians. /
        /// <b>[EN]</b> Argument of a in radians.
        /// </returns>
        public static double Arg(Complex a)
        {
            if (a.Re < 0)
            {
                if (a.Im < 0)
                    return System.Math.Atan(a.Im / a.Re) - System.Math.PI;
                else
                    return System.Math.PI - System.Math.Atan(-a.Im / a.Re);
            }
            else
                return System.Math.Atan(a.Im / a.Re);
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le cosinus complexe de a : (e^(ia) + e^(−ia)) / 2.</para>
        /// <para><b>[EN]</b> Computes the complex cosine of a: (e^(ia) + e^(−ia)) / 2.</para>
        /// </summary>
        public static Complex Cos(Complex a)
        {
            return .5 * (Exp(Complex.I * a) + Exp(-Complex.I * a));
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule le sinus complexe de a : (e^(ia) − e^(−ia)) / (2i).</para>
        /// <para><b>[EN]</b> Computes the complex sine of a: (e^(ia) − e^(−ia)) / (2i).</para>
        /// </summary>
        public static Complex Sin(Complex a)
        {
            return (Exp(Complex.I * a) - Exp(-Complex.I * a)) / (2 * Complex.I);
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la puissance complexe a^b = exp(b · ln(a)).</para>
        /// <para><b>[EN]</b> Computes the complex power a^b = exp(b · ln(a)).</para>
        /// </summary>
        /// <param name="a"><b>[FR]</b> Base complexe. / <b>[EN]</b> Complex base.</param>
        /// <param name="b"><b>[FR]</b> Exposant complexe. / <b>[EN]</b> Complex exponent.</param>
        public static Complex Pow(Complex a, Complex b)
        {
            return Exp(b * Log(a));
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la puissance complexe a^b pour une base réelle a.</para>
        /// <para><b>[EN]</b> Computes the complex power a^b for a real base a.</para>
        /// </summary>
        /// <param name="a"><b>[FR]</b> Base réelle. / <b>[EN]</b> Real base.</param>
        /// <param name="b"><b>[FR]</b> Exposant complexe. / <b>[EN]</b> Complex exponent.</param>
        public static Complex Pow(double a, Complex b)
        {
            return Exp(b * System.Math.Log(a));
        }

        /// <summary>
        /// <para><b>[FR]</b> Calcule la puissance complexe a^b pour un exposant réel b.</para>
        /// <para><b>[EN]</b> Computes the complex power a^b for a real exponent b.</para>
        /// </summary>
        /// <param name="a"><b>[FR]</b> Base complexe. / <b>[EN]</b> Complex base.</param>
        /// <param name="b"><b>[FR]</b> Exposant réel. / <b>[EN]</b> Real exponent.</param>
        public static Complex Pow(Complex a, double b)
        {
            return Exp(b * Log(a));
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this == Complex.Zero) return "0";

            string re, im, sign;

            if (this.Im < 0)
                sign = this.Re == 0 ? "-" : " - ";
            else if (this.Im > 0 && this.Re != 0)
                sign = " + ";
            else
                sign = "";

            re = this.Re == 0 ? "" : this.Re.ToString();
            im = this.Im == 0 ? "" : (this.Im == -1 || this.Im == 1) ? "i" : System.Math.Abs(this.Im).ToString() + "i";

            return re + sign + im;
        }

        /// <summary>
        /// <para><b>[FR]</b> Retourne la représentation textuelle du nombre complexe avec un format numérique.</para>
        /// <para><b>[EN]</b> Returns the string representation of the complex number with a numeric format.</para>
        /// </summary>
        /// <param name="format">
        /// <b>[FR]</b> Format numérique (ex. "F2"). /
        /// <b>[EN]</b> Numeric format string (e.g. "F2").
        /// </param>
        public string ToString(string format)
        {
            if (this == Complex.Zero) return "0";
            else if (double.IsInfinity(this.Re) || double.IsInfinity(this.Im)) return "oo";
            else if (double.IsNaN(this.Re) || double.IsNaN(this.Im)) return "?";

            string imval = System.Math.Abs(this.Im).ToString(format);
            string reval = this.Re.ToString(format);

            string sign;
            if (imval.StartsWith("-"))
                sign = reval == "0" ? "-" : " - ";
            else if (imval != "0" && reval != "0")
                sign = " + ";
            else
                sign = "";

            string im = imval == "0" ? "" : imval == "1" ? "i" : imval + "i";
            string re = reval == "0" ? (imval != "0" ? "" : "0") : reval;

            return re + sign + im;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj?.ToString() == this.ToString();
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return -1;
        }

        #endregion

        #region Instance Methods / Méthodes d'instance

        /// <summary>
        /// <para><b>[FR]</b> Indique si ce nombre complexe est purement réel (partie imaginaire nulle).</para>
        /// <para><b>[EN]</b> Indicates whether this complex number is purely real (zero imaginary part).</para>
        /// </summary>
        /// <returns>
        /// <b>[FR]</b> <c>true</c> si Im = 0 ; <c>false</c> sinon. /
        /// <b>[EN]</b> <c>true</c> if Im = 0; <c>false</c> otherwise.
        /// </returns>
        public bool IsReal()
        {
            return (this.Im == 0);
        }

        /// <summary>
        /// <para><b>[FR]</b> Indique si ce nombre complexe est purement imaginaire (partie réelle nulle).</para>
        /// <para><b>[EN]</b> Indicates whether this complex number is purely imaginary (zero real part).</para>
        /// </summary>
        /// <returns>
        /// <b>[FR]</b> <c>true</c> si Re = 0 ; <c>false</c> sinon. /
        /// <b>[EN]</b> <c>true</c> if Re = 0; <c>false</c> otherwise.
        /// </returns>
        public bool IsImaginary()
        {
            return (this.Re == 0);
        }

        #endregion
    }
}