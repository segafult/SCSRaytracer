//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SCSRaytracer
{
    sealed class SphericalMapper : Mapper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Point2D GetUV(Point3D hitPoint)
        {
            //Map hit point to a unit sphere by normalizing vector from origin to hit point.
            //Vector3 hitNormalized = Vector3.Normalize(hitPoint.Coordinates);
            Vect3D hitNormalized = new Vect3D(hitPoint).Hat();

            float phi = (float)Math.Atan2(hitNormalized.X, hitNormalized.Z) + FastMath.FPI;
            float omega = (float)Math.Acos(hitNormalized.Y);

            //Calculate spherical UV coordinates
            float u = phi * FastMath.FINVTWOPI;
            float v = 1 - (omega * FastMath.FINVPI);

            return new Point2D(u, v);
        }
    }
}
