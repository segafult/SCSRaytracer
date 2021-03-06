﻿//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Xml;

namespace SCSRaytracer
{
    sealed class Torus : RenderableObject
    {
        private float _a, _b;
        private BoundingBox _boundingBox;

        // accessors
        public override BoundingBox BoundingBox
        {
            get
            {
                return  _boundingBox;
            }
        }
        public float RingSize
        {
            get { return _a; }
            set
            {
                _a = value;
                _boundingBox = new BoundingBox(-_a - _b, _a + _b, -_b, _b, -_a - _b, _a + _b);
            }
        }
        public float RingRadius
        {
            get { return _b; }
            set
            {
                _b = value;
                _boundingBox = new BoundingBox(-_a - _b, _a + _b, -_b, _b, -_a - _b, _a + _b);
            }
        }

        public Torus()
        {
            _a = 2.0f;
            _b = 1.0f;

            _boundingBox = new BoundingBox(-_a - _b, _a + _b, -_b, _b, -_a - _b, _a + _b);
        }

        public Torus(float a_arg, float b_arg)
        {
            _a = a_arg;
            _b = b_arg;

            _boundingBox = new BoundingBox(-_a - _b, _a + _b, -_b, _b, -_a - _b, _a + _b);
        }

        //Gets and sets
        /*
        public void setA(float a_arg)
        {
            _a = a_arg;
            _boundingBox = new BoundingBox(-_a - _b, _a + _b, -_b, _b, -_a - _b, _a + _b);
        }
        public void setB(float b_arg)
        {
            _b = b_arg;
            _boundingBox = new BoundingBox(-_a - _b, _a + _b, -_b, _b, -_a - _b, _a + _b);
        }
        */

        public override string ToString()
        {
            return "Torus primitive:\n" +
                "  ID: " + id + "\n" +
                "  Mat: " + this.Material.id + "\n" +
                "  A: " + _a + "\n" +
                "  B: " + _b;

        }

        //public float getA() { return _a; }
        //public float getB() { return _b; }
        //public BoundingBox getBB() { return _boundingBox; }

        public override bool Hit(Ray r, ref float tmin, ref ShadeRec sr)
        {
            if(!_boundingBox.Hit(r, tmin))
            {
                return false;
            }
            /// Hit point on a torus as distance from point r.origin along vector r.direction
            /// can be solved for as a quartic equation of the form c4t^4 + c3t^3 + c2t^2 + c1t + c0 = 0
            /// where:
            /// c4 = (xd^2 + yd^2 + zd^2)^2
            /// c3 = 4(xd^2 + yd^2 + zd^2)(xoxd+yoyd+zozd)
            /// c2 = 2(xd^2 + yd^2 + zd^2)(xo^2+yo^2+zo^2-(a^2 + b^2))+4(xoxd + yoyd + zozd)^2 + 4a^2yd^2
            /// c1 = 4(xo^2 + yo^2 + zo^2 - (a^2 + b^2))(xoxd + yoyd + zozd) + 8a^2yoyd
            /// c0 = 4(xo^2 + yo^2 + zo^2 - (a^2 + b^2))^2 - 4a^2(b^2 - yo^2)
            /// 
            ///Using quartic solving algorithm
            /// 
            float x1 = r.Origin.X; float y1 = r.Origin.Y; float z1 = r.Origin.Z;
            float d1 = r.Direction.X; float d2 = r.Direction.Y; float d3 = r.Direction.Z;

            float[] coefficients = new float[5];
            float[] roots = new float[4];

            float sum_d_sqrd = d1 * d1 + d2 * d2 + d3 * d3;
            float e = x1 * x1 + y1 * y1 + z1 * z1 - _a * _a - _b * _b;
            float f = x1 * d1 + y1 * d2 + z1 * d3;
            float four_a_sqrd = 4.0f * _a * _a;


            coefficients[0] = e * e - four_a_sqrd * (_b * _b - y1 * y1);
            coefficients[1] = 4.0f * f * e + 2.0f * four_a_sqrd * y1 * d2;
            coefficients[2] = 2.0f * sum_d_sqrd * e + 4.0f * f * f + four_a_sqrd * d2 * d2;
            coefficients[3] = 4.0f * sum_d_sqrd * f;
            coefficients[4] = sum_d_sqrd * sum_d_sqrd;

            //Find roots
            int numroots = FastMath.SolveQuartic(coefficients, roots);
           
            bool hit = false;
            float t = GlobalVars.K_HUGE_VALUE;
            
            if (numroots == 0)
            {
                
                return false;
            }
            
            //Find the smallest root
            for(int i = 0;i< numroots;i++)
            {
                if(roots[i] > GlobalVars.K_EPSILON && roots[i] < t)
                {
                    hit = true;
                    t = roots[i];
                }
            }

            if(!hit)
            {
                return false;
            }

            tmin = t;
            sr.HitPointLocal = r.Origin + (t * r.Direction);
            sr.ObjectMaterial = _material;

            //Compute the normal at the hit point (see Thomas and Finney, 1996)
            sr.Normal = CalculateNormal(sr.HitPointLocal);

            return hit;
        }

        public override bool Hit(Ray r, float tmin)
        {
            /// Hit point on a torus as distance from point r.origin along vector r.direction
            /// can be solved for as a quartic equation of the form c4t^4 + c3t^3 + c2t^2 + c1t + c0 = 0
            /// where:
            /// c4 = (xd^2 + yd^2 + zd^2)^2
            /// c3 = 4(xd^2 + yd^2 + zd^2)(xoxd+yoyd+zozd)
            /// c2 = 2(xd^2 + yd^2 + zd^2)(xo^2+yo^2+zo^2-(a^2 + b^2))+4(xoxd + yoyd + zozd)^2 + 4a^2yd^2
            /// c1 = 4(xo^2 + yo^2 + zo^2 - (a^2 + b^2))(xoxd + yoyd + zozd) + 8a^2yoyd
            /// c0 = 4(xo^2 + yo^2 + zo^2 - (a^2 + b^2))^2 - 4a^2(b^2 - yo^2)
            /// 
            ///Using quartic solving algorithm
            /// 
            float x1 = r.Origin.X; float y1 = r.Origin.Y; float z1 = r.Origin.Z;
            float d1 = r.Direction.X; float d2 = r.Direction.Y; float d3 = r.Direction.Z;

            float[] coefficients = new float[5];
            float[] roots = new float[4];

            float sum_d_sqrd = d1 * d1 + d2 * d2 + d3 * d3;
            float e = x1 * x1 + y1 * y1 + z1 * z1 - _a * _a - _b * _b;
            float f = x1 * d1 + y1 * d2 + z1 * d3;
            float four_a_sqrd = 4.0f * _a * _a;


            coefficients[0] = e * e - four_a_sqrd * (_b * _b - y1 * y1);
            coefficients[1] = 4.0f * f * e + 2.0f * four_a_sqrd * y1 * d2;
            coefficients[2] = 2.0f * sum_d_sqrd * e + 4.0f * f * f + four_a_sqrd * d2 * d2;
            coefficients[3] = 4.0f * sum_d_sqrd * f;
            coefficients[4] = sum_d_sqrd * sum_d_sqrd;

            //Find roots
            int numroots = FastMath.SolveQuartic(coefficients, roots);


            if (numroots == 0)
            {

                return false;
            }

            //Find the smallest root
            for (int i = 0; i < numroots; i++)
            {
                if (roots[i] > GlobalVars.K_EPSILON && roots[i] < tmin)
                {
                    return true;
                }
            }

            return false;
        }

        private Normal CalculateNormal(Point3D point)
        {
            Normal result = new Normal();

            float param_squared = _a * _a + _b * _b;

            float x = point.X;
            float y = point.Y;
            float z = point.Z;
            float sum_squared = x * x + y * y + z * z;

            result.X = 4.0f * x * (sum_squared - param_squared);
            result.Y = 4.0f * y * (sum_squared - param_squared + 2.0f * _a * _a);
            result.Z = 4.0f * z * (sum_squared - param_squared);
            result.Normalize();

            return result;
        }

        public static Torus LoadTorus(XmlElement def)
        {
            Torus toReturn = new Torus();

            //Load a if provided
            try
            {
                XmlNode a = def.SelectSingleNode("a");
                if (a != null)
                {
                    float aDouble = Convert.ToSingle(((XmlText)a.FirstChild).Data);
                    toReturn.RingSize = aDouble;
                }
            }
            catch (System.FormatException e) { Console.WriteLine(e.ToString()); }

            //Load b if provided
            try
            {
                XmlNode b = def.SelectSingleNode("b");
                if (b != null)
                {
                    float bDouble = Convert.ToSingle(((XmlText)b.FirstChild).Data);
                    toReturn.RingRadius = bDouble;

                }
            }
            catch (System.FormatException e) { Console.WriteLine(e.ToString()); }
            return toReturn;
        }

        /*
        public override BoundingBox get_bounding_box()
        {
            return _boundingBox;
        }
        */
    }
}
