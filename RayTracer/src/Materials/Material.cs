using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public abstract class Material
    {
        public abstract RGBColor shade(ShadeRec sr);
    }
}
