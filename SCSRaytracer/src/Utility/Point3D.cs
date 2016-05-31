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
        //Default constructor at origin
        public Point3D(float x, float y, float z)
        {
            _coords = new Vector3(x, y, z);
        }

        public Point3D(Vector3 vector)
        {
            _coords = vector;
        }
        //Copy constructor
        public Point3D(Point3D point)
        {
            _coords = point.Coordinates;
        }
        public Point3D(Vect3D vector)
        {
            _coords = vector.Coordinates;
        }
        public Point3D(Normal normal)
        {
            _coords = normal.Coordinates;
        }

        public override string ToString()
        {
            return "[" + Coordinates.X +"," + Coordinates.Y + "," + Coordinates.Z + "]";
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
        public static Point3D operator +(Point3D point, Vect3D vector)
        {
            return new Point3D(point.Coordinates + vector.Coordinates);
        }

        //Subtraction of a vector from a point
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3D operator -(Point3D point, Vect3D vector)
        {
            return new Point3D(point.Coordinates - vector.Coordinates);
        }

        //Addition of a normal to a point (for shadow calculations)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3D operator +(Point3D point, Normal normal)
        {
            return new Point3D(point.Coordinates + normal.Coordinates);
        }

        //Displacement vector (subtraction of a point from a point
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator -(Point3D point1, Point3D point2)
        {
            return new Vect3D(point1.Coordinates-point2.Coordinates);
        }

        //Multiplication of a point by a matrix
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3D operator *(Matrix4x4 matrix, Point3D point)
        {
            return new Point3D(Vector3.Transform(point.Coordinates, matrix));
        }
    }
}
