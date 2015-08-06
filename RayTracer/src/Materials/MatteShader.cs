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

namespace RayTracer
{
    /// <summary>
    /// Simple shader for matte materials
    /// </summary>
    public class MatteShader : Material
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
        public void setKa(double ka) { ambient_brdf.setKd(ka); }
        public void setKd(double kd) { diffuse_brdf.setKd(kd); }
        public void setCd(RGBColor c) { ambient_brdf.setCd(c); diffuse_brdf.setCd(c); }

        public override RGBColor shade(ShadeRec sr)
        {
            Vect3D wo = -sr.ray.direction;
            RGBColor L = ambient_brdf.rho(sr, wo) * sr.w.ambientLight.L(sr);
            int numLights = sr.w.lightList.Count;
            Vect3D wi;
            double ndotwi;
            bool inShadow;
            Ray shadowRay;
            //double t = GlobalVars.kHugeValue;
            
            //Loop through list of lights and add radiance for each diffuse light source.
            for(int i = 0; i < numLights; i++)
            {
                wi = sr.w.lightList[i].getDirection(sr);
                ndotwi = sr.normal * wi;
                //Direction must not be 0,0,0 to be a diffuse light source.
                if(ndotwi > 0.0)
                {
                    inShadow = false;
                    if(sr.w.lightList[i].castsShadows())
                    {
                        shadowRay = new Ray(sr.hit_point + (GlobalVars.shadKEpsilon * wi), wi);
                        inShadow = sr.w.lightList[i].inShadow(sr, shadowRay);
                    }
                    if(!inShadow)
                    {
                        L += diffuse_brdf.f(sr, wo, wi) * sr.w.lightList[i].L(sr) * ndotwi;
                    }
                }
            }

            return L;
        }
    }
}
