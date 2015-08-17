//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace RayTracer
{
    /// <summary>
    /// Ray in 3D space defined by a point (origin) and unit vector (direction)
    /// </summary>
    struct Ray
    {
        //Note, members are public due to their frequent external access.
        public Point3D origin;
        public Vect3D direction;

        //Constructors
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
