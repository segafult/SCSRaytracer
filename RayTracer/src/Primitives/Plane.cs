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
            if(t > GlobalVars.kEpsilon && t < tmin)
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

        public override bool hit(Ray r, double tmin)
        {
            double t = (p - r.origin) * n / (r.direction * n);

            //Intersection is in front of camera
            if (t > GlobalVars.kEpsilon && t < tmin)
            {
                return true;
            }
            //Intersection is behind camera
            return false;
        }
    }
}
