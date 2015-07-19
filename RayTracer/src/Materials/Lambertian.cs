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
    public class Lambertian : BRDF
    {
        private double kd;
        private RGBColor cd;

        public Lambertian()
        {
            kd = 0.5F;
            cd = new RGBColor(0.5, 0.5, 0.5);
        }
        public Lambertian(double kdarg, RGBColor cdarg)
        {
            kd = kdarg;
            cdarg = new RGBColor(cdarg);
        }
        //Copy constructor
        public Lambertian(Lambertian clone)
        {
            kd = clone.getKd();
            cd = new RGBColor(clone.getCd());
        }

        public double getKd() { return kd; }
        public RGBColor getCd() { return cd; }
        public void setKd(double kdarg) { kd = kdarg; }
        public void setCd(RGBColor cdarg) { cd = new RGBColor(cdarg); }
        
        public override RGBColor f(ShadeRec sr, Vect3D wi, Vect3D wo)
        {
            return (kd * cd * GlobalVars.invPI);
        }
        public override RGBColor rho(ShadeRec sr, Vect3D wo)
        {
            return (kd * cd);
        }
    }
}
