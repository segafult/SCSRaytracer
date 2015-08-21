//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    
using System;

namespace SCSRaytracer
{
    class GlossySpecular : BRDF
    {
        private float exp;
        private float ks;
        private RGBColor  cs;

        public GlossySpecular()
        {
            exp = 1.0f;
            ks = 1.0f;
            cs = new RGBColor(1.0f, 1.0f, 1.0f);
        }
        //Copy constructor
        public GlossySpecular(GlossySpecular clone)
        {
            exp = clone.getExp();
            ks = clone.getKs();
            cs = clone.getCs();
            sampler_ptr = clone.getSampler();
        }

        public void setExp(float e) { exp = e; }
        public void setKs(float ks_a) { ks = ks_a; }
        public void setSampler(Sampler sampler_arg) { sampler_ptr = sampler_arg; }
        public void setCs(RGBColor color_arg) { cs = color_arg; }
        public float getExp() { return exp; }
        public float getKs() { return ks; }
        public RGBColor getCs() { return cs; }
        public Sampler getSampler() { return sampler_ptr; }

        public override RGBColor f(ShadeRec sr, Vect3D wi, Vect3D wo)
        {
            RGBColor L = new RGBColor(0.0f, 0.0f, 0.0f);
            float ndotwi = (sr.normal * wi); //Dot product of normal and angle of incidence gives the angle of mirror reflection
            Vect3D r = new Vect3D(-wi + 2.0f * sr.normal * ndotwi); //Vector describing direction of mirror reflection
            float rdotwo = (r * wo);

            if(rdotwo > 0.0)
            {
                L = ks * (float)Math.Pow(rdotwo, exp) * cs;
            }

            return L;
        }
    }
}
