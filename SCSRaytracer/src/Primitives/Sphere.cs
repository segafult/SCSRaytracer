//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Xml;
using System.Runtime.CompilerServices;

namespace SCSRaytracer
{
    sealed class Sphere : RenderableObject
    {
        private Point3D _center;
        private float _radius;

        public Point3D Center
        {
            get
            {
                return _center;
            }
            set
            {
                _center = new Point3D(value);
            }
        }
        public float Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
            }
        }
        public override BoundingBox BoundingBox
        {
            get
            {
                return new BoundingBox(_center.X - _radius, _center.X + _radius, _center.Y - _radius, _center.Y + _radius, _center.Z - _radius, _center.Z + _radius);
            }
        }

        //Constructors
        public Sphere()
        {
            _center = new Point3D(0, 0, 0);
            _radius = 1.0f;
        }
        public Sphere(Point3D center, float radius)
        {
            _center = new Point3D(center);
            _radius = radius;
        }

        public override string ToString()
        {
            return "Sphere primitive:\n" +
                "  ID: " + id + "\n" +
                "  Mat: " + this.Material.id + "\n" +
                "  c: " + _center.ToString() + "\n" +
                "  r: " + _radius;
        }

        /*
        public void set_center(Point3D center)
        {
            _center = new Point3D(center.X, center.Y, center.Z);
        }
        
        public void set_radius(float radius)
        {
            _radius = radius;
        }
        */

        /// <summary>
        /// Determines if given ray r intersects sphere.
        /// </summary>
        /// <param name="r">Ray</param>
        /// <param name="tmin">Return by reference, minimum distance to intersection</param>
        /// <param name="sr">Return by reference, shader parameters</param>
        /// <returns></returns>
        public override bool Hit(Ray ray, ref float tmin, ref ShadeRec sr)
        {
            float t;
            //Store variables locally to minimize member accesses
            Point3D rOrigin = ray.Origin;
            Vect3D rDirection = ray.Direction;
            Vect3D temp = rOrigin - this._center;

            //Rays are unit vectors
            //Intersection with a sphere is a quadratic equation (at^2 + bt + c = 0)
            //a = d*d , d = ray direction
            //b = 2(o-c)*d, o = ray origin, c = sphere center, d = ray direction
            //c = (o-c)*(o-c)-r^2, o = ray origin, c = sphere center, r = sphere radius
            float a = rDirection * rDirection;
            float b = 2.0f * temp * rDirection;
            float cv = (temp * temp) - (_radius*_radius);

            //Find discriminant, d = b^2 - 4ac
            //If d < 0, no intersection, if d = 0, one intersection, if d > 0, two intersections
            float discriminant = b * b - 4 * a * cv;

            if(discriminant < 0.0)
            {
                return false;
            }
            else
            {
                float e = (float)Math.Sqrt(discriminant);
                float invDenominator = 1.0f/(2.0f * a);

                t = (-b - e) * invDenominator; //Solve quadratic equation for smallest value
                if (t > GlobalVars.K_EPSILON)
                {
                    tmin = t;
                    sr.Normal = new Normal((temp + t * rDirection) / _radius);
                    //Reverse the normal if ray originated inside the sphere
                    if ((rOrigin - _center).Coordinates.Length() < _radius)
                    {
                        sr.Normal = -sr.Normal;
                    }
                    sr.HitPointLocal = rOrigin + t * rDirection;
                    sr.ObjectMaterial = _material;
                    return true;
                }

                t = (-b + e) * invDenominator; //Solve quadratic equation for largest value
                if(t > GlobalVars.K_EPSILON)
                {
                    tmin = t;
                    sr.Normal = new Normal((temp + t * rDirection) / _radius);
                    //Reverse the normal if the ray originated from inside the sphere.
                    if((rOrigin-_center).Coordinates.Length() < _radius)
                    {
                        sr.Normal = -sr.Normal;
                    }
                    sr.HitPointLocal = rOrigin + t * rDirection;
                    sr.ObjectMaterial = _material;
                    return true;
                }
            }

            //Codepath shouldn't get here
            return false;
        }

        /// <summary>
        /// Hit function for uniform grid optimization
        /// </summary>
        /// <param name="ray">Ray to intersect</param>
        /// <param name="tmin">Minimum distance for hit</param>
        /// <returns>True if ray intersects sphere, false if it does not</returns>
        public override bool Hit(Ray ray, float tmin)
        {
            float t;

            //Store variables locally to minimize member accesses
            Point3D rOrigin = ray.Origin;
            Vect3D rDirection = ray.Direction;
            Vect3D temp = rOrigin - this._center;

            //Rays are unit vectors
            //Intersection with a sphere is a quadratic equation (at^2 + bt + c = 0)
            //a = d*d , d = ray direction
            //b = 2(o-c)*d, o = ray origin, c = sphere center, d = ray direction
            //c = (o-c)*(o-c)-r^2, o = ray origin, c = sphere center, r = sphere radius
            float a = rDirection * rDirection;
            float b = 2.0f * temp * rDirection;
            float cv = (temp * temp) - (_radius * _radius);

            //Find discriminant, d = b^2 - 4ac
            //If d < 0, no intersection, if d = 0, one intersection, if d > 0, two intersections
            float d = b * b - 4 * a * cv;

            if (d < 0.0)
            {
                return false;
            }
            else
            {
                float e = (float)Math.Sqrt(d);
                float invdenominator = 1.0f / (2.0f * a);
                t = (-b - e) * invdenominator; //Solve quadratic equation for smallest value
                if (t > GlobalVars.K_EPSILON && t < tmin)
                {
                    return true;
                }

                t = (-b + e) * invdenominator; //Solve quadratic equation for largest value
                if (t > GlobalVars.K_EPSILON && t < tmin)
                {
                    return true;
                }
            }

            //Codepath shouldn't get here
            return false;
        }

        /// <summary>
        /// XML Loader function, instantiates sphere based on sphere definition in XML file
        /// </summary>
        /// <param name="def">XML Element in the DOM document tree</param>
        /// <returns>Handle for instantiated sphere</returns>
        public static Sphere LoadSphere(XmlElement def)
        {
            Sphere toReturn = new Sphere();

            //Load center of the sphere if provided
            XmlNode c = def.SelectSingleNode("point");
            if (c != null)
            {
                string cText = ((XmlText)c.FirstChild).Data;
                Point3D cObj = Point3D.FromCsv(cText);
                //if (cObj != null)
                //{
                toReturn.Center = cObj;
                //}
            }

            try
            {
                //Load radius of the sphere if provided
                XmlNode r = def.SelectSingleNode("r");
                if (r != null)
                {
                    float rDouble = Convert.ToSingle(((XmlText)r.FirstChild).Data);
                    toReturn.Radius = rDouble;
                }
            }
            catch (System.FormatException e)
            {
                Console.WriteLine(e.ToString());
            }

            return toReturn;
        }
        /*
        public override BoundingBox get_bounding_box()
        {
            return new BoundingBox(_center.X - _radius, _center.X + _radius, _center.Y - _radius, _center.Y + _radius, _center.Z - _radius, _center.Z + _radius);
        }
        */
    }
}
