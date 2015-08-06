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
using System.Xml;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RayTracer
{
    public class Box : RenderableObject
    {
        private double x0, x1;
        private double y0, y1;
        private double z0, z1;

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
        public Box(double x, double y, double z, double size)
        {
            x0 = x;
            y0 = y;
            z0 = z;
            double s = Math.Abs(size); //Error protection, dont want a negative size
            x1 = x + s;
            y1 = y + s;
            z1 = z + s;

        }

        public Box(double x0_arg, double x1_arg, double y0_arg, double y1_arg, double z0_arg, double z1_arg)
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
            x0 = p1.xcoord < p2.xcoord ? p1.xcoord : p2.xcoord;
            x1 = p1.xcoord > p2.xcoord ? p1.xcoord : p2.xcoord;
            y0 = p1.ycoord < p2.ycoord ? p1.ycoord : p2.ycoord;
            y1 = p1.ycoord > p2.ycoord ? p1.ycoord : p2.ycoord;
            z0 = p1.zcoord < p2.zcoord ? p1.zcoord : p2.zcoord;
            z1 = p1.zcoord > p2.zcoord ? p1.zcoord : p2.zcoord;
        }
        public override bool hit(Ray r, ref double tmin, ref ShadeRec sr)
        {
            ///-------------------------------------------------------------------------------------
            /// same as colision code for axis aligned bounding box
            double ox = r.origin.xcoord; double oy = r.origin.ycoord; double oz = r.origin.zcoord;
            double dx = r.direction.xcoord; double dy = r.direction.ycoord; double dz = r.direction.zcoord;

            double tx_min, ty_min, tz_min;
            double tx_max, ty_max, tz_max;
            
            double a = 1.0 / dx;
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

            double b = 1.0 / dy;
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

            double c = 1.0 / dz;
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
            double t0, t1;
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
                return true;
            }
            else
                return false;
        }

        public override bool hit(Ray r, double tmin)
        {
            double ox = r.origin.xcoord; double oy = r.origin.ycoord; double oz = r.origin.zcoord;
            double dx = r.direction.xcoord; double dy = r.direction.ycoord; double dz = r.direction.zcoord;

            double tx_min, ty_min, tz_min;
            double tx_max, ty_max, tz_max;

            //How this algorithm works:
            //Generate *x_min and *x_max values which indicate the minimum and maximum length a line segment
            //from origin in the ray direction can have and be within the volume of the bounding box. If all 3
            //distance ranges overlap, then the bounding box was hit.

            double a = 1.0 / dx;
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

            double b = 1.0 / dy;
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

            double c = 1.0 / dz;
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

            double t0, t1;

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

            return null;
        }

        public static Box LoadBox(XmlElement def, World w)
        {
            Box toReturn = new Box();

            toReturn.id = def.GetAttribute("id");
            toReturn.setMaterial(w.getMaterialById(def.GetAttribute("mat")));

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
