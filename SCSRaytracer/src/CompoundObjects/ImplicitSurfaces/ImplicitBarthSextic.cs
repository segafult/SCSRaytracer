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
            bbox = new BoundingBox();
            lowbound = new Vector3(-3.5f);
            highbound = new Vector3(3.5f);
            min_step = 1.0e-5f;
            max_step = 10.0f;
            dist_mult = 0.4f;
            trigger_dist = 0.01f;
        }

        public override float evalF(Point3D p)
        {
            float x_sqr = p.coords.X * p.coords.X;
            float y_sqr = p.coords.Y * p.coords.Y;
            float z_sqr = p.coords.Z * p.coords.Z;
            float phi_sqr = PHI * PHI;
            float fin = x_sqr + y_sqr + z_sqr - 1.1f;

            return (4.0f * (phi_sqr * x_sqr - y_sqr) * (phi_sqr * y_sqr - z_sqr) * (phi_sqr * z_sqr - x_sqr)) - (1.0f + 2.0f * phi_sqr) * (fin * fin);
        }
    }
}
