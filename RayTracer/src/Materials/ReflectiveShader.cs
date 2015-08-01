//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.If not, see<http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    public class ReflectiveShader : PhongShader
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
        virtual public void setReflectivity(double refl) { reflective_brdf.setKr(refl); }
        virtual public void setCr(RGBColor c)
        {
            reflective_brdf.setCr(c);
        }
        public override RGBColor shade(ShadeRec sr)
        {
            RGBColor L = base.shade(sr); //Factor in all direct illumination

            Vect3D wo = -sr.ray.direction; //Vector pointing towards camera
            Vect3D wi = new Vect3D(); //Vector equivalent to perfect reflection
            RGBColor fr = reflective_brdf.sample_f(sr, ref wi, ref wo); //Set vectors for reflection to correct values
            Point3D hit_point = sr.hit_point + (wi * GlobalVars.shadKEpsilon); //Avoid salt+pepper noise
            Ray reflected_ray = new Ray(hit_point, wi); //Cast ray from point of incidence

            L += fr * sr.w.tracer.trace_ray(reflected_ray, sr.depth + 1) * (sr.normal * wi); //Recurse!

            return L; 
        }
    }
}
