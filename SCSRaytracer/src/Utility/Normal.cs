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
        private Vector3 _coords;

        // accessors
        public Vector3 Coordinates
        {
            get
            {
                return _coords;
            }
            set
            {
                _coords = value;
            }
        }
        public float X
        {
            get
            {
                return _coords.X;
            }
            set
            {
                _coords.X = value;
            }
        }
        public float Y
        {
            get
            {
                return _coords.Y;
            }
            set
            {
                _coords.Y = value;
            }
        }
        public float Z
        {
            get
            {
                return _coords.Z;
            }
            set
            {
                _coords.Z = value;
            }
        }

        //Constructors
        public Normal(float x, float y, float z)
        {
            _coords = new Vector3(x, y, z);
        }
        public Normal(Vector3 v)
        {
            _coords = v;
        }
        //Copy constructor
        public Normal(Normal n)
        {
            _coords = n.Coordinates;
        }
        public Normal(Vect3D v)
        {
            _coords = v.Coordinates;
        }
        public Normal(Point3D p)
        {
            _coords = p.Coordinates;
        }

        public override string ToString()
        {
            return "[" + Coordinates.X + "," + Coordinates.Y + "," + Coordinates.Z + "]";
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
        public Normal Hat()
        {
            return new Normal(Vector3.Normalize(Coordinates));
        }

        /// <summary>
        /// Normalizes normal to a unit vector
        /// </summary>
        public void Normalize()
        {
            Coordinates = Vector3.Normalize(Coordinates);
        }

        ///
        ///Operator overloads
        /// 
        //Negative normal
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Normal operator -(Normal normal)
        {
            return new Normal(-normal.Coordinates);
        }

        //Addition of two normal vectors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Normal operator +(Normal normal1, Normal normal2)
        {
            return new Normal(normal1.Coordinates + normal2.Coordinates);
        }

        //Dot product of a normal and a vector
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator *(Normal normal, Vect3D vector)
        {
            return Vector3.Dot(normal.Coordinates, vector.Coordinates);
        } 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator *(Vect3D vector, Normal normal)
        {
            return Vector3.Dot(vector.Coordinates, normal.Coordinates);
        }

        //Multiplying a normal by a scalar
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Normal operator *(float scalar, Normal normal)
        {
            return new Normal(normal.Coordinates * scalar);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Normal operator *(Normal normal, float scalar)
        {
            return new Normal(normal.Coordinates * scalar);
        }

        //Adding a normal and a vector together
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator +(Normal normal, Vect3D vector)
        {
            return new Vect3D(normal.Coordinates + vector.Coordinates);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator +(Vect3D vector, Normal normal)
        {
            return new Vect3D(vector.Coordinates + normal. Coordinates);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Normal operator *(Matrix4x4 matrix, Normal normal)
        {
            Matrix4x4 t = matrix;
            t.Translation = new Vector3(0, 0, 0);
            //Normals are transformed by the transpose
            Matrix4x4 result = Matrix4x4.Transpose(t);
            return new Normal(Vector3.Transform(normal.Coordinates, result));
        }
    }
}
