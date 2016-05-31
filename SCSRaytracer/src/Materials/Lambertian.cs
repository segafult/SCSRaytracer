//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    class Lambertian : BRDF
    {
        private float kd;
        private RGBColor cd;
        private Texture cd_tex = null;

        public Lambertian()
        {
            kd = 0.5F;
            cd = new RGBColor(0.5f, 0.5f, 0.5f);
        }
        public Lambertian(float kdarg, RGBColor cdarg)
        {
            kd = kdarg;
            cd = cdarg;
        }
        //Copy constructor
        public Lambertian(Lambertian clone)
        {
            kd = clone.getKd();
            cd = clone.getCd();
        }

        public float getKd() { return kd; }
        public RGBColor getCd() { return cd; }
        public void setKd(float kdarg) { kd = kdarg; }
        public void setCd(RGBColor cdarg) { cd = cdarg; }
        public void setCd(Texture texarg) { cd_tex = texarg; }
        
        public override RGBColor f(ShadeRec sr, Vect3D wi, Vect3D wo)
        {
            if (cd_tex == null)
                return (kd * cd * GlobalVars.INVERSE_PI);
            else
                return (kd * cd_tex.getColor(sr) * GlobalVars.INVERSE_PI);
        }
        public override RGBColor rho(ShadeRec sr, Vect3D wo)
        {
            if (cd_tex == null)
                return (kd * cd);
            else
                return (kd * cd_tex.getColor(sr));
        }
    }
}
