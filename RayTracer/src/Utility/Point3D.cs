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
using System.Runtime.CompilerServices;

namespace RayTracer
{
    /// <summary>
    /// Representation of a point in 3D space
    /// </summary>
    sealed public class Point3D
    {
        public double xcoord, ycoord, zcoord;

        //Constructors
        //Default constructor at origin
        public Point3D()
        {
            xcoord = 0.0;
            ycoord = 0.0;
            zcoord = 0.0;
        }
        public Point3D(double x, double y, double z)
        {
            xcoord = x;
            ycoord = y;
            zcoord = z;
        }
        //Copy constructor
        public Point3D(Point3D p)
        {
            xcoord = p.xcoord;
            ycoord = p.ycoord;
            zcoord = p.zcoord;
        }
        public Point3D(Vect3D v)
        {
            xcoord = v.xcoord;
            ycoord = v.ycoord;
            zcoord = v.zcoord;
        }
        public Point3D(Normal n)
        {
            xcoord = n.xcoord;
            ycoord = n.ycoord;
            zcoord = n.zcoord;
        }

        public override string ToString()
        {
            return "[" + xcoord + "," + ycoord + "," + zcoord + "]";
        }

        //Gets and sets
        public double getXCoordinates() { return xcoord; }
        public double getYCoordinates() { return ycoord; }
        public double getZCoordinates() { return zcoord; }

        //Generators
        /// <summary>
        /// Generates a Point3D from a 3 element CSV string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Point3D(x,y,z) if properly formed, null if invalid.</returns>
        public static Point3D FromCsv(string input)
        {
            try {
                string[] args = input.Split(',');
                
                //Ensure there are 3 values
                if (args.Length == 3)
                {
                    double[] vals = new double[3];
                    for (int i = 0; i < 3; i++)
                    {
                        vals[i] = Convert.ToDouble(args[i]);
                    }

                    return new Point3D(vals[0], vals[1], vals[2]);
                }
                else
                {
                    throw new System.FormatException();
                }
            }
            catch(System.FormatException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        ///
        ///Operator overloads
        /// 
        //Addition of a vector to a point
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3D operator +(Point3D a, Vect3D u)
        {
            return new Point3D(a.xcoord + u.xcoord, a.ycoord + u.ycoord, a.zcoord + u.zcoord);
        }

        //Subtraction of a vector from a point
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3D operator -(Point3D a, Vect3D u)
        {
            return new Point3D(a.xcoord - u.xcoord, a.ycoord - u.ycoord, a.zcoord - u.zcoord);
        }

        //Addition of a normal to a point (for shadow calculations)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3D operator +(Point3D a, Normal n)
        {
            return new Point3D(a.xcoord + n.xcoord, a.ycoord + n.ycoord, a.zcoord + n.zcoord);
        }

        //Displacement vector (subtraction of a point from a point
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator -(Point3D a, Point3D b)
        {
            return new Vect3D(a.xcoord - b.xcoord, a.ycoord - b.ycoord, a.zcoord - b.zcoord);
        }
    }
}
