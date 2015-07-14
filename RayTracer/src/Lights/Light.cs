using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    /// <summary>
    /// Template class, Light.
    /// </summary>
    abstract public class Light
    {
        protected bool shadows;
        abstract public RGBColor L(ShadeRec sr);
        public virtual Vect3D getDirection(ShadeRec sr)
        {
            return new Vect3D(0, 0, 0);
        }
    }
}
