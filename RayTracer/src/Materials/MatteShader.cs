using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    /// <summary>
    /// Simple shader for matte materials
    /// </summary>
    public class MatteShader : Material
    {
        private Lambertian ambient_brdf;
        private Lambertian diffuse_brdf;

        public MatteShader()
        {
            ambient_brdf = new Lambertian();
            diffuse_brdf = new Lambertian();
        }

        //Setters for lambertian paramaters
        public void setKa(float ka) { ambient_brdf.setKd(ka); }
        public void setKd(float kd) { diffuse_brdf.setKd(kd); }
        public void setCd(RGBColor c) { ambient_brdf.setCd(c); diffuse_brdf.setCd(c); }

        public override RGBColor shade(ShadeRec sr)
        {
            Vect3D wo = -sr.ray.direction;
            RGBColor L = ambient_brdf.rho(sr, wo) * sr.w.ambientLight.L(sr);
            int numLights = sr.w.lightList.Count;
            Vect3D wi;
            double ndotwi;
            double t = GlobalVars.kHugeValue;
            
            //Loop through list of lights and add radiance for each diffuse light source.
            for(int i = 0; i < numLights; i++)
            {
                wi = sr.w.lightList[i].getDirection(sr);
                ndotwi = sr.normal * wi;
                //Direction must not be 0,0,0 to be a diffuse light source.
                if(ndotwi > 0.0 && !sr.w.hit_objects(new Ray(sr.hit_point+new Vect3D(0.001*sr.normal), wi)).hit_an_object)
                {
                    L += diffuse_brdf.f(sr, wo, wi) * sr.w.lightList[i].L(sr) * ndotwi;
                }
            }

            return L;
        }
    }
}
