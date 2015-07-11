using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public class WorldLight : Light
    {
        public WorldLight(Vect3D d)
        {
            direction = d.hat();
        }

        public override Vect3D getLightDirection(Point3D p)
        {
            //p is not used
            return direction;
        }
    }
}
