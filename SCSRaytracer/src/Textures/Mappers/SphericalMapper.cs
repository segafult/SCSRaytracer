//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.If not, see<http://www.gnu.org/licenses/>.
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace RayTracer
{
    class SphericalMapper : Mapper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Point2D get_uv(Point3D hit_point)
        {
            //Map hit point to a unit sphere by normalizing vector from origin to hit point.
            Vector3 hit_normalized = Vector3.Normalize(hit_point.coords);

            float phi = (float)Math.Atan2(hit_normalized.X, hit_normalized.Z) + FastMath.FPI;
            float omega = (float)Math.Acos(hit_normalized.Y);

            //Calculate spherical UV coordinates
            float u = phi * FastMath.FINVTWOPI;
            float v = 1 - (omega * FastMath.FINVPI);

            return new Point2D(u, v);
        }
    }
}
