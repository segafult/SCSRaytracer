using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public abstract class RenderableObject
    {
        public virtual bool hit(Ray r, ref double tmin, ref ShadeRec sr)
        {
            return false;
        }

        public RGBColor color;
    }
}
