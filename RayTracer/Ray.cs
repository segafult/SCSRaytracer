using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    /// <summary>
    /// Ray in 3D space defined by a point (origin) and unit vector (direction)
    /// </summary>
    public class Ray
    {
        //Note, members are public due to their frequent external access.
        public Point3D origin;
        public Vect3D direction;

        //Constructors
        public Ray ()
        {
            //Default ray is at the origin and pointing 1 in the X direction
            origin = new Point3D(0, 0, 0);
            direction = new Vect3D(1, 0, 0);
        }
        public Ray (Point3D o, Vect3D d)
        {
            origin = new Point3D(o);
            direction = new Vect3D(d.hat());
        }
        //Copy constructor
        public Ray(Ray r)
        {
            origin = new Point3D(r.origin);
            direction = new Vect3D(r.direction);
        }
    }
}
