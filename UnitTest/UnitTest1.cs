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
    }
}
