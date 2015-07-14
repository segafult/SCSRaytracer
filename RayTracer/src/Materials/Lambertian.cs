using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public class Lambertian : BRDF
    {
        private float kd;
        private RGBColor cd;

        public Lambertian()
        {
            kd = 0.5F;
            cd = new RGBColor(0.5, 0.5, 0.5);
        }
        public Lambertian(float kdarg, RGBColor cdarg)
        {
            kd = kdarg;
            cdarg = new RGBColor(cdarg);
        }

        public float getKd() { return kd; }
        public RGBColor getCd() { return cd; }
        public void setKd(float kdarg) { kd = kdarg; }
        public void setCd(RGBColor cdarg) { cd = new RGBColor(cdarg); }
        
        public override RGBColor f(ShadeRec sr, Vect3D wi, Vect3D wo)
        {
            return (kd * cd * GlobalVars.invPI);
        }
        public override RGBColor sample_f(ShadeRec sr, Vect3D wi, Vect3D wo)
        {
            return GlobalVars.color_black;
        }
        public override RGBColor rho(ShadeRec sr, Vect3D wo)
        {
            return (kd * cd);
        }
    }
}
