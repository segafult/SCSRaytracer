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
        public Vector3 corner0;
        public Vector3 corner1;

        public BoundingBox()
        {
            corner0 = new Vector3(-GlobalVars.K_HUGE_VALUE);
            corner1 = new Vector3(GlobalVars.K_HUGE_VALUE);
            //x0 = -GlobalVars.kHugeValue;
            //x1 = GlobalVars.kHugeValue;
            //y0 = -GlobalVars.kHugeValue;
            //y1 = GlobalVars.kHugeValue;
            //z0 = -GlobalVars.kHugeValue;
            //z1 = GlobalVars.kHugeValue;
        }
        public BoundingBox(float x0_arg, float x1_arg, float y0_arg, float y1_arg, float z0_arg, float z1_arg)
        {
            corner0 = new Vector3(x0_arg, y0_arg, z0_arg);
            corner1 = new Vector3(x1_arg, y1_arg, z1_arg);
            //x0 = x0_arg;
            //x1 = x1_arg;
            //y0 = y0_arg;
            //y1 = y1_arg;
            //z0 = z0_arg;
            //z1 = z1_arg;
        }

        public override bool Hit(Ray r, float tmin)
        {
            float ox = r.Origin.X; float oy = r.Origin.Y; float oz = r.Origin.Z;
            float dx = r.Direction.X; float dy = r.Direction.Y; float dz = r.Direction.Z;

            float tx_min, ty_min, tz_min;
            float tx_max, ty_max, tz_max;

            //How this algorithm works:
            //Generate *x_min and *x_max values which indicate the minimum and maximum length a line segment
            //from origin in the ray direction can have and be within the volume of the bounding box. If all 3
            //distance ranges overlap, then the bounding box was hit.

            float a = 1.0f / dx;
            if(a >= 0.0)
            {
                tx_min = (corner0.X - ox) * a;
                tx_max = (corner1.X - ox) * a;
            }
            else
            {
                tx_min = (corner1.X - ox) * a;
                tx_max = (corner0.X - ox) * a;
            }

            float b = 1.0f / dy;
            if( b >= 0.0)
            {
                ty_min = (corner0.Y - oy) * b;
                ty_max = (corner1.Y - oy) * b;
            }
            else
            {
                ty_min = (corner1.Y - oy) * b;
                ty_max = (corner0.Y - oy) * b;
            }

            float c = 1.0f / dz;
            if( c >= 0.0)
            {
                tz_min = (corner0.Z - oz) * c;
                tz_max = (corner1.Z - oz) * c;
            }
            else
            {
                tz_min = (corner1.Z - oz) * c;
                tz_max = (corner0.Z - oz) * c;
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
            return (t0 < t1 && t1 > GlobalVars.K_EPSILON);
        }

        public bool inside(Point3D parg)
        {
            return (parg.X > corner0.X && parg.X < corner1.X) &&
                (parg.Y > corner0.Y && parg.Y < corner1.Y) &&
                (parg.Z > corner0.Z && parg.Z < corner1.Z);
        }
    }
}
