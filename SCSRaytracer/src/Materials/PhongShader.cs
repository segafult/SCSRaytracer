//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace RayTracer
{
    /// <summary>
    /// Shader for simple phong model of specular reflection (sum of ambient, diffuse and specular components)
    /// </summary>
    class PhongShader : Material
    {
        protected Lambertian ambient_brdf;
        protected Lambertian diffuse_brdf;
        protected GlossySpecular specular_brdf;

        public override string ToString()
        {
            string toReturn = "Phong shader:\n";
            toReturn += "  ID: " + id + "\n";
            toReturn += "  Ka: " + ambient_brdf.getKd() + "\n";
            toReturn += "  Kd: " + diffuse_brdf.getKd() + "\n";
            toReturn += "  Cd: " + ambient_brdf.getCd() + "\n";
            toReturn += "  Exp: " + specular_brdf.getExp() + "\n";
            toReturn += "  Ks: " + specular_brdf.getKs();

            return toReturn;
        }
        public PhongShader()
        {
            ambient_brdf = new Lambertian();
            diffuse_brdf = new Lambertian();
            specular_brdf = new GlossySpecular();
            setExp(200);
        }

        public PhongShader(Lambertian ambient_brdf_arg, Lambertian diffuse_brdf_arg, GlossySpecular specular_brdf_arg)
        {
            ambient_brdf = new Lambertian(ambient_brdf_arg);
            diffuse_brdf = new Lambertian(diffuse_brdf_arg);
            specular_brdf = new GlossySpecular(specular_brdf_arg);
        }

        //Gets and sets
        virtual public void setKa(float ka) { ambient_brdf.setKd(ka); }
        virtual public void setKd(float kd) { diffuse_brdf.setKd(kd); }
        virtual public void setCd(RGBColor c) { ambient_brdf.setCd(c); diffuse_brdf.setCd(c); specular_brdf.setCs(c); }
        virtual public void setExp(float exp) { specular_brdf.setExp(exp); }
        virtual public void setKs(float ks) { specular_brdf.setKs(ks); }

        public override RGBColor shade(ShadeRec sr)
        {
            Vect3D wo = -sr.ray.direction; //Ray pointing from point of intersection to camera.
            RGBColor L = ambient_brdf.rho(sr, wo) * sr.w.ambientLight.L(sr); //Start with ambient
            int numlights = sr.w.lightList.Count;
            Vect3D wi; //Direction to incident light
            float ndotwi;
            bool inShadow;
            Ray shadowRay;

            //Add together light contributions for all light sources
            for(int i = 0; i<numlights; i++)
            {
                wi = sr.w.lightList[i].getDirection(sr); //Get the direction from the point of contact to the light source.
                ndotwi = (float)(sr.normal * wi); //Dot product of normal and light source, 0 if orthogonal, 1 if parallel.
                if(ndotwi > 0.0f)//Avoid unnecessary light summation
                {
                    inShadow = false;

                    if(sr.w.lightList[i].castsShadows())
                    {
                        shadowRay = new Ray(sr.hit_point+(GlobalVars.shadKEpsilon*wi), wi);
                        inShadow = sr.w.lightList[i].inShadow(sr, shadowRay);
                    }
                    if (!inShadow)
                    {
                        //Add diffuse and specular components.
                        L += (diffuse_brdf.f(sr, wo, wi) + specular_brdf.f(sr, wo, wi)) * sr.w.lightList[i].L(sr) * ndotwi;
                    }
                }
            }

            return L;
        }
    }
}
