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

namespace RayTracer
{
    class SphericalMapper : Mapper
    {
        public override Point2D get_uv(Point3D hit_point)
        {
            //Map hit point to a unit sphere by normalizing vector from origin to hit point.
            Vect3D hit_normalized = new Vect3D(hit_point);
            hit_normalized.normalize();
            double x = hit_normalized.xcoord;
            double y = hit_normalized.ycoord;
            double z = hit_normalized.zcoord;

            double phi = Math.Atan2(x, z) + Math.PI;
            double omega = Math.Acos(y);

            //Calculate spherical UV coordinates
            double u = phi / (2.0 * Math.PI);
            double v = 1 - (omega / Math.PI);

            return new Point2D(u, v);
        }
    }
}
