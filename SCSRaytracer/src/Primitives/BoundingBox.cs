//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System.Numerics;

namespace SCSRaytracer
{
    /// <summary>
    /// Axis aligned bounding box for expensive ray-object intersections.
    /// Won't cast shadows, and won't appear in renderings.
    /// </summary>
    sealed class BoundingBox : RenderableObject
    {
        //public float x0, x1;
        //public float y0, y1;
        //public float z0, z1;
        public Vector3 c0;
        public Vector3 c1;

        public BoundingBox()
        {
            c0 = new Vector3(-GlobalVars.kHugeValue);
            c1 = new Vector3(GlobalVars.kHugeValue);
            //x0 = -GlobalVars.kHugeValue;
            //x1 = GlobalVars.kHugeValue;
            //y0 = -GlobalVars.kHugeValue;
            //y1 = GlobalVars.kHugeValue;
            //z0 = -GlobalVars.kHugeValue;
            //z1 = GlobalVars.kHugeValue;
        }
        public BoundingBox(float x0_arg, float x1_arg, float y0_arg, float y1_arg, float z0_arg, float z1_arg)
        {
            c0 = new Vector3(x0_arg, y0_arg, z0_arg);
            c1 = new Vector3(x1_arg, y1_arg, z1_arg);
            //x0 = x0_arg;
            //x1 = x1_arg;
            //y0 = y0_arg;
            //y1 = y1_arg;
            //z0 = z0_arg;
            //z1 = z1_arg;
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
                tx_min = (c0.X - ox) * a;
                tx_max = (c1.X - ox) * a;
            }
            else
            {
                tx_min = (c1.X - ox) * a;
                tx_max = (c0.X - ox) * a;
            }

            float b = 1.0f / dy;
            if( b >= 0.0)
            {
                ty_min = (c0.Y - oy) * b;
                ty_max = (c1.Y - oy) * b;
            }
            else
            {
                ty_min = (c1.Y - oy) * b;
                ty_max = (c0.Y - oy) * b;
            }

            float c = 1.0f / dz;
            if( c >= 0.0)
            {
                tz_min = (c0.Z - oz) * c;
                tz_max = (c1.Z - oz) * c;
            }
            else
            {
                tz_min = (c1.Z - oz) * c;
                tz_max = (c0.Z - oz) * c;
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
            return (parg.coords.X > c0.X && parg.coords.X < c1.X) &&
                (parg.coords.Y > c0.Y && parg.coords.Y < c1.Y) &&
                (parg.coords.Z > c0.Z && parg.coords.Z < c1.Z);
        }
    }
}
