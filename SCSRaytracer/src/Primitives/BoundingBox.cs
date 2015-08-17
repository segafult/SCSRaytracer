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

namespace RayTracer
{
    /// <summary>
    /// Axis aligned bounding box for expensive ray-object intersections.
    /// Won't cast shadows, and won't appear in renderings.
    /// </summary>
    class BoundingBox : RenderableObject
    {
        public float x0, x1;
        public float y0, y1;
        public float z0, z1;

        public BoundingBox()
        {
            x0 = -GlobalVars.kHugeValue;
            x1 = GlobalVars.kHugeValue;
            y0 = -GlobalVars.kHugeValue;
            y1 = GlobalVars.kHugeValue;
            z0 = -GlobalVars.kHugeValue;
            z1 = GlobalVars.kHugeValue;
        }
        public BoundingBox(float x0_arg, float x1_arg, float y0_arg, float y1_arg, float z0_arg, float z1_arg)
        {
            x0 = x0_arg;
            x1 = x1_arg;
            y0 = y0_arg;
            y1 = y1_arg;
            z0 = z0_arg;
            z1 = z1_arg;
        }

        public override bool hit(Ray r, float tmin)
        {
            float ox = r.origin.coords.X; float oy = r.origin.coords.Y; float oz = r.origin.coords.Z;
            float dx = r.direction.coords.X; float dy = r.direction.coords.Y; float dz = r.direction.coords.Z;

            float tx_min, ty_min, tz_min;
            float tx_max, ty_max, tz_max;

            //How this algorithm works:
            //Generate *x_min and *x_max values which indicate the minimum and maximum length a line segment
            //from origin in the ray direction can have and be within the volume of the bounding box. If all 3
            //distance ranges overlap, then the bounding box was hit.

            float a = 1.0f / dx;
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

            float b = 1.0f / dy;
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

            float c = 1.0f / dz;
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

            float t0, t1;

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

        public bool inside(Point3D parg)
        {
            return (parg.coords.X > x0 && parg.coords.X < x1) &&
                (parg.coords.Y > y0 && parg.coords.Y < y1) &&
                (parg.coords.Z > z0 && parg.coords.Z < z1);
        }
    }
}
