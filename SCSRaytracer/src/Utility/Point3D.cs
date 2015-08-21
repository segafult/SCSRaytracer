//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Runtime.CompilerServices;
using System.Numerics;

namespace SCSRaytracer
{
    /// <summary>
    /// Representation of a point in 3D space
    /// </summary>
    struct Point3D
    {
        //public float xcoord, ycoord, zcoord;
        public Vector3 coords;
        //Constructors
        //Default constructor at origin
        public Point3D(float x, float y, float z)
        {
            coords = new Vector3(x, y, z);
        }

        public Point3D(Vector3 v)
        {
            coords = v;
        }
        //Copy constructor
        public Point3D(Point3D p)
        {
            coords = p.coords;
        }
        public Point3D(Vect3D v)
        {
            coords = v.coords;
        }
        public Point3D(Normal n)
        {
            coords = n.coords;
        }

        public override string ToString()
        {
            return "[" + coords.X +"," + coords.Y + "," + coords.Z + "]";
        }

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
                    float[] vals = new float[3];
                    for (int i = 0; i < 3; i++)
                    {
                        vals[i] = Convert.ToSingle(args[i]);
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
                return new Point3D(0,0,0);
            }
        }

        ///
        ///Operator overloads
        /// 
        //Addition of a vector to a point
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3D operator +(Point3D a, Vect3D u)
        {
            return new Point3D(a.coords + u.coords);
        }

        //Subtraction of a vector from a point
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3D operator -(Point3D a, Vect3D u)
        {
            return new Point3D(a.coords - u.coords);
        }

        //Addition of a normal to a point (for shadow calculations)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3D operator +(Point3D a, Normal n)
        {
            return new Point3D(a.coords + n.coords);
        }

        //Displacement vector (subtraction of a point from a point
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator -(Point3D a, Point3D b)
        {
            return new Vect3D(a.coords-b.coords);
        }

        //Multiplication of a point by a matrix
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3D operator *(Matrix4x4 m, Point3D p)
        {
            return new Point3D(Vector3.Transform(p.coords, m));
        }
    }
}
