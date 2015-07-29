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
    public class PerfectSpecular : BRDF
    {
        private double kr;
        private RGBColor cr;

        public PerfectSpecular()
        {
            kr = 1.0;
            cr = new RGBColor(1.0, 1.0, 1.0);
        }
        public PerfectSpecular(RGBColor color, double kr_arg)
        {
            kr = kr_arg;
            cr = color;
        }

        public void setKr(double kr_arg) { kr = kr_arg; }
        public void setCr(RGBColor cr_arg) { cr = cr_arg; }
        public double getKr() { return kr; }
        public RGBColor getCr() { return cr; }

        public override RGBColor sample_f(ShadeRec sr, ref Vect3D wi, ref Vect3D wo)
        {
            double ndotwo = sr.normal * wo;
            wi = -wo + 2.0 * sr.normal * ndotwo;

            return (kr * cr / (sr.normal * wi));
        }
    }
}
