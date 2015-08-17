//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Runtime.CompilerServices;
using System.Numerics;

namespace RayTracer
{
    /// <summary>
    /// Struct: Vect3D, a simple representation of a 3 dimensional Vector
    /// </summary>
    struct Vect3D
    {
        //public float xcoord, ycoord, zcoord;
        public Vector3 coords;
        
        //Constructors
        public Vect3D(float x, float y, float z)
        {
            coords = new Vector3(x, y, z);
        }
        public Vect3D(Vector3 vec)
        {

            coords = vec;
        }

        //Copy constructor
        public Vect3D(Vect3D v)
        {
            coords = v.coords;
        }
        public Vect3D(Normal n)
        {
            coords = n.coords;
        }
        public Vect3D(Point3D p)
        {
            coords = p.coords;
        }
        //End constructors

        public override string ToString()
        {
            return "[" + coords.X + "," + coords.Y + "," + coords.Z + "]";
        }

        public static Vect3D FromCsv(string input)
        {
            try
            {
                string[] args = input.Split(',');

                //Ensure there are 3 values
                if (args.Length == 3)
                {
                    float[] vals = new float[3];
                    for (int i = 0; i < 3; i++)
                    {
                        vals[i] = Convert.ToSingle(args[i]);
                    }

                    return new Vect3D(vals[0], vals[1], vals[2]);
                }
                else
                {
                    throw new System.FormatException();
                }
            }
            catch (System.FormatException e)
            {
                Console.WriteLine(e.ToString());
                return new Vect3D(0,0,0);
            }
        }
        /*
        /// <summary>
        /// Returns the magnitude of the Vector
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float magnitude()
        {
            return (float)Math.Sqrt(xcoord * xcoord + ycoord * ycoord + zcoord * zcoord);
        }

        /// <summary>
        /// Saves on calculations for instances where magnitude is squared
        /// </summary>
        /// <returns></returns>
        public float magnitudeSquared()
        {
            return (xcoord * xcoord + ycoord * ycoord + zcoord * zcoord);
        }
        */
        /// <summary>
        /// Hat (in reference to hat notation) is a unit vector that points in the same direction as the vector
        /// </summary>
        /// <returns>A unit vector pointing the same direction as the vector</returns>
        public Vect3D hat()
        {
            return new Vect3D(Vector3.Normalize(coords));
        }

        /// <summary>
        /// Reduces vector to a unit vector
        /// </summary>
        public void normalize()
        {
            coords = Vector3.Normalize(coords);
        }

        public float angleBetween(Vect3D b)
        {
            return (float)Math.Acos(this * b);
        }

        ///
        ///Operator overloads
        /// 
        //Addition of two vectors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator +(Vect3D u, Vect3D v)
        {
            return new Vect3D(u.coords + v.coords);
        }

        //Subtraction of two vectors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator -(Vect3D u, Vect3D v)
        {
            return new Vect3D(u.coords - v.coords);
        }

        //Negative of a vector
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator -(Vect3D u)
        {
            return new Vect3D(-u.coords);
        }

        //Scalar multiplication
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator *(float a, Vect3D u)
        {
            return new Vect3D(u.coords * a);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator *(Vect3D u, float a)
        {
            return new Vect3D(u.coords * a);
        }

        //Dot product of two vectors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator *(Vect3D u, Vect3D v)
        {
            return Vector3.Dot(u.coords, v.coords);
        }

        //Vector division by scalar
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator /(Vect3D u, float a)
        {
            return new Vect3D(Vector3.Divide(u.coords, a));
        }

        //Cross product of two vectors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator ^(Vect3D u, Vect3D v)
        {
            return new Vect3D(Vector3.Cross(u.coords, v.coords));
        }

        //Multiplication of a matrix with a vector
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator * (Matrix4x4 m, Vect3D v)
        {
            return new Vect3D(Vector3.Transform(v.coords, m));
        }
    }
}
