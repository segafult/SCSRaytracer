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
using System.Numerics;

namespace RayTracer
{
    /// <summary>
    /// Representation of a normal vector
    /// </summary>
    struct Normal
    {
        //public float xcoord, ycoord, zcoord;
        public Vector3 coords;

        //Constructors
        public Normal(float x, float y, float z)
        {
            coords = new Vector3(x, y, z);
        }
        public Normal(Vector3 v)
        {
            coords = v;
        }
        //Copy constructor
        public Normal(Normal n)
        {
            coords = n.coords;
        }
        public Normal(Vect3D v)
        {
            coords = v.coords;
        }
        public Normal(Point3D p)
        {
            coords = p.coords;
        }

        public override string ToString()
        {
            return "[" + coords.X + "," + coords.X + "," + coords.X + "]";
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
                    float[] vals = new float[3];
                    for(int i = 0;i<3;i++)
                    {
                        vals[i] = Convert.ToSingle(args[i]);
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
                return new Normal(0,0,0);
            }
        }

        /// <summary>
        /// Function returning a unit vector of a normal
        /// </summary>
        /// <returns>Normal vector as a unit vector</returns>
        public Normal hat()
        {
            return new Normal(Vector3.Normalize(coords));
        }

        /// <summary>
        /// Normalizes normal to a unit vector
        /// </summary>
        public void normalize()
        {
            coords = Vector3.Normalize(coords);
        }

        ///
        ///Operator overloads
        /// 
        //Negative normal
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Normal operator -(Normal n)
        {
            return new Normal(-n.coords);
        }

        //Addition of two normal vectors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Normal operator +(Normal n, Normal m)
        {
            return new Normal(n.coords + m.coords);
        }

        //Dot product of a normal and a vector
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator *(Normal n, Vect3D u)
        {
            return Vector3.Dot(n.coords, u.coords);
        } 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator *(Vect3D u, Normal n)
        {
            return Vector3.Dot(u.coords, n.coords);
        }

        //Multiplying a normal by a scalar
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Normal operator *(float a, Normal n)
        {
            return new Normal(n.coords * a);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Normal operator *(Normal n, float a)
        {
            return new Normal(n.coords * a);
        }

        //Adding a normal and a vector together
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator +(Normal n, Vect3D u)
        {
            return new Vect3D(n.coords + u.coords);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator +(Vect3D u, Normal n)
        {
            return new Vect3D(u.coords + n. coords);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Normal operator *(Matrix4x4 m, Normal n)
        {
            return new Normal(Vector3.TransformNormal(n.coords, m));
        }
    }
}
