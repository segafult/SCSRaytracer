//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Xml;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SCSRaytracer
{
    sealed class Box : RenderableObject
    {
        private float x0, x1;
        private float y0, y1;
        private float z0, z1;

        public Box()
        {
            x0 = -10;
            x1 = 10;
            y0 = -10;
            y1 = 10;
            z0 = -10;
            z1 = 10;
        }

        ///Constructor for perfect cube
        public Box(float x, float y, float z, float size)
        {
            x0 = x;
            y0 = y;
            z0 = z;
            float s = Math.Abs(size); //Error protection, dont want a negative size
            x1 = x + s;
            y1 = y + s;
            z1 = z + s;

        }

        public Box(float x0_arg, float x1_arg, float y0_arg, float y1_arg, float z0_arg, float z1_arg)
        {
            x0 = x0_arg;
            x1 = x1_arg;
            y0 = y0_arg;
            y1 = y1_arg;
            z0 = z0_arg;
            z1 = z1_arg;
        }

        public void setPoints(Point3D p1, Point3D p2)
        {
            x0 = p1.coords.X < p2.coords.X ? p1.coords.X : p2.coords.X;
            x1 = p1.coords.X > p2.coords.X ? p1.coords.X : p2.coords.X;
            y0 = p1.coords.Y < p2.coords.Y ? p1.coords.Y : p2.coords.Y;
            y1 = p1.coords.Y > p2.coords.Y ? p1.coords.Y : p2.coords.Y;
            z0 = p1.coords.Z < p2.coords.Z ? p1.coords.Z : p2.coords.Z;
            z1 = p1.coords.Z > p2.coords.Z ? p1.coords.Z : p2.coords.Z;
        }
        public override bool hit(Ray r, ref float tmin, ref ShadeRec sr)
        {
            ///-------------------------------------------------------------------------------------
            /// same as colision code for axis aligned bounding box
            float ox = r.origin.coords.X; float oy = r.origin.coords.Y; float oz = r.origin.coords.Z;
            float dx = r.direction.coords.X; float dy = r.direction.coords.Y; float dz = r.direction.coords.Z;

            float tx_min, ty_min, tz_min;
            float tx_max, ty_max, tz_max;
            
            float a = 1.0f / dx;
            if (a >= 0.0)
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
            if (b >= 0.0)
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
            if (c >= 0.0)
            {
                tz_min = (z0 - oz) * c;
                tz_max = (z1 - oz) * c;
            }
            else
            {
                tz_min = (z1 - oz) * c;
                tz_max = (z0 - oz) * c;
            }
            ///code differs after this point
            ///--------------------------------------------------------------------------
            /// 
            float t0, t1;
            int face_in, face_out; //for handling both inside and outside collisions with box

            //Find the largest entering t value
            if(tx_min > ty_min)
            {
                t0 = tx_min;
                face_in = (a >= 0.0) ? 0 : 3;
            }
            else
            {
                t0 = ty_min;
                face_in = (b >= 0.0) ? 1 : 4;
            }
            if(tz_min > t0)
            {
                t0 = tz_min;
                face_in = (c >= 0.0) ? 2 : 5;
            }

            //Find the smallest exiting t value
            if(tx_max < ty_max)
            {
                t1 = tx_max;
                face_out = (a >= 0.0) ? 3 : 0;
            }
            else
            {
                t1 = ty_max;
                face_out = (b >= 0.0) ? 4 : 1;
            }
            if(tz_max < t1)
            {
                t1 = tz_max;
                face_out = (c >= 0.0) ? 5 : 2;
            }

            //Hit conditions
            if (t0 < t1 && t1 > GlobalVars.kEpsilon)
            {
                if (t0 > GlobalVars.kEpsilon) //Ray hits outside surface
                {
                    tmin = t0;
                    sr.normal = get_normal(face_in);
                }
                else//Ray hits inside surface
                {
                    //Console.Write("hit inside");
                    tmin = t1;
                    sr.normal = get_normal(face_out);
                }

                sr.hit_point_local = r.origin + tmin * r.direction;
                sr.obj_material = mat;
                return true;
            }
            else
                return false;
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
            if (a >= 0.0)
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
            if (b >= 0.0)
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
            if (c >= 0.0)
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
            if (tx_min > ty_min)
            {
                t0 = tx_min;
            }
            else
            {
                t0 = ty_min;
            }

            if (tz_min > t0)
            {
                t0 = tz_min;
            }

            //smallest exiting t value
            if (tx_max < ty_max)
            {
                t1 = tx_max;
            }
            else
            {
                t1 = ty_max;
            }

            if (tz_max < t1)
            {
                t1 = tz_max;
            }

            //If the largest entering t value is less than the smallest exiting t value, then the ray is inside
            //the bounding box for the range of t values t0 to t1;
            return (t0 < t1 && t1 > GlobalVars.kEpsilon && t1 < tmin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Normal get_normal(int face_hit)
        {
            switch(face_hit)
            {
                case 0:
                    return (new Normal(-1, 0, 0)); //-x
                case 1:
                    return (new Normal(0, -1, 0)); //-y
                case 2:
                    return (new Normal(0, 0, -1)); //-z
                case 3:
                    return (new Normal(1, 0, 0)); //x
                case 4:
                    return (new Normal(0, 1, 0)); //y
                case 5:
                    return (new Normal(0, 0, 1)); //z
            }

            return new Normal(0,0,0);
        }

        public static Box LoadBox(XmlElement def)
        {
            Box toReturn = new Box();

            XmlNodeList points = def.SelectNodes("point");
            if (points.Count == 2)
            {
                List<Point3D> plist = new List<Point3D>();
                plist.Add(Point3D.FromCsv(((XmlText)points[0].FirstChild).Data));
                plist.Add(Point3D.FromCsv(((XmlText)points[1].FirstChild).Data));

                toReturn.setPoints(plist[0], plist[1]);
            }
            else
            {
                Console.WriteLine("Error: Box requires 2 points to be defined.");
            }
            return toReturn;
        }
    }
}
