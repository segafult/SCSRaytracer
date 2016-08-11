//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace SCSRaytracer
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
        virtual public void setCd(Texture tex) { ambient_brdf.setCd(tex); diffuse_brdf.setCd(tex); }

        public override RGBColor shade(ShadeRec sr)
        {
            Vect3D wo = -sr.Ray.Direction; //Ray pointing from point of intersection to camera.
            RGBColor L = ambient_brdf.rho(sr, wo) * sr.WorldPointer.AmbientLight.L(sr); //Start with ambient
            int numlights = sr.WorldPointer.LightList.Count;
            Vect3D wi; //Direction to incident light
            float ndotwi;
            bool inShadow;
            Ray shadowRay;

            //Add together light contributions for all light sources
            for(int i = 0; i<numlights; i++)
            {
                wi = sr.WorldPointer.LightList[i].getDirection(sr); //Get the direction from the point of contact to the light source.
                ndotwi = (float)(sr.Normal * wi); //Dot product of normal and light source, 0 if orthogonal, 1 if parallel.
                if(ndotwi > 0.0f)//Avoid unnecessary light summation
                {
                    inShadow = false;

                    if(sr.WorldPointer.LightList[i].castsShadows())
                    {
                        shadowRay = new Ray(sr.HitPoint+(GlobalVars.SHAD_K_EPSILON*wi), wi);
                        inShadow = sr.WorldPointer.LightList[i].inShadow(sr, shadowRay);
                    }
                    if (!inShadow)
                    {
                        //Add diffuse and specular components.
                        L += (diffuse_brdf.f(sr, wo, wi) + specular_brdf.f(sr, wo, wi)) * sr.WorldPointer.LightList[i].L(sr) * ndotwi;
                    }
                }
            }

            return L;
        }
    }
}
