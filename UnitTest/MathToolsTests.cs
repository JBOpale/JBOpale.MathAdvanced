// SPDX-License-Identifier: MIT
// Copyright (C) 2026 Jérôme FRANCOIS / JB-Opale — voir LICENSE / see LICENSE
using JBOPaleAPI.MathAdvanced;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class MathToolsTests
    {
        [TestMethod]
        public void GcdLcm()
        {
            Assert.AreEqual(6L, NumberTheory.Gcd(54, -24));
            Assert.AreEqual(0L, NumberTheory.Gcd(0, 0));
            Assert.AreEqual(216L, NumberTheory.Lcm(54, 24));
            Assert.AreEqual(0L, NumberTheory.Lcm(0, 5));
        }

        [TestMethod]
        public void PrimesAndFactors()
        {
            Assert.IsTrue(NumberTheory.IsPrime(2));
            Assert.IsTrue(NumberTheory.IsPrime(1000000007));
            Assert.IsFalse(NumberTheory.IsPrime(1));
            Assert.IsFalse(NumberTheory.IsPrime(91));
            CollectionAssert.AreEqual(new long[] { 2, 2, 2, 3, 3, 5 }, NumberTheory.PrimeFactors(360).ToArray());
            Assert.AreEqual(0, NumberTheory.PrimeFactors(1).Count);
        }

        [TestMethod]
        public void FactorialBinomialFibonacci()
        {
            Assert.AreEqual(120.0, NumberTheory.Factorial(5));
            Assert.AreEqual(1.0, NumberTheory.Factorial(0));
            Assert.AreEqual(252.0, NumberTheory.Binomial(10, 5));
            Assert.AreEqual(0.0, NumberTheory.Binomial(3, 5));
            Assert.AreEqual(55L, NumberTheory.Fibonacci(10));
            Assert.AreEqual(7540113804746346429L, NumberTheory.Fibonacci(92));
        }

        [TestMethod]
        public void PowMod()
        {
            Assert.AreEqual(445L, NumberTheory.PowMod(4, 13, 497));
            Assert.AreEqual(0L, NumberTheory.PowMod(5, 3, 1));
            Assert.AreEqual(1L, NumberTheory.PowMod(-2, 2, 3));
        }

        [TestMethod]
        public void DescriptiveStatistics()
        {
            double[] v = { 2, 4, 4, 4, 5, 5, 7, 9 };
            Assert.AreEqual(5.0, Statistics.Mean(v), 1e-12);
            Assert.AreEqual(4.5, Statistics.Median(v), 1e-12);
            Assert.AreEqual(2.0, Statistics.StandardDeviation(v, false), 1e-12);
            Assert.AreEqual(32.0 / 7.0, Statistics.Variance(v), 1e-12);
            Assert.AreEqual(7.25, Statistics.Percentile(v, 87.5), 1e-12);
        }

        [TestMethod]
        public void RegressionAndCorrelation()
        {
            double[] x = { 1, 2, 3, 4 };
            double[] y = { 3, 5, 7, 9 };
            double r2 = Statistics.LinearRegression(x, y, out double slope, out double intercept);
            Assert.AreEqual(2.0, slope, 1e-12);
            Assert.AreEqual(1.0, intercept, 1e-12);
            Assert.AreEqual(1.0, r2, 1e-12);
            Assert.AreEqual(1.0, Statistics.Correlation(x, y), 1e-12);
        }

        [TestMethod]
        public void NumericsBasics()
        {
            Assert.AreEqual(5.0, Numerics.Clamp(9, 0, 5));
            Assert.AreEqual(15.0, Numerics.Lerp(10, 20, 0.5));
            Assert.IsTrue(Numerics.AlmostEqual(0.1 + 0.2, 0.3));
            Assert.AreEqual(6.0, Numerics.Derivative(x => x * x * x, 2) / 2, 1e-4);
        }

        [TestMethod]
        public void IntegrationAndRoots()
        {
            Assert.AreEqual(2.0, Numerics.Integrate(System.Math.Sin, 0, System.Math.PI), 1e-8);
            Assert.AreEqual(System.Math.Sqrt(2), Numerics.Bisection(x => x * x - 2, 0, 2), 1e-9);
            Assert.AreEqual(System.Math.Sqrt(2), Numerics.Newton(x => x * x - 2, 1), 1e-9);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BisectionSameSignThrows()
        {
            Numerics.Bisection(x => x * x + 1, -1, 1);
        }
    }
}
