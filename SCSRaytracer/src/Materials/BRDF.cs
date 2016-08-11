//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    //Base class, bidirectional reflectance distribution function
    abstract class BRDF
    {
        protected Sampler samplerPointer;
        protected Normal normal;

        public Sampler Sampler
        {
            get
            {
                return samplerPointer;
            }
            set
            {
                samplerPointer = value;
            }
        }

        virtual public RGBColor F(ShadeRec sr, Vect3D incomingDirection, Vect3D reflectedDirection)
        {
            return GlobalVars.COLOR_BLACK;
        }
        virtual public RGBColor SampleF(ShadeRec sr, ref Vect3D incomingDirection, ref Vect3D reflectedDirection)
        {
            return GlobalVars.COLOR_BLACK;
        }
        virtual public RGBColor Rho(ShadeRec sr, Vect3D reflectedDirection)
        {
            return GlobalVars.COLOR_BLACK;
        }
    }
}
