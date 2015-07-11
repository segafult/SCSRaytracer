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
        

        //Gets and sets
        public double getXCoordinates() { return xcoord; }
        public double getYCoordinates() { return ycoord; }
        public double getZCoordinates() { return zcoord; }

        /*
        public Normal negative()
        {
            return new Normal(-xcoord, -ycoord, -zcoord);
        }

        public Normal addNormal(Normal m)
        {
            return new Normal(xcoord + m.xcoord, ycoord + m.ycoord, zcoord + m.zcoord);
        }

        public double dotProduct(Vect3D u)
        {
            return xcoord * u.xcoord + ycoord * u.ycoord + zcoord * u.zcoord;
        }

        public Normal multiplyScalar(double n)
        {
            return new Normal(xcoord * n, ycoord * n, zcoord * n);
        }

        public Vect3D addVector(Vect3D u)
        {
            return new Vect3D(xcoord + u.xcoord, ycoord + u.ycoord, zcoord + u.zcoord);
        }
        */
        /// <summary>
        /// Function returning a unit vector of a normal
        /// </summary>
        /// <returns>Normal vector as a unit vector</returns>
        public Normal hat()
        {
            double m = this.magnitude();
            return new Normal(xcoord / m, ycoord / m, zcoord / m);
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
            Normal n = this.hat();
            xcoord = n.xcoord;
            ycoord = n.ycoord;
            zcoord = n.zcoord;
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
