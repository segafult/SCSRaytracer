//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    /// <summary>
    /// Ray in 3D space defined by a point (origin) and unit vector (direction)
    /// </summary>
    struct Ray
    {
        private Point3D _origin;
        private Vect3D _direction;

        // accessors
        public Point3D Origin
        {
            get
            {
                return _origin;
            }
            set
            {
                _origin = value;
            }
        }
        public Vect3D Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }

        //Constructors
        public Ray (Point3D origin, Vect3D direction)
        {
            _origin = new Point3D(origin);
            _direction = new Vect3D(direction.Hat());
        }

        //Copy constructor
        public Ray(Ray ray)
        {
            _origin = new Point3D(ray.Origin);
            _direction = new Vect3D(ray.Direction);
        }
    }
}
