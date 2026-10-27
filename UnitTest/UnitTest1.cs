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
        public void EgaliteDeVecteursEtComplexesSansNullReference()
        {
            JBOpale.MathAdvanced.Vecteur v = new JBOpale.MathAdvanced.Vecteur(1, 2, 3);
            JBOpale.MathAdvanced.Vecteur w = new JBOpale.MathAdvanced.Vecteur(1, 2, 3);
            JBOpale.MathAdvanced.Vecteur vn = null;
            Assert.IsTrue(v == w);
            Assert.IsFalse(v != w);
            Assert.IsFalse(v == vn);
            Assert.IsTrue(v != vn);
            Assert.IsTrue(vn == null);

            JBOpale.MathAdvanced.Complex z = new JBOpale.MathAdvanced.Complex(1, 2);
            JBOpale.MathAdvanced.Complex zn = null;
            Assert.IsTrue(z == new JBOpale.MathAdvanced.Complex(1, 2));
            Assert.IsTrue(z != new JBOpale.MathAdvanced.Complex(1, 3));
            Assert.IsFalse(z == zn);
            Assert.IsTrue(zn != z);
            Assert.IsTrue(zn == null);
            Assert.IsTrue(new JBOpale.MathAdvanced.Complex(2) == 2.0);
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
