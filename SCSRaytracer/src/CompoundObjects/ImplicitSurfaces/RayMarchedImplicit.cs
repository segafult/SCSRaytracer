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
    /// <summary>
    /// Base class for ray marched implicit surface
    /// </summary>
    abstract class RayMarchedImplicit : RenderableObject
    {
        protected BoundingBox bbox;
        protected Vector3 lowbound, highbound;
        protected static float EPSILON = 1.0e-3f;
        protected static float INVTWOEPSILON = 1 / (EPSILON * 2.0f);
        protected float min_step; //Minimum value to increment ray by when asymptotically approaching a surface
        protected float max_step; //Maximum value to increment ray by, helpful for avoiding excessive steps due to saddle points
        protected float dist_mult; //Value to multiply distance by to guarantee not penetrating the surface of the implicit
        protected float trigger_dist; //Distance at which a hit is registered
        protected int RECURSIONDEPTH = 4;

        public RayMarchedImplicit()
        {
            bbox = new BoundingBox();
            lowbound = new Vector3(-4);
            highbound = new Vector3(4);
            min_step = 1.0e-5f;
            max_step = 4.0f;
            dist_mult = 0.1f;
            trigger_dist = 0.1f;
        }

        public void setBoundaries(Point3D min, Point3D max)
        {
            lowbound = Vector3.Min(min.Coordinates, max.Coordinates);
            highbound = Vector3.Max(min.Coordinates, max.Coordinates);
            setup_bounds();
        }
        
        public void setup_bounds()
        {
            //Construct bounding box
            bbox.c0 = lowbound;
            bbox.c1 = highbound;
            //bbox.x0 = lowbound.X; bbox.y0 = lowbound.Y; bbox.z0 = lowbound.Z;
            //bbox.x1 = highbound.X; bbox.y1 = highbound.Y; bbox.z1 = highbound.Z;
        }

        public override bool hit(Ray r, ref float tmin, ref ShadeRec sr)
        {
            //First verify intersection point of ray with bounding box.
            Vector3 o = r.Origin.Coordinates;
            //float ox = r.origin.coords.X;
            //float oy = r.origin.coords.Y;
            //float oz = r.origin.coords.Z;
            Vector3 d = r.Direction.Coordinates;
            //float dx = r.direction.coords.X;
            //float dy = r.direction.coords.Y;
            //float dz = r.direction.coords.Z;

            Vector3 c0 = bbox.c0;
            Vector3 c1 = bbox.c1;
            //float x0 = bbox.c0.X;
            //float y0 = bbox.c0.Y;
            //float z0 = bbox.c0.Z;
            //float x1 = bbox.c1.X;
            //float y1 = bbox.c1.Y;
            //float z1 = bbox.c1.Z;

            float tx_min, ty_min, tz_min;
            float tx_max, ty_max, tz_max;

            Vector3 invd = new Vector3(1.0f)/d;
            //Vector3 min;
            //Vector3 max;
            float a = invd.X;
            if (a >= 0)
            {
                tx_min = (c0.X - o.X) * a;
                tx_max = (c1.X - o.X) * a;
            }
            else
            {
                tx_min = (c1.X - o.X) * a;
                tx_max = (c0.X - o.X) * a;
            }

            float b = invd.Y;
            if (b >= 0)
            {
                ty_min = (c0.Y - o.Y) * b;
                ty_max = (c1.Y - o.Y) * b;
            }
            else
            {
                ty_min = (c1.Y - o.Y) * b;
                ty_max = (c0.Y - o.Y) * b;
            }

            float c = invd.Z;
            if (c >= 0)
            {
                tz_min = (c0.Z - o.Z) * c;
                tz_max = (c1.Z - o.Z) * c;
            }
            else
            {
                tz_min = (c1.Z - o.Z) * c;
                tz_max = (c0.Z - o.Z) * c;
            }

            //Determine if volume was hit
            float t0, t1;
            t0 = (tx_min > ty_min) ? tx_min : ty_min;
            if (tz_min > t0) t0 = tz_min;
            t1 = (tx_max < ty_max) ? tx_max : ty_max;
            if (tz_max < t1) t1 = tz_max;

            if (t0 > t1)
            {
                return false;
            }

            float tpos; //Entry value of t for ray, lowest possible t value
            if (!bbox.inside(r.Origin))
                tpos = t0; //Start casting from t0 if starting from outside bounding box
            else
                tpos = GlobalVars.K_EPSILON; //Start casting from origin if starting from inside bounding box
            float tdist = 0; //Value returned by the distance function approximation
            float tposprev = 0;
            float adjdist = 0; //Adjusted distance scaled by distance adjustment parameter
            float curval = 0;
            float prevval = 0;
            Point3D loc;
            //Traverse space using raymarching algorithm
            do
            {
                loc = r.Origin + r.Direction * tpos;
                tdist = evalD(loc,r.Direction, ref curval);
                adjdist = tdist * dist_mult;

                //Clamp the adjusted distance between the minimum and maximum steps
                adjdist = FastMath.clamp(adjdist, min_step, max_step);
                //Increment tpos by the adjusted distance
                tpos += adjdist;

                //Accidentally stepped over bounds, solve using bisection algorithm
                if(prevval*curval < 0.0f)
                {
                    solveRootByBisection(r, ref tmin, ref sr, tposprev, tpos, RECURSIONDEPTH);
                    return true;
                }
                tposprev = tpos;
                prevval = curval;
            } while (tpos < t1 && tdist > trigger_dist);

            //Hit
            if(tdist < trigger_dist)
            {
                tmin = tpos;
                sr.hit_point_local = r.Origin + tpos * r.Direction;
                sr.normal = approximateNormal(sr.hit_point_local, r.Direction);
                sr.obj_material = mat;
                return true;
            }
            else
            {
                return false;
            }
        }

        //Evaluates distance function at a given point in space. Can be overridden for special cases where exact distance function known (ie. sphere)
        protected virtual float evalD(Point3D p, Vect3D d, ref float cur)
        {
            //Distance (or at least the approximation of it) is a function d(x) = |f(x)/f'(x)|
            cur = evalF(p);
            return Math.Abs(cur / evalFPrime(p, d));
        }

        //Evaluates given implicit function at a point, should be overridden in subclasses.
        public virtual float evalF(Point3D p)
        {
            return 1.0f;
        }
        //Approximates the gradient of F using the small run method. Can be overridden if the
        //gradient is known.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual float evalFPrime(Point3D p, Vect3D d)
        {
            //Find the points just in front of the provided point, and just behind it along the ray
            Point3D behind = p + (d * EPSILON);
            Point3D infront = p - (d * EPSILON);

            //Slope = rise over run
            return (evalF(infront) - evalF(behind)) * INVTWOEPSILON;
        }

        private bool zeroExistsInInterval(Ray r, float low, float high)
        {
            float f_low = evalF(r.Origin + r.Direction * (float)low);
            float f_high = evalF(r.Origin + r.Direction * (float)high);
            return (f_low * f_high) < 0.0f;
        }

        private bool solveRootByBisection(Ray r, ref float tmin, ref ShadeRec sr, float lowbound, float highbound, int depth)
        {
            if(depth > 0)
            {
                //Find the mid point between the low and high bound
                float midbound;
                midbound = lowbound + ((highbound - lowbound) / 2.0f);
                if (zeroExistsInInterval(r, lowbound, midbound))
                    return solveRootByBisection(r, ref tmin, ref sr, lowbound, midbound, depth - 1);
                else if (zeroExistsInInterval(r, midbound, highbound))
                    return solveRootByBisection(r, ref tmin, ref sr, midbound, highbound, depth - 1);
                else
                {
                    //Converged to correct location!
                    tmin = lowbound;
                    sr.hit_point_local = r.Origin + lowbound * r.Direction;
                    sr.normal = approximateNormal(sr.hit_point_local, r.Direction);
                    sr.obj_material = mat;
                    return true;
                }
            }
            else
            {
                //Bottom of recursion stack, calculate relevant values
                tmin = lowbound;
                sr.hit_point_local = r.Origin + (float)lowbound * r.Direction;
                sr.normal = approximateNormal(sr.hit_point_local, r.Direction);
                sr.obj_material = sr.w.MaterialList[0];
                return true;
            }
        }

        //Approximates the gradient of F(p) at a given point p
        protected virtual Normal approximateNormal(Point3D p, Vect3D rd)
        {
            float f = evalF(p);
            float f_x = evalF(new Point3D(p.X + EPSILON, p.Y, p.Z));
            float f_y = evalF(new Point3D(p.X, p.Y + EPSILON, p.Z));
            float f_z = evalF(new Point3D(p.X, p.Y, p.Z + EPSILON));

            //Compute vector for normal
            Vect3D raw_normal = (new Vect3D((float)(f_x - f), (float)(f_y - f), (float)(f_z - f))).Hat();

            //Check if dot product is positive, if so, flip the vector so it's facing the ray origin
            if(raw_normal * rd.Hat() > 0.0f) { raw_normal = -raw_normal; }
            return (new Normal(raw_normal));
        }
    }
}
