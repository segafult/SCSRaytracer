//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace RayTracer
{
    abstract class BRDF
    {
        protected Sampler sampler_ptr;
        protected Normal normal;

        virtual public RGBColor f(ShadeRec sr, Vect3D wi, Vect3D wo)
        {
            return GlobalVars.color_black;
        }
        virtual public RGBColor sample_f(ShadeRec sr, ref Vect3D  wi, ref Vect3D wo)
        {
            return GlobalVars.color_black;
        }
        virtual public RGBColor rho(ShadeRec sr, Vect3D wo)
        {
            return GlobalVars.color_black;
        }
    }
}
