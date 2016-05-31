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
        public override Point2D get_uv(Point3D hit_point)
        {
            //Map hit point to a unit sphere by normalizing vector from origin to hit point.
            Vector3 hit_normalized = Vector3.Normalize(hit_point.Coordinates);

            float phi = (float)Math.Atan2(hit_normalized.X, hit_normalized.Z) + FastMath.FPI;
            float omega = (float)Math.Acos(hit_normalized.Y);

            //Calculate spherical UV coordinates
            float u = phi * FastMath.FINVTWOPI;
            float v = 1 - (omega * FastMath.FINVPI);

            return new Point2D(u, v);
        }
    }
}
