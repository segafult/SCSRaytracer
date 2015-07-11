using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{

    class Plane : RenderableObject
    {
        private Point3D p;
        private Normal n;

        public Plane()
        {
            p = new Point3D(0, 0, 0);
            n = new Normal(0, 1, 0);
        }
        public Plane(Point3D point, Normal normal)
        {
            p = new Point3D(point);
            n = new Normal(normal);
        }

        /// <summary>
        /// Determines t value for intersection of plane and given ray, passes shading info back through sr;
        /// </summary>
        /// <param name="r">Ray to determine intersection</param>
        /// <param name="tmin">Passed by reference, minimum t value</param>
        /// <param name="sr">ShadeRec to store shading info in</param>
        /// <returns></returns>
        override public bool hit(Ray r, ref double tmin, ref ShadeRec sr)
        {
            double t = (p - r.origin) * n / (r.direction * n);

            //Intersection is in front of camera
            if(t > GlobalVars.kEpsilon)
            {
                tmin = t;
                sr.normal = n;
                sr.hit_point = r.origin + t * r.direction;
                return true;
            }
            //Intersection is behind camera
            else
            {
                return false;
            }
        }
    }
}
