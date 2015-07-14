using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    abstract public class BRDF
    {
        protected Sampler sampler_ptr;
        protected Normal normal;

        abstract public RGBColor f(ShadeRec sr, Vect3D wi, Vect3D wo);
        abstract public RGBColor sample_f(ShadeRec sr, Vect3D wi, Vect3D wo);
        abstract public RGBColor rho(ShadeRec sr, Vect3D wo);
    }
}
