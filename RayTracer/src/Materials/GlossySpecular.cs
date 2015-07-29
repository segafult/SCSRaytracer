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
using System.Runtime.CompilerServices;

namespace RayTracer
{
    public class GlossySpecular : BRDF
    {
        private double exp;
        private double ks;
        private RGBColor  cs;

        public GlossySpecular()
        {
            exp = 1.0f;
            ks = 1.0f;
            cs = new RGBColor(1.0, 1.0, 1.0);
        }
        //Copy constructor
        public GlossySpecular(GlossySpecular clone)
        {
            exp = clone.getExp();
            ks = clone.getKs();
            cs = clone.getCs();
            sampler_ptr = clone.getSampler();
        }

        public void setExp(double e) { exp = e; }
        public void setKs(double ks_a) { ks = ks_a; }
        public void setSampler(Sampler sampler_arg) { sampler_ptr = sampler_arg; }
        public void setCs(RGBColor color_arg) { cs = color_arg; }
        public double getExp() { return exp; }
        public double getKs() { return ks; }
        public RGBColor getCs() { return cs; }
        public Sampler getSampler() { return sampler_ptr; }

        public override RGBColor f(ShadeRec sr, Vect3D wi, Vect3D wo)
        {
            RGBColor L = new RGBColor(0.0, 0.0, 0.0);
            double ndotwi = (sr.normal * wi); //Dot product of normal and angle of incidence gives the angle of mirror reflection
            Vect3D r = new Vect3D(-wi + 2.0 * sr.normal * ndotwi); //Vector describing direction of mirror reflection
            double rdotwo = (r * wo);

            if(rdotwo > 0.0)
            {
                L = ks * Math.Pow(rdotwo, exp) * cs;
            }

            return L;
        }
    }
}
