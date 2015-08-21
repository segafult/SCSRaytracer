//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System.Numerics;

namespace RayTracer
{
    class ImplicitHeart : RayMarchedImplicit
    {
        private static float NINEOVERFOUR = 9.0f / 4.0f;
        private static float NINEOVEREIGHTY = 9.0f / 80.0f;

        public ImplicitHeart()
        {
            bbox = new BoundingBox();
            lowbound = new Vector3(-1.5f);
            highbound = new Vector3(1.5f);
            min_step = 1.0e-5f;
            max_step = 4.0f;
            dist_mult = 0.1f;
        }

        protected override float evalF(Point3D p)
        {
            Vector3 sqrd = p.coords * p.coords;
            float cbedz = sqrd.Z * p.coords.Z;
            float cubeterm = (sqrd.X + NINEOVERFOUR * sqrd.Y + sqrd.Z - 1);
            return (cubeterm * cubeterm * cubeterm) - (sqrd.X * cbedz) - (NINEOVEREIGHTY * sqrd.Y * cbedz);
        }
    }
}
