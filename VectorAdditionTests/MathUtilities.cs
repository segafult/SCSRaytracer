using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Vect3DorTests
{
    [TestClass]
    public class MathUtilities
    {
        [TestCategory("Vect3D")]
        [TestMethod]
        public void TestVectorAddition()
        {
            //Test addition of Vectors <3,0,1> and <0,8,0>. Answer: <3,8,1>
            RayTracer.Vect3D Vect3D1 = new RayTracer.Vect3D(3, 0, 1);
            RayTracer.Vect3D Vect3D2 = new RayTracer.Vect3D(0, 8, 0);
            RayTracer.Vect3D result1 = Vect3D1 + Vect3D2;

            bool success1 = ((3 == (int)result1.getXCoordinates()) && (8 == (int)result1.getYCoordinates()) && (1 == (int)result1.getZCoordinates()));
            Assert.IsTrue(success1);
        }

        [TestMethod]
        public void TestVectorDotProduct()
        {
            //Test the dot product of Vectors <4,1,0.25> and <6,-3,-8>. Answer: 19
            RayTracer.Vect3D Vect3D1 = new RayTracer.Vect3D(4, 1, 0.25);
            RayTracer.Vect3D Vect3D2 = new RayTracer.Vect3D(6, -3, -8);
            int result1 = (int)(Vect3D1 * Vect3D2);

            Assert.AreEqual(result1, 19);
        }

        [TestMethod]
        public void TestVectorCrossProduct()
        {
            //Test the cross product of Vectors <6,0,-2> and <0,8,0>. Answer: <16, 0, 48>
            RayTracer.Vect3D Vect3D1 = new RayTracer.Vect3D(6, 0, -2);
            RayTracer.Vect3D Vect3D12 = new RayTracer.Vect3D(0, 8, 0);
            RayTracer.Vect3D crossProd1 = Vect3D1 ^ Vect3D12;
            bool passed1 = ((16 == (int)crossProd1.getXCoordinates()) &&
                (0 == (int)crossProd1.getYCoordinates()) &&
                (48 == (int)crossProd1.getZCoordinates()));

            //Test the cross product of Vectors <1,3,-2> and <-1,0,5>. Answer: <15, -3, 3>
            RayTracer.Vect3D Vect3D2 = new RayTracer.Vect3D(1, 3, -2);
            RayTracer.Vect3D Vect3D22 = new RayTracer.Vect3D(-1, 0, 5);
            RayTracer.Vect3D crossProd2 = Vect3D2 ^ Vect3D22;
            bool passed2 = ((15 == (int)crossProd2.getXCoordinates()) &&
                (-3 == (int)crossProd2.getYCoordinates()) &&
                (3 == (int)crossProd2.getZCoordinates()));

            Assert.IsTrue(passed1 && passed2);

        }

        [TestMethod]
        public void TestVectorSubtraction()
        {
            RayTracer.Vect3D u = new RayTracer.Vect3D(2, 1, 1);
            RayTracer.Vect3D v = new RayTracer.Vect3D(3, -2, 1);
            RayTracer.Vect3D result = u - v;
            bool passed1 = ((-1 == (int)result.xcoord) &&
                (3 == (int)result.ycoord) &&
                (0 == (int)result.zcoord));

            Assert.IsTrue(passed1);
        }

        [TestMethod]
        public void TestVectorInversion()
        {
            RayTracer.Vect3D vect = new RayTracer.Vect3D(-1, 2, -3);
            RayTracer.Vect3D inverted = -vect;

            bool passed1 = ((1 == (int)inverted.xcoord) &&
                (-2 == (int)inverted.ycoord) &&
                (3 == (int)inverted.zcoord));

            Assert.IsTrue(passed1);
        }

        [TestMethod]
        public void TestVectorScaling()
        {
            RayTracer.Vect3D vect1 = new RayTracer.Vect3D(2, 3, 4);
            RayTracer.Vect3D r1 = 2.0 * vect1;
            RayTracer.Vect3D r2 = vect1 * 2.0;

            bool passed1 = ((4 == r1.xcoord) &&
                (6 == r1.ycoord) &&
                (8 == r1.zcoord));
            bool passed2 = ((4 == r2.xcoord) &&
                (6 == r2.ycoord) &&
                (8 == r2.zcoord));

            Assert.IsTrue(passed1 && passed2);
        }

        [TestMethod]
        public void TestVectorDivision()
        {
            RayTracer.Vect3D v1 = new RayTracer.Vect3D(4, 6, 8);
            RayTracer.Vect3D r1 = v1 / 2.0;

            bool passed1 = ((2 == r1.xcoord) &&
                (3 == r1.ycoord) &&
                (4 == r1.zcoord));

            Assert.IsTrue(passed1);
        }

        [TestMethod]
        public void TestAddVectorToPoint()
        {
            RayTracer.Point3D p = new RayTracer.Point3D(0, 0, 0);
            RayTracer.Vect3D v = new RayTracer.Vect3D(2, 3, 5);
            RayTracer.Point3D r = p + v;

            bool passed1 = ((2 == r.xcoord) &&
                (3 == r.ycoord) &&
                (5 == r.zcoord));

            Assert.IsTrue(passed1);
        }

        [TestMethod]
        public void TestSubtractVectorFromPoint()
        {
            RayTracer.Point3D p = new RayTracer.Point3D(3, 4, 6);
            RayTracer.Vect3D v = new RayTracer.Vect3D(2, 3, 5);
            RayTracer.Point3D r = p - v;

            bool passed1 = ((1 == r.xcoord) &&
                (1 == r.ycoord) &&
                (1 == r.zcoord));

            Assert.IsTrue(passed1);
        }

        [TestMethod]
        public void TestSubtractPointFromPoint()
        {
            RayTracer.Point3D p1 = new RayTracer.Point3D(1, 2, 3);
            RayTracer.Point3D p2 = new RayTracer.Point3D(3, 5, 2);
            RayTracer.Vect3D r = p1 - p2;

            bool passed1 = ((-2 == r.xcoord) &&
                (-3 == r.ycoord) &&
                (1 == r.zcoord));

            Assert.IsTrue(passed1);
        }

        [TestMethod]
        public void TestIntersectionSphere()
        {
            double t = 0.0;
            RayTracer.World myworld = new RayTracer.World();
            RayTracer.ShadeRec mysr = new RayTracer.ShadeRec(myworld);

            //Sphere centered at origin with radius 2
            RayTracer.Sphere mySphere = new RayTracer.Sphere(new RayTracer.Point3D(0, 0, 0), 2.0);

            //Ray inside the sphere at origin
            RayTracer.Ray ray1 = new RayTracer.Ray(new RayTracer.Point3D(0, 0, 0), new RayTracer.Vect3D(0, 0, 1));

            Assert.IsTrue(mySphere.hit(ray1, ref t, ref mysr));
        }
    }
}
