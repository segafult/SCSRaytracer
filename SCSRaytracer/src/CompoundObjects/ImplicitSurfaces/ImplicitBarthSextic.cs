//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System.Numerics;

namespace SCSRaytracer
{
    class ImplicitBarthSextic : RayMarchedImplicit
    {
        private readonly float PHI = 1.61803398875f;

        public ImplicitBarthSextic()
        {
            boundingBox = new BoundingBox();
            lowBound = new Vector3(-3.5f);
            highBound = new Vector3(3.5f);
            minimumRaymarchStep = 1.0e-5f;
            maximumRaymarchStep = 10.0f;
            distanceMultiplier = 0.4f;
            triggerDistance = 0.01f;
        }

        public override float EvaluateImplicitFunction(Point3D p)
        {
            float x_sqr = p.X * p.X;
            float y_sqr = p.Y * p.Y;
            float z_sqr = p.Z * p.Z;
            float phi_sqr = PHI * PHI;
            float fin = x_sqr + y_sqr + z_sqr - 1.1f;

            return (4.0f * (phi_sqr * x_sqr - y_sqr) * (phi_sqr * y_sqr - z_sqr) * (phi_sqr * z_sqr - x_sqr)) - (1.0f + 2.0f * phi_sqr) * (fin * fin);
        }
    }
}
