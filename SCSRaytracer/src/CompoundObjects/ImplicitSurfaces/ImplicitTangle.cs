//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System.Numerics;

namespace SCSRaytracer
{
    class ImplicitTangle : RayMarchedImplicit
    {
        private float a, b, c;

        public ImplicitTangle() : base()
        {
            a = 0.0f;
            b = -5.0f;
            c = 11.8f;
        }

        public override float EvaluateImplicitFunction(Point3D p)
        {
            Vector3 sqrd = p.Coordinates * p.Coordinates;
            Vector3 fourpow = sqrd * sqrd;
            float sumsqrd = sqrd.X + sqrd.Y + sqrd.Z;
            return (fourpow.X + fourpow.Y + fourpow.Z + (a * (sumsqrd * sumsqrd)) + (b * sumsqrd) + c);
        }
    }
}
