// SPDX-License-Identifier: MIT
// Copyright (C) 2026 Jérôme FRANCOIS / JB-Opale — voir LICENSE / see LICENSE
using JBOPaleAPI.MathAdvanced;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTest
{
    [TestClass]
    public class MeshQualityTests
    {
        private const double Tol = 1e-9;

        private static Point P(double x, double y, double z = 0)
        {
            return new Point(x, y, z);
        }

        private static Point[] Cube(double sx = 1, double sy = 1, double sz = 1)
        {
            return new Point[]
            {
                P(0, 0, 0), P(sx, 0, 0), P(sx, sy, 0), P(0, sy, 0),
                P(0, 0, sz), P(sx, 0, sz), P(sx, sy, sz), P(0, sy, sz)
            };
        }

        [TestMethod]
        public void EquilateralTriangleIsIdeal()
        {
            Point a = P(0, 0), b = P(1, 0), c = P(0.5, System.Math.Sqrt(3) / 2);
            Assert.AreEqual(System.Math.Sqrt(3) / 4, MeshQuality.TriangleArea(a, b, c), Tol);
            Assert.AreEqual(60.0, MeshQuality.TriangleMinAngle(a, b, c), 1e-7);
            Assert.AreEqual(60.0, MeshQuality.TriangleMaxAngle(a, b, c), 1e-7);
            Assert.AreEqual(1.0, MeshQuality.TriangleEdgeRatio(a, b, c), Tol);
            Assert.AreEqual(1.0, MeshQuality.TriangleAspectRatio(a, b, c), Tol);
            Assert.AreEqual(1.0, MeshQuality.TriangleRadiusRatio(a, b, c), Tol);
            Assert.AreEqual(1.0, MeshQuality.TriangleShape(a, b, c), Tol);
            Assert.AreEqual(1.0, MeshQuality.TriangleCondition(a, b, c), Tol);
            Assert.AreEqual(0.0, MeshQuality.TriangleSkewness(a, b, c), 1e-7);
        }

        [TestMethod]
        public void RightIsoscelesTriangle()
        {
            Point a = P(0, 0), b = P(1, 0), c = P(0, 1);
            Assert.AreEqual(0.5, MeshQuality.TriangleArea(a, b, c), Tol);
            Assert.AreEqual(45.0, MeshQuality.TriangleMinAngle(a, b, c), 1e-7);
            Assert.AreEqual(90.0, MeshQuality.TriangleMaxAngle(a, b, c), 1e-7);
            Assert.AreEqual(System.Math.Sqrt(2), MeshQuality.TriangleEdgeRatio(a, b, c), Tol);
            Assert.AreEqual(4 * System.Math.Sqrt(3) * 0.5 / 4.0, MeshQuality.TriangleShape(a, b, c), Tol);
            Assert.AreEqual(0.25, MeshQuality.TriangleSkewness(a, b, c), 1e-7);
        }

        [TestMethod]
        public void DegenerateTriangleIsInfinite()
        {
            Point a = P(0, 0), b = P(1, 0), c = P(2, 0);
            Assert.IsTrue(double.IsPositiveInfinity(MeshQuality.TriangleAspectRatio(a, b, c)));
            Assert.AreEqual(0.0, MeshQuality.TriangleShape(a, b, c), Tol);
        }

        [TestMethod]
        public void UnitSquareIsIdeal()
        {
            Point a = P(0, 0), b = P(1, 0), c = P(1, 1), d = P(0, 1);
            Assert.AreEqual(1.0, MeshQuality.QuadArea(a, b, c, d), Tol);
            foreach (double angle in MeshQuality.QuadAngles(a, b, c, d)) Assert.AreEqual(90.0, angle, 1e-7);
            Assert.AreEqual(1.0, MeshQuality.QuadEdgeRatio(a, b, c, d), Tol);
            Assert.AreEqual(1.0, MeshQuality.QuadAspectRatio(a, b, c, d), Tol);
            Assert.AreEqual(0.0, MeshQuality.QuadSkew(a, b, c, d), Tol);
            Assert.AreEqual(0.0, MeshQuality.QuadTaper(a, b, c, d), Tol);
            Assert.AreEqual(1.0, MeshQuality.QuadStretch(a, b, c, d), Tol);
            Assert.AreEqual(0.0, MeshQuality.QuadWarpage(a, b, c, d), 1e-7);
            Assert.AreEqual(1.0, MeshQuality.QuadScaledJacobian(a, b, c, d), Tol);
        }

        [TestMethod]
        public void RectangleAndWarpedQuad()
        {
            Assert.AreEqual(2.0, MeshQuality.QuadAspectRatio(P(0, 0), P(2, 0), P(2, 1), P(0, 1)), Tol);
            Assert.AreEqual(2.0, MeshQuality.QuadEdgeRatio(P(0, 0), P(2, 0), P(2, 1), P(0, 1)), Tol);
            Assert.IsTrue(MeshQuality.QuadWarpage(P(0, 0, 0), P(1, 0, 0), P(1, 1, 0), P(0, 1, 1)) > 1.0);
        }

        [TestMethod]
        public void NonConvexQuadHasReflexAngle()
        {
            // « flèche » : le nœud c est rentrant
            double max = MeshQuality.QuadMaxAngle(P(0, 0), P(2, 0), P(0.5, 0.5), P(0, 2));
            Assert.IsTrue(max > 180.0);
            Assert.IsTrue(MeshQuality.QuadScaledJacobian(P(0, 0), P(2, 0), P(0.5, 0.5), P(0, 2)) < 0);
        }

        private static readonly Point[] RegularTet = new Point[] { P(1, 1, 1), P(1, -1, -1), P(-1, 1, -1), P(-1, -1, 1) };

        [TestMethod]
        public void RegularTetrahedronIsIdeal()
        {
            Point a = RegularTet[0], b = RegularTet[1], c = RegularTet[2], d = RegularTet[3];
            Assert.AreEqual(8.0 / 3.0, System.Math.Abs(MeshQuality.TetrahedronVolume(a, b, c, d)), Tol);
            Assert.AreEqual(1.0, MeshQuality.TetrahedronEdgeRatio(a, b, c, d), Tol);
            Assert.AreEqual(1.0, MeshQuality.TetrahedronAspectRatio(a, b, c, d), 1e-7);
            Assert.AreEqual(1.0, MeshQuality.TetrahedronRadiusRatio(a, b, c, d), 1e-7);
            Assert.AreEqual(1.0, MeshQuality.TetrahedronMeanRatio(a, b, c, d), 1e-7);
            foreach (double angle in MeshQuality.TetrahedronDihedralAngles(a, b, c, d))
                Assert.AreEqual(70.528779, angle, 1e-5);
        }

        [TestMethod]
        public void InvertedTetrahedronHasNegativeJacobian()
        {
            Point a = RegularTet[0], b = RegularTet[1], c = RegularTet[2], d = RegularTet[3];
            double sj = MeshQuality.TetrahedronScaledJacobian(a, b, c, d);
            double sjInverted = MeshQuality.TetrahedronScaledJacobian(a, c, b, d);
            Assert.AreEqual(1.0, System.Math.Abs(sj), 1e-7);
            Assert.IsTrue(sj * sjInverted < 0);
            Assert.IsTrue(MeshQuality.TetrahedronVolume(a, b, c, d) * MeshQuality.TetrahedronVolume(a, c, b, d) < 0);
        }

        [TestMethod]
        public void UnitCubeIsIdeal()
        {
            Point[] h = Cube();
            Assert.AreEqual(1.0, MeshQuality.HexahedronVolume(h), Tol);
            Assert.AreEqual(1.0, MeshQuality.HexahedronEdgeRatio(h), Tol);
            Assert.AreEqual(1.0, MeshQuality.HexahedronDiagonalRatio(h), Tol);
            Assert.AreEqual(1.0, MeshQuality.HexahedronStretch(h), Tol);
            Assert.AreEqual(1.0, MeshQuality.HexahedronScaledJacobian(h), Tol);
            Assert.AreEqual(1.0, MeshQuality.HexahedronCondition(h), Tol);
        }

        [TestMethod]
        public void StretchedAndInvertedHexahedron()
        {
            Point[] box = Cube(2, 1, 1);
            Assert.AreEqual(2.0, MeshQuality.HexahedronVolume(box), Tol);
            Assert.AreEqual(2.0, MeshQuality.HexahedronEdgeRatio(box), Tol);
            Assert.IsTrue(MeshQuality.HexahedronCondition(box) > 1.0);
            Assert.AreEqual(1.0, MeshQuality.HexahedronScaledJacobian(box), Tol);

            Point[] inverted = new Point[8];
            Point[] cube = Cube();
            for (int i = 0; i < 8; i++) inverted[i] = cube[(i + 4) % 8];
            Assert.AreEqual(-1.0, MeshQuality.HexahedronVolume(inverted), Tol);
            Assert.IsTrue(MeshQuality.HexahedronScaledJacobian(inverted) < 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HexahedronNeedsEightNodes()
        {
            MeshQuality.HexahedronVolume(new Point[] { P(0, 0), P(1, 0) });
        }

        [TestMethod]
        public void FiniteVolumeMetrics()
        {
            Assert.AreEqual(45.0, MeshQuality.NonOrthogonality(P(0, 0), P(1, 1), P(1, 0)), 1e-7);
            Assert.AreEqual(0.0, MeshQuality.NonOrthogonality(P(0, 0), P(2, 0), P(1, 0)), 1e-7);
            Assert.AreEqual(0.25, MeshQuality.FaceSkewness(P(0, 0), P(2, 0), P(1, 0.5), P(1, 0)), Tol);
            Assert.AreEqual(1.5, MeshQuality.ExpansionRatio(1.0, 1.5), Tol);
            Assert.AreEqual(4.0, MeshQuality.BoundingBoxAspectRatio(new Point[] { P(0, 0, 0), P(2, 1, 0.5) }), Tol);
        }

        [TestMethod]
        public void GlobalStatistics()
        {
            double[] v = { 0, 0.25, 0.5, 0.75, 1 };
            MeshQualityStatistics s = MeshQuality.Summarize(v);
            Assert.AreEqual(5, s.Count);
            Assert.AreEqual(0.0, s.Min, Tol);
            Assert.AreEqual(1.0, s.Max, Tol);
            Assert.AreEqual(0.5, s.Mean, Tol);
            CollectionAssert.AreEqual(new[] { 2, 3 }, MeshQuality.Histogram(v, 2, 0, 1));
            Assert.AreEqual(2, MeshQuality.CountBelow(v, 0.5));
        }
    }
}
