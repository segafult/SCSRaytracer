using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    abstract public class Light
    {
        protected Vect3D direction;

        abstract public Vect3D getLightDirection(Point3D p);
    }
}
