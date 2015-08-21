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
    /// Representation of a normal vector
    /// </summary>
    struct Normal
    {
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
            Matrix4x4 t = m;
            t.Translation = new Vector3(0, 0, 0);
            //Normals are transformed by the transpose
            Matrix4x4 result = Matrix4x4.Transpose(t);
            return new Normal(Vector3.Transform(n.coords, result));
        }
    }
}
