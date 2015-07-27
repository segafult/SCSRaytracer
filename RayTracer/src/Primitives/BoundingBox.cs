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
    /// <summary>
    /// Axis aligned bounding box for expensive ray-object intersections.
    /// Won't cast shadows, and won't appear in renderings.
    /// </summary>
    public class BoundingBox : RenderableObject
    {
        public double x0, x1;
        public double y0, y1;
        public double z0, z1;

        public BoundingBox(double x0_arg, double x1_arg, double y0_arg, double y1_arg, double z0_arg, double z1_arg)
        {
            x0 = x0_arg;
            x1 = x1_arg;
            y0 = y0_arg;
            y1 = y1_arg;
            z0 = z0_arg;
            z1 = z1_arg;
        }

        public override bool hit(Ray r, double tmin)
        {
            double ox = r.origin.xcoord; double oy = r.origin.ycoord; double oz = r.origin.zcoord;
            double dx = r.direction.xcoord; double dy = r.direction.ycoord; double dz = r.direction.zcoord;

            double tx_min, ty_min, tz_min;
            double tx_max, ty_max, tz_max;

            //How this algorithm works:
            //Generate *x_min and *x_max values which indicate the minimum and maximum length a line segment
            //from origin in the ray direction can have and be within the volume of the bounding box. If all 3
            //distance ranges overlap, then the bounding box was hit.

            double a = 1.0 / dx;
            if(a >= 0.0)
            {
                tx_min = (x0 - ox) * a;
                tx_max = (x1 - ox) * a;
            }
            else
            {
                tx_min = (x1 - ox) * a;
                tx_max = (x0 - ox) * a;
            }

            double b = 1.0 / dy;
            if( b >= 0.0)
            {
                ty_min = (y0 - oy) * b;
                ty_max = (y1 - oy) * b;
            }
            else
            {
                ty_min = (y1 - oy) * b;
                ty_max = (y0 - oy) * b;
            }

            double c = 1.0 / dz;
            if( c >= 0.0)
            {
                tz_min = (z0 - oz) * c;
                tz_max = (z1 - oz) * c;
            }
            else
            {
                tz_min = (z1 - oz) * c;
                tz_max = (z0 - oz) * c;
            }

            double t0, t1;

            //largest entering t value
            if(tx_min > ty_min)
            {
                t0 = tx_min;
            }
            else
            {
                t0 = ty_min;
            }

            if(tz_min > t0)
            {
                t0 = tz_min;
            }

            //smallest exiting t value
            if(tx_max < ty_max)
            {
                t1 = tx_max;
            }
            else
            {
                t1 = ty_max;
            }

            if(tz_max < t1)
            {
                t1 = tz_max;
            }

            //If the largest entering t value is less than the smallest exiting t value, then the ray is inside
            //the bounding box for the range of t values t0 to t1;
            return (t0 < t1 && t1 > GlobalVars.kEpsilon);
        }
    }
}
