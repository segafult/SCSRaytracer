//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    class PerfectSpecular : BRDF
    {
        private float kr;
        private RGBColor cr;
        private Texture cr_tex = null;

        public PerfectSpecular()
        {
            kr = 1.0f;
            cr = new RGBColor(1.0f, 1.0f, 1.0f);
        }
        public PerfectSpecular(RGBColor color, float kr_arg)
        {
            kr = kr_arg;
            cr = color;
        }

        public void setKr(float kr_arg) { kr = kr_arg; }
        public void setCr(RGBColor cr_arg) { cr = cr_arg; }
        public void setCr(Texture cr_arg) { cr_tex = cr_arg; }
        public float getKr() { return kr; }
        public RGBColor getCr() { return cr; }

        public override RGBColor sample_f(ShadeRec sr, ref Vect3D wi, ref Vect3D wo)
        {
            float ndotwo = sr.Normal * wo;
            wi = -wo + 2.0f * sr.Normal * ndotwo;
            if (cr_tex == null)
                return (kr * cr / (sr.Normal * wi));
            else
                return (kr * cr_tex.GetColor(sr) / (sr.Normal * wi));
        }
    }
}
