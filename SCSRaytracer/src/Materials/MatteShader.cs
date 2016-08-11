//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
{
    /// <summary>
    /// Simple shader for matte materials
    /// </summary>
    class MatteShader : Material
    {
        private Lambertian ambient_brdf;
        private Lambertian diffuse_brdf;

        public override string ToString()
        {
            string toreturn = "Matte shader:\n";
            toreturn += "  ID = " + this.id + "\n";
            toreturn += "  Ka = " + ambient_brdf.getKd() + "\n";
            toreturn += "  Kd = " + diffuse_brdf.getKd() + "\n";
            toreturn += "  Cd = " + ambient_brdf.getCd().ToString();
            return toreturn;
        }
        public MatteShader()
        {
            ambient_brdf = new Lambertian();
            diffuse_brdf = new Lambertian();
        }

        //Setters for lambertian paramaters
        public void setKa(float ka) { ambient_brdf.setKd(ka); }
        public void setKd(float kd) { diffuse_brdf.setKd(kd); }
        public void setCd(RGBColor c) { ambient_brdf.setCd(c); diffuse_brdf.setCd(c); }
        public void setCd(Texture tex) { ambient_brdf.setCd(tex); diffuse_brdf.setCd(tex); }

        public override RGBColor shade(ShadeRec sr)
        {
            Vect3D wo = -sr.Ray.Direction;
            RGBColor L = ambient_brdf.rho(sr, wo) * sr.WorldPointer.AmbientLight.L(sr);
            int numLights = sr.WorldPointer.LightList.Count;
            Vect3D wi;
            float ndotwi;
            bool inShadow;
            Ray shadowRay;
            //float t = GlobalVars.kHugeValue;
            
            //Loop through list of lights and add radiance for each diffuse light source.
            for(int i = 0; i < numLights; i++)
            {
                wi = sr.WorldPointer.LightList[i].getDirection(sr);
                ndotwi = sr.Normal * wi;
                //Direction must not be 0,0,0 to be a diffuse light source.
                if(ndotwi > 0.0)
                {
                    inShadow = false;
                    if(sr.WorldPointer.LightList[i].castsShadows())
                    {
                        shadowRay = new Ray(sr.HitPoint + (GlobalVars.SHAD_K_EPSILON * wi), wi);
                        inShadow = sr.WorldPointer.LightList[i].inShadow(sr, shadowRay);
                    }
                    if(!inShadow)
                    {
                        L += diffuse_brdf.f(sr, wo, wi) * sr.WorldPointer.LightList[i].L(sr) * ndotwi;
                    }
                }
            }

            return L;
        }
    }
}
