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
        protected BoundingBox boundingBox;
        protected Vector3 lowBound, highBound;
        protected static float EPSILON = 1.0e-3f;
        protected static float INVTWOEPSILON = 1 / (EPSILON * 2.0f);
        protected float minimumRaymarchStep; //Minimum value to increment ray by when asymptotically approaching a surface
        protected float maximumRaymarchStep; //Maximum value to increment ray by, helpful for avoiding excessive steps due to saddle points
        protected float distanceMultiplier; //Value to multiply distance by to guarantee not penetrating the surface of the implicit
        protected float triggerDistance; //Distance at which a hit is registered
        protected int RECURSIONDEPTH = 4;

        public RayMarchedImplicit()
        {
            boundingBox = new BoundingBox();
            lowBound = new Vector3(-4);
            highBound = new Vector3(4);
            minimumRaymarchStep = 1.0e-5f;
            maximumRaymarchStep = 4.0f;
            distanceMultiplier = 0.1f;
            triggerDistance = 0.1f;
        }

        public void SetBoundaries(Point3D min, Point3D max)
        {
            lowBound = Vector3.Min(min.Coordinates, max.Coordinates);
            highBound = Vector3.Max(min.Coordinates, max.Coordinates);
            SetupBounds();
        }
        
        public void SetupBounds()
        {
            //Construct bounding box
            boundingBox.corner0 = lowBound;
            boundingBox.corner1 = highBound;
            //bbox.x0 = lowbound.X; bbox.y0 = lowbound.Y; bbox.z0 = lowbound.Z;
            //bbox.x1 = highbound.X; bbox.y1 = highbound.Y; bbox.z1 = highbound.Z;
        }

        public override bool Hit(Ray ray, ref float tMin, ref ShadeRec sr)
        {
            //First verify intersection point of ray with bounding box.
            Vector3 o = ray.Origin.Coordinates;
            //float ox = r.origin.coords.X;
            //float oy = r.origin.coords.Y;
            //float oz = r.origin.coords.Z;
            Vector3 d = ray.Direction.Coordinates;
            //float dx = r.direction.coords.X;
            //float dy = r.direction.coords.Y;
            //float dz = r.direction.coords.Z;

            Vector3 c0 = boundingBox.corner0;
            Vector3 c1 = boundingBox.corner1;
            //float x0 = bbox.c0.X;
            //float y0 = bbox.c0.Y;
            //float z0 = bbox.c0.Z;
            //float x1 = bbox.c1.X;
            //float y1 = bbox.c1.Y;
            //float z1 = bbox.c1.Z;

            float tx_min, ty_min, tz_min;
            float tx_max, ty_max, tz_max;

            Vector3 inverseDenominator = new Vector3(1.0f)/d;
            //Vector3 min;
            //Vector3 max;
            float a = inverseDenominator.X;
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

            float b = inverseDenominator.Y;
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

            float c = inverseDenominator.Z;
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

            float tPosition; //Entry value of t for ray, lowest possible t value
            if (!boundingBox.inside(ray.Origin))
                tPosition = t0; //Start casting from t0 if starting from outside bounding box
            else
                tPosition = GlobalVars.K_EPSILON; //Start casting from origin if starting from inside bounding box
            float tDistance = 0; //Value returned by the distance function approximation
            float tPositionPrevious = 0;
            float adjustedDistance = 0; //Adjusted distance scaled by distance adjustment parameter
            float currentDistanceFunctionValue = 0;
            float previousDistanceFunctionValue = 0;
            Point3D location;
            //Traverse space using raymarching algorithm
            do
            {
                location = ray.Origin + ray.Direction * tPosition;
                tDistance = EvaluateDistanceFunction(location,ray.Direction, ref currentDistanceFunctionValue);
                adjustedDistance = tDistance * distanceMultiplier;

                //Clamp the adjusted distance between the minimum and maximum steps
                adjustedDistance = FastMath.clamp(adjustedDistance, minimumRaymarchStep, maximumRaymarchStep);
                //Increment tpos by the adjusted distance
                tPosition += adjustedDistance;

                //Accidentally stepped over bounds, solve using bisection algorithm
                if(previousDistanceFunctionValue*currentDistanceFunctionValue < 0.0f)
                {
                    SolveRootByBisection(ray, ref tMin, ref sr, tPositionPrevious, tPosition, RECURSIONDEPTH);
                    return true;
                }
                tPositionPrevious = tPosition;
                previousDistanceFunctionValue = currentDistanceFunctionValue;
            } while (tPosition < t1 && tDistance > triggerDistance);

            //Hit
            if(tDistance < triggerDistance)
            {
                tMin = tPosition;
                sr.HitPointLocal = ray.Origin + tPosition * ray.Direction;
                sr.Normal = ApproximateNormal(sr.HitPointLocal, ray.Direction);
                sr.ObjectMaterial = _material;
                return true;
            }
            else
            {
                return false;
            }
        }

        //Evaluates distance function at a given point in space. Can be overridden for special cases where exact distance function known (ie. sphere)
        protected virtual float EvaluateDistanceFunction(Point3D point, Vect3D distance, ref float cur)
        {
            //Distance (or at least the approximation of it) is a function d(x) = |f(x)/f'(x)|
            cur = EvaluateImplicitFunction(point);
            return Math.Abs(cur / EvaluateImplicitFunctionDerivative(point, distance));
        }

        //Evaluates given implicit function at a point, should be overridden in subclasses.
        public virtual float EvaluateImplicitFunction(Point3D p)
        {
            return 1.0f;
        }
        //Approximates the gradient of F using the small run method. Can be overridden if the
        //gradient is known.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual float EvaluateImplicitFunctionDerivative(Point3D point, Vect3D direction)
        {
            //Find the points just in front of the provided point, and just behind it along the ray
            Point3D behind = point + (direction * EPSILON);
            Point3D infront = point - (direction * EPSILON);

            //Slope = rise over run
            return (EvaluateImplicitFunction(infront) - EvaluateImplicitFunction(behind)) * INVTWOEPSILON;
        }

        private bool ZeroExistsInInterval(Ray ray, float low, float high)
        {
            float f_low = EvaluateImplicitFunction(ray.Origin + ray.Direction * (float)low);
            float f_high = EvaluateImplicitFunction(ray.Origin + ray.Direction * (float)high);
            return (f_low * f_high) < 0.0f;
        }

        private bool SolveRootByBisection(Ray ray, ref float tMin, ref ShadeRec sr, float lowBound, float highBound, int depth)
        {
            if(depth > 0)
            {
                //Find the mid point between the low and high bound
                float midbound;
                midbound = lowBound + ((highBound - lowBound) / 2.0f);
                if (ZeroExistsInInterval(ray, lowBound, midbound))
                    return SolveRootByBisection(ray, ref tMin, ref sr, lowBound, midbound, depth - 1);
                else if (ZeroExistsInInterval(ray, midbound, highBound))
                    return SolveRootByBisection(ray, ref tMin, ref sr, midbound, highBound, depth - 1);
                else
                {
                    //Converged to correct location!
                    tMin = lowBound;
                    sr.HitPointLocal = ray.Origin + lowBound * ray.Direction;
                    sr.Normal = ApproximateNormal(sr.HitPointLocal, ray.Direction);
                    sr.ObjectMaterial = _material;
                    return true;
                }
            }
            else
            {
                //Bottom of recursion stack, calculate relevant values
                tMin = lowBound;
                sr.HitPointLocal = ray.Origin + (float)lowBound * ray.Direction;
                sr.Normal = ApproximateNormal(sr.HitPointLocal, ray.Direction);
                sr.ObjectMaterial = sr.WorldPointer.MaterialList[0];
                return true;
            }
        }

        //Approximates the gradient of F(p) at a given point p
        protected virtual Normal ApproximateNormal(Point3D p, Vect3D rd)
        {
            float f = EvaluateImplicitFunction(p);
            float f_x = EvaluateImplicitFunction(new Point3D(p.X + EPSILON, p.Y, p.Z));
            float f_y = EvaluateImplicitFunction(new Point3D(p.X, p.Y + EPSILON, p.Z));
            float f_z = EvaluateImplicitFunction(new Point3D(p.X, p.Y, p.Z + EPSILON));

            //Compute vector for normal
            Vect3D raw_normal = (new Vect3D((float)(f_x - f), (float)(f_y - f), (float)(f_z - f))).Hat();

            //Check if dot product is positive, if so, flip the vector so it's facing the ray origin
            if(raw_normal * rd.Hat() > 0.0f) { raw_normal = -raw_normal; }
            return (new Normal(raw_normal));
        }
    }
}
