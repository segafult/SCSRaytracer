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
    class Lambertian : BRDF
    {
        private double kd;
        private RGBColor cd;
        private Texture cd_tex = null;

        public Lambertian()
        {
            kd = 0.5F;
            cd = new RGBColor(0.5, 0.5, 0.5);
        }
        public Lambertian(double kdarg, RGBColor cdarg)
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

        public double getKd() { return kd; }
        public RGBColor getCd() { return cd; }
        public void setKd(double kdarg) { kd = kdarg; }
        public void setCd(RGBColor cdarg) { cd = cdarg; }
        public void setCd(Texture texarg) { cd_tex = texarg; }
        
        public override RGBColor f(ShadeRec sr, Vect3D wi, Vect3D wo)
        {
            if (cd_tex == null)
                return (kd * cd * GlobalVars.invPI);
            else
                return (kd * cd_tex.getColor(sr) * GlobalVars.invPI);
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
