﻿//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

namespace RayTracer
{
    class ReflectiveShader : PhongShader
    {
        private PerfectSpecular reflective_brdf;

        public ReflectiveShader() : base()
        {
            reflective_brdf = new PerfectSpecular();
        }

        public override string ToString()
        {
            string toReturn = "Reflective/whittal shader:\n";
            toReturn += "  ID: " + id + "\n";
            toReturn += "  Ka: " + ambient_brdf.getKd() + "\n";
            toReturn += "  Kd: " + diffuse_brdf.getKd() + "\n";
            toReturn += "  Cd: " + ambient_brdf.getCd() + "\n";
            toReturn += "  Exp: " + specular_brdf.getExp() + "\n";
            toReturn += "  Ks: " + specular_brdf.getKs() +"\n";
            toReturn += "  Cr: " + reflective_brdf.getCr().ToString() + "\n";
            toReturn += "  Kr: " + reflective_brdf.getKr();

            return toReturn;
        }
        virtual public void setReflectivity(float refl) { reflective_brdf.setKr(refl); }
        virtual public void setCr(RGBColor c)
        {
            reflective_brdf.setCr(c);
        }
        public override RGBColor shade(ShadeRec sr)
        {
            RGBColor L = base.shade(sr); //Factor in all direct illumination

            Vect3D wo = -sr.ray.direction; //Vector pointing towards camera
            Vect3D wi = new Vect3D(1.0f,1.0f,1.0f); //Vector equivalent to perfect reflection
            RGBColor fr = reflective_brdf.sample_f(sr, ref wi, ref wo); //Set vectors for reflection to correct values
            Point3D hit_point = sr.hit_point + (wi * GlobalVars.shadKEpsilon); //Avoid salt+pepper noise
            Ray reflected_ray = new Ray(hit_point, wi); //Cast ray from point of incidence

            L += fr * sr.w.tracer.trace_ray(reflected_ray, sr.depth + 1) * (sr.normal * wi); //Recurse!

            return L; 
        }
    }
}
