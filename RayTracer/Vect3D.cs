using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace RayTracer
{
    /// <summary>
    /// Class: Vect3D, a simple representation of a 3 dimensional Vector
    /// </summary>
    sealed public class Vect3D
    {
        public double xcoord, ycoord, zcoord;
        
        //Constructors
        //Default coordinates at origin
        public Vect3D()
        {
            xcoord = 0.0;
            ycoord = 0.0;
            zcoord = 0.0;
        }
        public Vect3D(double x, double y, double z)
        {
            xcoord = x;
            ycoord = y;
            zcoord = z;
        }
        //Copy constructor
        public Vect3D(Vect3D v)
        {
            xcoord = v.xcoord;
            ycoord = v.ycoord;
            zcoord = v.zcoord;
        }
        public Vect3D(Normal n)
        {
            xcoord = n.xcoord;
            ycoord = n.ycoord;
            zcoord = n.zcoord;
        }
        public Vect3D(Point3D p)
        {
            xcoord = p.xcoord;
            ycoord = p.ycoord;
            zcoord = p.zcoord;
        }
        //End constructors

        //Gets and sets
        public double getXCoordinates() { return xcoord; }
        public double getYCoordinates() { return ycoord; }
        public double getZCoordinates() { return zcoord; }

        /*
        public Vect3D addVector(Vect3D v)
        {
            return new Vect3D(xcoord + v.xcoord, ycoord + v.ycoord, zcoord + v.zcoord);
        }
        
        public double dotProduct(Vect3D v)
        {
            return ((xcoord * v.xcoord) + (ycoord * v.ycoord) + (zcoord * v.zcoord));
        }
        
        public Vect3D crossProduct(Vect3D v)
        {
            return new Vect3D((ycoord * v.zcoord - zcoord * v.ycoord),
                (zcoord*v.xcoord - xcoord * v.zcoord),
                (xcoord * v.ycoord - ycoord * v.xcoord));
        }

        public Vect3D multiplyScalar(double s)
        {
            return new Vect3D(xcoord * s, ycoord * s, zcoord * s);
        }
        
        public Vect3D negative()
        {
            return new Vect3D(-xcoord, -ycoord, -zcoord);
        }
        */

        /// <summary>
        /// Returns the magnitude of the Vector
        /// </summary>
        /// <returns></returns>
        public double magnitude()
        {
            return Math.Sqrt((xcoord * xcoord) + (ycoord * ycoord) + (zcoord * zcoord));
        }

        /// <summary>
        /// Saves on calculations for instances where magnitude is squared
        /// </summary>
        /// <returns></returns>
        public double magnitudeSquared()
        {
            return (xcoord * xcoord + ycoord * ycoord + zcoord * zcoord);
        }

        /// <summary>
        /// Hat (in reference to hat notation) is a unit vector that points in the same direction as the vector
        /// </summary>
        /// <returns>A unit vector pointing the same direction as the vector</returns>
        public Vect3D hat()
        {
            double m = magnitude();
            return new Vect3D(xcoord / m, ycoord / m, zcoord / m);
        }

        /// <summary>
        /// Reduces vector to a unit vector
        /// </summary>
        public void normalize()
        {
            Vect3D v = this.hat();
            xcoord = v.xcoord;
            ycoord = v.ycoord;
            zcoord = v.ycoord;
        }

        public double angleBetween(Vect3D b)
        {
            return Math.Acos(this * b);
        }

        ///
        ///Operator overloads
        /// 
        //Addition of two vectors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator +(Vect3D u, Vect3D v)
        {
            return new Vect3D(u.xcoord + v.xcoord, u.ycoord + v.ycoord, u.zcoord + v.zcoord);
        }

        //Subtraction of two vectors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator -(Vect3D u, Vect3D v)
        {
            return new Vect3D(u.xcoord - v.xcoord, u.ycoord - v.ycoord, u.zcoord - v.zcoord);
        }

        //Negative of a vector
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator -(Vect3D u)
        {
            return new Vect3D(-u.xcoord,-u.ycoord,-u.zcoord);
        }

        //Scalar multiplication
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator *(double a, Vect3D u)
        {
            return new Vect3D(u.xcoord * a, u.ycoord * a, u.zcoord * a);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator *(Vect3D u, double a)
        {
            return new Vect3D(u.xcoord * a, u.ycoord * a, u.zcoord * a);
        }

        //Dot product of two vectors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double operator *(Vect3D u, Vect3D v)
        {
            return (u.xcoord * v.xcoord + u.ycoord * v.ycoord + u.zcoord * v.zcoord);
        }

        //Vector division by scalar
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator /(Vect3D u, double a)
        {
            double inva = 1 / a;
            return new Vect3D(u.xcoord * inva, u.ycoord * inva, u.zcoord * inva);
        }

        //Cross product of two vectors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator ^(Vect3D u, Vect3D v)
        {
            return new Vect3D(u.ycoord * v.zcoord - u.zcoord * v.ycoord,
                u.zcoord * v.xcoord - u.xcoord * v.zcoord,
                u.xcoord * v.ycoord - u.ycoord * v.xcoord);
        }
    }
}
