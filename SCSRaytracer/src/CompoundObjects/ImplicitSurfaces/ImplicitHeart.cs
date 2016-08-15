//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System.Numerics;

namespace SCSRaytracer
{
    class ImplicitHeart : RayMarchedImplicit
    {
        private static float NINEOVERFOUR = 9.0f / 4.0f;
        private static float NINEOVEREIGHTY = 9.0f / 80.0f;

        public ImplicitHeart()
        {
            boundingBox = new BoundingBox();
            lowBound = new Vector3(-1.5f);
            highBound = new Vector3(1.5f);
            minimumRaymarchStep = 1.0e-5f;
            maximumRaymarchStep = 4.0f;
            distanceMultiplier = 0.1f;
        }

        public override float EvaluateImplicitFunction(Point3D p)
        {
            Vector3 sqrd = p.Coordinates * p.Coordinates;
            float cbedz = sqrd.Z * p.Z;
            float cubeterm = (sqrd.X + NINEOVERFOUR * sqrd.Y + sqrd.Z - 1);
            return (cubeterm * cubeterm * cubeterm) - (sqrd.X * cbedz) - (NINEOVEREIGHTY * sqrd.Y * cbedz);
        }
    }
}
