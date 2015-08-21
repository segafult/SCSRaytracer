//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System.Numerics;

namespace RayTracer
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

        protected override float evalF(Point3D p)
        {
            Vector3 sqrd = p.coords * p.coords;
            Vector3 fourpow = sqrd * sqrd;
            float sumsqrd = sqrd.X + sqrd.Y + sqrd.Z;
            return (fourpow.X + fourpow.Y + fourpow.Z + (a * (sumsqrd * sumsqrd)) + (b * sumsqrd) + c);
        }
    }
}
