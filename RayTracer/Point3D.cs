using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace RayTracer
{
    /// <summary>
    /// Representation of a point in 3D space
    /// </summary>
    sealed public class Point3D
    {
        public double xcoord, ycoord, zcoord;

        //Constructors
        //Default constructor at origin
        public Point3D()
        {
            xcoord = 0.0;
            ycoord = 0.0;
            zcoord = 0.0;
        }
        public Point3D(double x, double y, double z)
        {
            xcoord = x;
            ycoord = y;
            zcoord = z;
        }
        //Copy constructor
        public Point3D(Point3D p)
        {
            xcoord = p.xcoord;
            ycoord = p.ycoord;
            zcoord = p.zcoord;
        }
        public Point3D(Vect3D v)
        {
            xcoord = v.xcoord;
            ycoord = v.ycoord;
            zcoord = v.zcoord;
        }
        public Point3D(Normal n)
        {
            xcoord = n.xcoord;
            ycoord = n.ycoord;
            zcoord = n.zcoord;
        }

        //Gets and sets
        public double getXCoordinates() { return xcoord; }
        public double getYCoordinates() { return ycoord; }
        public double getZCoordinates() { return zcoord; }

        /*
        public Point3D addVector(Vect3D v)
        {
            return new Point3D(xcoord + v.xcoord, ycoord + v.ycoord, zcoord + v.zcoord);
        }

        public Vect3D getDisplacementVector(Point3D p2)
        {
            return new Vect3D(xcoord - p2.xcoord, ycoord - p2.ycoord, zcoord - p2.zcoord);
        }
        */

        ///
        ///Operator overloads
        /// 
        //Addition of a vector to a point
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3D operator +(Point3D a, Vect3D u)
        {
            return new Point3D(a.xcoord + u.xcoord, a.ycoord + u.ycoord, a.zcoord + u.zcoord);
        }

        //Subtraction of a vector from a point
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3D operator -(Point3D a, Vect3D u)
        {
            return new Point3D(a.xcoord - u.xcoord, a.ycoord - u.ycoord, a.zcoord - u.zcoord);
        }

        //Displacement vector (subtraction of a point from a point
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vect3D operator -(Point3D a, Point3D b)
        {
            return new Vect3D(a.xcoord - b.xcoord, a.ycoord - b.ycoord, a.zcoord - b.zcoord);
        }
    }
}
