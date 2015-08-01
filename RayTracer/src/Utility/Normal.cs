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
    /// <summary>
    /// Representation of a normal vector
    /// </summary>
    sealed public class Normal
    {
        public double xcoord, ycoord, zcoord;

        //Constructors
        public Normal()
        {
            xcoord = 0.0;
            ycoord = 0.0;
            zcoord = 0.0;
        }
        public Normal(double x, double y, double z)
        {
            xcoord = x;
            ycoord = y;
            zcoord = z;
        }
        //Copy constructor
        public Normal(Normal n)
        {
            xcoord = n.xcoord;
            ycoord = n.ycoord;
            zcoord = n.zcoord;
        }
        public Normal(Vect3D v)
        {
            xcoord = v.xcoord;
            ycoord = v.ycoord;
            zcoord = v.zcoord;
        }
        public Normal(Point3D p)
        {
            xcoord = p.xcoord;
            ycoord = p.ycoord;
            zcoord = p.zcoord;
        }

        public override string ToString()
        {
            return "[" + xcoord + "," + ycoord + "," + zcoord + "]";
        }

        //Generators
        /// <summary>
        /// Generates a normal from a 3 element CSV string
        /// </summary>
        /// <param name="input">String to generate Normal from</param>
        /// <returns>Normal(x,y,z) if input is well formed, otherwise null.</returns>
        public static Normal FromCsv(string input)
        {
            try
            {
                string[] args = input.Split(',');
                if(args.Length == 3)
                {
                    double[] vals = new double[3];
                    for(int i = 0;i<3;i++)
                    {
                        vals[i] = Convert.ToDouble(args[i]);
                    }

                    return new Normal(vals[0], vals[1], vals[2]);
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
        

        //Gets and sets
        public double getXCoordinates() { return xcoord; }
        public double getYCoordinates() { return ycoord; }
        public double getZCoordinates() { return zcoord; }

        /// <summary>
        /// Function returning a unit vector of a normal
        /// </summary>
        /// <returns>Normal vector as a unit vector</returns>
        public Normal hat()
        {
            double invm = 1/this.magnitude();
            return new Normal(xcoord * invm, ycoord * invm, zcoord * invm);
        }

        private double magnitude()
        {
            return Math.Sqrt(xcoord * xcoord + ycoord * ycoord + zcoord * zcoord);
        }

        /// <summary>
        /// Normalizes normal to a unit vector
        /// </summary>
        public void normalize()
        {
            double invn = 1 / this.magnitude();
            xcoord = xcoord * invn;
            ycoord = ycoord * invn;
            zcoord = zcoord * invn;
        }

        ///
        ///Operator overloads
        /// 
        //Negative normal
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Normal operator -(Normal n)
        {
            return new Normal(-n.xcoord, -n.ycoord, -n.zcoord);
        }

        //Addition of two normal vectors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Normal operator +(Normal n, Normal m)
        {
            return new Normal(n.xcoord + m.xcoord, n.ycoord + m.ycoord, n.zcoord + m.zcoord);
        }

        //Dot product of a normal and a vector
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double operator *(Normal n, Vect3D u)
        {
            return n.xcoord * u.xcoord + n.ycoord * u.ycoord + n.zcoord * u.zcoord;
        } 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double operator *(Vect3D u, Normal n)
        {
            return n.xcoord * u.xcoord + n.ycoord * u.ycoord + n.zcoord * u.zcoord;
        }

        //Multiplying a normal by a scalar
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Normal operator *(double a, Normal n)
        {
            return new Normal(n.xcoord * a, n.ycoord * a, n.zcoord * a);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Normal operator *(Normal n, double a)
        {
            return new Normal(n.xcoord * a, n.ycoord * a, n.zcoord * a);
        }

        //Adding a normal and a vector together
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator +(Normal n, Vect3D u)
        {
            return new Vect3D(n.xcoord + u.xcoord, n.ycoord + u.ycoord, n.zcoord + u.zcoord);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator +(Vect3D u, Normal n)
        {
            return new Vect3D(n.xcoord + u.xcoord, n.ycoord + u.ycoord, n.zcoord + u.zcoord);
        }
    }
}
