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
    /// Struct: Vect3D, a simple representation of a 3 dimensional Vector
    /// </summary>
    struct Vect3D
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
        public Vect3D(float x, float y, float z)
        {
            _coords = new Vector3(x, y, z);
        }
        public Vect3D(Vector3 vector)
        {

            _coords = vector;
        }

        //Copy constructor
        public Vect3D(Vect3D vector)
        {
            _coords = vector.Coordinates;
        }
        public Vect3D(Normal normal)
        {
            _coords = normal.Coordinates;
        }
        public Vect3D(Point3D point)
        {
            _coords = point.Coordinates;
        }
        //End constructors

        public override string ToString()
        {
            return "[" + Coordinates.X + "," + Coordinates.Y + "," + Coordinates.Z + "]";
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

        /// <summary>
        /// Hat (in reference to hat notation) is a unit vector that points in the same direction as the vector
        /// </summary>
        /// <returns>A unit vector pointing the same direction as the vector</returns>
        public Vect3D Hat()
        {
            return new Vect3D(Vector3.Normalize(Coordinates));
        }

        /// <summary>
        /// Reduces vector to a unit vector
        /// </summary>
        public void Normalize()
        {
            Coordinates = Vector3.Normalize(Coordinates);
        }

        public float AngleBetween(Vect3D b)
        {
            return (float)Math.Acos(this * b);
        }

        ///
        ///Operator overloads
        /// 
        //Addition of two vectors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator +(Vect3D vector1, Vect3D vector2)
        {
            return new Vect3D(vector1.Coordinates + vector2.Coordinates);
        }

        //Subtraction of two vectors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator -(Vect3D vector1, Vect3D vector2)
        {
            return new Vect3D(vector1.Coordinates - vector2.Coordinates);
        }

        //Negative of a vector
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator -(Vect3D vector)
        {
            return new Vect3D(-vector.Coordinates);
        }

        //Scalar multiplication
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator *(float scalar, Vect3D vector)
        {
            return new Vect3D(vector.Coordinates * scalar);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator *(Vect3D vector, float scalar)
        {
            return new Vect3D(vector.Coordinates * scalar);
        }

        //Dot product of two vectors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator *(Vect3D vector1, Vect3D vector2)
        {
            return Vector3.Dot(vector1.Coordinates, vector2.Coordinates);
        }

        //Vector division by scalar
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator /(Vect3D vector, float scalar)
        {
            return new Vect3D(Vector3.Divide(vector.Coordinates, scalar));
        }

        //Cross product of two vectors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator ^(Vect3D vector1, Vect3D vector2)
        {
            return new Vect3D(Vector3.Cross(vector1.Coordinates, vector2.Coordinates));
        }

        //Multiplication of a matrix with a vector
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator * (Matrix4x4 matrix, Vect3D vector)
        {
            //Get new matrix with a 0 translation component
            Matrix4x4 t = matrix;
            t.Translation = new Vector3(0, 0, 0);
            return new Vect3D(Vector3.Transform(vector.Coordinates, t));
        }
    }
}
