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
using System.Runtime.CompilerServices;

namespace RayTracer
{
    public sealed class Sphere : RenderableObject
    {
        private Point3D c;
        private double r;

        //Constructors
        public Sphere()
        {
            c = new Point3D(0, 0, 0);
            r = 1.0;
        }
        public Sphere(Point3D center, double radius)
        {
            c = new Point3D(center);
            r = radius;
        }

        public override string ToString()
        {
            return "Sphere primitive:\n" +
                "  ID: " + id + "\n" +
                "  Mat: " + this.getMaterial().id + "\n" +
                "  c: " + c.ToString() + "\n" +
                "  r: " + r;
        }
        public void set_center(Point3D center)
        {
            c = new Point3D(center.xcoord, center.ycoord, center.zcoord);
        }
        public void set_radius(double radius)
        {
            r = radius;
        }
        /// <summary>
        /// Determines if given ray r intersects sphere.
        /// </summary>
        /// <param name="r">Ray</param>
        /// <param name="tmin">Return by reference, minimum distance to intersection</param>
        /// <param name="sr">Return by reference, shader parameters</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool hit(Ray ray, ref double tmin, ref ShadeRec sr)
        {
            double t;
            //Store variables locally to minimize member accesses
            Point3D rorigin = ray.origin;
            Vect3D rdirection = ray.direction;
            Vect3D temp = rorigin - this.c;
            //Rays are unit vectors
            //Intersection with a sphere is a quadratic equation (at^2 + bt + c = 0)
            //a = d*d , d = ray direction
            //b = 2(o-c)*d, o = ray origin, c = sphere center, d = ray direction
            //c = (o-c)*(o-c)-r^2, o = ray origin, c = sphere center, r = sphere radius
            double a = rdirection * rdirection;
            double b = 2.0 * temp * rdirection;
            double cv = (temp * temp) - (r*r);

            //Find discriminant, d = b^2 - 4ac
            //If d < 0, no intersection, if d = 0, one intersection, if d > 0, two intersections
            double d = b * b - 4 * a * cv;

            if(d < 0.0)
            {
                return false;
            }
            else
            {
                double e = Math.Sqrt(d);
                double invdenominator = 1/(2.0 * a);

                t = (-b - e) * invdenominator; //Solve quadratic equation for smallest value
                if (t > GlobalVars.kEpsilon)
                {
                    tmin = t;
                    sr.normal = new Normal((temp + t * rdirection) / r);
                    //Reverse the normal if ray originated inside the sphere
                    if ((rorigin - c).magnitude() < r)
                    {
                        sr.normal = -sr.normal;
                    }
                    sr.hit_point_local = rorigin + t * rdirection;
                    return true;
                }

                t = (-b + e) * invdenominator; //Solve quadratic equation for largest value
                if(t > GlobalVars.kEpsilon)
                {
                    tmin = t;
                    sr.normal = new Normal((temp + t * rdirection) / r);
                    //Reverse the normal if the ray originated from inside the sphere.
                    if((rorigin-c).magnitude() < r)
                    {
                        sr.normal = -sr.normal;
                    }
                    sr.hit_point_local = rorigin + t * rdirection;
                    return true;
                }
            }

            //Codepath shouldn't get here
            return false;
        }
        public override bool hit(Ray ray, double tmin)
        {
            double t;

            //Store variables locally to minimize member accesses
            Point3D rorigin = ray.origin;
            Vect3D rdirection = ray.direction;
            Vect3D temp = rorigin - this.c;

            //Rays are unit vectors
            //Intersection with a sphere is a quadratic equation (at^2 + bt + c = 0)
            //a = d*d , d = ray direction
            //b = 2(o-c)*d, o = ray origin, c = sphere center, d = ray direction
            //c = (o-c)*(o-c)-r^2, o = ray origin, c = sphere center, r = sphere radius
            double a = rdirection * rdirection;
            double b = 2.0 * temp * rdirection;
            double cv = (temp * temp) - (r * r);

            //Find discriminant, d = b^2 - 4ac
            //If d < 0, no intersection, if d = 0, one intersection, if d > 0, two intersections
            double d = b * b - 4 * a * cv;

            if (d < 0.0)
            {
                return false;
            }
            else
            {
                double e = Math.Sqrt(d);
                double invdenominator = 1 / (2.0 * a);
                t = (-b - e) * invdenominator; //Solve quadratic equation for smallest value
                if (t > GlobalVars.kEpsilon && t < tmin)
                {
                    return true;
                }

                t = (-b + e) * invdenominator; //Solve quadratic equation for largest value
                if (t > GlobalVars.kEpsilon && t < tmin)
                {
                    return true;
                }
            }

            //Codepath shouldn't get here
            return false;
        }
    }
}
