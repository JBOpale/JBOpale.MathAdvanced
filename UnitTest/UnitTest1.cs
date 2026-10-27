using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SurchageOperateur()
        {
            double[] pts = new double[] { 0, 0, 0 };

            JBOpale.MathAdvanced.Point p = pts;
        }

        [TestMethod]
        public void EgaliteDePoints()
        {
            JBOpale.MathAdvanced.Point a = new JBOpale.MathAdvanced.Point(1, 2, 3);
            JBOpale.MathAdvanced.Point b = new JBOpale.MathAdvanced.Point(1.0001, 2, 3);
            JBOpale.MathAdvanced.Point c = new JBOpale.MathAdvanced.Point(1, 2, 4);
            JBOpale.MathAdvanced.Point nul = null;

            Assert.IsTrue(a == b);
            Assert.IsFalse(a == c);
            Assert.IsTrue(a != c);
            Assert.IsFalse(a == nul);
            Assert.IsFalse(nul == a);
            Assert.IsTrue(nul == null);
        }
    }
}
